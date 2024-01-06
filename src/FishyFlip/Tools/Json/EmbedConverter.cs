﻿// <copyright file="EmbedConverter.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace FishyFlip.Tools.Json;

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
public class EmbedConverter : JsonConverter<Embed>
{
    public override Embed? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            if (doc.RootElement.TryGetProperty("$type", out var type))
            {
                var text = type.GetString()?.Trim() ?? string.Empty;
                switch (text)
                {
                    case Constants.EmbedTypes.ImageView:
                        if (doc.RootElement.TryGetProperty("images", out var t))
                        {
                            var item = JsonSerializer.Deserialize<ImageViewEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.ExternalView:
                        if (doc.RootElement.TryGetProperty("external", out var evm))
                        {
                            var item = JsonSerializer.Deserialize<ExternalViewEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.Images:
                        if (doc.RootElement.TryGetProperty("images", out var img))
                        {
                            var item = JsonSerializer.Deserialize<ImagesEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.External:
                        if (doc.RootElement.TryGetProperty("external", out var ext))
                        {
                            var test = ext.GetRawText();
                            var item = JsonSerializer.Deserialize<ExternalEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.Record:
                        if (doc.RootElement.TryGetProperty("record", out var value))
                        {
                            var item = JsonSerializer.Deserialize<RecordEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.RecordView:
                        if (doc.RootElement.TryGetProperty("record", out var rec))
                        {
                            var item = JsonSerializer.Deserialize<RecordViewEmbed>(doc.RootElement.GetRawText(), options);
                            return item;
                        }

                        break;
                    case Constants.EmbedTypes.RecordWithMediaView:
                        RecordViewEmbed? record1 = null;
                        ImageViewEmbed? media1 = null;

                        if (doc.RootElement.TryGetProperty("record", out var recordVal2))
                        {
                            record1 = JsonSerializer.Deserialize<RecordViewEmbed>(recordVal2.GetRawText(), options);
                        }

                        if (doc.RootElement.TryGetProperty("media", out var mediaVal2))
                        {
                            if (mediaVal2.TryGetProperty("$type", out var mediaType2))
                            {
                                var mediaText = mediaType2.GetString()?.Trim() ?? string.Empty;
                                switch (mediaText)
                                {
                                    case Constants.EmbedTypes.ImageView:
                                        media1 = JsonSerializer.Deserialize<ImageViewEmbed>(mediaVal2.GetRawText(), options);
                                        break;
                                }
                            }
                        }

                        return new RecordWithMediaViewEmbed(record1, media1);
                    case Constants.EmbedTypes.RecordWithMedia:
                        RecordEmbed? record = null;
                        ImagesEmbed? media = null;

                        if (doc.RootElement.TryGetProperty("record", out var recordVal))
                        {
                            record = JsonSerializer.Deserialize<RecordEmbed>(recordVal.GetRawText(), options);
                        }

                        if (doc.RootElement.TryGetProperty("media", out var mediaVal))
                        {
                            if (mediaVal.TryGetProperty("$type", out var mediaType))
                            {
                                var mediaText = mediaType.GetString()?.Trim() ?? string.Empty;
                                switch (mediaText)
                                {
                                    case Constants.EmbedTypes.Images:
                                        media = JsonSerializer.Deserialize<ImagesEmbed>(mediaVal.GetRawText(), options);
                                        break;
                                }
                            }
                        }

                        return new RecordWithMediaEmbed(record, media);
                }
            }
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, Embed value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = options.TypeInfoResolver,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
            Converters =
            {
                new AtUriJsonConverter(),
                new AtHandlerJsonConverter(),
                new AtDidJsonConverter(),
                new ATRecordJsonConverter(),
                new CidConverter(),
            },
        });
    }
}

#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
