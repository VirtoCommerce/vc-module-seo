angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainItemsController', [
        '$scope', '$timeout',
        function ($scope, $timeout) {

            var blade = $scope.blade;
            blade.headIcon = 'fa fa-table';
            blade.isLoading = true;

            $scope.gridOptions = {
                enableColumnMenus: false,
                enableSorting: true,
                rowHeight: 32,
                columnDefs: [
                    { name: 'seoInfo.pageTitle', displayName: 'Page Title', headerTooltip: true },
                    { name: 'seoInfo.semanticUrl', displayName: 'Semantic URL', headerTooltip: true },
                    { name: 'seoInfo.objectType', displayName: 'Object Type', headerTooltip: true },
                    { name: 'seoInfo.languageCode', displayName: 'Language', headerTooltip: true },
                    { name: 'seoInfo.storeId', displayName: 'Store', headerTooltip: true },
                    { name: 'score', displayName: 'Score', type: 'number', headerTooltip: true },
                    { name: 'objectTypePriority', displayName: 'Priority', type: 'number', headerTooltip: true }
                ],
                data: []
            };

            $timeout(function () {
                var items = (blade.data || []).map(function (item) {
                    item.seoInfo = item.seoInfo || {};
                    return item;
                });
                $scope.gridOptions.data = items;
                blade.isLoading = false;
            });
        }
    ]);
