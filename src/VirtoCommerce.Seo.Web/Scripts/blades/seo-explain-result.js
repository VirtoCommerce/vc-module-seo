angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainResultController', [
        '$scope',
        'platformWebApp.bladeNavigationService',
        'virtoCommerce.seo.explainApi',
        function ($scope, bladeNavigationService, explainApi) {
            var blade = $scope.blade;

            blade.title = 'seo.blades.seo-explain-result.title';
            blade.headIcon = 'fa fa-list';
            blade.isLoading = true;

            $scope.openStageDetails = function (stage) {
                var newBlade = {
                    id: 'seoExplainItems',
                    controller: 'virtoCommerce.seo.seoExplainItemsController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-items.html',
                    items: stage.items
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.gridStages = [];

            $scope.stageGridOptions = {
                data: 'gridStages',
                appScopeProvider: $scope,
                enableColumnMenus: false,
                enableColumnResizing: true,
                enableColumnMoving: true,
                enableSorting: false,
                rowHeight: 40
            };

            explainApi.explain(blade.data, function (stages) {
                blade.isLoading = false;
                $scope.gridStages = stages;
            });
        }
    ]);
