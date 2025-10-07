angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainWidgetController', [
        '$scope', 'platformWebApp.bladeNavigationService',
        function ($scope, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.count = '';

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoExplainMain",
                    controller: 'virtoCommerce.seo.seoExplainMainController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-main.html',
                    storeId: blade.currentEntityId
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };
        }]);
