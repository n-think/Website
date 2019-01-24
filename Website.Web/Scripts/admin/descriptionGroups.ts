import $ from "jquery";

module descGroups {
    if (window.location.pathname.lastIndexOf("/Admin/DescriptionGroups") == 0) {
        loadDescGroupsEventHandlers()
    }

    class UserClaims {
        canEditAdd: boolean;
        canDelete: boolean;
    }    
    
    let Claims: UserClaims = {canEditAdd: false, canDelete: false};
    
    // export function setUserClaims(canEditAdd:boolean, canDelete:boolean) {
    //     Claims.canEditAdd = canEditAdd;
    //     Claims.canDelete = canDelete;        
    // }

    function loadDescGroupsEventHandlers() {
        $(".description-group").on("click", viewDescriptionGroupItems);
        $("#add-group-button").on("click", addGroup);
        $("#cancel-add-group").on("click", cancelAddGroup);
        $(".remove-group").on("click", deleteGroup);
        $(".edit-group").on("click", editGroup);
        $(".save-edit-group").on("click", saveEditGroup);
        $(".cancel-edit-group").on("click", cancelEditGroup);


        $("#add-item-button").on("click", addItem);
        $("#confirm-add-item").on("click", confirmAddItem);
        $("#cancel-add-item").on("click", cancelAddItem);

        let container = $("#admin-content");
        container.on("click", ".remove-item", deleteItem);
        container.on("click", ".edit-item", editItem);
        container.on("click", ".save-edit-item", saveEditItem);
        container.on("click", ".cancel-edit-item", cancelEditItem);
    }

    function addGroup() {
        $("#add-group-form").removeClass("d-none");
        $(this).addClass("d-none");

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
        let button = $(this);
        button
            .parent()
            .addClass("d-none")
            .parent()
            .find(".group-edit-controls")
            .removeClass("d-none");
        let group = button.closest(".description-group");
        group
            .find(".group-view")
            .addClass("d-none");
        group
            .find(".group-edit-form")
            .removeClass("d-none");
    }

    function deleteGroup() {
        let id = $(this)
            .closest(".description-group")
            .data("id");
        location.href = `/admin/DeleteDescriptionGroup/${id}`;
    }

    function saveEditGroup() {
        let form = $(this)
            .closest(".description-group")
            .find(".group-edit-form");
        if (form.valid()) {
            form.trigger("submit");
        }
    }

    function cancelEditGroup() {
        let button = $(this);
        button
            .parent()
            .addClass("d-none")
            .parent()
            .find(".group-view-controls")
            .removeClass("d-none");

        let group = button.closest(".description-group");
        group
            .find(".group-view")
            .removeClass("d-none");

        let nameValue = group
            .find(".group-view")
            .find("span.desc-group-name")
            .data("val");
        group
            .find(".group-edit-form")
            .addClass("d-none")
            .find('input[name="Name"]')
            .val(nameValue)
            .removeClass("input-validation-error is-invalid");

        let descValue = group
            .find(".group-view")
            .find("span.desc-group-description")
            .data("val");
        group
            .find(".group-edit-form")
            .addClass("d-none")
            .find('input[name="Description"]')
            .val(descValue);
    }

    function addItem() {
        let groupId = $("#description-items").data("group-id");
        if (groupId === undefined) {
            return;
        }

        let form = $("#add-item-form");
        form.removeClass("d-none");

        let thisButton = $(this);
        thisButton.addClass("d-none");
    }

    function confirmAddItem() {
        let form = $("#add-item-form");
        if (!form.valid()) {
            return;
        }
        let groupId = $("#description-items").data("group-id");
        $("#item-group-id").val(groupId);

        let csrftoken = form.find(`input[name="__RequestVerificationToken"]`).val().toString();
        let alertDiv = $("#alert");
        $.ajax({
            type: "POST",
            url: '/admin/addDescriptionItem',
            headers: {
                RequestVerificationToken: csrftoken,
                "Content-Type": "application/x-www-form-urlencoded"
            },
            data: form.serialize(),
            success: function (response) {
                //console.log(response);
                let id = response;
                let name = $("#item-name").val().toString();
                let container = $("#description-items");
                let descItem = getDescItem(id, name, groupId);
                container.append(descItem);

                let alert = createAlert(`Описание ${name} успешно добавлено.`);
                alertDiv
                    .empty()
                    .append(alert);
                form.trigger("reset");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                let alert;
                if (jqXHR.status == 400) {
                    alert = createAlert(jqXHR.responseText);
                } else {
                    alert = createAlert(`Ошибка сервера ${jqXHR.status}.`);
                }
                alertDiv.empty().append(alert);
            }
        });
    }

    function createAlert(message: string): JQuery<HTMLElement> {
        return $(`
<div class="alert alert-warning alert-dismissible">
${message}
<button type="button" class="close" data-dismiss="alert" aria-label="Close">
<span aria-hidden="true">&times;</span>
</button>
</div>   
        `)
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
        let item = $(this)
            .closest(".description-item");
        item
            .find(".item-name-view")
            .addClass("d-none");
        let form = item
            .find(".item-edit-form");
        form.removeClass("d-none");
        $.validator.unobtrusive.parse(form);

        item
            .find(".item-view-controls")
            .addClass("d-none");
        item
            .find(".item-edit-controls")
            .removeClass("d-none");
    }

