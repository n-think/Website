import $ from "jquery";

module userEdit {
    if (window.location.pathname.lastIndexOf("/Admin/EditUser")==0){
        loadUserEditEventHandlers()
    }
    function loadUserEditEventHandlers() {
        $("#role-selector").on("change",switchRoleSelector);
        $(".admin-options label").on("click", func1);
        $(".admin-options input:checkbox, .admin-options label").on("click", func2);
    }
    
    function switchRoleSelector() {
        if ($(this).val() === "admin") {
            $(".admin-options").removeClass("d-none");
        } else {
            $(".admin-options").addClass("d-none");
        }
    }

    function func1 () {
        let classes = this.className.split(" ");
        let action = classes[0];
        let group = classes[1];
        $("." + action + "." + group + ":checkbox").trigger("click");
    }

    function func2 () {
        let checkbox = $(this);
        let classes = checkbox.attr("class").split(" ");
        let action = classes[0];
        let group = classes[1];

        if (action === "delete") {
            let flag: boolean = checkbox.prop("checked");
            $(`.view.${group}:checkbox`).prop("checked", flag).prop("disabled", flag);
            $(`.edit.${group}:checkbox`).prop("checked", flag).prop("disabled", flag);
        } else if (action === "edit") {
            let flag: boolean = checkbox.prop("checked");
            $(`.view.${group}:checkbox`).prop("checked", flag).prop("disabled", flag);
        }
    }
}