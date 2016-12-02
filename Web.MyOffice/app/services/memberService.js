(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('MemberService', ['$resource', MemberService]);

    function MemberService($resource) {
        return $resource('api/member/', {}, {
            query: { method: 'GET', params: {}, isArray: true },
        });
    };
})();
