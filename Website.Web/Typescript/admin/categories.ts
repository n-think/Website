$(".category-expander").on("click", categoryExpanderToggle);
$(".list-group-item.list-group-item-action.category").on("click", viewCategory);
$("#cat-parent-preview").on("click", viewParentCategory);

function categoryExpanderToggle() {
    $(this)
        .toggleClass("fa-minus")
        .toggleClass("fa-plus");
}

function viewCategory() {
    let thisCat = $(this);
    $("#cat-id-preview").val(thisCat.data("id"));
    $("#cat-name-preview").val(thisCat.data("name"));
    $("#cat-desc-preview").val(thisCat.data("desc"));
    let parent = thisCat.parent().parent().prev();
    if (parent.length === 0) {
        $("#cat-parent-preview")
            .val("*отсутствует*")
            .data("id", "-1");
    } else {
        $("#cat-parent-preview")
            .val(parent.data("name"))
            .data("id", parent.data("id"));
    }
    $("#cat-view-controls")
        .removeClass("d-none")
        .addClass("d-inline-block");
}

function viewParentCategory() {
    let element = $(this);
    if (element.data("id") !== -1) {
        $(`.list-group-item.list-group-item-action.category[data-id=${element.data("id")}]`).trigger("click");
    }
}