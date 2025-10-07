angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.explainApi', ['$resource', function ($resource) {
        return $resource('api/seoinfos/explain', {},
            {
                explain: {
                    method: 'GET',
                    isArray: true
                }
            });
    }]);
