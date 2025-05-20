angular.module('VirtoCommerce.BrokenLinks')
    .factory('VirtoCommerce.BrokenLinks.webApi', ['$resource', function ($resource) {
        return $resource('api/broken-links');
    }]);
