angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainWidgetController', [
        '$scope', 'platformWebApp.bladeNavigationService',
        function ($scope, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoExplainMain",
                    controller: 'virtoCommerce.seo.seoExplainMainController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-main.html',
                    store: blade.currentEntity
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };
        }]);
