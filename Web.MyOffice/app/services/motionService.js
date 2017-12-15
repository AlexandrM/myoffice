(function () {
    'use strict';

    angular.module('MyOffice.app').service('motionService', ['$resource', motionService]);

    function motionService($resource) {
        return $resource('api/motions/', {}, {
            MotionDelete: { method: 'DELETE', params: {}, isArray: false },
            MotionUpdate: { method: 'PUT', parmas: {}, isArray: false },
            MotionMerge: { method: 'POST', parmas: {}, isArray: false }
        });
    }
})();
