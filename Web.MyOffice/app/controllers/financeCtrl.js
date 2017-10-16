(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('financeCtrl',
        [
            '$scope',
            '$rootScope',
            '$routeParams',
            'FinanceService',
            'MemberDayReportService',
            'pdfService',
            'emailService',
            financeCtrl
        ]);

    function financeCtrl($scope, $rootScope, $routeParams, FinanceService, MemberDayReportService, pdfService, emailService) {
        $scope.refresh = function () {
            $scope.project = FinanceService.get({ id: $routeParams.id }, function () {

                $scope.currencyById = function (id) {
                    return $.grep($scope.project.Currencies, function (e) {
                        return e.CurrencyType === id;
                    })[0];
                };
                $scope.myCurrency = function () {
                    return $.grep($scope.project.Currencies, function (e) {
                        return e.Value === 1;
                    })[0];
                };
                $scope.myMemberType = function () {
                    return $.grep($scope.project.Members, function (e) {
                        if (e.Member !== null) {
                            return e.Member.Id === ASE.UserId;
                        }
                    })[0];
                };
                $scope.deleteDR = function (memberDayReport) {
                    bootbox.confirm(
                        ASE.L.Delete + ': ' +
                        moment(memberDayReport.DateTime).format('L LT') + '.' +
                        memberDayReport.Description,
                        function (result) {
                            if (result) {
                                MemberDayReportService.delete({ id: memberDayReport.Id }, function () {
                                    $.each($scope.project.Rests, function (index, rest) {
                                        $.each(rest.Details, function (index, value) {
                                            if ((value !== undefined) && (value.Id === memberDayReport.Id)) {
                                                rest.Details.splice(index, 1);
                                                return;
                                            }
                                        });
                                    });
                                });
                        }
                    });
                };
                $scope.sendPDF = function (project, member) {
                    bootbox.confirm(
                        ASE.L.SendTo + ': ' +
                        member.Member.Email + ' (' +
                        member.MemberTypeS + ', ' +
                        member.Member.FullName + ')',
                        function (result) {
                            if (result) {
                                pdfService.createPDF(
                                    $('#content'),
                                    function (doc) {
                                        var reader = new window.FileReader();
                                        reader.readAsDataURL(doc.output('blob'));

                                        reader.onloadend = function () {
                                            emailService.send(
                                                1,
                                                project.Id,
                                                member.Member.Id,
                                                [reader.result]
                                                );
                                        };
                                    },
                                    {
                                        format: 'a4',
                                        orientation: 'l'
                                    });
                            }
                    });
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
            });
        };
    };
})();
