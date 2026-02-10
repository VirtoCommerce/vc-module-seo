# VirtoCommerce SEO Module

[![CI status](https://github.com/VirtoCommerce/vc-module-seo/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-seo/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-seo&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-seo) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-seo&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-seo) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-seo&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-seo) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-seo&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-seo)

## Overview

The VirtoCommerce SEO module provides centralized infrastructure for managing SEO metadata across the platform. It handles permalink resolution through a multi-stage scoring pipeline, URL redirect rules (static and regex-based), and automatic broken link detection via event-driven background jobs. The module supports multiple database providers and integrates into the Virto Commerce platform as a standalone extension with its own REST API, export/import capabilities, and Admin UI widgets.

## Key Features

* **SEO Metadata Resolution** — Resolves SEO records (slugs, page titles, meta descriptions, keywords) per entity, store, language, and organization using a composite resolver that aggregates multiple `ISeoResolver` implementations.
* **Best-Match Scoring Pipeline** — A 6-stage pipeline (Original, Filtered, Scored, FilteredScore, Ordered, Final) selects the most relevant SEO entry when multiple candidates match a permalink, with configurable object type priorities.
* **Redirect Rules** — Static or regex-based URL rewrite rules with priority ordering and capture group substitution (`$1`, `$2`, etc.), validated via FluentValidation.
* **Broken Link Detection** — Event-driven tracking of unresolved permalinks. When no SEO info is found, a `SeoInfoNotFoundEvent` triggers a Hangfire background job that records or updates the broken link with hit count and timestamp.
* **SEO Duplicates Detection** — Extensible `ISeoDuplicatesDetector` interface for identifying conflicting SEO entries within an object's relationships (default implementation is a no-op).
* **Explain / Debug Tool** — REST API endpoint and Admin UI widget that traces the full permalink resolution pipeline stage by stage for diagnostics.
* **Export / Import** — Streaming JSON-based export and import of redirect rules and broken links with batch processing (100 items per batch) and progress reporting.
* **Multi-Database Support** — EF Core providers for SQL Server, MySQL, and PostgreSQL with version-controlled migrations.

## Configuration

### Application Settings

| Setting | Type | Default | Description |
|---|---|---|---|
| `Seo.Enabled` | Boolean | `true` | Enables or disables the SEO module globally |
| `Seo.BrokenLinkDetection.Enabled` | Boolean | `true` | Enables or disables automatic broken link recording when SEO info is not found |

**Object Type Priority (appsettings.json):**

| Setting | Type | Default | Description |
|---|---|---|---|
| `Seo:SeoInfoResolver:ObjectTypePriority` | `string[]` | `["CatalogProduct", "Category", "Catalog", "Brand", "Brands", "ContentFile", "Pages"]` | Configures the priority order for resolving SEO entries when multiple object types match. The last element has the highest priority. |

### Permissions

| Permission | Description |
|---|---|
| `seo:access` | Access the SEO module |
| `seo:create` | Create SEO records, broken links, and redirect rules |
| `seo:read` | Read SEO records, broken links, redirect rules, and duplicates |
| `seo:update` | Update SEO records, broken links, and redirect rules |
| `seo:delete` | Delete broken links and redirect rules |

## Architecture

### Key Flow

1. A client sends a request to resolve a permalink (e.g., via `POST /api/seoinfos/search` or a GraphQL query).
2. `CompositeSeoResolver` fans out the search criteria to all registered `ISeoResolver` implementations in parallel and merges the results.
3. If no SEO info is found, a `SeoInfoNotFoundEvent` is published.
4. `SeoInfoNotFoundEventHandler` receives the event, checks the `BrokenLinkDetection.Enabled` setting, and enqueues a Hangfire background job to create or update a `BrokenLink` record (incrementing `HitCount` and updating `LastHitDate`).
5. If SEO info records are found, the best-match scoring pipeline in `SeoExtensions.GetBestMatchingSeoInfo` is applied:
   - **Stage 1 (Original):** Snapshot of all returned candidates.
   - **Stage 2 (Filtered):** Filter by store, organization, and language.
   - **Stage 3 (Scored):** Calculate numeric score based on IsActive, StoreId match, OrganizationId match, and language match; assign object type priority from the configured priority list.
   - **Stage 4 (FilteredScore):** Remove candidates with non-positive scores.
   - **Stage 5 (Ordered):** Sort by score descending, then by object type priority descending.
   - **Stage 6 (Final):** Select the single best-match candidate.
6. For redirect resolution, `RedirectResolver` searches active redirect rules by store, sorted by priority descending, and evaluates each rule (static exact match or regex match with group substitution).

## Components

### Projects

| Project | Layer | Purpose |
|---|---|---|
| VirtoCommerce.Seo.Core | Core | Domain models, service interfaces, events, extensions, and module constants |
| VirtoCommerce.Seo.Data | Data | Service implementations, EF Core repositories, DbContext, event handlers, validation, and export/import |
| VirtoCommerce.Seo.Data.SqlServer | DB Provider | EF Core migrations and design-time factory for SQL Server |
| VirtoCommerce.Seo.Data.MySql | DB Provider | EF Core migrations and design-time factory for MySQL |
| VirtoCommerce.Seo.Data.PostgreSql | DB Provider | EF Core migrations and design-time factory for PostgreSQL |
| VirtoCommerce.Seo.Web | Web | ASP.NET Core module entry point, REST API controllers, and localizations (de, en, es, fi, fr, it, ja, no, pl, pt, ru, sv, zh) |
| VirtoCommerce.Seo.Tests | Tests | Unit tests |

### Key Services

| Service | Interface | Responsibility |
|---|---|---|
| `CompositeSeoResolver` | `ICompositeSeoResolver` | Aggregates multiple `ISeoResolver` implementations, merges results, and publishes `SeoInfoNotFoundEvent` when no records are found |
| `RedirectResolver` | `IRedirectResolver` | Resolves URL redirects by evaluating active redirect rules (static or regex) sorted by priority |
| `SeoExplainService` | `ISeoExplainService` | Produces stage-by-stage explain snapshots of the SEO resolution pipeline for diagnostics |
| `BrokenLinkService` | `IBrokenLinkService` | CRUD operations for broken link records with platform memory caching and domain events |
| `BrokenLinkSearchService` | `IBrokenLinkSearchService` | Search broken links by keyword, permalink, store, language, and status |
| `RedirectRuleService` | `IRedirectRuleService` | CRUD operations for redirect rules with FluentValidation and domain events |
| `RedirectRuleSearchService` | `IRedirectRuleSearchService` | Search redirect rules by keyword, store, and active status |
| `NullSeoDuplicateDetector` | `ISeoDuplicatesDetector` | Default no-op implementation of duplicate detection (returns empty results) |
| `SeoInfoNotFoundEventHandler` | `IEventHandler<SeoInfoNotFoundEvent>` | Handles broken link recording via Hangfire background job when SEO lookup returns no results |
| `SeoExportImport` | — | Streaming JSON export/import of redirect rules and broken links with batch processing |
| `RedirectRuleValidator` | `AbstractValidator<RedirectRule>` | Validates redirect rules: inbound regex correctness, outbound capture group consistency, and required StoreId |

### REST API

#### SEO Info — Base route: `api/seoinfos`

| Method | Endpoint | Description |
|---|---|---|
| PUT | `api/seoinfos/batchupdate` | Batch create or update SEO info records (not yet implemented) |
| GET | `api/seoinfos/duplicates?objectId={objectId}&objectType={objectType}` | Detect SEO duplicates for a given object |
| GET | `api/seoinfos/{slug}` | Find all SEO records matching a slug |
| POST | `api/seoinfos/search` | Search SEO records by criteria (slug, storeId, permalink, organizationId) |
| GET | `api/seoinfos/explain?storeId={storeId}&organizationId={organizationId}&storeDefaultLanguage={lang}&languageCode={lang}&permalink={permalink}` | Trace the full SEO resolution pipeline (explain/debug) |

#### Broken Links — Base route: `api/seo/broken-links`

| Method | Endpoint | Description |
|---|---|---|
| GET | `api/seo/broken-links/{id}` | Get a broken link by ID |
| POST | `api/seo/broken-links/search` | Search broken links by criteria (keyword, permalink, storeId, status) |
| POST | `api/seo/broken-links` | Create a new broken link record |
| PUT | `api/seo/broken-links` | Update an existing broken link record |
| PATCH | `api/seo/broken-links/{id}` | Partially update a broken link via JSON Patch |
| DELETE | `api/seo/broken-links?ids={id1}&ids={id2}` | Delete broken links by IDs |

#### Redirect Rules — Base route: `api/seo/redirect-rules`

| Method | Endpoint | Description |
|---|---|---|
| GET | `api/seo/redirect-rules/{id}` | Get a redirect rule by ID |
| POST | `api/seo/redirect-rules/search` | Search redirect rules by criteria (keyword, storeId, isActive) |
| POST | `api/seo/redirect-rules` | Create a new redirect rule |
| PUT | `api/seo/redirect-rules` | Update an existing redirect rule |
| PATCH | `api/seo/redirect-rules/{id}` | Partially update a redirect rule via JSON Patch |
| DELETE | `api/seo/redirect-rules?ids={id1}&ids={id2}` | Delete redirect rules by IDs |

## Documentation

* [SEO module user documentation]([https://docs.virtocommerce.org/platform/user-guide/push-messages/overview/](https://docs.virtocommerce.org/platform/user-guide/seo/overview/))
* [GraphQL API documentation](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/Push-messages/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.PushMessages)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-seo/)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-seo/releases)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
