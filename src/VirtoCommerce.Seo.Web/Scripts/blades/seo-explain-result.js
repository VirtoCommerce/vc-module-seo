angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainResultController', [
        '$scope',
        '$filter',
        'virtoCommerce.seo.explainApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, $filter, explainApi, bladeNavigationService) {
            var blade = $scope.blade;
            blade.headIcon = 'fa fa-list';
            blade.isLoading = true;

            $scope.openStageDetails = function(stage) {
                var newBlade = {
                    id: 'seoExplainItems',
                    title: 'seo.blades.seo-explain-items.title',
                    subtitle: stage.translatedDescription,
                    controller: 'virtoCommerce.seo.seoExplainItemsController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-items.html',
                    data: stage.seoExplainItems
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
                        headerCellTemplate: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result-header.html',
                        headerTooltip: true,
                        cellTemplate: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result-cell.html'
                    }
                ],
                data: []
            };

            explainApi.explain(blade.data).$promise
                .then(function (data) {
                    (data || []).forEach(function (stage) {
                        stage.itemsCount = (stage.items || []).length;
                        stage.descriptionKey = 'seo.blades.seo-explain-result.descriptions.' + stage.stage;
                        stage.translatedDescription = $filter('translate')(stage.descriptionKey);
                    });

                    $scope.stageGridOptions.data = (data || []).filter(function (s) {
                        return s.itemsCount > 0;
                    });
                })
                .finally(function () {
                    blade.isLoading = false;
                });
        }
    ]);
