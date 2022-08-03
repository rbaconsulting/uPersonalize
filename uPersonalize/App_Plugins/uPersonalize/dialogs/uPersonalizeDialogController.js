angular.module("umbraco").controller("uPersonalizeDialogController", function ($scope, entityResource, iconHelper, editorService) {
    $scope.renderModel = [];
    $scope.ids = [];
    $scope.ipAddress = ['', '', '', ''];

    var config = {
        multiPicker: true,
        entityType: "Document",
        type: "content",
        treeAlias: "content"
    };

    var vm = this;

    vm.uPersonalizeConfig = $scope.model.uPersonalizeConfig;

    vm.conditions = [
        {
            "label": "IP Address",
            "value": "IP_Address"
        },
        {
            "label": "Device Type",
            "value": "Device_Type"
        },
        {
            "label": "Visited Page",
            "value": "Visited_Page"
        },
        {
            "label": "Visited Page X Times",
            "value": "Visited_Page_Count"
        },
        {
            "label": "Event Triggered",
            "value": "Event_Triggered"
        },
        {
            "label": "Event Triggered X Times",
            "value": "Event_Triggered_Count"
        },
        {
            "label": "Is Logged In",
            "value": "Logged_In"
        },
        {
            "label": "Is Now Before Date & Time",
            "value": "DateTime_Before"
        },
        {
            "label": "Is Now After Date & Time",
            "value": "DateTime_After"
        }
    ];

    vm.devices = [
        {
            "label": "Android",
            "value": "Android"
        },
        {
            "label": "iPhone",
            "value": "iPhone"
        },
        {
            "label": "Windows",
            "value": "Desktop_Windows"
        },
        {
            "label": "Linux",
            "value": "Desktop_Linux"
        },
        {
            "label": "Mac",
            "value": "Desktop_Mac"
        }
    ];

    vm.actions = [
        {
            "label": "Hide content",
            "value": "Hide"
        },
        {
            "label": "Show content",
            "value": "Show"
        },
        {
            "label": "Apply Additional Classes",
            "value": "Additional_Classes"
        }
    ];

    vm.submit = function () {
        vm.uPersonalizeConfig.ipAddress = `${$scope.ipAddress[0]}.${$scope.ipAddress[1]}.${$scope.ipAddress[2]}.${$scope.ipAddress[3]}`;

        if ($scope.model && $scope.model.submit) {
            $scope.model.submit(vm.uPersonalizeConfig);
        }
    }

    vm.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        }
    }

    $scope.openTreePicker = function () {
        var treePicker = config;
        treePicker.section = config.type;

        treePicker.submit = function (model) {
            var data = config.multiPicker ? model.selection : model.selection[0];

            if (Utilities.isArray(data)) {
                _.each(data, function (item, i) {
                    $scope.add(item);
                });
            } else {
                $scope.clear();
                $scope.add(data);
            }

            vm.uPersonalizeConfig.pageId = trim($scope.ids.join(), ",");

            editorService.close();
        };

        treePicker.close = function () {
            editorService.close();
        };

        editorService.treePicker(treePicker);
    };

    $scope.remove = function (index) {
        $scope.renderModel.splice(index, 1);
        $scope.ids.splice(index, 1);
        vm.uPersonalizeConfig.pageId = trim($scope.ids.join(), ",");
    };

    $scope.clear = function () {
        vm.uPersonalizeConfig.pageId = "";
        $scope.renderModel = [];
        $scope.ids = [];
    };

    $scope.add = function (item) {
        if ($scope.ids.indexOf(item.id) < 0) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.ids.push(item.id);
            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon, udi: item.udi });
            vm.uPersonalizeConfig.pageId = trim($scope.ids.join(), ",");

            // store the index of the new item in the renderModel collection so we can find it again
            var itemRenderIndex = $scope.renderModel.length - 1;
            // get and update the path for the picked node
            entityResource.getUrl(item.id, config.entityType).then(function (data) {
                $scope.renderModel[itemRenderIndex].path = data;
            });
        }
    };

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    if (vm.uPersonalizeConfig.ipAddress) {
        var tempIpAddress = vm.uPersonalizeConfig.ipAddress.split('.');

        if (tempIpAddress.length == 4) {
            $scope.ipAddress = tempIpAddress;
        }
    }

    if (vm.uPersonalizeConfig.pageId && vm.uPersonalizeConfig.pageId.length > 0) {
        if (!Array.isArray(vm.uPersonalizeConfig.pageId)) {
            $scope.ids = vm.uPersonalizeConfig.pageId.split(",");
        } else {
            $scope.ids.push(vm.uPersonalizeConfig.pageId);
        }

        entityResource.getByIds($scope.ids, config.entityType).then(function (data) {
            _.each(data, function (item, i) {
                item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon, udi: item.udi });

                // store the index of the new item in the renderModel collection so we can find it again
                var itemRenderIndex = $scope.renderModel.length - 1;
                // get and update the path for the picked node
                entityResource.getUrl(item.id, config.entityType).then(function (data) {
                    $scope.renderModel[itemRenderIndex].path = data;
                });
            });
        });
    }
});