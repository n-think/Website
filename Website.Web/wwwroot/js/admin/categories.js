var categories;
(function (categories) {
    $(".category-expander").on("click", categoryExpanderToggle);
    $(".list-group-item.list-group-item-action.category").on("click", viewCategory);
    $("#cat-parent-preview").on("click", viewParentCategory);
    $("#edit-cat").on("click", categoryEdit);
    $("#remove-cat").on("click", categoryRemove);
    $("#save-edit-cat").on("click", categorySaveEdit);
    $("#cancel-edit-cat").on("click", categoryCancelEdit);
    var currCatViewId = "";
    var catViewControls = $("#cat-view-controls");
    var catEditControls = $("#cat-edit-controls");
    var editActive = false;
    function categoryEdit() {
        editActive = true;
        currCatViewId = $("#cat-id-preview").val().toString();
        $(".cat-input-edit").removeAttr("readonly");
        catViewControls.addClass("d-none");
        catEditControls.removeClass("d-none");
        var id = $("#cat-parent-preview").addClass("d-none").data("id");
        $("#edit-category-select-container").removeClass("d-none");
        $("#edit-category-select")
            .val(id)
            .selectpicker("refresh");
    }
    function categoryRemove() {
    }
    function categorySaveEdit() {
    }
    function categoryCancelEdit() {
        editActive = false;
        $(".cat-input-edit").attr("readonly", "");
        $(".list-group-item.list-group-item-action.category[data-id=" + currCatViewId + "]").trigger("click");
        catViewControls.removeClass("d-none");
        catEditControls.addClass("d-none");
        $("#cat-parent-preview").removeClass("d-none");
        $("#edit-category-select-container").addClass("d-none");
    }
    function categoryExpanderToggle() {
        $(this)
            .toggleClass("fa-minus")
            .toggleClass("fa-plus");
    }
    function viewCategory() {
        if (editActive) {
            categoryCancelEdit();
        }
        var thisCat = $(this);
        $("#cat-id-preview").val(thisCat.data("id"));
        $("#cat-name-preview").val(thisCat.data("name"));
        $("#cat-desc-preview").val(thisCat.data("desc"));
        var parent = thisCat.parent().parent().prev();
        if (parent.length === 0) {
            $("#cat-parent-preview")
                .val("*отсутствует*")
                .data("id", "-1");
        }
        else {
            $("#cat-parent-preview")
                .val(parent.data("name"))
                .data("id", parent.data("id"));
        }
        $("#cat-view-controls").removeClass("d-none");
    }
    function viewParentCategory() {
        var element = $(this);
        if (element.data("id") !== -1) {
            $(".list-group-item.list-group-item-action.category[data-id=" + element.data("id") + "]").trigger("click");
        }
    }
})(categories || (categories = {}));
