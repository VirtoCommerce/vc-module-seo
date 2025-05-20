angular.module('VirtoCommerce.Seo')
    .factory('VirtoCommerce.Seo.webApi', ['$resource', function ($resource) {
        return $resource('api/seo');
    }]);
