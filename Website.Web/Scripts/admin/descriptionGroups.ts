import $ from "jquery";

module descGroups {
    if (window.location.pathname.lastIndexOf("/Admin/DescriptionGroups") == 0) {
        loadDescGroupsEventHandlers()
    }

    function loadDescGroupsEventHandlers() {
        $(".description-group").on("click", viewDescriptionGroupItems);
        $("#add-group-button").on("click", addGroup);
        $("#cancel-add-group").on("click", cancelAddGroup);


        $("#add-item-button").on("click", addItem);
        $("#cancel-add-item").on("click", cancelAddItem);
    }

    function addGroup() {
        $("#add-group-form").removeClass("d-none");
        $(this).addClass("d-none");

    }

    function confirmAddGroup() {

    }

    function cancelAddGroup() {
        $("#add-group-form")
            .trigger("reset")
            .addClass("d-none")
            .find("span.field-validation-valid")            
            .empty();
        $("#add-group-button").removeClass("d-none");
    }

    function editGroup() {

    }

    function deleteGroup() {

    }

    function saveEditGroup() {

    }

    function cancelEditGroup() {

    }

    function addItem() {
        $("#add-item-form").removeClass("d-none");
        $(this).addClass("d-none");
    }

    function confirmAddItem() {

    }

    function cancelAddItem() {
        $("#add-item-form")
            .trigger("reset")
            .addClass("d-none")
            .find("span.field-validation-valid")
            .empty();
        $("#add-item-button").removeClass("d-none");
    }

    function editItem() {

    }

    function deleteItem() {

    }

    function saveEditItem() {

    }

    function cancelEditItem() {

    }

    function viewDescriptionGroupItems() {
        let descGroup = $(this);
        let descGroupId = descGroup.data("id");
        let descItemsContainer = $("#description-items");

        descItemsContainer.data("group-id", descGroupId);

        let itemsData = descGroup.data("items");
        if (itemsData === undefined) {
            $.ajax({
                method: "GET",
                url: `/api/admin/DescriptionItems/${descGroupId}`,
                beforeSend: function (jqXHR) {
                    // console.log(`before loading items for #${descGroupId}`);

                    descGroup.data("items", "loading")
                },
                success: function (response) {
                    // console.log(`success loading items for #${descGroupId}`);
                    // console.log(response);

                    descGroup.data("items", response);
                    if (descGroupId === descItemsContainer.data("group-id")) {
                        formDescGroupItemList(response)
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(`error loading items for #${descGroupId}, ${textStatus}, ${errorThrown}.`);
                    //adderrorstolist$("#asd").html("Ошибка загрузки");
                }
            });
        } else if (itemsData === "loading") {
            return;
        }
        formDescGroupItemList(itemsData);
        descGroup
            .parent()
            .find(".active")
            .removeClass("active text-white")
            .parent()
            .find(".badge")
            .addClass("badge-primary")
            .removeClass("badge-light");

        descGroup
            .addClass("active text-white")
            .find(".badge")
            .addClass("badge-light")
            .removeClass("badge-primary");

        $("#add-item-button").removeClass("d-none");
    }

    class DescriptionItem {
        id: number;
        name: string;
    }

    function formDescGroupItemList(data) {
        let dataArray = data as Array<DescriptionItem>;
        if (dataArray === undefined)
            return;

        let container = $("#description-items");
        container.empty();
        for (let i = 0; i < dataArray.length; i++) {
            let item = dataArray[i];
            let descItem = $("<a>")
                .addClass("list-group-item list-group-item-action description-item list-group-item-light")
                .data("id", item.id);
            let descItemControls = `
<div class="d-flex flex-row">
 <div class="mr-auto">
  <span class="item-name">${item.name}</span>                                
 </div>
 <div class="align-self-center">
  <span class="item-controls">
   <span class="item-view-controls" style="vertical-align: top">
    <span class="edit-item btn-pushy btn btn-sm btn-outline-success fa fa-pencil" style="vertical-align: top"></span>
    <span class="remove-item btn-pushy btn btn-sm btn-outline-danger fa fa-remove" style="vertical-align: top"></span>
   </span>
   <span class="item-edit-controls d-none">
    <span id="save-edit-item" class="btn-pushy btn btn-sm btn-outline-success fa fa-check" style="vertical-align: top"></span>
    <span id="cancel-edit-item" class="btn-pushy btn btn-sm btn-outline-warning fa fa-undo" style="vertical-align: top"></span>
   </span>
  </span>
 </div>
</div>`;
            descItem.append(descItemControls);
            container.append(descItem);
        }
    }
}
