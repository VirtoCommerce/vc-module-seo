angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.storeWidgetController', ['$scope',
        'virtoCommerce.seo.webApi',
        'platformWebApp.bladeNavigationService',
        function ($scope, resources, bladeNavigationService) {
            var blade = $scope.widget.blade;

            $scope.count = '...';

            function initWidget() {
                var criteria = {
                    storeid: blade.currentEntityId,
                    // status: 'active',
                };
                blade.currentEntityId && resources.search(criteria, function (result) {
                    $scope.count = result.totalCount;
                });
            }

            $scope.openBlade = function () {
                var newBlade = {
                    id: "seoBrokenLinkList",
                    storeId: blade.currentEntityId,
                    headIcon: 'fa fa-file-o',
                    title: 'seo.blades.broken-link-list.title',
                    titleValues: { name: store.name },
                    subtitle: 'seo.blades.broken-link-list.subtitle',
                    controller: 'virtoCommerce.seo.brokenLinkListController',
                    template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/broken-link-list.tpl.html',
                };
                bladeNavigationService.showBlade(newBlade, blade);
            };

            initWidget();
        }]);
