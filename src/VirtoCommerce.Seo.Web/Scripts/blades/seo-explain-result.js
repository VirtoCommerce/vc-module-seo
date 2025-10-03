angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainResultController', [
        '$scope', 'virtoCommerce.seo.explainApi',
        function ($scope, explainApi) {
            var blade = $scope.blade;
            blade.headIcon = 'fa fa-list';
            blade.isLoading = true;

            // Always initialize gridOptions with empty data
            $scope.gridOptions = {
                rowTemplate: 'seo-explain.row.html',
                columnDefs: [
                    { name: 'stage', displayName: 'Stage' },
                    { name: 'description', displayName: 'Description' },
                    { name: 'seoExplainItems[0].seoInfo.name', displayName: 'Name' },
                    { name: 'seoExplainItems[0].seoInfo.semanticUrl', displayName: 'Semantic URL' },
                    { name: 'seoExplainItems[0].seoInfo.pageTitle', displayName: 'Page Title' },
                    { name: 'seoExplainItems[0].seoInfo.languageCode', displayName: 'Language' },
                    { name: 'seoExplainItems[0].score', displayName: 'Score' }
                ],
                data: [] // must always exist
            };

            // Call API
            explainApi.getExplain(blade.data)
                .then(function (data) {
                    // If API returns nothing, keep empty array
                    $scope.gridOptions.data = Array.isArray(data) ? data : [];
                })
                .catch(function () {
                    // In case of error, still keep empty grid
                    $scope.gridOptions.data = [];
                })
                .finally(function () {
                    blade.isLoading = false;
                });
        }
    ]);
