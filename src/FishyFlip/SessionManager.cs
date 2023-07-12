﻿// <copyright file="SessionManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace FishyFlip;

internal class SessionManager : IDisposable
{
    private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler = new();

    private readonly TokenValidationParameters defaultTokenValidationParameters = new()
    {
        ValidateActor = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuer = false,
        ValidateSignatureLast = false,
        ValidateTokenReplay = false,
        ValidateIssuerSigningKey = false,
        ValidateWithLKG = false,
        LogValidationExceptions = false,
        SignatureValidator = (token, parameters) =>
        {
            var jwt = new JwtSecurityToken(token);
            return jwt;
        },
    };

    private ATProtocol protocol;
    private Session? session;
    private bool disposed;
    private System.Timers.Timer? timer;
    private int refreshing;
    private ILogger? logger;

    public SessionManager(ATProtocol protocol)
    {
        this.protocol = protocol;
        this.logger = this.protocol.Options.Logger;
    }

    public SessionManager(ATProtocol protocol, Session session)
    {
        this.protocol = protocol;
        this.logger = this.protocol.Options.Logger;
        this.SetSession(session);
    }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// This will change which APIs are called and what data is returned.
    /// For example, if you are unauthorized, it will use "com.atproto" instead of "app.bsky"
    /// for endpoints that exist in both places.
    /// </summary>
    public bool IsAuthenticated => this.session is not null;

    public Session? Session => this.session;

    public void Dispose() => this.Dispose(true);

    internal void UpdateBearerToken(Session session)
    {
        this.protocol.Client
                .DefaultRequestHeaders
                .Authorization =
            new AuthenticationHeaderValue("Bearer", session.AccessJwt);
    }

    internal void SetSession(Session session)
    {
        this.session = session;
        this.UpdateBearerToken(session);

        this.logger?.LogDebug($"Session set, {session.Did}");
        if (!this.protocol.Options.AutoRenewSession)
        {
            this.logger?.LogDebug("AutoRenewSession is disabled.");
            return;
        }

        this.logger?.LogDebug("AutoRenewSession is enabled.");
        this.timer ??= new System.Timers.Timer();

        this.ConfigureRefreshTokenTimer();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed || this.timer is null)
        {
            return;
        }

        if (disposing)
        {
            this.timer.Enabled = false;
            this.timer.Dispose();
            this.timer = null;
        }

        this.disposed = true;
    }

    private void ConfigureRefreshTokenTimer()
    {
        this.timer.ThrowIfNull();
        TimeSpan timeToNextRenewal = this.protocol.Options.SessionRefreshInterval ?? this.GetTimeToNextRenewal(this.session.ThrowIfNull());
        this.logger?.LogDebug($"Next renewal in {timeToNextRenewal.TotalSeconds}.");
        this.timer!.Elapsed += this.RefreshToken;
        this.timer.Interval = timeToNextRenewal.TotalMilliseconds >= Int32.MaxValue ? Int32.MaxValue : timeToNextRenewal.TotalMilliseconds;
        this.timer.Enabled = true;
        this.timer.Start();
    }

    private TimeSpan GetTimeToNextRenewal(Session session)
    {
        this.jwtSecurityTokenHandler
            .ValidateToken(
                session.RefreshJwt,
                this.defaultTokenValidationParameters,
                out SecurityToken token);
        return token.ValidTo.ToUniversalTime() - DateTime.UtcNow;
    }

    private async void RefreshToken(object? sender, ElapsedEventArgs e)
    {
        if (Interlocked.Increment(ref this.refreshing) > 1)
        {
            this.logger?.LogDebug("Already refreshing.");
            Interlocked.Decrement(ref this.refreshing);
            return;
        }

        this.logger?.LogDebug("Refreshing session.");
        this.timer.ThrowIfNull().Enabled = false;
        try
        {
            if (this.session is not null)
            {
                Multiple<Session, Error> result =
                await this.protocol.ThrowIfNull().RefreshSessionAsync(this.session, CancellationToken.None);

                result
                    .Switch(
                    s =>
                    {
                        this.SetSession(s);
                    },
                    e =>
                    {
                        this.logger?.LogError(e.ToString(), e);
                    });
            }
            else
            {
                this.logger?.LogInformation("Session is null, skipping refresh.");
            }
        }
        finally
        {
            Interlocked.Decrement(ref this.refreshing);
            this.logger?.LogDebug("Session refreshed.");
        }
    }
}