angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainMainController', [
        '$scope',
        '$http',
        'platformWebApp.bladeNavigationService',
        'virtoCommerce.storeModule.stores',
        function ($scope, $http, bladeNavigationService, stores) {
            var blade = $scope.blade;
            // blade.title is now set by the widget that opens this blade.
            blade.headIcon = 'fa fa-search';
            blade.isLoading = false;

            // Form model
            blade.currentEntity = {
                storeId: null,
                storeDefaultLanguage: null,
                languageCode: null,
                permalink: ''
            };

            // Update languages when store changes
            $scope.onStoreChange = function () {
                var store = ($scope.stores || []).find(function (s) { return s.id === blade.currentEntity.storeId; });
                if (store) {
                    $scope.languages = store.languages || [];
                    blade.currentEntity.storeDefaultLanguage = store.defaultLanguage || null;

                    if (!blade.currentEntity.languageCode && store.defaultLanguage) {
                        blade.currentEntity.languageCode = store.defaultLanguage;
                    }
                } else {
                    $scope.languages = [];
                    blade.currentEntity.storeDefaultLanguage = null;
                    blade.currentEntity.languageCode = null;
                }
            };

            // Load stores and pre-select if passed from widget
            stores.query({}, function (data) {
                $scope.stores = data || [];
                if (blade.storeId) {
                    blade.currentEntity.storeId = blade.storeId;
                    $scope.onStoreChange();
                }
            });

            // Call backend SeoController.explain
            $scope.explain = function () {
                var params = angular.copy(blade.currentEntity);

                var resultBlade = {
                    id: 'seoExplainResultBlade',
                    title: 'seo.blades.seo-explain-result.title',
                    controller: 'virtoCommerce.seo.seoExplainResultController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result.html',
                    isClosingDisabled: false,
                    data: params
                };
                bladeNavigationService.showBlade(resultBlade, blade);
            };
        }
    ]);
