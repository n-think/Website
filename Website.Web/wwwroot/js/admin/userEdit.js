var userEdit;
(function (userEdit) {
    $("#role-selector").change(function () {
        if ($(this).val() === "admin") {
            $(".admin-options").removeClass("d-none");
        }
        else {
            $(".admin-options").addClass("d-none");
        }
    });
    $(".admin-options label").click(function () {
        var classes = this.className.split(" ");
        var action = classes[0];
        var group = classes[1];
        $("." + action + "." + group + ":checkbox").click();
    });
    $(".admin-options input:checkbox, .admin-options label").click(function () {
        var checkbox = $(this);
        var classes = checkbox.attr("class").split(" ");
        var action = classes[0];
        var group = classes[1];
        if (action === "delete") {
            var flag = checkbox.prop("checked");
            $(".view." + group + ":checkbox").prop("checked", flag).prop("disabled", flag);
            $(".edit." + group + ":checkbox").prop("checked", flag).prop("disabled", flag);
        }
        else if (action === "edit") {
            var flag = checkbox.prop("checked");
            $(".view." + group + ":checkbox").prop("checked", flag).prop("disabled", flag);
        }
    });
})(userEdit || (userEdit = {}));
