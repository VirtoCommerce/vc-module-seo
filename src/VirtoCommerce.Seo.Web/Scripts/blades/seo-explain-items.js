angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainItemsController', [
        '$scope',
        function ($scope) {

            var blade = $scope.blade;
            blade.headIcon = 'fa fa-table';
            blade.isLoading = true;

            var translatedHeaderCellTemplate = '<div class="ui-grid-cell-contents">{{ col.displayName | translate }}</div>';

            $scope.gridOptions = {
                enableColumnMenus: false,
                enableSorting: true,
                rowHeight: 32,
                columnDefs: [
                    {
                        name: 'seoInfo.pageTitle',
                        displayName: 'seo.blades.seo-explain-items.labels.page-title',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.semanticUrl',
                        displayName: 'seo.blades.seo-explain-items.labels.semantic-url',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.objectType',
                        displayName: 'seo.blades.seo-explain-items.labels.object-type',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.languageCode',
                        displayName: 'seo.blades.seo-explain-items.labels.language',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        headerTooltip: true
                    },
                    {
                        name: 'seoInfo.storeId',
                        displayName: 'seo.blades.seo-explain-items.labels.store',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        headerTooltip: true
                    },
                    {
                        name: 'score',
                        displayName: 'seo.blades.seo-explain-items.labels.score',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        type: 'number',
                        headerTooltip: true
                    },
                    {
                        name: 'objectTypePriority',
                        displayName: 'seo.blades.seo-explain-items.labels.priority',
                        headerCellTemplate: translatedHeaderCellTemplate,
                        type: 'number',
                        headerTooltip: true
                    }
                ],
                data: []
            };

            var items = (blade.data || []).map(function (item) {
                item.seoInfo = item.seoInfo || {};
                return item;
            });
            $scope.gridOptions.data = items;
            blade.isLoading = false;
        }
    ]);
