angular.module('virtoCommerce.seo')
    .controller('virtoCommerce.seo.seoExplainCandidatesController', [
        '$scope',
        '$filter',
        function ($scope, $filter) {
            var blade = $scope.blade;
            blade.title = 'seo.blades.seo-explain-candidates.title';
            blade.headIcon = 'fa fa-table';
            blade.isLoading = false;

            blade.items.forEach(function (item) {
                item.reasonsText = item.reasons.map(formatReason).join('; ');
            });

            $scope.gridOptions = {
                data: 'blade.items',
                enableColumnMenus: false,
                enableColumnResizing: true,
                enableColumnMoving: true,
                rowHeight: 40
            };

            function formatReason(reason) {
                var text = $filter('fallbackTranslate')('seo.candidate-reasons.' + reason.code, reason.code);
                return reason.details ? text + ': ' + reason.details : text;
            }
        }
    ]);
