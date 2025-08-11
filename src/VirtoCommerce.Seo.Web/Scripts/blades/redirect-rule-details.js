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

            $scope.redirectRuleTypes = ["Static", "Regex"];

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

            $scope.validateInboundRule = function (value) {
                return (!!value && blade.editEntity.redirectRuleType === 'Static') || validateRegex(value);
            };

            $scope.validateOutboundRule = function (value) {
                return (!!value && blade.editEntity.redirectRuleType === 'Static') || validateParameters(value);
            }

            $scope.revalidate = function () {
                blade.formScope.inbound.$validate();
                blade.formScope.outbound.$validate();
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
                    var data = { storeId: blade.storeId, priority: 0, redirectRuleType: "Static" };
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

            function validateRegex(value) {
                if (!value) {
                    return false;
                }
                try {
                    new RegExp(value);
                    return true;
                } catch (e) {
                    return false;
                }
            }

            function validateParameters(value) {
                if (!value || !blade.editEntity.inbound) {
                    return false;
                }

                // Count groups in inboundRule (regex)
                const inbound = blade.editEntity.inbound;
                let groupCount = 0;
                try {
                    // Match all non-escaped opening parentheses that are not part of a non-capturing group
                    // This regex counts capturing groups: ( ... )
                    const regex = /(?:[^\\]|^)\((?!\?)/g;
                    groupCount = (inbound.match(regex) || []).length;
                } catch (e) {
                    return false;
                }

                // Find all $n in outboundRule
                const paramRegex = /\$(\d+)/g;
                let match;
                let maxParam = 0;
                while ((match = paramRegex.exec(value)) !== null) {
                    const n = parseInt(match[1], 10);
                    if (n > maxParam) {
                        maxParam = n;
                    }
                }

                // Valid if maxParam <= groupCount
                return maxParam <= groupCount;
            }

            blade.refresh();
        }]);
