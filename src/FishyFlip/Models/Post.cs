﻿// <copyright file="Post.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Org.BouncyCastle.Ocsp;

namespace FishyFlip.Models;

/// <summary>
/// Post.
/// </summary>
public class Post : ATRecord
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Post"/> class.
    /// </summary>
    public Post(CBORObject obj)
    {
        this.Type = Constants.FeedType.Post;
        if (!obj["embed"]?.IsNull ?? false)
        {
            this.Embed = obj["embed"].ToEmbed();
        }

        this.CreatedAt = obj["createdAt"].ToDateTime();
        this.Text = obj["text"].AsString();
        this.Langs = obj["langs"]?.Values.Select(n => n.AsString()).ToArray();
        this.Facets = obj["facets"]?.Values.Select(n => new Facet(n)).ToArray();
    }

    /// <summary>
    /// Gets the Embed.
    /// </summary>
    public Embed? Embed { get; }

    public Facet[]? Facets { get; }

    public DateTime? CreatedAt { get; }

    public string Text { get; }

    public string[]? Langs { get; }
}