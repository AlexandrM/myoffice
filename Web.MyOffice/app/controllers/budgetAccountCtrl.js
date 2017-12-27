(function () {

    'use strict';

    var accountApp = angular.module('MyOffice.app');
    accountApp.controller('budgetAccountCtrl', ['$scope', '$http','$q', 'ModalWindowService', 'budgetAccountsService', budgetAccountCtrl]);

    function budgetAccountCtrl($scope, $http, $q,ModalWindowService, budgetAccountsService) {
        /*________________________Infrastructure section________________________*/
        $scope.getCategories = function (budgetUsers) {
            var categories = [];
            var currentBudgetCategories = null;
            for (var i = 0; i < budgetUsers.length; i++) {
                currentBudgetCategories = budgetUsers[i].Budget.CategoryAccounts;
                for (var j = 0; j < currentBudgetCategories.length; j++) {
                    if (categories.find(function (_category) {
                        return _category.Id === currentBudgetCategories[j].Id;
                    }) === undefined) {
                        categories.push(currentBudgetCategories[j]);
                    };
                };
            };
            return categories.sort($scope.comparer);
        };
        $scope.getBudgets = function (users) {
            var budgets = [];
            for (var i = 0; i < users.length; i++) {
                if (budgets.find(function (_budget) {
                    return _budget.Id === users[i].Budget.Id;
                }) === undefined) {
                    budgets.push(users[i].Budget);
                };
            };
            return budgets.sort($scope.comparer);
        };
        $scope.getAccounts = function (categories) {
            var accounts = [];
            var acc = null;
            for (var i = 0; i < categories.length; i++) {
                for (var j = 0; j < categories[i].Accounts.length; j++) {
                    acc = categories[i].Accounts[j];
                    acc.Motions = acc.Motions === undefined ? [] : acc.Motions;
                    acc.Motions = acc.Motions === null ? [] : acc.Motions;
                    accounts.push(acc);
                };
            };
            return accounts.sort($scope.comparer);
        };
        $scope.nameComparer = function (prev, next) {
            if (prev.Name < next.Name) {
                return -1;
            } else if (prev.Name > next.Name) {
                return 1;
            } else {
                return 0;
            }
        };

        $scope.sortData = function (data, comparer) {
            data = data.sort(comparer);
            return data;
        };

        $scope.showCategory = function (category) {
            var currentAccount = null;
            for (var i = 0; i < category.Accounts.length; i++) {
                currentAccount = category.Accounts[i];
                currentAccount.Motions = budgetAccountsService.getAccountMotions({ accountId: currentAccount.Id });
            }
            $scope.selectedCategory = category;
        };

        $scope.windowClose = function (name) {
            ModalWindowService.close(name);
        };

        /*________________________***Update data***________________________*/
        $scope.refresh = function () {
            $scope.model = budgetAccountsService.getUsersBudgets({},
                function () {
                    $scope.budgets = $scope.getBudgets($scope.model.BudgetUsers);
                    $scope.categories = $scope.getCategories($scope.model.BudgetUsers);
                    $scope.accounts = $scope.getAccounts($scope.categories);

                    $scope.sortData($scope.budgets, $scope.nameComparer);
                    $scope.sortData($scope.categories, $scope.nameComparer);
                    for (var i = 0; i < $scope.categories.length; i++) {
                        $scope.sortData($scope.categories[i].Accounts, $scope.nameComparer);
                    };
                    $scope.showCategory($scope.categories[0]);
                });
        };

        /*________________________Account section________________________*/
        $scope.newAccountAdd = function (form, newFormAccount) {
            if (form.$valid) {
                budgetAccountsService.putAccount({
                    Id: newFormAccount.Id === undefined ? null : newFormAccount.Id,
                    Name: newFormAccount.Name,
                    BudgetId: newFormAccount.BudgetId === undefined ? newFormAccount.Category.BudgetId : newFormAccount.BudgetId,
                    CategoryId: newFormAccount.Category.Id ,
                    CurrencyId: newFormAccount.CurrencyId === undefined ? newFormAccount.Currency : newFormAccount.CurrencyId,
                    CreditLimit: newFormAccount.CreditLimit,
                    Description: newFormAccount.Description,
                    Deleted: newFormAccount.Deleted,
                    ShowInRest: newFormAccount.ShowInRest
                    },
                    function () {
                        $scope.windowClose('AccountAdd');
                        $scope.selectedCategory = $scope.categories.find(function(cat) {
                            return cat.Id === newFormAccount.CategoryId;
                        });
                        $scope.refresh();
                    });
            };
        };
        $scope.accountDelete = function (account) {
            $scope.selectedCategory.Accounts = $scope.selectedCategory.Accounts.filter(function (_account) {
                return _account.Name !== account.Name ||
                    _account.Description !== account.Description ||
                    _account.Currency !== account.Currency ||
                    _account.BudgetId !== account.BudgetId;
            });
            budgetAccountsService.deleteAccount({ accountId: account.Id }, function () {
                $scope.refresh();
            });
        };
        $scope.AccountFormAddInvoke = function () {
            $scope.newAccount = {};
            $scope.newAccountWindow = $scope.openForm('AccountAdd', 'modal-sm');
        };
        $scope.accountFormEditInvoke = function (account) {
            $scope.newAccount = account;
            $scope.newAccount.Category = $scope.categories.find(function (cat) {
                return cat.Id === $scope.newAccount.CategoryId;
            });
            $scope.openForm('AccountAdd', 'modal-sm');
        };

        /*****************************Form get method code****************************/
        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('AccountsController', viewName, $scope ,size);
        };

        /*________________________Category section________________________*/
        $scope.categoryFormAdd = function () {
            $scope.newCategory = {};
            $scope.openForm('CategoryAdd', 'modal-sm');
        };
        $scope.categoryFormEdit = function (category) {
            $scope.newCategory = category;
            $scope.openForm('CategoryEdit', 'modal-sm');
        };
        $scope.categoryListFormInvoke = function () {
            $scope.openForm('CategoryList', 'modal-sm');
        };
        $scope.categoryAddFormInvoke = function () {
            $scope.newCategory = {};
            $scope.openForm('CategoryAdd', 'modal-sm');
        };
        $scope.newCategories = [];
        $scope.CategoryAdd = function (newCategory) {
            newCategory.Accounts = [];
            newCategory.BudgetId = newCategory.Budget.Id;
            newCategory.Budget = undefined;

            $scope.newCategories.push(newCategory);
            $scope.categories.push(newCategory);
            $scope.windowClose('CategoryAdd');
        };
        $scope.editCategories = [];
        $scope.categoryEdit = function (editCategory) {
            if ($scope.newCategories.filter(function (_category) {
                return _category.Name === editCategory.Name ||
                    _category.BudgetId === editCategory.BudgetId ||
                    _category.Descripition === editCategory.Descripition;
            })) {
                editCategory.Budget = undefined;
                $scope.editCategories.push(editCategory);
            }
            $scope.windowClose('CategoryEdit');
        };
        $scope.delCategories = [];
        $scope.categoryDelete = function (delCategory) {
            $scope.newCategories = $scope.newCategories.filter(function (_category) {
                return _category.Name !== delCategory.Name ||
                    _category.BudBudgetId !== delCategory.BudgetId ||
                    _category.Descripition !== delCategory.Descripition;
            });
            $scope.categories = $scope.categories.filter(function (_category) {
                return _category.Name !== delCategory.Name ||
                    _category.BudgetId !== delCategory.BudgetId ||
                    _category.Descripition !== delCategory.Descripition;
            });
            $scope.delCategories.push(delCategory);
        };
        $scope.updateCategoryList = function () {
            budgetAccountsService.postCategoryList({
                NewCategories: $scope.newCategories,
                EditCategories: $scope.editCategories,
                DelCategories: $scope.delCategories
             },
                function() {
                    $scope.refresh();
                    $scope.newCategories = [];
                    $scope.editCategories = [];
                    $scope.delCategories = [];
                    $scope.windowClose('CategoryList');
                });
        };
    };
})();
