(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('currencyCtrl', ['$scope', 'CurrencyService', 'ModalWindowService', currencyCtrl]);

    function currencyCtrl($scope, CurrencyService, ModalWindowService) {
        /*____________________________ Infrastructure section ____________________________*/
        $scope.currencySorter = function (currencies, sortProperty) {
            if (currencies !== undefined || sortProperty !== undefined) {
                var currencySortFunc = function (next, prev) {
                    if (next[sortProperty] > prev[sortProperty]) {
                        return 1;
                    } else if (next[sortProperty] < prev[sortProperty]) {
                        return -1;
                    } else {
                        return 0;
                    }
                };
                return currencies.sort(currencySortFunc);
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
                $scope.allCurrencies = $scope.currencySorter($scope.model.currencies, 'Name');
                $scope.currencies = $scope.allCurrencies;
                $scope.CurrencyTypes = $scope.model.types;
                $scope.warnings = $scope.model.warnings;
            });
        };

        $scope.filter = function ($event) {
            $scope.currencies = $scope.currencyFilter($event.currentTarget.value);
        };

        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('CurrenciesController', viewName, $scope, size);
        };
        /*____________________________ Currency CRUD section ____________________________*/
        $scope.currencyAddFormInvoke = function (currency) {
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
            var newCurrencyExists = $scope.currencies.find(function(currency){
                return currency.CurrencyType === Number(newCurrency.CurrencyType);
            }) !== undefined;
            if (!newCurrencyExists) {
                $scope.postCurrency(newCurrency);
            } else {
                bootbox.alert($scope.warnings[0]);
            }
            };

        $scope.deleteCurrency = function (id) {
            $scope.currencies = $scope.currencies.filter(function (currency) {
                return currency.Id !== id;
            });
                CurrencyService.deleteCurrency({ delCurrencyId: id }, function () {
                    $scope.refresh();
                });
            };

            $scope.setMyCurrency = function (checked) {
                var unchecked = $scope.allCurrencies.find(function (currency) {
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
