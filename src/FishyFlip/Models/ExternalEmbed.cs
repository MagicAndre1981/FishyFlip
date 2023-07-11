﻿// <copyright file="ExternalEmbed.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace FishyFlip.Models;

public class ExternalEmbed : Embed
{
    public ExternalEmbed(CBORObject obj)
    {
        this.Type = Constants.EmbedTypes.External;
        if (obj["thumb"] is not null)
        {
            this.Thumb = new Image(obj["thumb"]);
        }

        this.Uri = obj["uri"].AsString();
        this.Title = obj["title"].AsString();
        this.Description = obj["description"].AsString();
    }

    public Image? Thumb { get; }

    public string? Title { get; }

    public string? Description { get; }

    public string? Uri { get; }
}