"use strict;"

controllers.controller('memberCtrl', ['$scope', '$rootScope', 'MemberService',
    function ($scope, $rootScope, MemberService) {
        $scope.refresh = function () {
            $scope.members = MemberService.query({}, function () {
            });
        };
        $scope.$on("refreshMembersEvent", function (event, args) {
            $scope.refresh();
        });
        $scope.memberSelect = function (member) {
            $rootScope.$broadcast("selectMemberEvent", { member });
        };
    }]);
