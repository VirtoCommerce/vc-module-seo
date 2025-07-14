// Call this to register your module to main application
const moduleName = 'virtoCommerce.seo';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .run(['platformWebApp.widgetService',
        function (widgetService) {
            widgetService.registerWidget({
                controller: 'virtoCommerce.seo.brokenLinksWidgetController',
                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/widgets/broken-links-widget.html'
            }, 'storeDetail');

            widgetService.registerWidget({
                controller: 'virtoCommerce.seo.redirectRulesWidgetController',
                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/widgets/redirect-rules-widget.html'
            }, 'storeDetail');
        }]
    );
