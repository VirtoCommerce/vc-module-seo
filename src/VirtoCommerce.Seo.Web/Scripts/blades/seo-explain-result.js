angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainResultController', [
        '$scope',
        '$filter',
        'virtoCommerce.seo.explainApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, $filter, explainApi, bladeNavigationService) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-result.title';
            blade.headIcon = 'fa fa-list';
            blade.isLoading = true;

            $scope.openStageDetails = function (stage) {
                var newBlade = {
                    id: 'seoExplainItems',
                    controller: 'virtoCommerce.seo.seoExplainItemsController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-items.html',
                    stageDescription: stage.translatedDescription,
                    items: stage.items
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            $scope.stageGridOptions = {
                appScopeProvider: $scope,
                enableColumnMenus: false,
                enableSorting: false,

                rowHeight: 36,
                columnDefs: [
                    {
                        name: 'stage',
                        displayName: 'seo.blades.seo-explain-result.labels.stage',
                        headerCellTemplate: 'seo-explain-result-header-id',
                        headerTooltip: true,
                        cellTemplate: 'seo-explain-result-cell-id'
                    }
                ],
                data: []
            };

            explainApi.explain(blade.data).$promise
                .then(function (stages) {
                    (stages || []).forEach(function (stage) {
                        stage.itemsCount = (stage.items || []).length;
                        stage.descriptionKey = 'seo.blades.seo-explain-result.descriptions.' + stage.stage;
                        stage.translatedDescription = $filter('translate')(stage.descriptionKey);
                    });

                    $scope.stageGridOptions.data = (stages || []).filter(function (s) {
                        return s.itemsCount > 0;
                    });
                })
                .finally(function () {
                    blade.isLoading = false;
                });
        }
    ]);
