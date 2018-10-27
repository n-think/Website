// read client height and submit max items per page 

$("a.read-height").click(function (event) { //first load from anchors
    var itemsPerPage = getItemCountFromHeight();
    event.target.href = event.target.href + "?&c=" + itemsPerPage;
});
$("form#admin-search-form").submit(function () { //submit from search
    var itemsPerPage = getItemCountFromHeight();
    $(this).append('<input type="hidden" name="c" value="' + itemsPerPage + '"/>');
});
$("select.admin-selector").change(function () { //submit form from selector
    var form = $("form#admin-search-form");
    form.submit();
});function getItemCountFromHeight() {
    var clientHeight = window.innerHeight;
    var value = Math.round((clientHeight - 400) / 65);
    return value < 3 ? 3 : value;
}

// **** item edit scripts ****

$("div#admin-content").on("click", "img.admin-img-thumb", setImage);
$("div#admin-content").on("click", "#image-primary-button", setPrimaryImage);
$("div#admin-content").on("click", "#image-remove-button", setDeleteImage);
$("div#admin-content").on("change", "#file-upload-button", loadImagesFromInput);

function validateAndSubmitJson() {
    var result = $("#edit-form").validate().valid();
    //console.log(result);
    if (!result)
        return;
    var data = $("#edit-form").serializeArray();
    //console.log(data);
    var dataToJson = data.reduce((res, item) => {
        res[item.name] = item.value;
        return res;
    }, {});
    dataToJson.Price = dataToJson.Price.replace(",", ".");

    //form json here
    dataToJson.Images = getImagesFromContainer($("div#image-container")[0]);

    console.log(dataToJson);
    var json = JSON.stringify(dataToJson);
    console.log(json);
    console.log("sending json");
    $.ajax({
        type: 'POST',
        url: '/Admin/EditItem',
        headers: {
            'RequestVerificationToken': dataToJson.__RequestVerificationToken,
            'Content-Type': 'application/json'
        },
        data: json,
        success: function (response) {
            $("div#admin-content").html(response);
            $.validator.unobtrusive.parse("form#edit-form");
            //document.open();
            //document.write(response);
            //document.close();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //console.log('Error on ajax:', jqXHR, textStatus, errorThrown);
            var errorList = $("div.validation-summary-valid>ul>li")[0];
            errorList.removeAttribute("style");
            if (jqXHR.status === 400) {
                errorList.innerText = jqXHR.responseText;
            } else {
                errorList.innerText = "Возникла ошибка при отправке запроса серверу.";
            }

        },
        async: true
    });
}

function getImagesFromContainer(container) {
    if (container === undefined) {
        return;
    }
    var images = [];
    var contImages = container.children;
    for (var i = 0; i < contImages.length; i++) {
        var e = contImages[i];
        images[i] = {
            Id: e.dataset.id,
            Primary: e.classList.contains("img-primary"),
            PendingDel: e.classList.contains("img-delete"),
            PendingAdd: e.classList.contains("img-add")
        };
        if (images[i].PendingAdd) {
            images[i].UriBase64Data = e.src;
        } else {
            images[i].Path = e.dataset.path;
            images[i].ThumbPath = e.src;
        }
    }
    return images;
}

