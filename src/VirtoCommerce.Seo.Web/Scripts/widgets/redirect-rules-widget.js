angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.redirectRulesWidgetController', [
        '$scope',
        'virtoCommerce.seo.redirectRulesApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, redirectRulesApi, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.count = '...';

            function initWidget() {
                const criteria = {
                    storeid: blade.currentEntityId,
                    isActive: true,
                    take: 0,
                };
                blade.currentEntityId && redirectRulesApi.search(criteria, function (result) {
                    $scope.count = result.totalCount;
                });
            }

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoRedirectRuleList",
                    controller: 'virtoCommerce.seo.redirectRuleListController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/redirect-rule-list.html',
                    refreshWidget: initWidget,
                    storeId: blade.currentEntityId,
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
