angular.module("umbraco").controller("Cogworks.Tinifier.TinifierEditTSetting.Controller",
    function ($scope, $http, notificationsService, editorService, appState, eventsService) {
        // Get settings


        $scope.timage = {};


        // Fill select dropdown
        $scope.options = [
            { value: false, label: "False" },
            { value: true, label: "True" }
        ];

        $scope.preserveMetadataSetting = function () {
            eventsService.emit("preserveMetadataValue", { value: !$scope.timage.PreserveMetadata });
            $scope.timage.PreserveMetadata = !$scope.timage.PreserveMetadata;
        };

        $scope.enableUndoOptimizationSetting = function () {
            eventsService.emit("enableUndoOptimizationValue", { value: !$scope.timage.EnableUndoOptimization });
            $scope.timage.EnableUndoOptimization = !$scope.timage.EnableUndoOptimization;
        };

        $scope.enableOptimizationOnUploadSetting = function () {
            eventsService.emit("enableUndoOptimizationValue", { value: !$scope.timage.EnableOptimizationOnUpload });
            $scope.timage.EnableOptimizationOnUpload = !$scope.timage.EnableOptimizationOnUpload;
        };

        // Fill form from web api
        $http.get(`/umbraco/backoffice/api/TinifierSettings/GetTSetting`).then(
            function (response) {
                if (response.data){
                    $scope.timage = response.data;
                }
            },
            function (error) {
                notificationsService.error(error.message);
            });


        $scope.submit = function () {

            SubmitSettings();
        };

        $scope.stopTinifing = function () {
            var options = {
                title: "The confirmation",
                view: "/App_Plugins/Cogworks.Tinifier/Backoffice/Dashboards/StopTinifyingConfirmation.html"
            };
            editorService.open(options);
        };

        $scope.tinifyEverything = function () {
            var options = {
                title: "The confirmation",
                view: "/App_Plugins/Cogworks.Tinifier/Backoffice/Dashboards/TinifyAll.html"
            };

            editorService.open(options);
        };
        $scope.timage.ApiKey = $('#apiKey').val();
        var processNotification = null;
        function SubmitSettings() {
            notificationsService.success("Saving in progress ...");

            processNotification = notificationsService.current[0];

            $http.post(`/umbraco/backoffice/api/TinifierSettings/PostTSetting`, JSON.stringify($scope.timage)).then(postSuccessCallback, postErrorCallback);

            function postSuccessCallback(response) {

                for (var i = 0; i < notificationsService.current.length; i++) {
                    notificationsService.remove(notificationsService.current[i]);
                };

                notificationsService.remove(processNotification);
                notificationsService.success("✔️ Settings successfully saved!");
            }

            function postErrorCallback(error) {
                notificationsService.remove();

                if (error.Error === 1) {
                    notificationsService.warning("Warning", error.message);
                }
                else {
                    notificationsService.error("Error", error.data.headline + " " + error.data.message);
                }
            }
        }

        $scope.saveSettings = function () {
            SubmitSettings();
        };


        $(document).ready(function () {
            var previousApiKey = "";

            $("#apiKey").focusout(function () {
                ValidateApiKey();
            });

            $("#apiKey").focus(function () {
                previousApiKey = $('#apiKey').val();
            });

            $("#apiKey").keypress(function (event) {
                if (event.charCode == 13)
                    ValidateApiKey();
            });



            function ValidateApiKey() {
                var actualApiKey = $('#apiKey').val();

                if (previousApiKey == null)
                    previousApiKey = "";

                if (previousApiKey !== actualApiKey)
                    SubmitSettings();

                previousApiKey = actualApiKey;
            }

        });
    });