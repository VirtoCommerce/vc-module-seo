angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainResultController', [
        '$scope',
        'virtoCommerce.seo.explainApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, explainApi, bladeNavigationService) {
            var blade = $scope.blade;
            blade.headIcon = 'fa fa-list';
            blade.isLoading = true;

            $scope.openStageDetails = function(stage) {
                var newBlade = {
                    id: 'seoExplainItems',
                    title: 'SEO Explain Items',
                    subtitle: stage.description,
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
                        displayName: 'Stage',
                        width: 220,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents" ng-click="grid.appScope.openStageDetails(row.entity)" style="cursor:pointer;">' +
                            '<strong>{{row.entity.stage}}</strong> ({{row.entity.itemsCount}})' +
                            '</div>',
                        headerTooltip: true
                    },
                    {
                        name: 'description',
                        displayName: 'Description',
                        headerTooltip: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" ng-click="grid.appScope.openStageDetails(row.entity)" style="cursor:pointer;">{{COL_FIELD}}</div>'
                    }
                ],
                data: []
            };

            explainApi.getExplain(blade.data)
                .then(function (data) {
                    (data || []).forEach(function (stage) {
                        stage.description = stage.description || '';
                        stage.itemsCount = (stage.seoExplainItems || []).length;
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
