angular.module('VirtoCommerce.Seo')
    .controller('VirtoCommerce.Seo.helloWorldController', ['$scope', 'VirtoCommerce.Seo.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'Seo';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'Seo.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
