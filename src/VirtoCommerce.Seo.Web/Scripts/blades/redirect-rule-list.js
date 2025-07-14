angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.redirectRuleListController', [
        '$scope',
        'virtoCommerce.seo.redirectRulesApi',
        'platformWebApp.bladeUtils',
        'platformWebApp.uiGridHelper',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.dialogService',
        function ($scope, redirectRulesApi, bladeUtils, uiGridHelper, bladeNavigationService, dialogService) {
            const blade = $scope.blade;
            blade.headIcon = 'fa fa-compass';
            blade.title = 'seo.blades.redirect-rule-list.title';
            blade.updatePermission = 'seo:update';
            blade.searchKeyword = null;

            $scope.data = [];

            blade.refresh = function (refreshParent) {
                blade.isLoading = true;
                if ($scope.pageSettings.currentPage !== 1) {
                    $scope.pageSettings.currentPage = 1;
                }

                redirectRulesApi.search(getSearchCriteria(), function (data) {
                    $scope.data = data.results;
                    $scope.pageSettings.totalItems = data.totalCount;

                    blade.isLoading = false;
                });
                if (refreshParent && blade.refreshWidget) {
                    blade.refreshWidget();
                }
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
                    executeMethod: function () { blade.refresh(true); },
                    canExecuteMethod: function () {
                        return true;
                    }
                },
                {
                    name: "platform.commands.add", icon: 'fas fa-plus',
                    executeMethod: function () {
                        openDetailsBlade();
                    },
                    canExecuteMethod: function () {
                        return true;
                    },
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
                    id: 'redirectRuleDetails',
                    controller: 'virtoCommerce.seo.redirectRuleDetailsController',
                    template: 'Modules/$(virtoCommerce.seo)/Scripts/blades/redirect-rule-details.html',
                    currentEntityId: listItem?.id,
                    storeId: blade.storeId,
                    parentBlade: blade,
                    updatePermission: blade.updatePermission,
                };
                bladeNavigationService.showBlade(newBlade, blade);
            }

            function onDeleteList(selection) {
                const message = selection.length > 1 ? 'seo.dialogs.redirect-rule-delete.messagePlural' : 'seo.dialogs.redirect-rule-delete.message';
                const dialog = {
                    id: 'redirectRuleConfirmDelete',
                    title: 'seo.dialogs.redirect-rule-delete.title',
                    message: message,
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;
                            const ids = selection.map(function (item) {
                                return item.id;
                            });
                            redirectRulesApi.delete({ ids }, function () {
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
