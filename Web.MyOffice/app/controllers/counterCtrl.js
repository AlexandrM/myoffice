(function () {
    'use strict';
    angular.module('MyOffice.app').controller('counterCtrl', ['$scope', 'ModalWindowService', counterCtrl]);

    function counterCtrl($scope, ModalWindowService) {
        $scope.casualNominals = [1, 2, 5, 10, 20, 50, 100];
        $scope.selectedNominal = $scope.casualNominals.shift();
        $scope.banknotes = [];
        $scope.AddBanknote = function (nominal) {
            if ($scope.banknotes.find(function (banknote) {
                    return banknote.nominal === nominal;
                }) ===
                undefined && nominal !== undefined) {
                var banknote = new Banknote(nominal);
                $scope.banknotes.push(banknote);
                $scope.banknotes.sort($scope.banknoteSorter);
                $scope.getTotalMoney();
                if ($scope.casualNominals.length !== 0) {
                    $scope.selectedNominal = $scope.casualNominals.shift();
                } else {
                    $scope.selectedNominal = undefined;
                }

            }
        };

        $scope.DeleteBanknote = function (banknote) {
            $scope.banknotes = $scope.banknotes.filter(function(_banknote) {
                return _banknote.nominal !== banknote.nominal;
            });
            $scope.banknotes = $scope.banknotes.filter(function (_banknote) {
                return _banknote.nominal !== banknote.nominal;
            });
            $scope.banknotes.sort($scope.banknoteSorter);
            $scope.getTotalMoney();
        };
        $scope.banknoteSorter = function(prev, next) {
            if (prev.nominal > next.nominal) {
                return 1;
            } else if (prev < next) {
                return -1;
            } else {
                return 0;
            }
        };

        $scope.getTotalMoney = function () {
            $scope.totalMoney = 0;
            $scope.banknotes.forEach(function(banknote) {
                $scope.totalMoney += banknote.totalCost;
            });
        };
        function Banknote(nominal) {
            this.nominal = nominal;
            this.quantity = 1;
            this.totalCost = this.nominal * this.quantity;
            this.totalCostCalc = function () {
                this.totalCost =  this.nominal * this.quantity;
                $scope.getTotalMoney();
            };
        }
        ModalWindowService.open('CounterController', 'list', $scope, 'modal-sm');
        $scope.closeDialog = function () {
            ModalWindowService.close('list');
            history.back();
        };
    }
})();
