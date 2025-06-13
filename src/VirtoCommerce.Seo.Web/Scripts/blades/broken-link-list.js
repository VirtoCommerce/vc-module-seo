angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinkListController', ['$scope', '$injector', function ($scope, $injector) {

        const api = $injector.get('virtoCommerce.seo.webApi');
        const bladeUtils = $injector.get('platformWebApp.bladeUtils');
        const uiGridHelper = $injector.get('platformWebApp.uiGridHelper');

        $scope.data = [];

        var blade = $scope.blade;
        blade.title = 'SEO';
        blade.searchKeyword = null;

        blade.refresh = function () {
            api.search({ storeId: blade.storeId }, function (data) {
                $scope.data = data.result;

                blade.title = 'seo.blades.broken-link-list.title';
                blade.isLoading = false;
            });
        };

        blade.getSelectedRows = function () {
            return $scope.gridApi.selection.getSelectedRows();
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.refresh", icon: 'fa fa-refresh',
                executeMethod: blade.refresh,
                canExecuteMethod: function () {
                    return true;
                }
            },
            //{
            //    name: "platform.commands.delete", icon: 'fa fa-trash-o',
            //    executeMethod: function () { onDeleteList($scope.gridApi.selection.getSelectedRows()); },
            //    canExecuteMethod: isItemsChecked,
            //    permission: 'content:delete'
            //},
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () { onDeleteList($scope.gridApi.selection.getSelectedRows()); },
                canExecuteMethod: isItemsChecked,
                permission: 'content:delete'
            }
        ];

        function openDetailsBlade(listItem) {
            console.log(listItem);
        }

        function onDeleteList(selection) {
            console.log(selection);
        }

        function isItemsChecked() {
            return !blade.pasteMode && $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
        }

        $scope.selectNode = function (listItem) {
            openDetailsBlade(listItem);
        };

        $scope.delete = function (listItem) {
            onDeleteList([listItem]);
        };

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions,
                function (gridApi) {
                    $scope.gridApi = gridApi;
                    // uiGridHelper.bindRefreshOnSortChanged($scope);
                });
        };
        bladeUtils.initializePagination($scope, false);

        blade.refresh();
    }]);
