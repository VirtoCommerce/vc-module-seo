angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.seoApi', ['$resource', function ($resource) {
        return $resource('api/seoinfos/duplicates', null, {
            batchUpdate: { url: 'api/seoinfos/batchupdate', method: 'PUT' }
        });
    }]);
