// Call this to register your module to main application
const moduleName = 'virtoCommerce.seo';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .run(['platformWebApp.widgetService',
        function (widgetService) {
            widgetService.registerWidget({
                controller: 'virtoCommerce.seo.storeWidgetController',
                template: 'Modules/$(VirtoCommerce.Seo)/Scripts/widgets/storeWidget.html'
            }, 'storeDetail');
        }]
    );
