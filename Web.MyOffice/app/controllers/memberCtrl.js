(function () {

    'use strict';

    angular.module('MyOffice.app')
        .controller('memberCtrl', ['$scope', '$rootScope', 'MemberService', memberCtrl]);

    function memberCtrl($scope, $rootScope, MemberService) {
        $scope.refresh = function () {
            $scope.members = MemberService.query({}, function () {
            });
        };
        $scope.$on('refreshMembersEvent', function () {
            $scope.refresh();
        });
        $scope.memberSelect = function (member) {
            $rootScope.$broadcast('selectMemberEvent', member );
        };
    };
})();
