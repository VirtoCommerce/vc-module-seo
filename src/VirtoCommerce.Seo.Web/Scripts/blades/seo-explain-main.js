angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainMainController', [
        '$scope',
        'platformWebApp.bladeNavigationService',
        function ($scope, bladeNavigationService) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-main.title';
            blade.headIcon = 'fa fa-search';
            blade.isLoading = false;

            var store = blade.store;

            $scope.languages = store.languages || [];

            blade.currentEntity = {
                storeId: store.id,
                storeName: store.name,
                storeDefaultLanguage: store.defaultLanguage || null,
                languageCode: store.defaultLanguage,
                permalink: ''
            };

            $scope.explain = function () {
                var params = angular.copy(blade.currentEntity);

                var resultBlade = {
                    id: 'seoExplainResultBlade',
                    controller: 'virtoCommerce.seo.seoExplainResultController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result.html',
                    data: params
                };
                bladeNavigationService.showBlade(resultBlade, blade);
            };
        }
    ]);
