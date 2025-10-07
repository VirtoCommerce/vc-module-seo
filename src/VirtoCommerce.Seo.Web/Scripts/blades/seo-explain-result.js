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
                    subtitle: $filter('translate')(stage.descriptionKey),
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
                        headerCellTemplate: '<div class="ui-grid-cell-contents">{{ col.displayName | translate }}</div>',
                        headerTooltip: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" ng-click="grid.appScope.openStageDetails(row.entity)" style="cursor:pointer;">{{row.entity.descriptionKey | translate}} ({{row.entity.itemsCount}})</div>'
                    }
                ],
                data: []
            };

            explainApi.explain(blade.data).$promise
                .then(function (data) {
                    (data || []).forEach(function (stage) {
                        stage.itemsCount = (stage.seoExplainItems || []).length;
                        var stageKey = stage.stage;
                        if (stageKey === 'FilteredScore') {
                            stageKey = 'filtered-score';
                        } else {
                            stageKey = stageKey.toLowerCase();
                        }
                        stage.descriptionKey = 'seo.blades.seo-explain-result.descriptions.' + stageKey;
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
