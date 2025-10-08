angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainItemsController', [
        '$scope',
        function ($scope) {

            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-items.title';
            blade.headIcon = 'fa fa-table';
            blade.isLoading = true;

            $scope.gridOptions = {
                enableColumnMenus: false,
                rowHeight: 32,
                columnDefs: [
                    {
                        name: 'seoInfo.pageTitle',
                        displayName: 'seo.blades.seo-explain-items.labels.page-title',
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.semanticUrl',
                        displayName: 'seo.blades.seo-explain-items.labels.semantic-url',
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.objectType',
                        displayName: 'seo.blades.seo-explain-items.labels.object-type',
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.languageCode',
                        displayName: 'seo.blades.seo-explain-items.labels.language',
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.storeId',
                        displayName: 'seo.blades.seo-explain-items.labels.store',
                        headerTooltip: true
                    },
                    {
                        name: 'score',
                        displayName: 'seo.blades.seo-explain-items.labels.score',
                        type: 'number',
                        headerTooltip: true
                    },
                    {
                        name: 'objectTypePriority',
                        displayName: 'seo.blades.seo-explain-items.labels.priority',
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
