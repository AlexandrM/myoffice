(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('debtsCtrl', ['$scope', '$rootScope', '$routeParams', 'DebtsService', 'MemberDayReportService', 'pdfService', debtsCtrl]);

    function debtsCtrl($scope, $rootScope, $routeParams, DebtsService, MemberDayReportService, pdfService) {
        $scope.refresh = function () {
            $scope.list = DebtsService.get({ mode: $routeParams.mode }, function () {

                $scope.currencyById = function (id) {
                    return $.grep($scope.list.Currencies, function (e) {
                        return e.CurrencyType === id;
                    })[0];
                };

                $scope.myCurrency = function () {
                    return $.grep($scope.list.Currencies, function (e) {
                        return e.MyCurrency;
                    })[0];
                };

                $scope.mode = function () {
                    return $routeParams.mode;
                };

                $scope.getTotal = function () {
                    var total = 0;
                    for (var i = 0; i < $scope.list.Rests.length; i++) {
                        var rest = $scope.list.Rests[i];
                        total += rest.Amount * $scope.currencyById(rest.Project.RateCurrencyType).Value;
                    }
                    return total;
                };

                $scope.getCurrencyTotal = function (currency) {
                    var total = 0;
                    for (var i = 0; i < $scope.list.Rests.length; i++) {
                        var rest = $scope.list.Rests[i];
                        if (currency.CurrencyType === rest.Project.RateCurrencyType) {
                            total += rest.Amount;
                        }
                    }
                    return total;
                };

                $scope.toPDF = function () {
                    pdfService.createPDF();
                };

                $scope.myMemberType = function (project) {
                    return $.grep(project.Members, function (e) {
                        if (e.Member != null) {
                            return e.Member.Id === ASE.UserId;
                        }
                    })[0];
                };
            });
        };
    };
})();
