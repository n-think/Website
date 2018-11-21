var productEdit;
(function (productEdit) {
    $("div#admin-content")
        .on("click", "#edit-form-submit", validateAndSubmitJson)
        .on("click", "img.admin-img-thumb", setImage)
        .on("click", "#image-primary-button", setPrimaryImage)
        .on("click", "#image-remove-button", setDeleteImage)
        .on("change", "#file-upload-button", loadImagesFromInput)
        .on("click", ".category-btn", loadCategoriesDropdown)
        .on("click", ".add-cat", addCategory)
        .on("click", ".remove-cat", removeCategory)
        .on("click", ".desc-group-btn", loadDescGroupDropdown)
        .on("click", ".add-desc-group", addDescGroup)
        .on("click", ".remove-desc-group", removeDescGroup)
        .on("click", ".desc-group-items-btn", loadDescGroupItemsDropdown)
        .on("click", ".add-desc-group-item", addDescGroupItem)
        .on("click", ".remove-desc-group-item", removeDescGroupItem)
        .on("click", ".edit-desc-group-item", editDescItem)
        .on("click", ".save-desc-group-item", saveEditDescItem)
        .on("click", ".cancel-desc-group-item", cancelEditDescItem);
    var state = enums.DtoState;
    function validateAndSubmitJson() {
        var editForm = $("#edit-form");
        var result = editForm.validate().valid();
        if (!result)
            return;
        var data = editForm.serializeArray();
        var dataToJson = data.reduce(function (res, item) {
            res[item.name] = item.value;
            return res;
        }, {});
        var csrftoken = $("input[name=\"__RequestVerificationToken\"]").val().toString();
        dataToJson.Price = dataToJson.Price.replace(",", ".");
        dataToJson.Images = getImagesData();
        dataToJson.Categories = getCategoriesData();
        dataToJson.Descriptions = getDescriptionsData();
        var json = JSON.stringify(dataToJson);
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
                var errorListItem = $("div.validation-summary-valid>ul>li").first();
                errorListItem.removeAttr("style");
                if (jqXHR.status === 400) {
                    errorListItem.text(jqXHR.responseText);
                }
                else {
                    errorListItem.text("Возникла ошибка при отправке запроса серверу.");
                }
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
                images[i].DtoState = state.Added;
                images[i].DataUrl = e.attr("src");
            }
            else {
                if (e.hasClass("img-delete")) {
                    images[i].DtoState = state.Deleted;
                }
                else {
                    images[i].DtoState = state.Unchanged;
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
                Description: cat.find(".category-desc").text(),
                DtoState: state.Unchanged
            };
            catData[i].Description = catData[i].Description.slice(1, catData[i].Description.length - 1);
            if (cat.hasClass("cat-add")) {
                catData[i].DtoState = state.Added;
            }
            else if (cat.hasClass("cat-delete")) {
                catData[i].DtoState = state.Deleted;
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
                    DtoState: state.Unchanged
                };
                if (item.hasClass("desc-item-add")) {
                    data[i].Items[x].DtoState = state.Added;
                }
                else if (item.hasClass("desc-item-modified")) {
                    data[i].Items[x].DtoState = state.Modified;
                }
                if (item.hasClass("desc-item-delete") || group.hasClass("desc-group-delete")) {
                    data[i].Items[x].DtoState = state.Deleted;
                }
            });
        });
        return data;
    }
    function generateImgId() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + "-" + s4() + "-" + s4() + "-" + s4() + "-" + s4() + s4() + s4();
    }
    function setPrimaryImage() {
        var thumbs = $("img.admin-img-thumb");
        if (thumbs.length === 0) {
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
        if (thumbs.length === 0) {
            return;
        }
        var bigImage = $("img#admin-image-view");
        var id = bigImage.data("id");
        var thumbImage = $(".admin-img-thumb[data-id=" + id + "]");
        if (thumbImage.hasClass("img-delete")) {
            thumbImage.removeClass("img-delete");
            bigImage.removeClass("img-delete");
            if ($(".admin-img-thumb.img-primary").length === 0) {
                thumbImage.addClass("img-primary");
                bigImage.removeClass("img-primary");
            }
        }
        else {
            var removedAdded = thumbImage.hasClass("img-add");
            if (removedAdded) {
                thumbImage.remove();
                var thumbsLeft = $("img.admin-img-thumb");
                if (thumbsLeft.length === 0) {
                    $("img#admin-image-view").attr("src", "");
                }
                else {
                    setImage($(".img-primary"));
                }
            }
            else {
                thumbImage.addClass("img-delete");
                bigImage.addClass("img-delete");
            }
            if (thumbImage.hasClass("img-primary")) {
                thumbImage.removeClass("img-primary");
                bigImage.removeClass("img-primary");
                var i = 0;
                thumbs = $("img.admin-img-thumb");
                do {
                    thumbImage = $(thumbs[i++]);
                } while (thumbImage.hasClass("img-delete") && thumbs.length > i);
                if (thumbImage.hasClass("img-delete")) {
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
        }
        else {
            img = $(e);
        }
        var newSrc = img.attr("src");
        var target = $("img#admin-image-view").first();
        if (img.attr("src").substring(0, 10) !== "data:image") {
            newSrc = img.data("path");
        }
        target.attr("src", newSrc);
        target.data("id", img.data("id"));
        if (img.hasClass("img-primary")) {
            target.addClass("img-primary");
        }
        else {
            target.removeClass("img-primary");
        }
        if (img.hasClass("img-delete")) {
            target.addClass("img-delete");
        }
        else {
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
                    $("#category-select").append($("<option>", {
                        "value": item.id,
                        "data-subtext": item.description,
                        "text": item.name
                    }));
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
        if (select.val() === "" || existing.length) {
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
        var descSpan = $("<span>").addClass("category-desc text-muted").text("(" + desc + ")");
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
        }
        else {
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
                    $("#desc-group-select").append($("<option>", {
                        "value": item.id,
                        "data-subtext": item.description,
                        "text": item.name
                    }));
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
        var descGroups = $("#desc-groups");
        if (select.val() === "" || existing.length) {
            if (existing.hasClass("desc-group-delete")) {
                existing.removeClass("d-none desc-group-delete");
                descGroups.append(existing);
                return;
            }
            $("#desc-group-select+.dropdown-toggle").fadeIn(100).fadeOut(100).fadeIn(100).fadeOut(100).fadeIn(100);
            return;
        }
        var name = select.find("option:selected").text();
        var desc = select.find("option:selected").data().subtext;
        var strVar = "\n<div class=\"desc-group my-2 desc-group-add\" data-id=\"" + id + "\">\n<div>\n<span class=\"desc-group-name h6\">" + name + "</span>\n<span class=\"desc-group-desc text-muted\">(" + desc + ")</span>\n<span class=\"remove-desc-group btn-pushy btn btn-outline-danger btn-sm fa fa-close mb-1\"></span>\n<div>\n<select id=\"desc-group-items-select\" class=\"selectpicker\" hidden data-live-search=\"true\" data-live-search-normalize=\"true\"\ndata-live-search-style=\"contains\" data-style=\"btn-outline-primary desc-group-items-btn p-1\" data-width=\"fit\"\ntitle=\"\u0414\u043E\u0431\u0430\u0432\u0438\u0442\u044C \u043E\u043F\u0438\u0441\u0430\u043D\u0438\u0435\" data-live-search-placeholder=\"\u041F\u043E\u0438\u0441\u043A\"></select>\n<span class=\"add-desc-group-item btn-pushy btn btn-outline-success fa fa-check\"></span>\n</div>\n</div>\n<div class=\"desc-group-items\">\n</div>\n</div>";
        var descGroup = $.parseHTML(strVar);
        descGroups.append(descGroup);
        $(descGroup).find("#desc-group-items-select").selectpicker("refresh");
    }
    function removeDescGroup() {
        var btn = $(this);
        var container = btn.closest("div.desc-group");
        if (container.hasClass("desc-group-add")) {
            container.remove();
        }
        else {
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
                    container.find("#desc-group-items-select").append($("<option>", {
                        "value": item.id,
                        "text": item.name
                    }));
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
        }
        else {
            var name_1 = select.find("option:selected").text();
            var value = "*не указано*";
            var strVar = "\n<div class=\"desc-item desc-item-add\" data-id=\"" + id + "\">\n<span class=\"desc-item-name\">" + name_1 + "</span> : <span class=\"desc-item-value\">" + value + "</span>\n<textarea rows=\"3\" class=\"form-control d-none desc-item-input\"></textarea>\n<div class=\"desc-item-save d-none\">\n<span class=\"btn btn-sm btn-pushy btn-outline-success fa fa-check save-desc-group-item\"></span>\n<span class=\"btn btn-sm btn-pushy btn-outline-danger fa fa-undo cancel-desc-group-item\"></span>\n</div>\n<div class=\"desc-item-edit-delete d-inline-block\">\n<span class=\"btn btn-sm btn-pushy btn-outline-primary fa fa-edit edit-desc-group-item\"></span>\n<span class=\"btn btn-sm btn-pushy btn-outline-danger fa fa-close remove-desc-group-item\"></span>\n</div>\n</div>";
            var descItem = $.parseHTML(strVar);
            container.find(".desc-group-items").append(descItem);
        }
    }
    function removeDescGroupItem() {
        var btn = $(this);
        var container = btn.closest("div.desc-item");
        if (container.hasClass("desc-item-add")) {
            container.remove();
        }
        else {
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
        var countFiles = $(this)[0].files.length;
        var imageContainer = $("#image-container");
        if (typeof FileReader !== undefined) {
            for (var i = 0; i < countFiles; i++) {
                var imgPath = $(this)[0].files[i].name;
                var extn = imgPath.substring(imgPath.lastIndexOf(".") + 1).toLowerCase();
                if (extn === "gif" || extn === "png" || extn === "jpg" || extn === "jpeg") {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var imgClass = "admin-img-thumb img-add m-2 border";
                        var flag = $(".admin-img-thumb.img-primary").length === 0;
                        if (flag) {
                            imgClass = imgClass + " img-primary";
                        }
                        $("<img />", {
                            "class": imgClass,
                            "click": setImage,
                            "data-id": Math.floor(Math.random() * 10000000 + 10000000),
                            "src": e.target.result
                        }).appendTo(imageContainer);
                        if (flag) {
                            var img = $("img.admin-img-thumb.img-add");
                            setImage(img);
                        }
                    };
                    imageContainer.show();
                    reader.readAsDataURL($(this)[0].files[i]);
                }
            }
            $(this).val("");
        }
        else {
            alert("Браузер не поддерживает загрузку картинок.");
        }
    }
})(productEdit || (productEdit = {}));
