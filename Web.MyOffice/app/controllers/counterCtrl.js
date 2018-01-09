(function () {
    'use strict';
    angular.module('MyOffice.app').controller('counterCtrl', ['$scope', counterCtrl]);

    function counterCtrl($scope) {
        $scope.banknotes = [];
        $scope.nominals = [1, 2, 5, 10, 20, 50, 100];
        $scope.AddBanknote = function () {
            var banknote = new Banknote({ name: 'USD', value: 1 });
            if ($scope.nominals.length !== 0) {
                $scope.banknotes.push(banknote);
            }
            $scope.nominals = $scope.nominals.filter(function (_nominal) {
                return _nominal !== banknote.nominal;
            });
            if ($scope.nominals.length === 0) {
                $scope.disableButton = true;
            }
        };
        $scope.DeleteBanknote = function (banknote) {
            $scope.disableButton = false;
            $scope.nominals.push(banknote.nominal);
            $scope.nominals.sort(function (prev, next) {
                if (prev > next) {
                    return 1;
                } else if(prev < next){
                    return -1;
                } else {
                    return 0;
                }
            });
            $scope.banknotes = $scope.banknotes.filter(function(_banknote) {
                return _banknote.nominal !== banknote.nominal;
            });
            $scope.getTotalMoney();
        };
        $scope.totalMoney = 0;
        $scope.getTotalMoney = function () {
            $scope.totalMoney = 0;
            $scope.banknotes.forEach(function(banknote) {
                $scope.totalMoney += banknote.totalCost;
            });
        };
        function Banknote(currency) {
            this.currency = currency;
            this.nominal = $scope.nominals[0];
            this.quantity = 0;
            this.totalCost = 0;
            this.totalCostCalc = function () {
                this.totalCost = this.currency.value * this.nominal * this.quantity;
                $scope.getTotalMoney();
            };
        }
    }
})();
