(function () {
    'use strict';
    angular.module('MyOffice.app').controller('counterCtrl', ['$scope', 'ModalWindowService', counterCtrl]);

    function counterCtrl($scope, ModalWindowService) {
        var nominalSearchKey = 'banknote';
        var maxBanknoteNominals = 20;

        $scope.nominalStorage = new NominalStorage(nominalSearchKey, maxBanknoteNominals);

        $scope.banknotes = [];
        var banknoteSorter = function (prev, next) {
            if (prev.nominal > next.nominal) {
                return 1;
            } else if (prev < next) {
                return -1;
            } else {
                return 0;
            }
        };
        var nominalSorter = function (prev, next) {
            if (prev > next) {
                return 1;
            } else if (prev < next) {
                return -1;
            } else {
                return 0;
            }
        };

        $scope.AddBanknote = function (nominal) {
            var banknoteExists = $scope.banknotes.find(function (banknote) {
                return banknote.nominal === nominal;
            }) !== undefined;

            if (!banknoteExists && nominal !== undefined) {
                var banknote = new Banknote(nominal);
                $scope.banknotes.push(banknote);
                $scope.banknotes.sort(banknoteSorter);
                $scope.getTotalMoney();
                $scope.nominalStorage.add(nominal);
                $scope.nominalStorage.nominals.sort(nominalSorter);
            }
        };
        $scope.DeleteBanknote = function (banknote) {
            $scope.banknotes = $scope.banknotes.filter(function(_banknote) {
                return _banknote.nominal !== banknote.nominal;
            });
            $scope.banknotes.sort(banknoteSorter);
            $scope.getTotalMoney();
            $scope.nominalStorage.remove(banknote.nominal);
            $scope.nominalStorage.nominals.sort(nominalSorter);
        };

        $scope.getTotalMoney = function () {
            $scope.totalMoney = 0;
            $scope.banknotes.forEach(function(banknote) {
                $scope.totalMoney += banknote.totalCost;
            });
        };
        $scope.openDialog = function () {
            ModalWindowService.open('CounterController', 'list', $scope, 'modal-sm');
        };
        $scope.closeDialog = function () {
            ModalWindowService.close('list');
        };
        function Banknote(nominal) {
            this.nominal = nominal;
            this.quantity = 1;
            this.totalCost = this.nominal * this.quantity;
            this.totalCostCalc = function () {
                this.totalCost = this.nominal * this.quantity;
                $scope.getTotalMoney();
            };
        }
    }



    function NominalStorage(nominalSearchKey, maxBanknoteNominals) {
        this.maxBanknoteNominals = maxBanknoteNominals;
        this.nominalSearchKey = nominalSearchKey;
        this.counter = -1;
        this.nominals = null;

        this.getAll = function () {
            var vals = [];
            for (var key in localStorage) {
                if (key.indexOf(nominalSearchKey) !== -1) {
                    vals.push(+localStorage[key]);
                }
            }
            this.nominals = vals.sort();
            return vals;
        };
        this.getAll();


        this.add = function (nominal) {
            if (this.nominals.indexOf(nominal) === -1) {
                this.nominals.push(nominal);
                localStorage.setItem(this.nominalSearchKey + '_' + nominal, nominal);
                if (this.nominals.length > this.maxBanknoteNominals) {
                    localStorage.removeItem(nominals[0]);
                }
                this.getAll();
            }
            this.counter = this.nominals.indexOf(nominal);
            this.next();
        };
        this.remove = function (nominal) {
            if (localStorage.length > 0 && localStorage.key(this.nominalSearchKey + '_' + nominal) !== null) {
                localStorage.removeItem(this.nominalSearchKey + '_' + nominal);
                this.counter = this.nominals.indexOf(nominal);
                this.getAll();
                this.prev();
            }
        };
        this.removeAll = function () {
            for (var key in localStorage) {
                if (key.indexOf(nominalSearchKey) !== -1) {
                    localStorage.removeItem(key);
                }
            }
        };

        this.next = function () {
            if (++this.counter > this.nominals.length - 1) {
                this.counter = 0;
            }
            this.current = this.nominals[this.counter];
            return this.current;
        };
        this.current = this.nominals[0];
        this.prev = function () {
            if (--this.counter < 0) {
                this.counter = this.nominals.length - 1;
            }
            this.current = this.nominals[this.counter];
            return this.current;
        };
    }
})();
