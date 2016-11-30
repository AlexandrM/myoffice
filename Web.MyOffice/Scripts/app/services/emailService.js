"use strinct;"

services.factory('emailService', ['$http', '$location',
    function ($http, $location) {
        return {
            send: function (action, projectId, memberId, attachments) {
                $http({
                    method: 'POST',
                    url: $("#linkRoot").attr("href") + 'api/email',
                    data: {
                        action: action,
                        projectId: projectId,
                        memberId: memberId,
                        attachments: attachments
                    }
                }).then(function successCallback(response) {
                }, function errorCallback(response) {
                    bootbox.alert("Response status: " + response.status, function () {
                        
                    });
                });
            }
    };
}]);