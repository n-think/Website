import $ from "jquery";

module userEdit {
    $("#role-selector").change(function () {
        if ($(this).val() === "admin") {
            $(".admin-options").removeClass("d-none");
        } else {
            $(".admin-options").addClass("d-none");
        }
    });

    $(".admin-options label").click(function () {
        let classes = this.className.split(" ");
        let action = classes[0];
        let group = classes[1];
        $("." + action + "." + group + ":checkbox").click();
    });

    $(".admin-options input:checkbox, .admin-options label").click(function () {
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
    });
}