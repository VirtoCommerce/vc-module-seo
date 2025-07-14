angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.redirectRuleDetailsController', [
        '$scope',
        'virtoCommerce.seo.redirectRulesApi',
        'platformWebApp.dialogService',
        'platformWebApp.bladeNavigationService',
        function ($scope, redirectRulesApi, dialogService, bladeNavigationService) {

            const blade = $scope.blade;
            blade.formScope = null;
            blade.isNew = true;

            blade.refresh = function (parentRefresh) {
                initializeBlade();
                if (parentRefresh) {
                    blade.parentBlade.refresh(true);
                }
            };

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback,
                    "seo.dialogs.redirect-rule-save.title", "seo.dialogs.redirect-rule-save.message");
            };

            blade.toolbarCommands = [
                {
                    name: "platform.commands.save", icon: 'fa fa-save',
                    permission: blade.updatePermission,
                    executeMethod: function () {
                        saveChanges();
                    },
                    canExecuteMethod: function () {
                        return canSave();
                    }
                },
                {
                    name: "platform.commands.reset", icon: 'fa fa-undo',
                    executeMethod: function () {
                        angular.copy(blade.currentEntity, blade.editEntity);
                    },
                    canExecuteMethod: isDirty
                },
                {
                    name: "platform.commands.delete", icon: 'fa fa-trash-o',
                    permission: 'seo:delete',
                    executeMethod: deleteEntry,
                    canExecuteMethod: function () {
                        return true;
                    }
                }
            ];

            $scope.setForm = function (form) {
                blade.formScope = form;
            };

            $scope.validateRedirectUrl = function (value) {
                return blade.editEntity?.status !== "Resolved" || !!value;
            };

            $scope.revalidate = function () {
                blade.formScope.redirectUrl.$validate();
            };

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.editEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return isDirty() && blade.formScope && blade.formScope.$valid;
            }

            function initializeBlade() {
                blade.isNew = !blade.currentEntityId;
                if (!blade.isNew) {
                    blade.isLoading = true;
                    redirectRulesApi.get({ id: blade.currentEntityId }, function (data) {
                        blade.currentEntity = data;
                        //blade.title = blade.currentEntity.permalink;
                        blade.editEntity = angular.copy(blade.currentEntity);
                        blade.isLoading = false;
                    }, function (error) {
                        console.log(error);
                        blade.isLoading = false;
                    });
                } else {
                    blade.isLoading = false;
                    var data = { storeId: blade.storeId };
                    blade.currentEntity = data;
                    //blade.title = blade.currentEntity.permalink;
                    blade.editEntity = angular.copy(blade.currentEntity);
                }
            }

            function saveChanges() {
                blade.isLoading = true;
                redirectRulesApi.save(blade.editEntity, function (data) {
                    blade.isLoading = false;
                    blade.currentEntityId = data.id;
                    blade.currentEntity = angular.copy(data);
                    blade.editEntity = data;
                    blade.refresh(true);
                }, function (error) {
                    console.log(error);
                    blade.isLoading = false;
                });
            }

            function deleteEntry() {
                var dialog = {
                    id: "redirectRuleConfirmDelete",
                    title: "seo.dialogs.redirect-rule-delete.title",
                    message: "seo.dialogs.redirect-rule-delete.message",
                    callback: function (remove) {
                        if (remove) {
                            blade.isLoading = true;
                            redirectRulesApi.delete({ ids: [blade.currentEntity.id] }, function () {
                                blade.parentBlade.refresh();
                                $scope.bladeClose();
                            });
                        }
                    }
                };
                dialogService.showConfirmationDialog(dialog);
            }

            blade.refresh();
        }]);
