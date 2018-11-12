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
    if (action === "delete" && checkbox.prop("checked") === true) {
        $(".view." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $(".edit." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    }
    else if (action === "delete" && checkbox.prop("checked") === false) {
        $(".view." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $(".edit." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
    else if (action === "edit" && checkbox.prop("checked") === true) {
        $(".view." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    }
    else if (action === "edit" && checkbox.prop("checked") === false) {
        $(".view." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
});
//# sourceMappingURL=userEdit.js.map