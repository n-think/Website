import $ from "jquery";
import {DtoState} from "./enums";
import "jquery-validation-unobtrusive"

module productEdit {

    if (window.location.pathname.lastIndexOf("/Admin/EditItem") == 0 ||
        window.location.pathname.lastIndexOf("/Admin/AddItem") == 0) {
        loadProductEditEventHandlers()
    }

    if (window.location.pathname.lastIndexOf("/Admin/ViewItem") == 0) {
        loadProductViewEventHandlers()
    }

    function loadProductEditEventHandlers() {
        $("div#admin-content")
            .on("click", "#edit-form-submit", validateAndSubmitJson)
            .on("click", "img.admin-img-thumb", setImage)
            .on("click", "#image-primary-button", setPrimaryImage)
            .on("click", "#image-remove-button", setDeleteImage)
            .on("change", "#file-upload-button", loadImagesFromInput)
            .on("click", ".product-category-btn", loadProductCategoriesDropdown)
            .on("click", ".add-cat", addCategory)
            .on("click", ".remove-cat", removeCategory)
            .on("click", ".product-desc-group-btn", loadProductDescGroupDropdown)
            .on("click", ".add-desc-group", addDescGroup)
            .on("click", ".remove-desc-group", removeDescGroup)
            .on("click", ".desc-group-items-btn", loadProductDescGroupItemsDropdown)
            .on("click", ".add-desc-group-item", addDescGroupItem)
            .on("click", ".remove-desc-group-item", removeDescGroupItem)
            .on("click", ".edit-desc-group-item", editDescItem)
            .on("click", ".save-desc-group-item", saveEditDescItem)
            .on("click", ".cancel-desc-group-item", cancelEditDescItem);
    }

    function loadProductViewEventHandlers() {
        $("div#admin-content")            
            .on("click", "img.admin-img-thumb", setImage)
    }

