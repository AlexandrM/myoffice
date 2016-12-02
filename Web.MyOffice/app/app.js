(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('MyOffice.app', [
        'ngRoute',
        'ngResource',

        'angularMoment',

        'ase.com.ua',
        'MyOffice.app',
    ]);

    angular.module('MyOffice.app').config(['$routeProvider',
        function ($routeProvider) {
            $routeProvider.
            when('/project', {
                templateUrl: 'project/list',
                controller: 'projectCtrl'
            }).
            when('/project/:id', {
                templateUrl: 'project/edit',
                controller: 'projectCtrl'
            }).
            when('/project/members/:id', {
                templateUrl: 'project/members',
                controller: 'projectCtrl'
            }).
            when('/member', {
                templateUrl: 'member/list',
                controller: 'memberCtrl'
            }).
            when('/tasks/:id', {
                templateUrl: 'projectTask/list',
                controller: 'projectCtrl'
            }).
            when('/projectTask/:id/:projectId', {
                templateUrl: 'projectTask/edit',
                controller: 'projectTaskCtrl'
            }).
            when('/finance/:id?', {
                templateUrl: 'finance/list',
                controller: 'financeCtrl'
            }).
            when('/memberdayreport', {
                templateUrl: 'memberdayreport/list',
                controller: 'memberDayReportCtrl'
            }).
            when('/memberdayreport/:id/:projectId?', {
                templateUrl: 'memberdayreport/edit',
                controller: 'memberDayReportCtrl'
            }).
            when('/memberpayment', {
                templateUrl: 'memberpayment/list',
                controller: 'memberPaymentCtrl'
            }).
            when('/memberpayment/:id/:projectId?', {
                templateUrl: 'memberpayment/edit',
                controller: 'memberPaymentCtrl'
            }).
            when('/membermotions/:projectId', {
                templateUrl: 'membermotions/list',
                controller: 'memberMotionsCtrl'
            }).
            when('/Debts/:mode', {
                templateUrl: 'debts/list',
                controller: 'debtsCtrl'
            }).
            otherwise({
                redirectTo: '/project'
            });
        }
    ]).run(function ($UserSettingsService) {
        jQuery.extend($UserSettingsService, JSON.parse(localStorage.getItem('SkypeBot.MyOffice.$UserSettingsService') || '{}'));
        $UserSettingsService.Language = $UserSettingsService.Language || moment.locale();
        $UserSettingsService.Prices = $UserSettingsService.Prices || [];
    });
})();
