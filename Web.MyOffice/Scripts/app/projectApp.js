"use strict";

var app = ASE.AngularInit('app', ['ngRoute', 'ngResource', 'controllers', 'services', 'angularMoment']);
var controllers = angular.module('controllers', []);
var services = angular.module('services', ['ngResource']);

app.config(['$routeProvider',
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
]);
