angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoWidgetController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.seo.seoApi',
            function ($scope, bladeNavigationService, seoApi) {
                const blade = $scope.blade;
                let promise;

                $scope.openSeoBlade = function () {
                    if (promise) {
                        promise.then(openBlade);
                    } else {
                        openBlade();
                    }
                };

                function openBlade(duplicates) {
                    const newBlade = {
                        id: "seoList",
                        title: blade.title,
                        duplicates: duplicates,
                        objectType: $scope.widget.objectType,
                        seoContainerObject: $scope.data,
                        fixedStoreId: $scope.widget.getFixedStoreId ? $scope.widget.getFixedStoreId(blade) : undefined,
                        defaultContainerId: $scope.widget.getDefaultContainerId(blade),
                        languages: $scope.widget.getLanguages(blade),
                        storeDataSource: $scope.widget.getStoreDataSource ? $scope.widget.getStoreDataSource(blade) : undefined,
                        updatePermission: blade.updatePermission,
                        controller: 'virtoCommerce.seo.seoListController',
                        template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-list.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                }

                $scope.$watch('data', function (data) {
                    if (data && $scope.widget.getDefaultContainerId(blade)) {
                        promise = seoApi.query({ objectId: data.id, objectType: $scope.widget.objectType }).$promise;
                        promise.then(function (promiseData) {
                            $scope.widget.UIclass = _.any(promiseData) ? 'error' : '';
                        });
                    }
                });
            }]);
