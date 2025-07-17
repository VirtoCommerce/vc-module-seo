# SEO Module

The **VirtoCommerce SEO Module** provides a flexible infrastructure for managing SEO-related data across the platform. It supports SEO metadata (e.g., slugs, titles, meta descriptions) for various entities such as catalogs, categories, products, and custom pages.

## Key Features

- **SEO Info Lookup**: Efficient retrieval of SEO records matching given criteria such as `permalink`, `storeId`, `language`, and others.
- **Best Match Resolution**: Logic to determine the most relevant SEO entry when multiple matches are found.
- **Duplicate Detection**: Extensible interface `ISeoDuplicatesDetector` for identifying and resolving conflicting SEO entries.
- **Broken Links Detection and management** : Identify and report dead or misconfigured SEO links.
- **Custom rewrite rules**: detect inbound requests by rules and return redirectUrl.

## Configuration

Currently, the only configurable setting is the priority order for resolving SEO entries when multiple matches exist for a given permalink.

Use the following configuration key:

```
Seo:SeoInfoResolver:ObjectTypePriority
```

### Default value:

```
"Pages", "ContentFile", "Catalog", "Category", "CatalogProduct"
```

This setting defines the precedence for resolving SEO entries. For example, if both a `Page` and a `Category` are associated with the same permalink, the system will prioritize and display the `Page` because it has higher priority in the list.

## Important note

The SEO module does not affect server-side routing or HTTP status codes (such as 301 redirects). In a storefrontless architecture, all incoming requests are handled the same way — the server always returns the initial page of the SPA frontend. Then, the frontend determines what to display using the getSlugInfo GraphQL query.

The SEO module affects the result of getSlugInfo, meaning it informs the frontend which page type should be shown for a given URL and whether a client-side redirect should happen (e.g., from `/old-url` to `/new-url`). However, it does not affect what the server returns, and cannot trigger server-side HTTP 301/302 redirects on its own.

If server-side redirects are required (with correct HTTP status codes), this currently needs to be handled at the frontend server level (e.g., via IIS, Nginx, or similar), or via edge middleware in the cloud (e.g., Azure Front Door, Cloudflare Workers, etc.).

Here is some info about it:

https://www.virtocommerce.org/t/virto-commerce-frontend-spa-architecture-for-seo-and-404-handling/793

## Rewrite rules

You can configure rewrite rules to intercept incoming requests and respond with a redirect to a new URL.

![UI for rewrite rules](docs/images/rewrite-rules.png)

These rules are evaluated in the `getSlugInfo` GraphQL query. They are checked **before** attempting to resolve a general SEO object. If a matching rule is found, the user will receive a response containing a `redirectUrl`.

## Future Enhancements

- Support for automatic detection and management of broken or orphaned SEO links
<!-- 
- UI for managing SEO priorities
- Integration with sitemap and robots.txt generation
-->

## Documentation links

## References links

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

<https://virtocommerce.com/open-source-license>

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
