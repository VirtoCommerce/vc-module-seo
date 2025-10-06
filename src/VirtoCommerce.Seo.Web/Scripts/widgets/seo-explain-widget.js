angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainWidgetController', [
        '$scope', 'platformWebApp.bladeNavigationService',
        function ($scope, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.count = '';

            function initWidget() {
                // No data to load for this widget, but the function is here for structural consistency.
            }

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoExplainMain",
                    title: 'seo.blades.seo-explain-main.title',
                    controller: 'virtoCommerce.seo.seoExplainMainController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-main.html',
                    storeId: blade.currentEntityId
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
