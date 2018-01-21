(function() {
    'use strict';

    angular.module('MyOffice.app')
        .controller('budgetCtrl', [
            '$scope',
            '$http',
            '$ngBootbox',
            'ModalWindowService',
            'BudgetService',
            budgetCtrl]);

    function budgetCtrl($scope, $http, $ngBootbox, ModalWindowService, BudgetService) {

        $scope.MWS = ModalWindowService;

        $scope.refresh = function () {
            BudgetService.getUsersBudgets({}, function (data) {
                $scope.budgets = data.Budgets;
                $scope.userId = data.UserId;
            });
        };

        $scope.budgetAdd = function (budget) {
            $scope.newBudget = budget || {};
            ModalWindowService.open('UserBudgetsController', 'BudgetEdit', $scope, 'modal-sm');
        };

        $scope.budgetEdit = function (form) {
            form.$submited = true;
            if (form.$invalid) {
                return;
            }
            BudgetService.postBudget({ Id: $scope.newBudget.Id, Name: $scope.newBudget.Name }, function () {
                ModalWindowService.close('BudgetEdit');
                $scope.refresh();
            });
        };

        $scope.budgetDelete = function (budget) {
            $ngBootbox.confirm('Delete budget ' + budget.Name).then(function () {
                BudgetService.delete({ Id: budget.Id }, function (data) {
                    if (!data.ok) {
                        $ngBootbox.alert(data.message);
                    } else {
                        $scope.refresh();
                    }
                });
            }, function () { });
        };

        $scope.budgetUsers = function (budget) {
            $scope.currentBudget = budget;
            ModalWindowService.open('UserBudgetsController', 'BudgetUsers', $scope, 'modal-sm');
        };

        $scope.budgetUsersAdd = function (email, form) {
            form.$submited = true;
            if (!email) {
                return;
            }
            BudgetService.findUser({ email: email }, function (data) {
                if (!data.ok) {
                    $ngBootbox.alert(data.message);
                } else {
                    BudgetService.addUser({ userId: data.userId, budgetId: $scope.currentBudget.Id }, function (data) {
                        if (!data.ok) {
                            $ngBootbox.alert(data.message);
                        } else {
                            ModalWindowService.close('BudgetUsers');
                            $scope.refresh();
                        }
                    });
                }
            });
        };

        $scope.budgetUsersDelete = function (user) {
            $ngBootbox.confirm('Delete user ' + user.User.FullName).then(function () {
                BudgetService.deleteUser({ userId: user.User.MainMemberId }, function (data) {
                    if (!data.ok) {
                        $ngBootbox.alert(data.message);
                    } else {
                        ModalWindowService.close('BudgetUsers');
                        $scope.refresh();
                    }
                });
            }, function () { });
        };
    };
})();
