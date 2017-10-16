(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('memberPaymentCtrl',
        [
            '$scope',
            '$rootScope',
            '$routeParams',
            '$location',
            'MemberPaymentService',
            memberPaymentCtrl
        ]);

    function memberPaymentCtrl($scope, $rootScope, $routeParams, $location, MemberPaymentService) {
        $scope.refresh = function () {
            $scope.memberPayment = MemberPaymentService.get(
                {
                    id: $routeParams.id,
                    projectId: $routeParams.projectId,
                    currencyId: $routeParams.currencyId
                }, function () {
            });
        };

        $scope.addMember = function () {
            $rootScope.$broadcast('refreshMembersEvent', {});
            $('#dlgM').modal({
            });
        };

        $scope.$on('selectMemberEvent', function (event, args) {
            $('#dlgM').modal('hide');
            $scope.memberPayment.Member = args.member;
        });

        $scope.addCurrency = function () {
            $rootScope.$broadcast('refreshCurrencyEvent', {});
            $('#dlgC').modal({
            });
        };

        $scope.$on('selectCurrencyEvent', function (event, args) {
            $('#dlgC').modal('hide');
            $scope.memberPayment.Currency = args.currency;
        });

        $scope.save = function () {
            if ($routeParams.id === '0') {
                $scope.project = MemberPaymentService.post($scope.memberPayment, function () {
                    $location.path('/finance/' + $scope.memberPayment.ProjectId);
                });
            } else {
                $scope.project = MemberPaymentService.put($scope.memberPayment, function () {
                    $location.path('/finance/' + $scope.memberPayment.ProjectId);
                });
            }
        };

        $scope.edDateFrom = localStorage.getItem('memberPaymentCtrl.edDateFrom') || moment().format('L');
        $scope.edDateTo = localStorage.getItem('memberPaymentCtrl.edDateTo') || moment().format('L');

        $scope.refreshList = function () {
            localStorage.setItem('memberPaymentCtrl.edDateFrom', $scope.edDateFrom);
            localStorage.setItem('memberPaymentCtrl.edDateTo', $scope.edDateTo);
            $scope.memberPayments = MemberPaymentService.query({ dateFrom: $scope.edDateFrom, dateTo: $scope.edDateTo }, function () {
                $scope.getTotal = function () {
                    var total = 0;
                    for (var i = 0; i < $scope.memberPayments.Details.length; i++) {
                        var rest = $scope.memberPayments.Details[i];
                        total += rest.Amount * $scope.currencyById(rest.Project.RateCurrencyType).Value;
                    }
                    return total;
                };
            });
        };

        $scope.myCurrency = function () {
            if ($scope.memberPayments.Currencies === undefined) {
                return;
            }
            return $.grep($scope.memberPayments.Currencies, function (e) {
                return e.Value === 1;
            })[0];
        };

        $scope.currencyById = function (id) {
            if ($scope.memberPayments.Currencies === undefined) {
                return;
            }
            return $.grep($scope.memberPayments.Currencies, function (e) {
                return e.CurrencyType === id;
            })[0];
        };

        $scope.UserId = ASE.UserId;

        $scope.delete = function (memberPayment) {
            bootbox.confirm(
                ASE.L.Delete + ': ' +
                moment(memberPayment.DateTime).format('L LT') + '. ' +
                memberPayment.Amount + ' ' +
                $scope.currencyById(memberPayment.Project.RateCurrencyType).ShortName,
                function (result) {
                    if (result) {
                        MemberPaymentService.delete({ id: memberPayment.Id }, function () {
                            $scope.refreshList();
                        });
                }
            });
        };
    };
})();
