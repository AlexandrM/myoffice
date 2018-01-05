(function () {

    'use strict';

    angular.module('MyOffice.app').controller('budgetAccountCtrl',
        ['$scope', '$http', '$q', '$timeout', 'ModalWindowService', 'budgetAccountsService', budgetAccountCtrl]);

    function budgetAccountCtrl($scope, $http, $q, $timeout, ModalWindowService, budgetAccountsService) {

        $scope.activeTab = 1;
        $scope.MWS = ModalWindowService;

        $scope.refresh = function () {
            budgetAccountsService.getUsersBudgets({},
                function (data) {
                    $scope.currencies = data.Currencies;
                    $scope.categories = data.Categories;
                    $scope.budgets = data.Budgets;
                    var tab = $scope.activeTab;
                    $scope.activeTab = 1;
                    $timeout(function () {
                        $scope.activeTab = tab;
                    });
                });
        };

        $scope.categoryListDialog = function () {
            ModalWindowService.open('AccountsController', 'CategoryList', $scope, 'modal-sm');
        };

        $scope.categoryEditDialog = function (category) {
            $scope.newCategory = angular.copy(category);
            ModalWindowService.open('AccountsController', 'CategoryEdit', $scope, 'modal-sm');
        };

        $scope.categoryUpdate = function (newCategory,form) {
            if (form.$valid) {
            budgetAccountsService.postCategory({
                Id: newCategory.Id,
                BudgetId: newCategory.BudgetId,
                Name: newCategory.Name,
                Description: newCategory.Description,
            },
                function () {
                    $scope.refresh();
                    ModalWindowService.close('CategoryEdit');
                    ModalWindowService.close('CategoryList');
                });
            }
        };

        $scope.categoryDelete = function (category) {
            budgetAccountsService.deleteCategory({
                deleteId: category.Id,
                totalDelete: false
            },
            function () {
                $scope.refresh();
            });
        };

        $scope.accountEditDialog = function (account) {
            $scope.newAccount = angular.copy(account);
            ModalWindowService.open('AccountsController', 'AccountEdit', $scope, 'modal-sm');
        };

        $scope.newAccountAdd = function (newAccount) {
            newAccount.BudgetId = $scope.categories.find(function (val) {
                return val.Id = newAccount.CategoryId;
            }).BudgetId;

            budgetAccountsService.putAccount({
                    Id: newAccount.Id,
                    Name: newAccount.Name,
                    Description: newAccount.Description,
                    CreditLimit: newAccount.CreditLimit,
                    CategoryId: newAccount.CategoryId,
                    BudgetId: newAccount.BudgetId,
                    CurrencyId: newAccount.CurrencyId
                },
                    function () {
                        ModalWindowService.close('AccountEdit');
                        $scope.refresh();
                    });
        };

        $scope.accountDelete = function (account) {
            budgetAccountsService.deleteAccount({
                accountId: account.Id
            }, function () {
                $scope.refresh();
            });
        };
    };
})();
