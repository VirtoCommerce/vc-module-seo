/*
angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.seoExplainApi', ['$resource', function ($resource) {
        return $resource('api/seoinfos/explain');
    }]);
*/
angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.explainApi', ['$http', function ($http) {
        var apiUrl = 'api/seo/explain';

        return {
            // GET with query params
            getExplain: function (params) {
                return $http.get(apiUrl, { params: params }).then(x => x.data);
            }
        };
    }]);
