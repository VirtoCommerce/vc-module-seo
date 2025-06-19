angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinkListController', [
        '$scope',
        'virtoCommerce.seo.brokenLinksApi',
        'platformWebApp.bladeUtils',
        'platformWebApp.uiGridHelper',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.dialogService',
        function ($scope, brokenLinksApi, bladeUtils, uiGridHelper, bladeNavigationService, dialogService) {
            const blade = $scope.blade;
            blade.headIcon = 'fa fa-chain-broken';
            blade.title = 'seo.blades.broken-link-list.title';
            blade.updatePermission = 'seo:update';
            blade.searchKeyword = null;

            $scope.data = [];

            blade.refresh = function () {
                blade.isLoading = true;
                if ($scope.pageSettings.currentPage !== 1) {
                    $scope.pageSettings.currentPage = 1;
                }

                brokenLinksApi.search(getSearchCriteria(), function (data) {
                    $scope.data = data.results;
                    $scope.pageSettings.totalItems = data.totalCount;

                    blade.isLoading = false;
                });
            };

            blade.getSelectedRows = function () {
                return $scope.gridApi.selection.getSelectedRows();
            };

            blade.selectEntity = function (entity) {
                openDetailsBlade(entity);
            };

            blade.deleteEntity = function (entity) {
                onDeleteList([entity]);
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
                    currentEntityId: listItem.id,
                    parentBlade: blade,
                    updatePermission: blade.updatePermission,
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }

            function onDeleteList(selection) {
                const message = selection.length > 1 ? 'seo.dialogs.broken-link-delete.messagePlural' : 'seo.dialogs.broken-link-delete.message';
                const dialog = {
                    id: 'brokenLinkConfirmDelete',
                    title: 'seo.dialogs.broken-link-delete.title',
                    message: message,
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;
                            const ids = selection.map(function (item) {
                                return item.id;
                            });
                            brokenLinksApi.delete({ ids }, function () {
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
