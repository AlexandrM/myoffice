(function () {

    'use strict';

    angular.module('MyOffice.app')
        .service('emailService', ['$http', emailService]);

    function emailService($http) {
        return {
            send: function (action, projectId, memberId, attachments) {
                $http({
                    method: 'POST',
                    url: $('#linkRoot').attr('href') + 'api/email',
                    data: {
                        action: action,
                        projectId: projectId,
                        memberId: memberId,
                        attachments: attachments
                    }
                }).then(function successCallback() {
                }, function errorCallback(response) {
                    bootbox.alert('Response status: ' + response.status, function () {
                    });
                });
            }
        };
    };
})();
