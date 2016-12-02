(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('currencyCtrl', ['$scope', '$rootScope', 'CurrencyService', currencyCtrl]);

    function currencyCtrl($scope, $rootScope, CurrencyService) {
        $scope.refresh = function () {
            $scope.currencies = CurrencyService.query({}, function () {
            });
        };
        $scope.$on('refreshCurrencyEvent', function () {
            $scope.refresh();
        });
        $scope.currencySelect = function (currency) {
            $rootScope.$broadcast('selectCurrencyEvent',  currency );
        };
    };
})();
