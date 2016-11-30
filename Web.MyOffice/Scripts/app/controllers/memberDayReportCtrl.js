"use strict;"

controllers.controller('memberDayReportCtrl', ['$scope', '$rootScope', '$routeParams', '$location', 'MemberDayReportService',
    function ($scope, $rootScope, $routeParams, $location, MemberDayReportService) {
        $scope.refresh = function () {
            $scope.memberDayReport = MemberDayReportService.get({ id: $routeParams.id, projectId: $routeParams.projectId, currencyType: $routeParams.currencyType }, function () {
            });
        };
        $scope.addMember = function (memberDayReport) {
            $rootScope.$broadcast("refreshMembersEvent", {});
            $('#dlgM').modal({
            });
        };
        $scope.$on("selectMemberEvent", function (event, args) {
            $('#dlgM').modal('hide');
            $scope.memberDayReport.Member = args.member;
        });
        $scope.addCurrency = function (memberDayReport) {
            $rootScope.$broadcast("refreshCurrencyEvent", {});
            $('#dlgC').modal({
            });
        };
        $scope.$on("selectCurrencyEvent", function (event, args) {
            $('#dlgC').modal('hide');
            $scope.memberDayReport.Currency = args.currency;
        });
        $scope.save = function () {
            if ($routeParams.id == 0)
                $scope.project = MemberDayReportService.post($scope.memberDayReport, function () {
                    $location.path("/finance/" + $scope.memberDayReport.ProjectId);
                });
            else
                $scope.project = MemberDayReportService.put($scope.memberDayReport, function () {
                    $location.path("/finance/" + $scope.memberDayReport.ProjectId);
                });
        };

        $scope.edDateFrom = localStorage.getItem("memberDayReportCtrl.edDateFrom") || moment().format('L');
        $scope.edDateTo = localStorage.getItem("memberDayReportCtrl.edDateTo") || moment().format('L');
        $scope.refreshList = function () {
            localStorage.setItem("memberDayReportCtrl.edDateFrom", $scope.edDateFrom);
            localStorage.setItem("memberDayReportCtrl.edDateTo", $scope.edDateTo);
            $scope.memberDayReports = MemberDayReportService.query({ dateFrom: $scope.edDateFrom, dateTo: $scope.edDateTo }, function () {
                $scope.getTotal = function () {
                    var total = 0;
                    for (var i = 0; i < $scope.memberDayReports.Details.length; i++) {
                        var item = $scope.memberDayReports.Details[i];
                        total += item.Amount * item.Value * $scope.currencyById(item.Project.RateCurrencyType).Value;
                    }
                    return total;
                };
            });
        };
        $scope.myCurrency = function () {
            if ($scope.memberDayReports.Currencies === undefined)
                return;
            return $.grep($scope.memberDayReports.Currencies, function (e) {
                return e.MyCurrency;
            })[0];
        };
        $scope.currencyById = function (id) {
            if ($scope.memberDayReports.Currencies === undefined)
                return;
            return $.grep($scope.memberDayReports.Currencies, function (e) {
                return e.CurrencyType == id;
            })[0];
        };
        $scope.UserId = ASE.UserId;
        $scope.delete = function (memberDayReport) {
            bootbox.confirm(ASE.L.Delete + ': ' + moment(memberDayReport.DateTime).format("L LT") + ". " + (memberDayReport.Amount * memberDayReport.Value) + ' ' + $scope.currencyById(memberDayReport.Project.RateCurrencyType).ShortName, function (result) {
                if (result) {
                    MemberDayReportService.delete({ id: memberDayReport.Id }, function () {
                        $scope.refreshList();
                    });
                }
            });
        };
    }]);
