(function() {

    'use strict';

    var projectModule = angular.module('MyOffice.app')
        .controller('projectCtrl',
        [
            '$scope',
            '$routeParams',
            '$location',
            '$rootScope',
            'ProjectService',
            'ProjectMemberService',
            'ProjectTaskService',
            projectCtrl
        ]);

    projectModule.filter('IsNotArchive',
        function() {
            return function(projects) {
                return projects.filter(function(project, index, projectArray) {
                    return !project.IsArchive;
                });
            };
        });

    projectModule.filter('IsArchive',
        function() {
            return function(projects) {
                return projects.filter(function (project, index, projectArray) {
                    return project.IsArchive;
                });
            };
        });

    function projectCtrl($scope,
        $routeParams,
        $location,
        $rootScope,
        ProjectService,
        ProjectMemberService,
        ProjectTaskService) {

        $scope.notAccepted = true;
        $scope.showArchive = false;

        $scope.toggleTable = function() {
            $scope.showArchive = !$scope.showArchive;
            return $scope.showArchive;
        };

        $scope.query = function() {
            $scope.projects = ProjectService.query({ notAccepted: $scope.notAccepted },
                function() {
                });
        };


        $scope.delete = function(project) {
            bootbox.confirm(ASE.L.Delete + ': ' + project.Name,
                function(result) {
                    if (result) {
                        ProjectService.delete({ id: project.Id, memberId: '', mode: 'project' },
                            function() {
                                $scope.query();
                            });
                    }
                });
        };

        $scope.restore = function(project) {
            project.IsArchive = false;
            ProjectService.put(project,
                function() {
                    $location.path('/project');
                });
        };

        $scope.get = function() {
            $scope.project = ProjectService.get({ id: $routeParams.id },
                function() {
                });
        };

        $scope.save = function() {
            if ($routeParams.id.toString() === '0') {
                $scope.project = ProjectService.post($scope.project,
                    function() {
                        $location.path('/project');
                    });
            } else {
                $scope.project = ProjectService.put($scope.project,
                    function() {
                        $location.path('/project');
                    });
            }
        };


        $scope.addMember = function() {
            $rootScope.$broadcast('refreshMembersEvent', {});
            $('#dialog')
                .modal({
                });
        };
        $scope.$on('selectMemberEvent',
            function(event, args) {
                $('#dialog').modal('hide');
                ProjectMemberService.post({ projectId: $scope.project.Id, memberId: args.Id },
                    function() {
                        $scope.get();
                    });
            });
        $scope.deleteMember = function(member) {
            bootbox.confirm(ASE.L.Delete + ': ' + member.Member.FullName,
                function(result) {
                    if (result) {
                        ProjectMemberService.delete({ projectId: $scope.project.Id, memberId: member.MemberId },
                            function() {
                                $scope.get();
                            });
                    }
                });
        };
        $scope.memberTypeChange = function(member) {
            ProjectMemberService.put(member,
                function() {
                    $scope.get();
                });
        };

        $scope.edDateFrom = moment().format('L');
        $scope.edDateTo = moment().format('L');
        $scope.notAccepted = true;
        $scope.refreshTasks = function() {
            $scope.project = ProjectService.get({ id: $routeParams.id },
                function() {
                    $scope.projectTasks = ProjectTaskService.query(
                        {
                            projectId: $scope.project.Id,
                            dateFrom: $scope.edDateFrom,
                            dateTo: $scope.edDateTo,
                            notAccepted: $scope.notAccepted
                        },
                        function() {
                        });
                });
        };

        $scope.addDate = function(task, days) {
            var date = new Date(task.Limitation);
            date.setDate(date.getDate() + days);
            task.Limitation = date;

            ProjectTaskService.addDay(task,
                function() {
                    $scope.projectTasks = ProjectTaskService.query(
                        {
                            projectId: $scope.project.Id,
                            dateFrom: $scope.edDateFrom,
                            dateTo: $scope.edDateTo,
                            notAccepted: $scope.notAccepted
                        },
                        function() {
                        });
                });
        };

        $scope.setState = function(task, state) {
            task.State = state;
            ProjectTaskService.setState(task,
                function() {
                    $scope.projectTasks = ProjectTaskService.query(
                        {
                            projectId: $scope.project.Id,
                            dateFrom: $scope.edDateFrom,
                            dateTo: $scope.edDateTo,
                            notAccepted: $scope.notAccepted
                        },
                        function() {
                        });
                });
        };

        $scope.deleteTask = function(projectTask) {
            bootbox.confirm(ASE.L.Delete + ': ' + projectTask.Name,
                function(result) {
                    if (result) {
                        ProjectTaskService.delete({ id: projectTask.Id, mode: '' },
                            function() {
                                $scope.refreshTasks();
                            });
                    }
                });
        };

    };
})();
