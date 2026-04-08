angular.module('virtoCommerce.seo')
    .factory('virtoCommerce.seo.seoUtils', function () {
        const invalidUrlPattern = /[$+;=%{}[\]|\\/@ ~#!^*&?:'<>,]/;

        return {
            isValidSemanticUrl: function (value) {
                return !invalidUrlPattern.test(value);
            }
        };
    });