    function deleteItem() {
        let id = $(this)
            .closest(".description-item")
            .data("id");
        location.href = `/admin/DeleteDescriptionItem/${id}`;
    }

    function saveEditItem() {
        let item = $(this)
            .closest(".description-item");
        let form = item
            .find(".item-edit-form");
        let newName = form.find(".item-name-edit").val();
        let itemId = item.data("id");

        if (!form.valid()) {
            return;
        }

        let alertDiv = $("#alert");

        $.ajax({
            type: "POST",
            url: '/admin/editDescriptionItem',
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            data: form.serialize(),
            success: function (response) {
                //console.log(response);
                let alert = createAlert(`Описание ${newName} успешно изменено.`);
                alertDiv
                    .empty()
                    .append(alert);

                item.data("name", newName);
                item
                    .find(".cancel-edit-item")
                    .trigger("click");


                let descGroup = $("#description-groups").find("a.description-group.active");

                let itemsArray: Array<DescriptionItem> = descGroup.data("items");

                for (let i = 0; i < itemsArray.length; i++) {
                    let item = itemsArray[i];
                    if (item.id === itemId) {
                        item.name = newName.toString();
                        break;
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                let alert;
                if (jqXHR.status == 400) {
                    alert = createAlert(jqXHR.responseText);
                } else {
                    alert = createAlert(`Ошибка сервера ${jqXHR.status}.`);
                }
                alertDiv.empty().append(alert);
            }
        });
    }

    function cancelEditItem() {
        let item = $(this)
            .closest(".description-item");
        item
            .find(".item-name-view")
            .removeClass("d-none");
        item
            .find(".item-edit-form")
            .addClass("d-none")
            .find(".item-name-edit")
            .removeClass("input-validation-error is-invalid");
        item
            .find(".item-name-edit")
            .val(item.data("name"));
        item
            .find(".item-name-view")
            .text(item.data("name"));
        item
            .find(".item-edit-controls")
            .addClass("d-none");
        item
            .find(".item-view-controls")
            .removeClass("d-none");
    }

    function viewDescriptionGroupItems() {
        cancelAddItem();

        let descGroup = $(this);
        let descGroupId = descGroup.data("id");
        let descItemsContainer = $("#description-items");
        descItemsContainer.data("group-id", descGroupId);

        $("#item-group-id").val(descGroupId);

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

        let itemsData = descGroup.data("items");
        if (descGroup.data("items") === undefined) {
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
        }
        if (descGroup.data("items") === "loading") {
            return;
        }

        formDescGroupItemList(itemsData);
    }

    class DescriptionItem {
        id: string;
        name: string;
        groupId: string;
    }

    function formDescGroupItemList(data) {
        let dataArray = data as Array<DescriptionItem>;
        if (dataArray === undefined)
            return;

        let container = $("#description-items");
        container.empty();
        for (let i = 0; i < dataArray.length; i++) {
            let item = dataArray[i];
            let descItem = getDescItem(item.id, item.name, item.groupId);
            container.append(descItem);

            $("#add-item-button").removeClass("d-none");
        }
    }

    function getDescItem(itemId, itemName, groupId): JQuery<HTMLElement> {
        let descItemContainer = $("<a>")
            .addClass("list-group-item list-group-item-action description-item list-group-item-light")
            .data("id", itemId)
            .data("name", itemName)
            .data("groupId", groupId);
        // @ts-ignore //from view script //todo export claims hz kak
        let userPermissions = Claims || UserDescriptionClaims;
        let descItem = `
<div class="d-flex flex-row">
 <div class="w-75">
  <span class="item-name-view">${itemName}</span>
  <form class="item-edit-form  d-none">
   <input name="DescriptionGroupId" value="${groupId}" hidden>
   <input name="Id" value="${itemId}" hidden>
   <input name="Name" class="item-name-edit form-control form-control-sm" value="${itemName}" required data-val="true"/>  
  </form>                                
 </div>`;
        if (userPermissions.canDelete || userPermissions.canEditAdd) {
            descItem += `
 <div class="align-self-center ml-auto">
  <span class="item-controls">
   <span class="item-view-controls" style="vertical-align: top">
    <span class="edit-item btn-pushy btn btn-sm btn-outline-success fa fa-pencil" style="vertical-align: top"></span>`;
            if (userPermissions.canDelete) {
                descItem += `
<span class="remove-item btn-pushy btn btn-sm btn-outline-danger fa fa-remove" style="vertical-align: top"></span>`;
            }
            descItem += `
  </span>
   <span class="item-edit-controls d-none">
    <span class="save-edit-item btn-pushy btn btn-sm btn-outline-success fa fa-check" style="vertical-align: top"></span>
    <span class="cancel-edit-item btn-pushy btn btn-sm btn-outline-warning fa fa-undo" style="vertical-align: top"></span>
   </span>
  </span>
 </div>`;
        }
        descItem += `</div>`;
        descItemContainer.append(descItem);
        return descItemContainer;
    }
}