function getImgId() { // random guid like id
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

function setPrimaryImage() {
    var thumbs = $("img.admin-img-thumb");
    if (thumbs.length === 0) { //if no images to set return
        return;
    }
    var img = $("img#admin-image-view")[0];
    var id = img.dataset.id;
    $(".img-primary").each((i, e) => {
        e.classList.remove("img-primary");
    });
    var thumb = $("[data-id=" + id + "]")[1];
    thumb.classList.add("img-primary");
    thumb.classList.remove("img-delete");
    //$("img#admin-image-view")[0].classList.add("img-primary");
    setImage(thumb);
}

function setDeleteImage() {
    var thumbs = $("img.admin-img-thumb");
    if (thumbs.length === 0) { //if no images to delete return
        return;
    }
    var bigImage = $("img#admin-image-view")[0];
    var id = bigImage.dataset.id;
    var thumbImage = $("[data-id=" + id + "]")[1];
    if (thumbImage.classList.contains("img-delete")) { //on undeleting: remove class
        thumbImage.classList.remove("img-delete");
        bigImage.classList.remove("img-delete");

        if (!checkExistingPrimaries()) { //set primary image if there are none present
            thumbImage.classList.add("img-primary");
            bigImage.classList.add("img-primary");
        }
    } else { // on deleting: add class
        var removedAdded = thumbImage.classList.contains("img-add");
        if (removedAdded) { // remove html if image was just added
            thumbImage.remove();
            $("#file-upload-button")[0].form.reset(); //reset upload form
            var thumbsLeft = $("img.admin-img-thumb");
            if (thumbsLeft.length === 0) { // if no images left, clear big image
                $("img#admin-image-view")[0].src = "";
            } else {
                setImage($(".img-primary")[0]);
            }
        } else { // else just remove class
            thumbImage.classList.add("img-delete");
            bigImage.classList.add("img-delete");
        }
        if (thumbImage.classList.contains("img-primary")) { // if we delete primary image set another image as such
            thumbImage.classList.remove("img-primary");
            bigImage.classList.remove("img-primary");
            var i = 0;
            thumbs = $("img.admin-img-thumb"); // get updated image list
            thumbImage = thumbs[i++];
            while (thumbImage.classList.contains("img-delete") && thumbs.length > i) {
                thumbImage = thumbs[i++];
            }
            if (thumbImage.classList.contains("img-delete")) { // if all images are deleted return
                if (removedAdded) {
                    setImage(thumbs[0]);
                }
                return;
            }
            thumbImage.classList.add("img-primary");
            if (removedAdded) {
                setImage(thumbImage);
            }
        }
    }
}

function checkExistingPrimaries() { //check if there are any primary images currently present
    var exist = false;
    $("img.admin-img-thumb").each((i, e) => {
        if (e.classList.contains("img-primary")) {
            exist = true;
        }
    });
    return exist;
}

function isImage(i) {
    return i instanceof HTMLImageElement;
}

function setImage(e) {
    var img;
    if (isImage(e)) {
        img = e;
    } else {
        img = e.target;
    }
    var newSrc = img.src;
    var target = $("img#admin-image-view")[0];
    //set src
    if (img.src.substring(0, 10) !== "data:image") {
        newSrc = img.dataset.path;
    }
    target.src = newSrc;
    target.dataset.id = img.dataset.id;
    //set img-primary class
    if (img.classList.contains("img-primary")) {
        target.classList.add("img-primary");
    } else {
        target.classList.remove("img-primary");
    }
    //set img-delete class
    if (img.classList.contains("img-delete")) {
        target.classList.add("img-delete");
    } else {
        target.classList.remove("img-delete");
    }
}

// admin product image upload button
function loadImagesFromInput() {
    //Get count of selected files
    var countFiles = $(this)[0].files.length;
    var imgPath = $(this)[0].value;
    var extn = imgPath.substring(imgPath.lastIndexOf(".") + 1).toLowerCase();
    var imageContainer = $("#image-container");
    //image_holder.empty();
    if (extn === "gif" || extn === "png" || extn === "jpg" || extn === "jpeg") {
        if (typeof (FileReader) !== undefined) {
            //loop for each file selected for uploaded.
            for (var i = 0; i < countFiles; i++) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var imgClass = "admin-img-thumb img-add border";
                    var flag = !checkExistingPrimaries();
                    if (flag) { //if no primary image present set this image as primary
                        imgClass = imgClass + " img-primary";
                    }
                    $("<img />",
                        {
                            "class": imgClass,
                            "click": setImage,
                            "data-id": Math.floor((Math.random() * 10000000) + 10000000),//getImgId() //need only int id
                            "src": e.target.result
                        }).appendTo(imageContainer);
                    if (flag) {
                        var img = $("img.admin-img-thumb.img-add")[0];
                        setImage(img); // set big image
                    }
                };
                imageContainer.show();
                reader.readAsDataURL($(this)[0].files[i]);
            }
        } else {
            alert("Браузер не поддерживает загрузку картинок.");
        }
    } else {
        alert("Выберите изображения.");
    }
}

// **** user edit scripts

$("#role-selector").change(function () {
    if ($(this).val() === "admin") {
        $(".admin-options").removeClass("d-none");
    } else {
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
    var classes = this.className.split(" ");
    var action = classes[0];
    var group = classes[1];

    if (action === "delete" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "create" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    } else if (action === "delete" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "create" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    } else if (action === "create" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    } else if (action === "create" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
        $("." + "edit" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    } else if (action === "edit" && this.checked === true) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", true).prop("disabled", true);
    } else if (action === "edit" && this.checked === false) {
        $("." + "view" + "." + group + ":checkbox").prop("checked", false).prop("disabled", false);
    }
});