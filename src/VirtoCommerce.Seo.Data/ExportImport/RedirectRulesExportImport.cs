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
public sealed class RedirectRulesExportImport(
    IRedirectRuleSearchService redirectRuleSearchService,
    IRedirectRuleService redirectRulesService,
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
            foreach (var member in searchResponse.Results)
            {
                jsonSerializer.Serialize(writer, member);
            }
            await writer.FlushAsync();
            progressInfo.Description = $"{Math.Min(rulesCount, i + batchSize)} of {rulesCount} redirect rules exported";
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
        var rulesTotalCount = 0;

        const int batchSize = 100;

        using var streamReader = new StreamReader(inputStream);
        await using var reader = new JsonTextReader(streamReader);
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
                            await redirectRulesService.SaveChangesAsync(redirectRules.Skip(i).Take(batchSize).ToArray());

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
}
