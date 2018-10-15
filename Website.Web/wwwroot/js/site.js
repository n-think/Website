window.onscroll = function () { stickyNavbar() };

var navbar = document.getElementById("navbar");
var sticky = navbar.offsetTop;

function stickyNavbar() {
    if (window.pageYOffset >= sticky) {
        navbar.classList.add("navbar-fixed-top");
    }
    else {
        navbar.classList.remove("navbar-fixed-top");
    }
}

function image_select(img) {
    var newSrc = img.src.slice(0, img.src.length - 6) + ".jpg";
    $("#item-image-view")[0].src = newSrc;
}


$("#role-selector").change(function () {
    if ($(this).val() === "admin") {
        $(".admin-options").removeClass("hidden");
    } else {
        $(".admin-options").addClass("hidden");
    }
});

$(".admin-options label").click(function () {
    var classes = this.className.split(" ");
    var action = classes[0];
    var group = classes[1];
    $("." + action + "." + group + ":checkbox").click();
});

$(".admin-options input:checkbox, .admin-options label").click(function () {
    var classes = this.className.split(" ");
    var action = classes[0];
    var group = classes[1];

    if (action === "delete" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "create" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    }
    else if (action === "delete" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "create" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
    else if (action === "create" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    }
    else if (action === "create" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
    else if (action === "edit" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    }
    else if (action === "edit" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
});


