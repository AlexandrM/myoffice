﻿(function() {
    'use strict';

    var budgetApp = angular.module('MyOffice.app');
    budgetApp.controller('budgetCtrl', ['$scope', '$http', 'ModalWindowService', 'BudgetsService', budgetCtrl]);

    function budgetCtrl($scope, $http, ModalWindowService, BudgetsService) {
    /*______________Infrastructure section__________*/
        $scope.windowClose = function (name) {
            ModalWindowService.close(name);
        };

        $scope.propertyNameSorter = function (array, sortProperty) {
            var comparer = function (prev, next) {
                if (prev[sortProperty] < next[sortProperty]) {
                    return -1;
                } else if (prev[sortProperty] > next[sortProperty]) {
                    return 1;
                } else {
                    return 0;
                }
            };
            return array.sort(comparer);
        };

        $scope.propertyObjectNameSorter = function (array, property, sortProperty) {
            var comparer = function (prev, next) {
                if (prev[property][sortProperty] < next[property][sortProperty]) {
                    return -1;
                } else if (prev[property][sortProperty] > next[property][sortProperty]) {
                    return 1;
                } else {
                    return 0;
                }
            };
            return array.sort(comparer);
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
            return budgets.sort($scope.comparer);;
        };
        $scope.openForm = function (viewName, size) {
            ModalWindowService.open('UserBudgetsController', viewName, $scope, size);
        };
        /******************************Get Data********************************/
        $scope.refresh = function () {
            $scope.model = BudgetsService.getUsersBudgets({}, function () {
                $scope.budgetUsers = $scope.propertyObjectNameSorter($scope.model.budgetUsers, 'Budget', 'Name');
                $scope.users = $scope.propertyNameSorter($scope.model.users, 'Name');
                $scope.budgets = $scope.getBudgets($scope.budgetUsers);
            });

        };
        /*________________________Budget section______________________*/
        $scope.budgetAddFormInvoke = function () {
            $scope.newBudget = {};
            $scope.openForm('BudgetEdit', 'modal-sm');
        };

        $scope.budgetEditFormInvoke = function (_budget) {
            $scope.newBudget = _budget;
            $scope.openForm('BudgetEdit', 'modal-sm');
        };

        $scope.BudgetEdit = function () {
            BudgetsService.postBudget({
                Id: $scope.newBudget.Id,
                Name: $scope.newBudget.Name
            },
        function() {
            $scope.windowClose('BudgetEdit');
            $scope.refresh();
        });
        };

        /*________________________User section______________________*/
        $scope.getUsersBudgets = function(budgetId) {
            var usersForBudget = $scope.budgetUsers.filter(function (budgetUser) {
                return budgetUser.BudgetId === budgetId;
            });
            return $scope.users.filter(function(user) {
                return usersForBudget.some(function(userBudget) {
                    return userBudget.UserId === user.Id;
                });
            });
        };

        $scope.budgetUsersFormInvoke = function (_budget) {
            $scope.currentBudget = _budget;
            $scope.usersForBudget = $scope.getUsersBudgets(_budget.Id);
            $scope.openForm('BudgetUsers', 'modal-sm');
        };

        $scope.newBudgetUsers = [];
        $scope.BudgetUsersAdd = function (newUserStr) {
            if (newUserStr !== undefined) {
                var newUser = JSON.parse(newUserStr);
                $scope.newBudgetUsers.push(newUser);
                $scope.usersForBudget.push(newUser);
                $scope.usersForBudget = $scope.usersForBudget.sort($scope.nameComparer);
            };
        };

        $scope.delBudgetUsers = [];
        $scope.BudgetUsersDelete = function (delUser) {
            if (delUser !== undefined) {
                $scope.newBudgetUsers = $scope.newBudgetUsers.filter(function (budgetUser) {
                    return delUser.Id !== budgetUser.Id;
                });
                $scope.usersForBudget = $scope.usersForBudget.filter(function (budgetUser) {
                    return delUser.Id !== budgetUser.Id;
                }).sort($scope.nameComparer);
                $scope.delBudgetUsers.push(delUser);
            }
        };

        $scope.updateBudgetList = function() {
            BudgetsService.putBudgetUserList({
                NewUsers: $scope.newBudgetUsers,
                DelUsers: $scope.delBudgetUsers,
                BudgetId: $scope.currentBudget.Id
        }, function () {
                $scope.newBudgetUsers = [];
                $scope.delBudgetUsers = [];
                $scope.windowClose('BudgetUsers');
                $scope.refresh();
            });
        };
    };
})();