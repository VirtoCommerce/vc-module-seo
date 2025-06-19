angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinkListController', ['$scope', '$injector', function ($scope, $injector) {

        const api = $injector.get('virtoCommerce.seo.webApi');
        const bladeUtils = $injector.get('platformWebApp.bladeUtils');
        const uiGridHelper = $injector.get('platformWebApp.uiGridHelper');
        const bladeNavigationService = $injector.get('platformWebApp.bladeNavigationService');
        const dialogService = $injector.get('platformWebApp.dialogService');

        $scope.data = [];

        const blade = $scope.blade;
        blade.searchKeyword = null;
        blade.title = 'seo.blades.broken-link-list.title';
        blade.updatePermission = 'seo:update';

        blade.refresh = function () {
            blade.isLoading = true;
            if ($scope.pageSettings.currentPage !== 1) {
                $scope.pageSettings.currentPage = 1;
            }

            api.search(getSearchCriteria(), function (data) {
                $scope.data = data.results;
                $scope.pageSettings.totalItems = data.totalCount;

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
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: function () { onDeleteList($scope.gridApi.selection.getSelectedRows()); },
                canExecuteMethod: isItemsChecked,
                permission: 'content:delete'
            }
        ];

        function openDetailsBlade(listItem) {
            const newBlade = {
                id: 'brokenLinkDetails',
                controller: 'virtoCommerce.seo.brokenLinkDetailsController',
                template: 'Modules/$(virtoCommerce.seo)/Scripts/blades/broken-link-details.html',
                currentEntity: listItem,
                parentBlade: blade,
                title: listItem.permalink,
                updatePermission: blade.updatePermission,
            };
            bladeNavigationService.showBlade(newBlade, blade);
        }

        function onDeleteList(selection) {
            var dialog = {
                id: "brokenLinkConfirmDelete",
                title: "seo.dialogs.broken-link-delete.title",
                message: "seo.dialogs.broken-link-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        const ids = selection.map(function (item) {
                            return item.id;
                        });
                        api.delete({ ids }, function () {
                            blade.isLoading = false;
                            blade.refresh();
                        }, function (error) {
                            console.log(error);
                            blade.isLoading = false;
                        });
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        }

        function isItemsChecked() {
            return $scope.gridApi && _.any($scope.gridApi.selection.getSelectedRows());
        }

        function getSearchCriteria() {
            return {
                storeId: blade.storeId,
                keyword: blade.searchKeyword,
                sort: uiGridHelper ? uiGridHelper.getSortExpression($scope) : null,
                skip: ($scope.pageSettings.currentPage - 1) * $scope.pageSettings.itemsPerPageCount,
                take: $scope.pageSettings.itemsPerPageCount,
            };
        }

        $scope.selectNode = function (listItem) {
            openDetailsBlade(listItem);
        };

        $scope.delete = function (listItem) {
            onDeleteList([listItem]);
        };

        $scope.setGridOptions = function (gridOptions) {
            bladeUtils.initializePagination($scope, false);
            $scope.pageSettings.itemsPerPageCount = 20;
            uiGridHelper.initialize($scope, gridOptions,
                function (gridApi) {
                    $scope.gridApi = gridApi;
                });

            blade.refresh();
        };
    }]);
