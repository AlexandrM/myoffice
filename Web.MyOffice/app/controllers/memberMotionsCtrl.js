(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('memberMotionsCtrl',
        [
            '$scope',
            '$rootScope',
            '$routeParams',
            'pdfService',
            'MemberDayReportService',
            'MemberMotionsService',
            'MemberPaymentService',
            memberMotionsCtrl
        ]);

    function memberMotionsCtrl($scope, $rootScope, $routeParams, pdfService, MemberDayReportService, MemberMotionsService, MemberPaymentService) {
        $scope.edDateFrom = localStorage.getItem('memberMotionsCtrl.edDateFrom') || moment().format('L');
        $scope.edDateTo = localStorage.getItem('memberMotionsCtrl.edDateTo') || moment().format('L');

        $scope.refresh = function () {
            localStorage.setItem('memberMotionsCtrl.edDateFrom', $scope.edDateFrom);
            localStorage.setItem('memberMotionsCtrl.edDateTo', $scope.edDateTo);
            $scope.project = MemberMotionsService.query(
                {
                    projectId: $routeParams.projectId,
                    currencyId: $routeParams.currencyId,
                    dateFrom: $scope.edDateFrom,
                    dateTo: $scope.edDateTo
                }, function () {
            });
        };

        $scope.toPDF = function () {
            pdfService.createPDF();
        };

        $scope.currencyById = function (id) {
            if ($scope.project.Currencies !== undefined) {
                return $.grep($scope.project.Currencies, function (e) {
                    return e.CurrencyType === id;
                })[0];
            }
        };

        $scope.myCurrency = function () {
            if ($scope.project.Currencies !== undefined) {
                return $.grep($scope.project.Currencies, function (e) {
                    return e.Value === 1;
                })[0];
            }
        };

        $scope.getTotal = function (reports) {
            if ($scope.project.Currencies === undefined) {
                return 0;
            }

            var total = 0;
            for (var i = 0; i < $scope.project.Motions.length; i++) {
                var rest = $scope.project.Motions[i];
                if ((reports) & (rest.Value !== undefined)) {
                    total += rest.Amount * rest.Value;
                }
                if ((!reports) & (rest.Value === undefined)) {
                    total += rest.Amount;
                }
            }
            return total;
        };

        $scope.toPDF = function () {
            pdfService.createPDF(
                $('#content'),
                'report.pdf',
                {
                    format: 'a4',
                    orientation: 'l'
                });
        };

        $scope.delete = function (motion, type) {
            bootbox.confirm(
                ASE.L.Delete + ': ' +
                motion.Description + ', ' +
                moment(motion.DateTime).format('L LT') + ', ' +
                ASE.L.Summ + ' ' + motion.Amount,
                function (result) {
                    if (result) {
                        if (type === 1) {
                            MemberDayReportService.delete({ id: motion.Id }, function () {
                                $scope.refresh();
                            });
                        }
                        if (type === 2) {
                            MemberPaymentService.delete({ id: motion.Id }, function () {
                                $scope.refresh();
                            });
                        }
                }
            });
        };
    };
})();
