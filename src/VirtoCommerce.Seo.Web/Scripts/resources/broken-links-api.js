angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.brokenLinksApi', ['$resource', function ($resource) {
        return $resource('api/seo/broken-links/:id', { id: '@Id' }, {
            search: {
                method: 'POST',
                url: 'api/seo/broken-links/search',
            },
            save: { method: 'POST' },
        });
    }]);
