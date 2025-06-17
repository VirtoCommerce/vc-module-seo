angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.brokenLinkDetailController', ['$scope', "$injector", function ($scope, $injector) {
        const api = $injector.get('virtoCommerce.seo.webApi');
        const dialogService = $injector.get('platformWebApp.dialogService');
        const bladeNavigationService = $injector.get('platformWebApp.bladeNavigationService');

        const blade = $scope.blade;
        blade.formScope = null;

        blade.availableStatuses = ["Active", "Resolved", "Accepted"];

        blade.refresh = function (parentRefresh) {
            blade.isLoading = true;
            initializeBlade();
        };

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, saveChanges, closeCallback,
                "seo.dialogs.broken-list-save.title", "seo.dialogs.broken-list-save.message");
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
            return blade.editEntity.status !== "Resolved" || !!value;
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
            blade.editEntity = angular.copy(blade.currentEntity);
            blade.isLoading = false;
        }

        function saveChanges() {
            blade.isLoading = true;
            api.save(blade.editEntity, function () {
                blade.isLoading = false;
                blade.currentEntity = angular.copy(blade.editEntity);
                blade.parentBlade.refresh();
            }, function (error) {
                console.log(error);
                blade.isLoading = false;
            });
        }

        function deleteEntry() {
            var dialog = {
                id: "brokenLinkConfirmDelete",
                title: "seo.dialogs.broken-link-delete.title",
                message: "seo.dialogs.broken-link-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        api.delete({ ids: [blade.currentEntity.id] }, function () {
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
