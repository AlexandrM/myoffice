(function () {

    'use strict';

    var accountApp = angular.module('MyOffice.app');
    accountApp.controller('budgetAccountCtrl', ['$scope', '$http','$q', 'ModalWindowService', 'budgetAccountsService', budgetAccountCtrl]);

    function budgetAccountCtrl($scope, $http, $q, ModalWindowService, budgetAccountsService) {
        /*________________________Infrastructure section________________________*/
        $scope.showCategory = function (category) {
            if (category === null || category === undefined) {
                 return;
            }
            var currentAccount = null;
            var motionPromise = [];
            for (var i = 0; i < category.Accounts.length; i++) {
                currentAccount = category.Accounts[i];
                motionPromise.push(
                    currentAccount.HasMotions = budgetAccountsService.getAccountMotionsFlag({ accountId: currentAccount.Id })
                );
            }
            $scope.selectedCategory = category;
        };

        $scope.windowClose = function (name) {
            ModalWindowService.close(name);
        };

        $scope.categoryDeletion = function(category) {
            var deletion = category.Accounts.length === 0;
            if (!deletion) {
                deletion = category.Accounts.filter(function(acc) {
                    return acc.Deleted;
                }).length === category.Accounts.length;
            }
            return deletion;
        };

        /*________________________***Update data***________________________*/
        $scope.refresh = function () {
            budgetAccountsService.getUsersBudgets({},
                function (data) {
                    $scope.currencies = data.Currencies;
                    $scope.categories = data.Categories;
                    $scope.budgets = data.Budgets;
                    $scope.showCategory($scope.categories[0]);
                });
        };

        /*________________________Account section________________________*/
        $scope.newAccountAdd = function (newAccount) {
            newAccount.CategoryId = newAccount.Category.Id;
            newAccount.BudgetId = newAccount.Category.BudgetId;
            newAccount.CurrencyId = newAccount.Currency.Id;
            newAccount.Category = null;
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
                        $scope.windowClose('AccountEdit');
                        var accountCategory = $scope.categories.find(function(cat) {
                            return cat.Id === newAccount.categoryId;
                        });
                        $scope.showCategory(accountCategory);
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
        $scope.accountEditDialog = function (account) {
            $scope.newAccount = account;
            $scope.newAccount.Category = $scope.categories.find(function (cat) {
                return cat.Id === $scope.newAccount.CategoryId;
            });
            $scope.openForm('AccountEdit', 'modal-sm');
        };

        /*****************************Form get method code****************************/
        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('AccountsController', viewName, $scope ,size);
        };
        /*________________________Category section________________________*/
        $scope.categoryEditDialog = function (category) {
            $scope.newCategory = category;
            $scope.openForm('CategoryEdit', 'modal-sm');
        };
        $scope.categoryListDialog = function () {
            $scope.openForm('CategoryList', 'modal-sm');
        };
        $scope.categoryDelete = function (delCategory) {
            budgetAccountsService.deleteCategory(
                {
                    deleteId: delCategory.Id,
                    totalDelete: false
                },
                function() {
                    $scope.refresh();
                });
        };

        $scope.categoryUpdate = function (newCategory) {
            newCategory.BudgetId = newCategory.Budget.Id;
            newCategory.Budget = null;
            budgetAccountsService.postCategory({
                Id:newCategory.Id,
                Name: newCategory.Name,
                Description: newCategory.Description,
                BudgetId: newCategory.BudgetId
                },
                function() {
                    $scope.refresh();
                    $scope.windowClose('CategoryEdit');
                });
        };
    };
})();
