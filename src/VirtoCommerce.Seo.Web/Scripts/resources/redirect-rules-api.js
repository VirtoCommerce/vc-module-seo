angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.redirectRulesApi', ['$resource', function ($resource) {
        return $resource('api/seo/redirect-rules/:id', { id: '@Id' }, {
            search: {
                method: 'POST',
                url: 'api/seo/redirect-rules/search',
            },
            save: { method: 'POST' },
        });
    }]);
