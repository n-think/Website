import $ from "jquery";

module descGroups {
    if (window.location.pathname.lastIndexOf("/Admin/DescriptionGroups") == 0) {
        loadDescGroupsEventHandlers()
    }

    function loadDescGroupsEventHandlers() {
        $(".description-group").on("click", viewDescriptionGroupItems)
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
            container.append($("<a>")
                .addClass("list-group-item list-group-item-action description-item list-group-item-light")
                .data("id", item.id)                
                .text(item.name)
            );
        }
    }
}
