angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.explainApi', ['$http', function ($http) {
        // Correct API endpoint for SEO Explain
        var apiUrl = 'api/seoinfos/explain';

        return {
            /**
             * GET explain results
             * @param {Object} params - { storeId, storeDefaultLanguage, languageCode, permalink }
             * @returns {Promise<Array>}
             */
            getExplain: function (params) {
                return $http.get(apiUrl, { params: params })
                    .then(function (response) {
                        // Always return an array, even if backend returns null or object
                        return Array.isArray(response.data) ? response.data : [];
                    })
                    .catch(function () {
                        // On error, return empty array to keep grid stable
                        return [];
                    });
            }
        };
    }]);
