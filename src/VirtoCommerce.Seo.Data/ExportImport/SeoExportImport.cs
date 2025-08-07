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

public sealed class SeoExportImport(
    IRedirectRuleSearchService redirectRuleSearchService,
    IRedirectRuleService redirectRuleService,
    IBrokenLinkSearchService brokenLinkSearchService,
    IBrokenLinkService brokenLinkService,
    JsonSerializer jsonSerializer)
{

    public async Task ExportAsync(Stream outStream, ExportImportOptions options,
        Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var sw = new StreamWriter(outStream, Encoding.UTF8);
        await using var writer = new JsonTextWriter(sw);

        await ExportRedirectRules(writer, progressCallback, cancellationToken);
        await ExportBrokenLinks(writer, progressCallback, cancellationToken);
    }

    public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var streamReader = new StreamReader(inputStream);
        await using var reader = new JsonTextReader(streamReader);

        await ImportRedirectRules(reader, progressCallback, cancellationToken);
        await ImportBrokenLinks(reader, progressCallback, cancellationToken);
    }

    private async Task ExportRedirectRules(JsonTextWriter writer, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
        progressCallback(progressInfo);
        await writer.WriteStartObjectAsync();

        progressInfo.Description = "Redirect rules exporting...";
        progressCallback(progressInfo);

        var rules = await redirectRuleSearchService.SearchAsync(new RedirectRuleSearchCriteria { Take = 0 });
        var rulesCount = rules.TotalCount;
        await writer.WritePropertyNameAsync("RedirectRulesTotalCount");
        await writer.WriteValueAsync(rulesCount);

        cancellationToken.ThrowIfCancellationRequested();

        await writer.WritePropertyNameAsync("RedirectRules");
        await writer.WriteStartArrayAsync();

        const int batchSize = 100;

        for (var i = 0; i < rulesCount; i += batchSize)
        {
            var searchResponse = await redirectRuleSearchService.SearchAsync(new RedirectRuleSearchCriteria { Skip = i, Take = batchSize });
            foreach (var rule in searchResponse.Results)
            {
                jsonSerializer.Serialize(writer, rule);
            }
            await writer.FlushAsync();
            progressInfo.Description = $"{Math.Min(rulesCount, i + batchSize)} of {rulesCount} redirect rules exported";
            progressCallback(progressInfo);
            cancellationToken.ThrowIfCancellationRequested();
        }
        await writer.WriteEndArrayAsync();

        await writer.WriteEndObjectAsync();
        await writer.FlushAsync();

    }

    private async Task ExportBrokenLinks(JsonTextWriter writer, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
        progressCallback(progressInfo);
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

    private async Task ImportRedirectRules(JsonTextReader reader, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
    {
        var progressInfo = new ExportImportProgressInfo();
        var rulesTotalCount = 0;

        const int batchSize = 100;
        while (await reader.ReadAsync())
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var readerValueString = reader.Value?.ToString();

                if (readerValueString.EqualsIgnoreCase("RedirectRulesTotalCount"))
                {
                    rulesTotalCount = await reader.ReadAsInt32Async() ?? 0;
                }
                else if (readerValueString.EqualsIgnoreCase("RedirectRules"))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await reader.ReadAsync();
                    if (reader.TokenType == JsonToken.StartArray)
                    {
                        await reader.ReadAsync();

                        var redirectRules = new List<RedirectRule>();
                        var rulesCount = 0;
                        while (reader.TokenType != JsonToken.EndArray)
                        {
                            var redirectRule = jsonSerializer.Deserialize<RedirectRule>(reader);
                            redirectRules.Add(redirectRule);
                            rulesCount++;

                            await reader.ReadAsync();
                        }

                        cancellationToken.ThrowIfCancellationRequested();

                        for (var i = 0; i < rulesCount; i += batchSize)
                        {
                            await redirectRuleService.SaveChangesAsync(redirectRules.Skip(i).Take(batchSize).ToArray());

                            progressInfo.Description = rulesTotalCount > 0
                                ? $"{i} of {rulesTotalCount} redirect rules imported"
                                : $"{i} redirect rules imported";

                            progressCallback(progressInfo);
                        }
                    }
                }
            }
        }
    }

    private async Task ImportBrokenLinks(JsonTextReader reader, Action<ExportImportProgressInfo> progressCallback,
        ICancellationToken cancellationToken)
    {
        var progressInfo = new ExportImportProgressInfo();
        var linksTotalCount = 0;

        const int batchSize = 100;

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
