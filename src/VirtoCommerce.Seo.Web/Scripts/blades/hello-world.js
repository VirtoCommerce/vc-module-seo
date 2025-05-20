angular.module('VirtoCommerce.BrokenLinks')
    .controller('VirtoCommerce.BrokenLinks.helloWorldController', ['$scope', 'VirtoCommerce.BrokenLinks.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'BrokenLinks';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'BrokenLinks.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
