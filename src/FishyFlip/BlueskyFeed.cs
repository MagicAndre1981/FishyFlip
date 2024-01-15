﻿// <copyright file="BlueskyFeed.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace FishyFlip;

public sealed class BlueskyFeed
{
    private ATProtocol proto;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlueskyFeed"/> class.
    /// </summary>
    /// <param name="proto"><see cref="ATProtocol"/>.</param>
    internal BlueskyFeed(ATProtocol proto)
    {
        this.proto = proto;
    }

    private ATProtocolOptions Options => this.proto.Options;

    private HttpClient Client => this.proto.Client;

    public async Task<Result<ThreadPostViewFeed>> GetPostThreadAsync(
       ATUri uri,
       int depth = 0,
       CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetPostThread}?uri={uri}";
        if (depth > 0)
        {
            url += $"&depth={depth}";
        }

        Multiple<ThreadPostViewFeed?, Error> result = await this.Client.Get<ThreadPostViewFeed>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<ThreadPostViewFeed>>(
                timeline => timeline!,
                error => error!);
    }

    public async Task<Result<RepostedFeed>> GetRepostedByAsync(
    ATUri uri,
    int limit = 50,
    Cid? cid = default,
    string? cursor = default,
    CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetRepostedBy}?uri={uri.ToString()}&limit={limit}";

        if (cid is not null)
        {
            url += $"&cid={cid}";
        }

        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        Multiple<RepostedFeed?, Error> result = await this.Client.Get<RepostedFeed>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<RepostedFeed>>(
                timeline => (timeline ?? new RepostedFeed(Array.Empty<FeedProfile>(), null))!,
                error => error!);
    }

    public async Task<Result<LikesFeed>> GetLikesAsync(ATUri uri, int limit = 50, Cid? cid = default, string? cursor = default, CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetLikes}?uri={uri.ToString()}&limit={limit}";

        if (cid is not null)
        {
            url += $"&cid={cid}";
        }

        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        Multiple<LikesFeed?, Error> result = await this.Client.Get<LikesFeed>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<LikesFeed>>(
                timeline => (timeline ?? new LikesFeed(Array.Empty<Like>(), null))!,
                error => error!);
    }

    public async Task<Result<ListFeed>> GetListFeedAsync(ATUri uri, int limit = 30, CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetListFeed}?list={uri.ToString()}&limit={limit}";

        Multiple<ListFeed?, Error> result = await this.Client.Get<ListFeed>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<ListFeed>>(
                timeline => (timeline ?? new ListFeed(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }

    public async Task<Result<Timeline>> GetAuthorFeedAsync(ATIdentifier handle, int limit = 50, string? cursor = default, CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetAuthorFeed}?actor={handle.ToString()}&limit={limit}";
        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        Multiple<Timeline?, Error> result = await this.Client.Get<Timeline>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<Timeline>>(
                authorFeed => (authorFeed ?? new Timeline(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }

    public async Task<Result<PostCollection>> GetPostsAsync(IEnumerable<ATUri> query, CancellationToken cancellationToken = default)
    {
        var answer = string.Join("&", query.Select(n => $"uris={n}"));
        string url = $"{Constants.Urls.Bluesky.Feed.GetPosts}?{answer}";
        Multiple<PostCollection?, Error> result = await this.Client.Get<PostCollection>(url, this.Options.JsonSerializerOptions, cancellationToken);
        return result
            .Match<Result<PostCollection>>(
                timeline => (timeline ?? new PostCollection(new PostView[0]))!,
                error => error!);
    }

    public async Task<Result<Timeline>> GetTimelineAsync(int limit = 50, string? cursor = default, string algorithm = "reverse-chronological", CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetTimeline}?algorithm={algorithm}&limit={limit}";
        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        Multiple<Timeline?, Error> result = await this.Client.Get<Timeline>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<Timeline>>(
                timeline => (timeline ?? new Timeline(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }

    public async Task<Result<FeedPostList>> GetFeedAsync(ATUri uri, int limit = 30, string? cursor = default, CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetFeed}?feed={uri}&limit={limit}";
        if (cursor is not null)
        {
            url += $"&cursor={cursor}";
        }

        Multiple<FeedPostList?, Error> result = await this.Client.Get<FeedPostList>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<FeedPostList>>(
                timeline => (timeline ?? new FeedPostList(Array.Empty<FeedViewPost>(), null))!,
                error => error!);
    }

    public async Task<Result<FeedGeneratorRecord>> GetFeedGeneratorAsync(ATUri uri, CancellationToken cancellationToken = default)
    {
        string url = $"{Constants.Urls.Bluesky.Feed.GetFeedGenerator}?feed={uri}";

        Multiple<FeedGeneratorRecord?, Error> result = await this.Client.Get<FeedGeneratorRecord>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<FeedGeneratorRecord>>(
                timeline => timeline!,
                error => error!);
    }

    public async Task<Result<FeedCollection>> GetFeedGeneratorsAsync(IEnumerable<ATUri> query, CancellationToken cancellationToken = default)
    {
        var answer = string.Join("&", query.Select(n => $"feeds={n}"));
        string url = $"{Constants.Urls.Bluesky.Feed.GetFeedGenerators}?{answer}";

        Multiple<FeedCollection?, Error> result = await this.Client.Get<FeedCollection>(url, this.Options.JsonSerializerOptions, cancellationToken, this.Options.Logger);
        return result
            .Match<Result<FeedCollection>>(
                timeline => timeline!,
                error => error!);
    }
}
