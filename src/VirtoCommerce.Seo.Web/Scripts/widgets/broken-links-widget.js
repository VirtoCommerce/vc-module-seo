angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinksWidgetController', ['$scope',
        'virtoCommerce.seo.brokenLinksApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, brokenLinksApi, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.count = '...';

            function initWidget() {
                const criteria = {
                    storeid: blade.currentEntityId,
                    // status: 'active',
                };
                blade.currentEntityId && brokenLinksApi.search(criteria, function (result) {
                    $scope.count = result.totalCount;
                });
            }

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoBrokenLinkList",
                    controller: 'virtoCommerce.seo.brokenLinkListController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/broken-link-list.html',
                    storeId: blade.currentEntityId,
                    headIcon: 'fa fa-file-o',
                    titleValues: { name: store.name },
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