    function validateAndSubmitJson() {
        let editForm = $("#edit-form");
        let result = editForm.valid();
        if (!result)
            return;
        let data = editForm.serializeArray();
        //console.log(data);
        let dataToJson: any = data.reduce(function (res, item) {
            res[item.name] = item.value;
            return res;
        }, {});
        let csrftoken = $(`input[name="__RequestVerificationToken"]`).val().toString();
        dataToJson.Price = (dataToJson.Price as string).replace(",", ".");
        dataToJson.Images = getImagesData();
        dataToJson.Categories = getCategoriesData();
        dataToJson.DescriptionGroups = getDescriptionsData();
        //console.log(dataToJson);
        let json = JSON.stringify(dataToJson);
        //console.log(json);
        //console.log("sending json");
        $.ajax({
            type: "POST",
            url: "/admin/editItem",
            headers: {
                RequestVerificationToken: csrftoken,
                "Content-Type": "application/json"
            },
            data: json,
            //timeout: 15000,
            beforeSend: function (jqXHR) {
                $("button#edit-form-submit").prop("disabled", true);
                $("span#edit-form-submit-icon").removeClass("fa-check").addClass("fa-spinner fa-spin");
            },
            success: function (response) {
                window.location.replace(response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //debug
                // console.log("Error on ajax:", jqXHR, textStatus, errorThrown);
                // document.open();
                // document.write(jqXHR.responseText);
                // document.close();

                let errorList = $("div.validation-summary-valid>ul");
                errorList.empty();
                $.validator.unobtrusive.parse("form#edit-form");                
                if (jqXHR.status === 400) {
                    var responseJson = JSON.parse(jqXHR.responseText);
                    if (Array.isArray(responseJson)) {                        
                        for (let i = 0; i < responseJson.length; i++) {
                            errorList.append("<li>").text(responseJson[i]);
                        }
                    } else {
                        errorList.append("<li>").text(jqXHR.responseText);
                    }
                }
                else if (jqXHR.status === 500) {
                    errorList.append("<li>").text("Ошибка сервера при обработке запроса.");
                }
                else {
                    errorList.append("<li>").text("Возникла ошибка при отправке запроса серверу.");
                }
                $("button#edit-form-submit").removeAttr("disabled");
                $("span#edit-form-submit-icon").addClass("fa-check").removeClass("fa-spinner fa-spin");
            }
        });
    }

    function getImagesData() {
        let container = $("div#image-container");
        if (container.length === 0) {
            return null;
        }
        let images = [];
        let contImages = container.children();
        contImages.each(function (i, element) {
            let e = $(element);
            images[i] = {
                Id: e.data("id"),
                Primary: e.hasClass("img-primary")
            };
            if (e.hasClass("img-add")) {
                images[i].DtoState = DtoState.Added;
                images[i].DataUrl = e.attr("src");
            } else {
                if (e.hasClass("img-delete")) {
                    images[i].DtoState = DtoState.Deleted;
                } else {
                    images[i].DtoState = DtoState.Unchanged;
                }
                images[i].Path = e.data("path");
                images[i].ThumbPath = e.attr("src");
            }
        });
        return images;
    }

    function getCategoriesData() {

        let cats = $(".category");
        if (cats.length === 0) {
            return null;
        }
        let catData = [];
        for (let i = 0; i < cats.length; i++) {
            let cat = $(cats[i]);
            catData[i] = {
                Id: cat.data("id"),
                Name: cat.find(".category-name").text(),
                Description: cat.find(".category-desc").text(),
                DtoState: DtoState.Unchanged
            };
            catData[i].Description = catData[i].Description.slice(1, catData[i].Description.length - 1);
            if (cat.hasClass("cat-add")) {
                catData[i].DtoState = DtoState.Added;
            } else if (cat.hasClass("cat-delete")) {
                catData[i].DtoState = DtoState.Deleted;
            }
        }
        return catData;
    }

    function getDescriptionsData() {
        let data = [];
        let descGroups = $(".desc-group");
        let productId = $("#edit-form").find("#Id").val();
        descGroups.each(function (i, e) {
            let group = $(e);
            data[i] = {
                Id: group.data("id"),
                Name: group.find(".desc-group-name").text(),
                Description: group.find(".desc-group-desc").text(),
                DescriptionItems: []
            };
            data[i].Description = data[i].Description.slice(1, data[i].Description.length - 1);
            let items = group.find(".desc-item");
            items.each(function (x, el) {
                let item = $(el);
                data[i].DescriptionItems[x] = {
                    Id: item.data("id"),
                    Name: item.find(".desc-item-name").text(),
                    DescriptionId: item.find(".desc-item-value").data("id"),
                    DescriptionValue: item.find(".desc-item-value").text(),
                    ProductId: productId,
                    DescriptionGroupId: data[i].Id,
                    DtoState: DtoState.Unchanged
                };
                if (item.hasClass("desc-item-add")) {
                    data[i].DescriptionItems[x].DtoState = DtoState.Added;
                } else if (item.hasClass("desc-item-modified")) {
                    data[i].DescriptionItems[x].DtoState = DtoState.Modified;
                }
                if (item.hasClass("desc-item-delete") || group.hasClass("desc-group-delete")) {
                    data[i].DescriptionItems[x].DtoState = DtoState.Deleted;
                }
            });
        });
        return data;
    }

    function generateImgId() { // random guid-like id //not used
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4();
    }

    function setPrimaryImage() {
        let thumbs = $("img.admin-img-thumb");
        if (thumbs.length === 0) { //if no images to set return
            return;
        }
        let img = $("img#admin-image-view");
        $(".img-primary").each(function (i, e) {
            e.classList.remove("img-primary");
        });
        let thumb = $(".admin-img-thumb[data-id=" + img.data("id") + "]");
        thumb.addClass("img-primary").removeClass("img-delete");
        setImage(thumb);
    }

    function setDeleteImage() {
        let thumbs = $("img.admin-img-thumb");
        if (thumbs.length === 0) { //if no images to delete return
            return;
        }
        let bigImage = $("img#admin-image-view");
        let id = bigImage.data("id");
        let thumbImage = $(".admin-img-thumb[data-id=" + id + "]");
        if (thumbImage.hasClass("img-delete")) { //on un-deleting: remove class
            thumbImage.removeClass("img-delete");
            bigImage.removeClass("img-delete");
            if ($(".admin-img-thumb.img-primary").length === 0) { //set primary image if there are none present
                thumbImage.addClass("img-primary");
                bigImage.removeClass("img-primary");
            }
        } else { // on deleting: add class
            let removedAdded = thumbImage.hasClass("img-add");
            if (removedAdded) { // remove html if image was just added
                thumbImage.remove();
                let thumbsLeft = $("img.admin-img-thumb");
                if (thumbsLeft.length === 0) { // if no images left, clear big image
                    $("img#admin-image-view").attr("src", "");
                } else {
                    setImage($(".img-primary"));
                }
            } else { // else just remove class
                thumbImage.addClass("img-delete");
                bigImage.addClass("img-delete");
            }
            if (thumbImage.hasClass("img-primary")) { // if we delete primary image set another image as such
                thumbImage.removeClass("img-primary");
                bigImage.removeClass("img-primary");
                let i = 0;
                thumbs = $("img.admin-img-thumb"); // get updated image list
                do {
                    thumbImage = $(thumbs[i++]);
                } while (thumbImage.hasClass("img-delete") && thumbs.length > i);
                if (thumbImage.hasClass("img-delete")) { // if all images are deleted return
                    if (removedAdded) {
                        setImage(thumbs.first());
                    }
                    return;
                }
                thumbImage.addClass("img-primary");
                if (removedAdded) {
                    setImage(thumbImage);
                }
            }
        }
    }

    function setImage(e) {
        let img;
        if (e.target) {
            img = $(e.target);
        } else {
            img = $(e);
        }
        let newSrc = img.attr("src");
        let target = $("img#admin-image-view").first();
        //set src
        if (img.attr("src").substring(0, 10) !== "data:image") {
            newSrc = img.data("path");
        }
        target.attr("src", newSrc);
        target.data("id", img.data("id"));
        //set img-primary class
        if (img.hasClass("img-primary")) {
            target.addClass("img-primary");
        } else {
            target.removeClass("img-primary");
        }
        //set img-delete class
        if (img.hasClass("img-delete")) {
            target.addClass("img-delete");
        } else {
            target.removeClass("img-delete");
        }
    }

    function loadProductCategoriesDropdown() {
        let loadingBar = $("<div>").attr("id", "cat-loading").addClass("text-center mt-1").text("Загрузка")
            .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
        $.ajax({
            method: "GET",
            url: "/api/admin/Categories",
            beforeSend: function (jqXHR) {
                $(".bootstrap-select>.category-btn+.dropdown-menu>.inner")
                    .prepend(loadingBar);
            },
            success: function (response) {
                response.forEach(function (item) {
                    $("#category-select").append($("<option>",
                        {
                            "value": item.id,
                            "data-subtext": item.description,
                            "text": item.name
                        }
                    ));
                });
                loadingBar.remove();
                $("#category-select").selectpicker("refresh");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#cat-loading").html("Ошибка загрузки");
            }
        });
    }

    function addCategory() {
        let select = $("#category-select");
        let id = select.val() === "" ? -1 : select.val();
        let existing = $(".category[data-id=" + id + "]");
        if (select.val() === "" || existing.length) {
            if (existing.hasClass("cat-delete")) {
                existing.removeClass("d-none cat-delete");
                $("#categories").append(existing);
                return;
            }
            $("#category-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100).fadeIn(100);
            return;
        }
        let name = select.find("option:selected").text();
        let desc = select.find("option:selected").data().subtext;
        let nameSpan = $("<span>").addClass("category-name").text(name);
        let descSpan = $("<span>").addClass("category-desc text-muted").text(`(${desc})`);
        let buttonSpan = $("<span>").addClass("remove-cat btn-pushy btn btn-sm btn-outline-danger fa fa-close");
        let category = $("<div>").addClass("category cat-add").data("id", id).append([nameSpan, descSpan, buttonSpan]);
        category.children().after(" ");
        $("#categories").append(category);
    }

    function removeCategory() {
        let catBtn = $(this);
        let parent = catBtn.parent();
        if (parent.hasClass("cat-add")) {
            parent.remove();
        } else {
            parent.addClass("d-none cat-delete");
        }
    }

    function loadProductDescGroupDropdown() {
        let loadingBar = $("<div>").attr("id", "desc-group-loading").addClass("text-center mt-1").text("Загрузка")
            .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
        $.ajax({
            method: "GET",
            url: "/api/admin/DescriptionGroups",
            beforeSend: function (jqXHR) {
                $(".bootstrap-select>.desc-group-btn+.dropdown-menu>.inner")
                    .prepend(loadingBar);
            },
            success: function (response) {
                response.forEach(function (item) {
                    $("#desc-group-select").append($("<option>")
                        .val(item.id)
                        .data("subtext", item.description)
                        .text(item.name));                    
                });
                loadingBar.remove();
                $("#desc-group-select").selectpicker("refresh");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#desc-group-loading").html("Ошибка загрузки");
            }
        });
    }

    function addDescGroup() {
        let select = $("#desc-group-select");
        let id = select.val() === "" ? -1 : select.val();
        let existing = $(".desc-group[data-id=" + id + "]");
        var descGroups = $("#desc-groups");
        if (select.val() === "" || existing.length) {
            if (existing.hasClass("desc-group-delete")) {
                existing.removeClass("d-none desc-group-delete");
                descGroups.append(existing);
                return;
            }
            $("#desc-group-select+.dropdown-toggle")
                .fadeIn(100)
                .fadeOut(100)
                .fadeIn(100)
                .fadeOut(100)
                .fadeIn(100);
            return;
        }
        let name = select.find("option:selected").text();
        let desc = select.find("option:selected").data().subtext;

        let strVar = `
<div class="desc-group my-2 desc-group-add" data-id="${id}">
<div>
<span class="desc-group-name h6">${name}</span>`;
        if (desc != "" || desc != undefined) {
            strVar += `<span class="desc-group-desc text-muted">(${desc})</span>`
        }
        strVar += `
<span class="remove-desc-group btn-pushy btn btn-outline-danger btn-sm fa fa-close mb-1"></span>
<div>
<select id="desc-group-items-select" class="selectpicker" hidden data-live-search="true" data-live-search-normalize="true"
data-live-search-style="contains" data-style="btn-outline-primary desc-group-items-btn p-1" data-width="fit"
title="Добавить описание" data-live-search-placeholder="Поиск"></select>
<span class="add-desc-group-item btn-pushy btn btn-outline-success fa fa-check"></span>
</div>
</div>
<div class="desc-group-items">
</div>
</div>`;
        let descGroup: any = $.parseHTML(strVar);
        descGroups.append(descGroup);
        $(descGroup).find("#desc-group-items-select").selectpicker("refresh");
    }

    function removeDescGroup() {
        let btn = $(this);
        let container = btn.closest("div.desc-group");
        if (container.hasClass("desc-group-add")) {
            container.remove();
        } else {
            container.addClass("d-none desc-group-delete");
        }
    }

    function loadProductDescGroupItemsDropdown() {
        let loadingBar = $("<div>")
            .attr("id", "desc-group-items-loading")
            .addClass("text-center mt-1")
            .text("Загрузка")
            .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
        let container = $(this).closest(".desc-group");
        let id = container.data("id");
        $.ajax({
            type: "GET",
            url: `/api/admin/DescriptionItems/${id}`,
            beforeSend: function (jqXHR) {
                container.find(".bootstrap-select>.desc-group-items-btn+.dropdown-menu>.inner")
                    .prepend(loadingBar);
            },
            success: function (response) {
                response.forEach(function (item) {
                    container.find("#desc-group-items-select").append($("<option>",
                        {
                            "value": item.id,
                            "text": item.name
                        }
                    ));
                });
                loadingBar.remove();
                container.find("#desc-group-items-select").selectpicker("refresh");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                container.find("#desc-group-items-loading").html("Ошибка загрузки");
            }
        });
    }

    function addDescGroupItem() {
        let container = $(this).closest(".desc-group");
        let select = container.find("#desc-group-items-select");
        let id = select.val() === "" ? -1 : select.val();
        let existing = $(".desc-item[data-id=" + id + "]");
        if (select.val() === "" || existing.length) {
            if (existing.hasClass("desc-item-delete")) {
                existing.removeClass("d-none desc-item-delete");
                return;
            }
            container.find("#desc-group-items-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100)
                .fadeIn(100);
        } else {
            let name = select.find("option:selected").text();
            let value = "*не указано*";
            let strVar = `
<div class="desc-item desc-item-add" data-id="${id}">
<span class="desc-item-name">${name}<\/span> : <span class="desc-item-value">${value}</span>
<textarea rows="3" class="form-control d-none desc-item-input"></textarea>
<div class="desc-item-save d-none">
<span class="btn btn-sm btn-pushy btn-outline-success fa fa-check save-desc-group-item"></span>
<span class="btn btn-sm btn-pushy btn-outline-danger fa fa-undo cancel-desc-group-item"></span>
</div>
<div class="desc-item-edit-delete d-inline-block">
<span class="btn btn-sm btn-pushy btn-outline-primary fa fa-edit edit-desc-group-item"></span>
<span class="btn btn-sm btn-pushy btn-outline-danger fa fa-close remove-desc-group-item"></span>
</div>
</div>`;
            let descItem = $.parseHTML(strVar);
            container.find(".desc-group-items").append(descItem);
        }
        //sort ??
    }

    function removeDescGroupItem() {
        let btn = $(this);
        let container = btn.closest("div.desc-item");
        if (container.hasClass("desc-item-add")) {
            container.remove();
        } else {
            container.addClass("d-none desc-item-delete");
        }
    }

    function editDescItem() {
        let container = $(this).closest(".desc-item");
        let textToEdit = container.find(".desc-item-value").addClass("d-none").text();
        container.find(".desc-item-input").removeClass("d-none").text(textToEdit);
        container.find(".desc-item-save").removeClass("d-none");
        container.find(".desc-item-edit-delete").addClass("d-none").removeClass("d-inline-block");
    }

    function saveEditDescItem() {
        let container = $(this).closest(".desc-item").addClass("desc-item-modified");
        let textToSave = container.find(".desc-item-input").addClass("d-none").val();
        container.find(".desc-item-value").removeClass("d-none").text(textToSave.toString());
        container.find(".desc-item-save").addClass("d-none");
        container.find(".desc-item-edit-delete").removeClass("d-none").addClass("d-inline-block");
    }

    function cancelEditDescItem() {
        let container = $(this).closest(".desc-item");
        container.find(".desc-item-input").addClass("d-none");
        container.find(".desc-item-value").removeClass("d-none");
        container.find(".desc-item-save").addClass("d-none");
        container.find(".desc-item-edit-delete").removeClass("d-none").addClass("d-inline-block");
    }

    function loadImagesFromInput() {
        //Get count of selected files
        let countFiles = $(this)[0].files.length;
        let imageContainer = $("#image-container");
        //image_holder.empty();
        if (typeof FileReader !== undefined) {
            //loop for each file selected for uploaded.
            for (let i = 0; i < countFiles; i++) {
                let imgPath = $(this)[0].files[i].name;
                let extn = imgPath.substring(imgPath.lastIndexOf(".") + 1).toLowerCase();
                if (extn === "gif" || extn === "png" || extn === "jpg" || extn === "jpeg") {
                    let reader = new FileReader();
                    reader.onload = function (e: any) {
                        let imgClass = "admin-img-thumb img-add m-2 border";
                        let flag = $(".admin-img-thumb.img-primary").length === 0;
                        if (flag) { //if no primary image present set this image as primary
                            imgClass = imgClass + " img-primary";
                        }
                        $("<img />",
                            {
                                "class": imgClass,
                                "click": setImage,
                                "data-id": Math.floor(Math.random() * 10000000 + 10000000) * -1, //getImgId() //need only unique int id
                                "src": e.target.result
                            }).appendTo(imageContainer);
                        if (flag) {
                            let img = $("img.admin-img-thumb.img-add");
                            setImage(img); // set big image
                        }
                    };
                    imageContainer.show();
                    reader.readAsDataURL($(this)[0].files[i]);
                }
                //else {
                //    alert("Выберите изображения.");
                //}
            }
            $(this).val(""); //reset this upload form to enable adding same file(s)
        } else {
            alert("Браузер не поддерживает загрузку картинок.");
        }
    }
}