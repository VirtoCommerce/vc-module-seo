angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinksWidgetController', [
        '$scope',
        'virtoCommerce.seo.brokenLinksApi',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.settings',
        function ($scope, brokenLinksApi, bladeNavigationService, settings) {
            const blade = $scope.widget.blade;

            $scope.count = '...';

            function initWidget() {
                settings.getValues({ id: 'Seo.BrokenLinkDetection.Enabled' }, function (setting) {
                    const value = setting[0];
                    if (value) {
                        const criteria = {
                            storeid: blade.currentEntityId,
                            status: 'Active',
                            take: 0,
                        };
                        blade.currentEntityId && brokenLinksApi.search(criteria, function (result) {
                            $scope.count = result.totalCount;
                        });
                    }
                });
            }

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoBrokenLinkList",
                    controller: 'virtoCommerce.seo.brokenLinkListController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/broken-link-list.html',
                    refreshWidget: initWidget,
                    storeId: blade.currentEntityId,
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
