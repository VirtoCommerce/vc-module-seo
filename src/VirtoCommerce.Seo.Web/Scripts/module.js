// Call this to register your module to main application
var moduleName = 'VirtoCommerce.Seo';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            $stateProvider
                .state('workspace.SeoState', {
                    url: '/seo',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        'platformWebApp.bladeNavigationService',
                        function (bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'VirtoCommerce.Seo.helloWorldController',
                                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/hello-world.html',
                                isClosingDisabled: true,
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['platformWebApp.mainMenuService', '$state',
        function (mainMenuService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/seo',
                icon: 'fa fa-cube',
                title: 'Seo',
                priority: 100,
                action: function () { $state.go('workspace.SeoState'); },
                permission: 'seo:access',
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);
