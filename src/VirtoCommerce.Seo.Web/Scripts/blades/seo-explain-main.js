angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainMainController', [
        '$scope',
        '$http',
        'platformWebApp.bladeNavigationService',
        'virtoCommerce.storeModule.stores',
        function ($scope, $http, bladeNavigationService, stores) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-main.title';
            blade.headIcon = 'fa fa-search';
            blade.isLoading = false;

            // Form model
            blade.currentEntity = {
                storeId: blade.storeId,
                storeName: null,
                storeDefaultLanguage: null,
                languageCode: null,
                permalink: ''
            };

            // Load store
            stores.get({ id: blade.storeId }, function (store) {
                blade.currentEntity.storeName = store.name;
                $scope.languages = store.languages || [];
                blade.currentEntity.storeDefaultLanguage = store.defaultLanguage || null;

                if (!blade.currentEntity.languageCode && store.defaultLanguage) {
                    blade.currentEntity.languageCode = store.defaultLanguage;
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
