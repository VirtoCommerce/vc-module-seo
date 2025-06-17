angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.storeWidgetController', ['$scope',
        'virtoCommerce.seo.webApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, resources, bladeNavigationService) {
            const blade = $scope.widget.blade;

            $scope.count = '...';

            function initWidget() {
                const criteria = {
                    storeid: blade.currentEntityId,
                    // status: 'active',
                };
                blade.currentEntityId && resources.search(criteria, function (result) {
                    $scope.count = result.totalCount;
                });
            }

            $scope.openBlade = function () {
                const newBlade = {
                    id: "seoBrokenLinkList",
                    storeId: blade.currentEntityId,
                    headIcon: 'fa fa-file-o',
                    title: 'seo.blades.broken-link-list.title',
                    titleValues: { name: store.name },
                    subtitle: 'seo.blades.broken-link-list.subtitle',
                    controller: 'virtoCommerce.seo.brokenLinkListController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/broken-link-list.html',
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
