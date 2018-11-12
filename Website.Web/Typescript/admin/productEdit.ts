
$("div#admin-content").on("click", "img.admin-img-thumb", setImage);
$("div#admin-content").on("click", "#image-primary-button", setPrimaryImage);
$("div#admin-content").on("click", "#image-remove-button", setDeleteImage);
$("div#admin-content").on("change", "#file-upload-button", loadImagesFromInput);
$("div#admin-content").on("click", ".category-btn", loadCategoriesDropdown);
$("div#admin-content").on("click", ".add-cat", addCategory);
$("div#admin-content").on("click", ".remove-cat", removeCategory);
$("div#admin-content").on("click", ".desc-group-btn", loadDescGroupDropdown);
$("div#admin-content").on("click", ".add-desc-group", addDescGroup);
$("div#admin-content").on("click", ".remove-desc-group", removeDescGroup);
$("div#admin-content").on("click", ".desc-group-items-btn", loadDescGroupItemsDropdown);
$("div#admin-content").on("click", ".add-desc-group-item", addDescGroupItem);
$("div#admin-content").on("click", ".remove-desc-group-item", removeDescGroupItem);
$("div#admin-content").on("click", ".edit-desc-group-item", editDescItem);
$("div#admin-content").on("click", ".save-desc-group-item", saveEditDescItem);
$("div#admin-content").on("click", ".cancel-desc-group-item", cancelEditDescItem);

function validateAndSubmitJson() {
    var result = $("#edit-form").validate().valid();
    if (!result)
        return;
    var data = $("#edit-form").serializeArray();
    //console.log(data);
    var dataToJson: any = data.reduce(function (res, item) {
        res[item.name] = item.value;
        return res;
    }, {});
    var csrftoken = $("input[name=\"__RequestVerificationToken\"").val().toString();
    dataToJson.Price = (dataToJson.Price as string).replace(",", ".");
    dataToJson.Images = getImagesData();
    dataToJson.Categories = getCategoriesData();
    dataToJson.Descriptions = getDescriptionsData();
    //console.log(dataToJson);
    var json = JSON.stringify(dataToJson);
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
        beforeSend: function (jqXHR) {
            $("button#edit-form-submit").prop("disabled", true);
            $("span#edit-form-submit-icon").removeClass("fa-check").addClass("fa-spinner fa-spin");
        },
        success: function (response) {
            $("div#admin-content").html(response);
            $.validator.unobtrusive.parse("form#edit-form");
            $("select.selectpicker").selectpicker("refresh");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //debug
            console.log("Error on ajax:", jqXHR, textStatus, errorThrown);
            document.open();
            document.write(jqXHR.responseText);
            document.close();

            //release
            //var errorList = $("div.validation-summary-valid>ul>li")[0];
            //errorList.removeAttribute("style");
            //if (jqXHR.status === 400) {
            //    errorList.innerText = jqXHR.responseText; 
            //} else {
            //    errorList.innerText = "Возникла ошибка при отправке запроса серверу.";
            //}
        }
    });
}

function getImagesData() {
    var container = $("div#image-container");
    if (container.length === 0) {
        return null;
    }
    var images = [];
    var contImages = container.children();
    contImages.each(function (i, element) {
        var e = $(element);
        images[i] = {
            Id: e.data("id"),
            Primary: e.hasClass("img-primary")
        };
        if (e.hasClass("img-add")) {
            images[i].DtoState = "added";
            images[i].DataUrl = e.attr("src");
        } else {
            if (e.hasClass("img-delete")) {
                images[i].DtoState = "deleted";
            } else {
                images[i].DtoState = "unchanged";
            }
            images[i].Path = e.data("path");
            images[i].ThumbPath = e.attr("src");
        }
    });
    return images;
}

