(function () {
    'use strict';

    angular.module('MyOffice.app')
        .controller('counterCtrl', ['$scope', '$uibModal', 'ModalWindowService', counterCtrl]);

    function counterCtrl($scope, $uibModal, ModalWindowService) {
        $scope.MWC = ModalWindowService;
        $scope.model = {
            nominal: 1,
            data: JSON.parse(localStorage.getItem('counterCtrl.data'))
        };
        $scope.model.data = $scope.model.data === null ? [] : $scope.model.data;
        $scope.model.data.forEach(function (e) {
            e.count = 0;
        });

        $scope.openDialog = function () {
            ModalWindowService.open('CounterController', 'list', $scope, 'modal-sm');
        };

        $scope.save = function () {
            localStorage.setItem('counterCtrl.data', JSON.stringify($scope.model.data));
        };

        $scope.addBanknote = function () {
            $scope.model.data.push({ nominal: $scope.model.nominal, count: 0 });
            $scope.save();
        };

        $scope.deleteBanknote = function (idx) {
            $scope.model.data.splice(idx, 1);
            $scope.save();
        };

        $scope.next = function (event) {
            if (event.keyCode === 13) {
                var elements = document.getElementsByTagName('input');
                for (var i = 0; i < elements.length; i++) {
                    if (document.activeElement === elements[i]) {
                        elements[i + 3].focus();
                        break;
                    }
                }
            }
        };

        $scope.getTotal = function () {
            return $scope.model.data.length === 0 ? 0 : $scope.model.data.reduce(function (t, e) {
                return (t.nominal ? (t.nominal * t.count) : t) + (e.nominal * e.count);
            });
        };
    }
})();
