"use strict;"

controllers.controller('currencyCtrl', ['$scope', '$rootScope', 'CurrencyService',
    function ($scope, $rootScope, CurrencyService) {
        $scope.refresh = function () {
            $scope.currencies = CurrencyService.query({}, function () {
            });
        };
        $scope.$on("refreshCurrencyEvent", function (event, args) {
            $scope.refresh();
        });
        $scope.currencySelect = function (currency) {
            $rootScope.$broadcast("selectCurrencyEvent", { currency });
        };
    }]);
