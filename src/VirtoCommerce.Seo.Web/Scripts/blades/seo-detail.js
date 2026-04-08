angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoDetailController',
        ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.metaFormsService',
            function ($scope, bladeNavigationService, metaFormsService) {
                const blade = $scope.blade;

                function initializeBlade() {
                    blade.origEntity = blade.data;
                    blade.currentEntity = angular.copy(blade.origEntity);
                    blade.isLoading = false;
                }

                blade.metaFields = metaFormsService.getMetaFields("seoDetails");
                $scope.storeDataSource = blade.storeDataSource || undefined;

                $scope.cancelChanges = function () {
                    angular.copy(blade.origEntity, blade.currentEntity);
                    $scope.bladeClose();
                };

                $scope.saveChanges = function () {
                    if (blade.isNew) {
                        if (!blade.seoContainerObject.seoInfos) {
                            blade.seoContainerObject.seoInfos = [];
                        }
                        blade.seoContainerObject.seoInfos.push(blade.currentEntity);
                    }

                    angular.copy(blade.currentEntity, blade.origEntity);
                    if (!blade.noClose) {
                        $scope.bladeClose();
                    }
                }

                function saveChanges_noClose() {
                    blade.noClose = true;
                    $scope.saveChanges();
                }

                function isValid(data) {
                    // check required and valid Url requirements
                    return data.semanticUrl &&
                        $scope.semanticUrlValidator(data.semanticUrl);
                }

                $scope.semanticUrlValidator = function (value) {
                    const pattern = /[$+;=%{}[\]|@ ~#!^*&?:'<>,]/;
                    return !pattern.test(value);
                };

                $scope.duplicateValidator = function (value) {
                    return _.all(blade.seoContainerObject.seoInfos, function (x) {
                        return x === blade.origEntity ||
                            x.storeId !== blade.currentEntity.storeId ||
                            x.semanticUrl !== value;
                    });
                };

                function isDirty() {
                    return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
                }

                function canSave() {
                    return (blade.isNew || isDirty()) && isValid(blade.currentEntity);
                }

                $scope.isValid = canSave;

                blade.onClose = function (closeCallback) {
                    bladeNavigationService.showConfirmationIfNeeded(
                        isDirty(), canSave(), blade, saveChanges_noClose, closeCallback,
                        "seo.dialogs.seo-save.title", "seo.dialogs.seo-save.message");
                };

                blade.toolbarCommands = [
                    {
                        name: "platform.commands.reset", icon: 'fa fa-undo',
                        executeMethod: function () {
                            angular.copy(blade.origEntity, blade.currentEntity);
                        },
                        canExecuteMethod: isDirty,
                        permission: blade.updatePermission
                    }
                ];

                blade.headIcon = 'fa fa-globe';
                blade.title = blade.isNew ? 'seo.blades.seo-detail.title-new' : blade.data.semanticUrl;
                blade.subtitle = 'seo.blades.seo-detail.subtitle';

                initializeBlade();
            }]);

