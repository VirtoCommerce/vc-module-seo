angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainItemsController', [
        '$scope',
        function ($scope) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-items.title';
            blade.headIcon = 'fa fa-table';
            blade.isLoading = false;

            $scope.gridOptions = {
                data: 'blade.items',
                enableColumnMenus: false,
                rowHeight: 40
            };
        }
    ]);