function getCategoriesData() {

    var cats = $(".category");
    if (cats.length === 0) {
        return null;
    }
    var catData = [];
    for (var i = 0; i < cats.length; i++) {
        var cat = $(cats[i]);
        catData[i] = {
            Id: cat.data("id"),
            Name: cat.find(".category-name").text(),
            Description: cat.find(".category-desc").text()
        };
        catData[i].Description = catData[i].Description.slice(1, catData[i].Description.length - 1);
        if (cat.hasClass("cat-add")) {
            catData[i].DtoState = "added";
        }
        else if (cat.hasClass("cat-delete")) {
            catData[i].DtoState = "deleted";
        } else {
            catData[i].DtoState = "unchanged";
        }
    }
    return catData;
}

function getDescriptionsData() {
    var data = [];
    var descGroups = $(".desc-group");
    descGroups.each(function (i, e) {
        var group = $(e);
        data[i] = {
            Id: group.data("id"),
            Name: group.find(".desc-group-name").text(),
            Description: group.find(".desc-group-desc").text(),
            Items: []
        };
        data[i].Description = data[i].Description.slice(1, data[i].Description.length - 1);
        var items = group.find(".desc-item");
        items.each(function (x, el) {
            var item = $(el);
            data[i].Items[x] = {
                Id: item.data("id"),
                Name: item.find(".desc-item-name").text(),
                DescriptionId: item.find(".desc-item-value").data("id"),
                DescriptionValue: item.find(".desc-item-value").text(),
                DtoState: "unchanged"
            };
            if (item.hasClass("desc-item-add")) {
                data[i].Items[x].DtoState = "added";
            }
            else if (item.hasClass("desc-item-modified")) {
                data[i].Items[x].DtoState = "modified";
            }
            if (item.hasClass("desc-item-delete") || group.hasClass("desc-group-delete")) {
                data[i].Items[x].DtoState = "deleted";
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
    var thumbs = $("img.admin-img-thumb");
    if (thumbs.length === 0) { //if no images to set return
        return;
    }
    var img = $("img#admin-image-view");
    $(".img-primary").each(function (i, e) {
        e.classList.remove("img-primary");
    });
    var thumb = $(".admin-img-thumb[data-id=" + img.data("id") + "]");
    thumb.addClass("img-primary").removeClass("img-delete");
    setImage(thumb);
}

function setDeleteImage() {
    var thumbs = $("img.admin-img-thumb");
    if (thumbs.length === 0) { //if no images to delete return
        return;
    }
    var bigImage = $("img#admin-image-view");
    var id = bigImage.data("id");
    var thumbImage = $(".admin-img-thumb[data-id=" + id + "]");
    if (thumbImage.hasClass("img-delete")) { //on un-deleting: remove class
        thumbImage.removeClass("img-delete");
        bigImage.removeClass("img-delete");
        if ($(".admin-img-thumb.img-primary").length === 0) { //set primary image if there are none present
            thumbImage.addClass("img-primary");
            bigImage.removeClass("img-primary");
        }
    } else { // on deleting: add class
        var removedAdded = thumbImage.hasClass("img-add");
        if (removedAdded) { // remove html if image was just added
            thumbImage.remove();
            var thumbsLeft = $("img.admin-img-thumb");
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
            var i = 0;
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
    var img;
    if (e.target) {
        img = $(e.target);
    } else {
        img = $(e);
    }
    var newSrc = img.attr("src");
    var target = $("img#admin-image-view").first();
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

function loadCategoriesDropdown() {
    var loadingBar = $("<div>").attr("id", "cat-loading").addClass("text-center mt-1").text("Загрузка")
        .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
    $.ajax({
        type: "GET",
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
    var select = $("#category-select");
    var id = select.val() === "" ? -1 : select.val();
    var existing = $(".category[data-id=" + id + "]");
    if ($("#category-select").val() === "" || existing.length) {
        if (existing.hasClass("cat-delete")) {
            existing.removeClass("d-none cat-delete");
            $("#categories").append(existing);
            return;
        }
        $("#category-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100).fadeIn(100);
        return;
    }
    var name = select.find("option:selected").text();
    var desc = select.find("option:selected").data().subtext;
    var nameSpan = $("<span>").addClass("category-name").text(name);
    var descSpan = $("<span>").addClass("category-desc text-muted").text(`(${desc})`);
    var buttonSpan = $("<span>").addClass("remove-cat btn-pushy btn btn-outline-danger fa fa-close");
    var category = $("<div>").addClass("category cat-add").data("id", id).append([nameSpan, descSpan, buttonSpan]);
    category.children().after(" ");
    $("#categories").append(category);
}

function removeCategory() {
    var catBtn = $(this);
    var parent = catBtn.parent();
    if (parent.hasClass("cat-add")) {
        parent.remove();
    } else {
        parent.addClass("d-none cat-delete");
    }
}

function loadDescGroupDropdown() {
    var loadingBar = $("<div>").attr("id", "desc-group-loading").addClass("text-center mt-1").text("Загрузка")
        .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
    $.ajax({
        type: "GET",
        url: "/api/admin/DescriptionGroups",
        beforeSend: function (jqXHR) {
            $(".bootstrap-select>.desc-group-btn+.dropdown-menu>.inner")
                .prepend(loadingBar);
        },
        success: function (response) {
            response.forEach(function (item) {
                $("#desc-group-select").append($("<option>",
                    {
                        "value": item.id,
                        "data-subtext": item.description,
                        "text": item.name
                    }
                ));
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
    var select = $("#desc-group-select");
    var id = select.val() === "" ? -1 : select.val();
    var existing = $(".desc-group[data-id=" + id + "]");
    if ($("#desc-group-select").val() === "" || existing.length) {
        if (existing.hasClass("desc-group-delete")) {
            existing.removeClass("d-none desc-group-delete");
            $("#desc-groups").append(existing);
            return;
        }
        $("#desc-group-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100).fadeIn(100);
        return;
    }
    var name = select.find("option:selected").text();
    var desc = select.find("option:selected").data().subtext;

    var strVar = "";
    strVar += '<div class="desc-group my-2 desc-group-add" data-id="${id}">';
    strVar += " <div>";
    strVar += `  <span class="desc-group-name h6">${name}</span>`;
    strVar += `  <span class="desc-group-desc text-muted">(${desc})</span>`;
    strVar += `  <span class="remove-desc-group btn-pushy btn btn-outline-danger btn-sm fa fa-close mb-1"></span>`;
    strVar += "   <div>";
    strVar += `    <select id="desc-group-items-select" class="selectpicker" hidden data-live-search="true" data-live-search-normalize="true" data-live-search-style="contains"`;
    strVar += `data-style="btn-outline-primary desc-group-items-btn p-1" data-width="fit" title="Добавить описание" data-live-search-placeholder="Поиск"></select>`;
    strVar += `    <span class="add-desc-group-item btn-pushy btn btn-outline-success fa fa-check"></span>`;
    strVar += "   </div>";
    strVar += " </div>";
    strVar += ` <div class="desc-group-items">`;
    strVar += " </div>";
    strVar += "</div>";
    var descGroup: any = $.parseHTML(strVar);
    $("#desc-groups").append(descGroup);
    $(descGroup).find("#desc-group-items-select").selectpicker("refresh");
}

function removeDescGroup() {
    var btn = $(this);
    var container = btn.closest("div.desc-group");
    if (container.hasClass("desc-group-add")) {
        container.remove();
    } else {
        container.addClass("d-none desc-group-delete");
    }
}

function loadDescGroupItemsDropdown() {
    var loadingBar = $("<div>").attr("id", "desc-group-items-loading").addClass("text-center mt-1").text("Загрузка")
        .append($("<span>").addClass("fa fa-spinner fa-spin ml-2"));
    var container = $(this).closest(".desc-group");
    var id = container.data("id");
    $.ajax({
        type: "GET",
        url: "/api/admin/DescriptionItems/" + id,
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
    var container = $(this).closest(".desc-group");
    var select = container.find("#desc-group-items-select");
    var id = select.val() === "" ? -1 : select.val();
    var existing = $(".desc-item[data-id=" + id + "]");
    if ($("#desc-group-items-select").val() === "" || existing.length) {
        if (existing.hasClass("desc-item-delete")) {
            existing.removeClass("d-none desc-item-delete");
            return;
        }
        container.find("#desc-group-items-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100)
            .fadeIn(100);
    } else {
        var name = select.find("option:selected").text();
        var value = "*не указано*";
        var strVar = "";
        strVar += `<div class="desc-item desc-item-add" data-id="${id}">`;
        strVar += ` <span class="desc-item-name">${name}<\/span> : <span class="desc-item-value">${value}</span>`;
        strVar += ` <textarea rows="3" class="form-control d-none desc-item-input"></textarea>`;
        strVar += ` <div class="desc-item-save d-none">`;
        strVar += `  <span class="btn btn-sm btn-pushy btn-outline-success fa fa-check save-desc-group-item"></span>`;
        strVar += `  <span class="btn btn-sm btn-pushy btn-outline-danger fa fa-undo cancel-desc-group-item"></span>`;
        strVar += " </div>";
        strVar += ` <div class="desc-item-edit-delete d-inline-block">`;
        strVar += `  <span class="btn btn-sm btn-pushy btn-outline-primary fa fa-edit edit-desc-group-item"></span>`;
        strVar += `  <span class="btn btn-sm btn-pushy btn-outline-danger fa fa-close remove-desc-group-item"></span>`;
        strVar += " </div>";
        strVar += "</div>";
        var descItem = $.parseHTML(strVar);
        container.find(".desc-group-items").append(descItem);
    }
    //sort ??
}

function removeDescGroupItem() {
    var btn = $(this);
    var container = btn.closest("div.desc-item");
    if (container.hasClass("desc-item-add")) {
        container.remove();
    } else {
        container.addClass("d-none desc-item-delete");
    }
}

function editDescItem() {
    var container = $(this).closest(".desc-item");
    var textToEdit = container.find(".desc-item-value").addClass("d-none").text();
    container.find(".desc-item-input").removeClass("d-none").text(textToEdit);
    container.find(".desc-item-save").removeClass("d-none");
    container.find(".desc-item-edit-delete").addClass("d-none").removeClass("d-inline-block");
}

function saveEditDescItem() {
    var container = $(this).closest(".desc-item").addClass("desc-item-modified");
    var textToSave = container.find(".desc-item-input").addClass("d-none").val();
    container.find(".desc-item-value").removeClass("d-none").text(textToSave.toString());
    container.find(".desc-item-save").addClass("d-none");
    container.find(".desc-item-edit-delete").removeClass("d-none").addClass("d-inline-block");
}

function cancelEditDescItem() {
    var container = $(this).closest(".desc-item");
    container.find(".desc-item-input").addClass("d-none");
    container.find(".desc-item-value").removeClass("d-none");
    container.find(".desc-item-save").addClass("d-none");
    container.find(".desc-item-edit-delete").removeClass("d-none").addClass("d-inline-block");
}

function loadImagesFromInput() {
    //Get count of selected files
    var countFiles = $(this)[0].files.length;
    var imageContainer = $("#image-container");
    //image_holder.empty();
    if (typeof FileReader !== undefined) {
        //loop for each file selected for uploaded.
        for (var i = 0; i < countFiles; i++) {
            var imgPath = $(this)[0].files[i].name;
            var extn = imgPath.substring(imgPath.lastIndexOf(".") + 1).toLowerCase();
            if (extn === "gif" || extn === "png" || extn === "jpg" || extn === "jpeg") {
                var reader = new FileReader();
                reader.onload = function (e: any) {
                    var imgClass = "admin-img-thumb img-add m-2 border";
                    var flag = $(".admin-img-thumb.img-primary").length === 0;
                    if (flag) { //if no primary image present set this image as primary
                        imgClass = imgClass + " img-primary";
                    }
                    $("<img />",
                        {
                            "class": imgClass,
                            "click": setImage,
                            "data-id": Math.floor(Math.random() * 10000000 + 10000000), //getImgId() //need only unique int id
                            "src": e.target.result
                        }).appendTo(imageContainer);
                    if (flag) {
                        var img = $("img.admin-img-thumb.img-add");
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