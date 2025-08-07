using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

namespace VirtoCommerce.Seo.Data.ExportImport;

public sealed class BrokenLinksExportImport(
    IBrokenLinkSearchService brokenLinkSearchService,
    IBrokenLinkService brokenLinkService,
    JsonSerializer jsonSerializer)
{

    public async Task ExportAsync(Stream outStream, ExportImportOptions options,
        Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
        progressCallback(progressInfo);

        await using var sw = new StreamWriter(outStream, Encoding.UTF8);
        await using var writer = new JsonTextWriter(sw);

        await writer.WriteStartObjectAsync();

        progressInfo.Description = "Broken links exporting...";
        progressCallback(progressInfo);

        var links = await brokenLinkSearchService.SearchAsync(new BrokenLinkSearchCriteria { Take = 0 });
        var linksCount = links.TotalCount;
        await writer.WritePropertyNameAsync("BrokenLinksTotalCount");
        await writer.WriteValueAsync(linksCount);

        cancellationToken.ThrowIfCancellationRequested();

        await writer.WritePropertyNameAsync("BrokenLinks");
        await writer.WriteStartArrayAsync();

        const int batchSize = 100;

        for (var i = 0; i < linksCount; i += batchSize)
        {
            var searchResponse = await brokenLinkSearchService.SearchAsync(new BrokenLinkSearchCriteria { Skip = i, Take = batchSize });
            foreach (var member in searchResponse.Results)
            {
                jsonSerializer.Serialize(writer, member);
            }
            await writer.FlushAsync();
            progressInfo.Description = $"{Math.Min(linksCount, i + batchSize)} of {linksCount} broken links exported";
            progressCallback(progressInfo);
        }
        await writer.WriteEndArrayAsync();

        await writer.WriteEndObjectAsync();
        await writer.FlushAsync();
    }

    public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var progressInfo = new ExportImportProgressInfo();
        var linksTotalCount = 0;

        const int batchSize = 100;

        using var streamReader = new StreamReader(inputStream);
        await using var reader = new JsonTextReader(streamReader);
        while (await reader.ReadAsync())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var readerValueString = reader.Value?.ToString();

                if (readerValueString.EqualsIgnoreCase("BrokenLinksTotalCount"))
                {
                    linksTotalCount = await reader.ReadAsInt32Async() ?? 0;
                }
                else if (readerValueString.EqualsIgnoreCase("BrokenLinks"))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await reader.ReadAsync();
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        await reader.ReadAsync();

                        var brokenLinks = new List<BrokenLink>();
                        var linksCount = 0;
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            var brokenLink = jsonSerializer.Deserialize<BrokenLink>(reader);
                            brokenLinks.Add(brokenLink);
                            linksCount++;

                            await reader.ReadAsync();
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        for (var i = 0; i < linksCount; i += batchSize)
                        {
                            await brokenLinkService.SaveChangesAsync(brokenLinks.Skip(i).Take(batchSize).ToArray());

                            progressInfo.Description = linksTotalCount > 0
                                ? $"{i} of {linksTotalCount} broken links imported"
                                : $"{i} broken links imported";

                            progressCallback(progressInfo);
                        }
                    }
                }
            }
        }
    }
}

