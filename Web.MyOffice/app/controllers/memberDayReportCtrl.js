(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('memberDayReportCtrl',
    [
        '$scope',
        '$rootScope',
        '$routeParams',
        '$location',
        'MemberDayReportService',
        'BotIterationService',
        memberDayReportCtrl
    ]);

    function memberDayReportCtrl($scope,
        $rootScope,
        $routeParams,
        $location,
        MemberDayReportService,
        BotIterationService) {

        $scope.memberDayReport = {
            Amount: 0,
            AmountH: 0,
            AmountM: 0,
        };

        $scope.refresh = function () {
            MemberDayReportService.get(
                {
                    id: $routeParams.id,
                    projectId: $routeParams.projectId,
                    currencyType: $routeParams.currencyType
                }, function (data) {
                    $scope.memberDayReport = data;
                    $scope.calcAmount(true);
                });
        };

        $scope.calcAmount = function (fromAmount) {
            if (fromAmount) {
                var amount = $scope.memberDayReport.Amount;
                var h = Math.floor(amount);
                var min = Math.round((amount - h) * 60);
                $scope.memberDayReport.AmountH = h;
                $scope.memberDayReport.AmountM = min;
            } else {
                $scope.memberDayReport.Amount = $scope.memberDayReport.AmountH + (Math.round($scope.memberDayReport.AmountM / 60 * 100) / 100);
            }
        };

        $scope.addMember = function () {
            $rootScope.$broadcast('refreshMembersEvent', {});
            $('#dlgM').modal({
            });
        };

        $scope.$on('selectMemberEvent', function (event, args) {
            $('#dlgM').modal('hide');
            $scope.memberDayReport.Member = args.member;
        });

        $scope.addCurrency = function () {
            $rootScope.$broadcast('refreshCurrencyEvent', {});
            $('#dlgC').modal({
            });
        };

        $scope.$on('selectCurrencyEvent', function (event, args) {
            $('#dlgC').modal('hide');
            $scope.memberDayReport.Currency = args.currency;
        });

        $scope.save = function () {
            if ($routeParams.id.toString() === '0') {
                $scope.project = MemberDayReportService.post($scope.memberDayReport, function () {
                    $location.path('/finance/' + $scope.memberDayReport.ProjectId);
                });
            } else {
                $scope.project = MemberDayReportService.put($scope.memberDayReport, function () {
                    $location.path('/finance/' + $scope.memberDayReport.ProjectId);
                });
            }
        };

        $scope.edDateFrom = localStorage.getItem('memberDayReportCtrl.edDateFrom') || moment().format('L');
        $scope.edDateTo = localStorage.getItem('memberDayReportCtrl.edDateTo') || moment().format('L');

        $scope.refreshList = function () {
            localStorage.setItem('memberDayReportCtrl.edDateFrom', $scope.edDateFrom);
            localStorage.setItem('memberDayReportCtrl.edDateTo', $scope.edDateTo);
            $scope.memberDayReports = MemberDayReportService.query({ dateFrom: $scope.edDateFrom, dateTo: $scope.edDateTo }, function () {
                $scope.setMyCurrencyRates = function (currencies) {
                    var myCurrencyRate = $scope.myCurrency().Value;
                    for (var i = 0; i < currencies.length; i++) {
                        currencies[i].Value = currencies[i].Value / myCurrencyRate;
                    };
                };

                $scope.getTotal = function (reports) {
                    var total = 0;
                    if (reports != null) {
                        for (var i = 0; i < reports.length; i++) {
                            total += reports[i].Amount * reports[i].Value * $scope.currencyById(reports[i].Project.RateCurrencyType).Value;
                        }
                    }
                    return total;
                };

                $scope.getTotalDate = function (dateString1) {
                    var reports = $scope.memberDayReports.Details.filter(function(elem) {
                        return $scope.dateCompare(elem.DateTime, dateString1);
                    });
                    return $scope.getTotal(reports);
                };
            });
        };

        $scope.dateCompare = function (dateString1, dateString2) {
            var date1 = new Date(dateString1);
            var date2 = new Date(dateString2);
            return date1.getDay()   === date2.getDay() &&
                   date1.getMonth() === date2.getMonth() &&
                   date1.getYear()  === date2.getYear();
        };

        $scope.ToggleDayTotals = function($route) {
            $scope.ShowDayTotalsFlag = !$scope.ShowDayTotalsFlag;
            $route.reload();
        };
        $scope.ShowDayTotalsFlag = true;
        $scope.ShowDayTotals = function (currentDetails) {
            var curIndex = $scope.memberDayReports.Details.indexOf(currentDetails);
            if (curIndex + 1 >= $scope.memberDayReports.Details.length) {
                return $scope.ShowDayTotalsFlag;
            }else {
                var nextDetails = $scope.memberDayReports.Details[curIndex + 1];
                if (!$scope.dateCompare(currentDetails.DateTime, nextDetails.DateTime)) {
                    return $scope.ShowDayTotalsFlag;
                } else {
                    return false;
                };
            };
            return false;
        };

        $scope.myCurrency = function () {
            if ($scope.memberDayReports.Currencies === undefined) {
                return;
            }
            return $.grep($scope.memberDayReports.Currencies, function (e) {
                return e.MyCurrency;
            })[0];
        };

        $scope.currencyById = function (id) {
            if ($scope.memberDayReports.Currencies === undefined) {
                return;
            }
            return $.grep($scope.memberDayReports.Currencies, function (e) {
                return e.CurrencyType === id;
            })[0];
        };

        $scope.UserId = ASE.UserId;

        $scope.delete = function (memberDayReport) {
            bootbox.confirm(
                ASE.L.Delete + ': ' +
                moment(memberDayReport.DateTime).format('L LT') + '. ' +
                (memberDayReport.Amount * memberDayReport.Value) + ' ' +
                $scope.currencyById(memberDayReport.Project.RateCurrencyType).ShortName,
                function (result) {
                    if (result) {
                        MemberDayReportService.delete({ id: memberDayReport.Id }, function () {
                            $scope.refreshList();
                        });
                }
            });
        };

        $scope.importData = [];
        $scope.importModel = {
            Round: '15',
            From: new Date(),
            To: new Date(),
            Day: new Date(),
        };

        $scope.importRefresh = function (day) {
            if (day) {
                $scope.importModel.From = new Date($scope.importModel.Day);
                $scope.importModel.To = new Date($scope.importModel.Day);
            }
            $scope.importModel.From.setHours(0, 0, 0, 0);
            $scope.importModel.To.setHours(23, 59, 59, 999);
            $scope.importModel.UTC = $scope.memberDayReport.Project.UTC;
            $scope.importModel.Id = $scope.memberDayReport.Project.BotId;

            BotIterationService.query($scope.importModel, function (data) {
                $scope.importData = data;
            });
        };

        $scope.importStart = function () {
            $('#import').modal();
            $scope.importRefresh();
        };

        $scope.realUsed = function (iteration) {
            var started = moment(iteration.started).toDate();
            var stoped = moment(iteration.stoped).toDate();
            if (stoped > moment().toDate()) {
                return 0;
            }
            return Math.ceil(Math.abs(stoped.getTime() - started.getTime()) / (1000 * 60));
        };

        $scope.total = function (all) {
            var total = 0;
            for (var i = 0; i < $scope.importData.length; i++) {
                if ((all) || (!$scope.importData[i].skip)) {
                    total = total + $scope.realUsed($scope.importData[i]);
                }
            }
            return total;
        };

        $scope.import = function () {
            $('#import').modal('hide');

            var amount = $scope.total(false);
            var h = Math.floor(amount / 60);
            var min = Math.round(amount - (h * 60));
            $scope.memberDayReport.AmountH = h;

            if ($scope.importModel.Round === '15') {
                $scope.memberDayReport.AmountM = (Math.round((min + 7) / 15) * 15) % 60;
                if ((min > 0) & ($scope.memberDayReport.AmountM === 0)) {
                    $scope.memberDayReport.AmountH++;
                }
            } else if ($scope.importModel.Round === '5') {
                $scope.memberDayReport.AmountM = (Math.round((min + 2) / 5) * 5) % 60;
                if ((min > 0) & ($scope.memberDayReport.AmountM === 0)) {
                    $scope.memberDayReport.AmountH++;
                }
            } else {
                $scope.memberDayReport.AmountM = min;
            }
            $scope.calcAmount(false);
            var text = '';
            for (var i = 0; i < $scope.importData.length; i++) {
                if (!$scope.importData[i].skip) {
                    for (var ic = 0; ic < $scope.importData[i].comments.length; ic++) {
                        if ($scope.importData[i].comments[ic].text.substring(0, 1) !== '[') {
                            text += $scope.importData[i].comments[ic].text + '\n';
                        }
                    }
                }
            }
            $scope.memberDayReport.Description = text;
        };

        $scope.comments = function (item) {
            var text = '';
            for (var ic = 0; ic < item.comments.length; ic++) {
                if (item.comments[ic].text.substring(0, 1) !== '[') {
                    text += item.comments[ic].text + '\n';
                }
            }
            return text;
        };
    };
})();
