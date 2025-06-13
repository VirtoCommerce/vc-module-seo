# SEO Module

The **VirtoCommerce SEO Module** provides a flexible infrastructure for managing SEO-related data across the platform. It supports SEO metadata (e.g., slugs, titles, meta descriptions) for various entities such as catalogs, categories, products, and custom pages.

## Key Features

- **SEO Info Lookup**: Efficient retrieval of SEO records matching given criteria such as `permalink`, `storeId`, `language`, and others.
- **Best Match Resolution**: Logic to determine the most relevant SEO entry when multiple matches are found.
- **Duplicate Detection**: Extensible interface `ISeoDuplicatesDetector` for identifying and resolving conflicting SEO entries.
- **Broken Links Detection and management** *(coming soon)*: Identify and report dead or misconfigured SEO links.

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
