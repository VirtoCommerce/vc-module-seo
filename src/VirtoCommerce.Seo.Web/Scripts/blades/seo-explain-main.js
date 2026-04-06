angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainMainController', [
        '$scope',
        '$injector',
        'platformWebApp.bladeNavigationService',
        function ($scope, $injector, bladeNavigationService) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-main.title';
            blade.headIcon = 'fa fa-search';
            blade.isLoading = false;

            var store = blade.store;

            $scope.languages = store.languages || [];

			// MVP approach. seo module should not know about the customer module
			// will be changed in the future release
            $scope.organizations = [];
            $scope.organizationsAvailable = false;

            var organizationsApiName = 'virtoCommerce.customerModule.organizations';
            var organizationsApi = $injector.has(organizationsApiName)
                ? $injector.get(organizationsApiName)
                : null;

            if (organizationsApi) {
                $scope.organizationsAvailable = true;
                organizationsApi.search({}, function (data) {
                    $scope.organizations = data.results;
                });
            }

            blade.currentEntity = {
                storeId: store.id,
                storeName: store.name,
                storeDefaultLanguage: store.defaultLanguage,
                languageCode: store.defaultLanguage,
                organizationId: null,
                permalink: ''
            };

            $scope.explain = function () {
                var params = angular.copy(blade.currentEntity);
                params.organizationId = params.organizationId?.id || null;

                var newBlade = {
                    id: 'seoExplainResultBlade',
                    controller: 'virtoCommerce.seo.seoExplainResultController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-result.html',
                    data: params
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };
        }
    ]);
