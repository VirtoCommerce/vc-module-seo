// Call this to register your module to main application
var moduleName = 'VirtoCommerce.BrokenLinks';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            $stateProvider
                .state('workspace.BrokenLinksState', {
                    url: '/broken-links',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        'platformWebApp.bladeNavigationService',
                        function (bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'VirtoCommerce.BrokenLinks.helloWorldController',
                                template: 'Modules/$(VirtoCommerce.BrokenLinks)/Scripts/blades/hello-world.html',
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
                path: 'browse/broken-links',
                icon: 'fa fa-cube',
                title: 'BrokenLinks',
                priority: 100,
                action: function () { $state.go('workspace.BrokenLinksState'); },
                permission: 'broken-links:access',
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);
