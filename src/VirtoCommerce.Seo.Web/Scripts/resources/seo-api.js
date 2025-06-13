angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.webApi', ['$resource', function ($resource) {
        return $resource('api/seo/broken-links', null, {
            search: {
                url: 'api/seo/broken-links/search',
                method: 'POST',
            },
        });
    }]);
