angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainItemsController', [
        '$scope',
        function ($scope) {

            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-items.title';
            blade.headIcon = 'fa fa-table';
            blade.isLoading = true;

            var headerCellId = 'item-header-cell-id';

            $scope.gridOptions = {
                enableColumnMenus: false,
                rowHeight: 32,
                columnDefs: [
                    {
                        name: 'seoInfo.pageTitle',
                        displayName: 'seo.blades.seo-explain-items.labels.page-title',
                        headerCellTemplate: headerCellId,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.semanticUrl',
                        displayName: 'seo.blades.seo-explain-items.labels.semantic-url',
                        headerCellTemplate: headerCellId,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.objectType',
                        displayName: 'seo.blades.seo-explain-items.labels.object-type',
                        headerCellTemplate: headerCellId,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.languageCode',
                        displayName: 'seo.blades.seo-explain-items.labels.language',
                        headerCellTemplate: headerCellId,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.storeId',
                        displayName: 'seo.blades.seo-explain-items.labels.store',
                        headerCellTemplate: headerCellId,
                        headerTooltip: true
                    },
                    {
                        name: 'score',
                        displayName: 'seo.blades.seo-explain-items.labels.score',
                        headerCellTemplate: headerCellId,
                        type: 'number',
                        headerTooltip: true
                    },
                    {
                        name: 'objectTypePriority',
                        displayName: 'seo.blades.seo-explain-items.labels.priority',
                        headerCellTemplate: headerCellId,
                        type: 'number',
                        headerTooltip: true
                    }
                ],
                data: blade.items
            };

            $scope.gridOptions.data = blade.items;

            blade.isLoading = false;
        }
    ]);
