(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('MemberPaymentService', ['$resource', MemberPaymentService]);

    function MemberPaymentService($resource) {
        return $resource('api/memberPayment/', {}, {
            query: { method: 'GET', params: {}, isArray: false },
            get: { method: 'GET', params: {}, isArray: false },
            post: { method: 'POST', params: {}, isArray: false },
            put: { method: 'PUT', params: {}, isArray: false },
            delete: { method: 'DELETE', params: {}, isArray: false },
        });
    };
})();
