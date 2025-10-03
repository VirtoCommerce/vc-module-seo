// Call this to register your module to main application
const moduleName = 'virtoCommerce.seo';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, ['platformWebApp', 'virtoCommerce.storeModule', 'ui.grid.expandable'])
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('workspace.seoExplain', {
            url: '/seo/explain',
            templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
            controller: ['$scope', 'platformWebApp.bladeNavigationService',
                function ($scope, bladeNavigationService) {
                    var newBlade = {
                        id: 'SeoExplainMainBlade',
                        title: 'SEO Explain',
                        controller: 'virtoCommerce.seo.seoExplainMainController',
                        template: 'Modules/$(VirtoCommerce.Seo)/Scripts/blades/seo-explain-main.html',
                        isClosingDisabled: true
                    };
                    bladeNavigationService.showBlade(newBlade);
                }]
        });
    }])
    .run([
        'platformWebApp.widgetService',
        'platformWebApp.mainMenuService',
        'platformWebApp.bladeNavigationService',
        '$state',
        function (widgetService, mainMenuService, bladeNavigationService, $state) {

            // Existing widgets
            widgetService.registerWidget({
                controller: 'virtoCommerce.seo.brokenLinksWidgetController',
                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/widgets/broken-links-widget.html'
            }, 'storeDetail');

            widgetService.registerWidget({
                controller: 'virtoCommerce.seo.redirectRulesWidgetController',
                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/widgets/redirect-rules-widget.html'
            }, 'storeDetail');

            mainMenuService.addMenuItem({
                path: 'browse/seoExplain',
                icon: 'fa fa-search',
                title: 'SEO Explain',
                priority: 100,
                action: function () { $state.go('workspace.seoExplain'); },
                permission: 'seo:access'
            });
        }
    ]);
