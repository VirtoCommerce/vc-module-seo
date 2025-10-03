angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainMainController', [
        '$scope',
        '$http',
        'platformWebApp.bladeNavigationService',
        'virtoCommerce.storeModule.stores',
        function ($scope, $http, bladeNavigationService, stores) {
            var blade = $scope.blade;
            blade.title = 'SEO Explain';
            blade.headIcon = 'fa fa-search';
            blade.isLoading = false;

            // Form model
            blade.currentEntity = {
                storeId: null,
                storeDefaultLanguage: null,
                languageCode: null,
                permalink: ''
            };

            // Load stores
            $scope.stores = [];
            $scope.languages = [];

            stores.query({}, function (data) {
                $scope.stores = data || [];
            });

            // Update languages when store changes
            $scope.onStoreChange = function () {
                var store = ($scope.stores || []).find(function (s) { return s.id === blade.currentEntity.storeId; });
                if (store) {
                    $scope.languages = store.languages || [];
                    blade.currentEntity.storeDefaultLanguage = store.defaultLanguage || null;

                    // если язык ещё не выбран, подставляем дефолтный
                    if (!blade.currentEntity.languageCode && store.defaultLanguage) {
                        blade.currentEntity.languageCode = store.defaultLanguage;
                    }
                } else {
                    $scope.languages = [];
                    blade.currentEntity.storeDefaultLanguage = null;
                    blade.currentEntity.languageCode = null;
                }
            };


            // Call backend SeoController.explain
            $scope.explain = function () {
                var params = angular.copy(blade.currentEntity);

                var resultBlade = {
                    id: 'seoExplainResultBlade',
                    title: 'SEO Explain Result',
                    controller: 'virtoCommerce.seo.seoExplainResultController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result.html',
                    isClosingDisabled: false,
                    data: params
                };
                bladeNavigationService.showBlade(resultBlade, blade);
            };
        }
    ]);
