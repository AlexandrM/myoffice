(function () {
    ROOT = $('base').attr('href');

    'use strict';

    angular.module('MyOffice.app', [
        'ngRoute',
        'ngResource',
        'angularMoment',
        'ase.com.ua',
        'MyOffice.app',
		'ui.bootstrap'
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
            when('/finance/:id/:lang?', {
               templateUrl: function (params) {
                   return 'finance/list/' + params.id + '?lang=' + params.lang;
               },
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
			when('/Budgets', {
                 templateUrl: 'UserBudgets/BudgetList',
                 controller: 'budgetCtrl'
             }).
			when('/Currencies', {
                  templateUrl: 'Currencies/CurrencyList',
                    controller: 'currencyCtrl'
                }).
			when('/BankAccounts', {
                templateUrl: 'Accounts/AccountList',
                controller: 'budgetAccountCtrl'
				}).
			when('/items', {
                templateUrl: 'expenditure/Items',
                controller: 'itemCtrl'
            }).
			when('/counter', {
                template: '',
                controller: 'counterCtrl'
            }).
            otherwise({
                redirectTo: '/project'
            });
        }
    ])
    .config(function () {
    })
    .run(function ($UserSettingsService) {
        jQuery.extend($UserSettingsService, JSON.parse(localStorage.getItem('SkypeBot.MyOffice.$UserSettingsService') || '{}'));
        $UserSettingsService.Language = $UserSettingsService.Language || moment.locale();
        $UserSettingsService.Prices = $UserSettingsService.Prices || [];
    });
})();
