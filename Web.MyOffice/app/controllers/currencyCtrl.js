(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('currencyCtrl', ['$scope', 'CurrencyService', 'ModalWindowService', currencyCtrl]);

    function currencyCtrl($scope, CurrencyService, ModalWindowService) {
        /*____________________________ Infrastructure section ____________________________*/
        $scope.sortByProp = function (array, sortProperty) {
            if (array !== undefined || array !== undefined) {
                var sorter = function (next, prev) {
                    if (array[sortProperty] > array[sortProperty]) {
                        return 1;
                    } else if (array[sortProperty] < array[sortProperty]) {
                        return -1;
                    } else {
                        return 0;
                    }
                };
                return array.sort(sorter);
            }
        };

        $scope.currencyFilter = function (criteria) {
            if (criteria === '' || criteria === null || criteria === undefined) {
                return $scope.allCurrencies;
            }
            return $scope.allCurrencies.filter(function (currency) {
                return currency.Name.indexOf(criteria) !== -1;
            });
        };

        $scope.closeWindow = function (name) {
            ModalWindowService.close(name);
        };

        /*______________________________ Data processing ______________________________*/
        $scope.sortPropertyName = '';
        $scope.refresh = function () {
            $scope.model = CurrencyService.getCurrencyList({}, function () {
                $scope.activeCurrencies = $scope.sortByProp(
                    $scope.model.currencies.filter(function(currency){
                        return !currency.IsArchive;
                    }),
                'Name');
                $scope.archiveCurrencies = $scope.sortByProp(
                   $scope.model.currencies.filter(function (currency) {
                       return currency.IsArchive;
                   }),
               'Name');
                $scope.CurrencyTypes = $scope.model.types;
                $scope.warnings = $scope.model.warnings;
                $scope.showArchive = true;
            });
        };

        $scope.filter = function ($event) {
            $scope.currencies = $scope.currencyFilter($event.currentTarget.value);
        };

        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('CurrenciesController', viewName, $scope, size);
        };
        /*____________________________ Currency CRUD section ____________________________*/
        $scope.currencyEditDialog = function (currency) {
            $scope.newCurrency = currency;
            $scope.openForm('CurrencyAdd','modal-sm');
        };

        $scope.postCurrency = function(newCurrency) {
            CurrencyService.postCurrency({
                    Id: newCurrency.Id,
                    Name: newCurrency.Name,
                    ShortName: newCurrency.ShortName,
                    CurrencyType: newCurrency.CurrencyType,
                    MyCurrency: newCurrency.MyCurrency,
                    Value: newCurrency.Value
                },
                function() {
                    if (windows['CurrencyAdd'] !== undefined) {
                        $scope.refresh();
                    }
                    $scope.closeWindow('CurrencyAdd');
                });
        };

        $scope.addCurrency = function (newCurrency) {
                $scope.postCurrency(newCurrency);
            };

        $scope.CurrencyArchive = function (currency) {
            if (!currency.MyCurrency) {
                CurrencyService.editCurrency({ currencyId: currency.Id, deleted: true }, function () {
                    $scope.refresh();
                });
            } else {
                bootbox.alert($scope.model.warnings[1]);
            }
        };

        $scope.restoreCurrency = function (currency) {
            CurrencyService.editCurrency({ currencyId: currency.Id, deleted: false }, function () {
                $scope.refresh();
            });
        };

        $scope.deleteCurrency = function (Id) {
            CurrencyService.deleteCurrency({ currencyId: Id }, function () {
                $scope.refresh();
            });
        };

        $scope.setMyCurrency = function (checked) {
                var unchecked = $scope.activeCurrencies.find(function (currency) {
                    return currency.MyCurrency && currency.Id !== checked.Id;
                });
                if (unchecked !== undefined) {
                    unchecked.MyCurrency = false;
                    $scope.postCurrency(unchecked);
                }
                $scope.postCurrency(checked);
            };

        $scope.ratesUpdate = function(sourceName){
                CurrencyService.updateRates({ sourceName: sourceName }, function () {
                    $scope.refresh();
                });
            };
        }
})();
