(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('itemCtrl', ['$scope', '$timeout', 'ModalWindowService', 'itemService', 'motionService', itemCtrl]);

    function itemCtrl($scope, $timeout, ModalWindowService, itemService, motionService) {

        $scope.MWS = ModalWindowService;
        $scope.selectedCategory = {};

        $scope.refresh = function () {
            $scope.userBudgets = itemService.categoryList({}, function (data) {
                $scope.Categories = data.Categories;
                $scope.Budgets = data.Budgets;
                var c = $scope.Categories.find(function (val) {
                    return val.Id === $scope.selectedCategory.Id;
                });
                if (c !== undefined) {
                    $scope.selectCategory(c);
                }
            });
        };

        $scope.selectCategory = function (category) {
            $scope.selectedCategory = category;
            $scope.userBudgets = itemService.itemsList({ categoryId: category.Id }, function (data) {
                $scope.Items = data.Items;
            });
        };

        $scope.categoryItemEditDialog = function (category) {
            $scope.newCategory = category || {};
            ModalWindowService.open('ExpenditureController', 'CategoryItemEdit', $scope, 'modal-sm');
        };

        $scope.newCategorySave = function (category) {
            itemService.categoryPost(category, function () {
                $scope.refresh();
                ModalWindowService.close('CategoryItemEdit');
            });
        };

        $scope.deleteCategory = function () {
            itemService.categoryDelete({ categoryId: $scope.selectedCategory.Id }, function () {
                $scope.selectedCategory = {};
                $scope.refresh();
            });
        };

        $scope.addItemDialog = function () {
            $scope.newItem = {};
            ModalWindowService.open('ExpenditureController', 'ItemAdd', $scope, 'modal-sm');
        };

        $scope.itemEditDialog = function (newItem) {
            $scope.newItem = newItem;
            ModalWindowService.open('ExpenditureController', 'ItemAdd', $scope, 'modal-sm');
        };

        $scope.addItem = function (item) {
            item.BudgetId = $scope.Categories.find(function (val) {
                return val.Id === item.CategoryId;
            }).BudgetId;

            itemService.itemAdd(item, function () {
                ModalWindowService.close('ItemAdd');
                $scope.refresh();
            });
        };

        $scope.itemDelete = function (item, del) {
            itemService.itemDelete({ itemId: item.Id, delete: del }, function () {
                $scope.refresh();
            });
        };

        $scope.itemMotionList = function (item) {
            motionService.motionList({ itemId: item.Id }, function (data) {
                $scope.Motions = data;
                ModalWindowService.open('MotionsList');
            });
        };
    };
})();
