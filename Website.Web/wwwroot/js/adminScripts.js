
$(".admin-img-thumb").click(setImage);

function setImage () {
    var newSrc = this.src;
    if (this.src.substring(0, 10) !== "data:image") {
        newSrc = this.dataset.path;}
    $("#admin-image-view")[0].src = newSrc;
}

$(document).ready(function () {
    $("#role-selector").change(function() {
        if ($(this).val() === "admin") {
            $(".admin-options").removeClass("hidden");
        } else {
            $(".admin-options").addClass("hidden");
        }
    });
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



    console.log(dataToJson);
    var json = JSON.stringify(dataToJson);
    //console.log(json);
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
            //$("html").html(response);
            document.open();
            document.write(response);
            document.close();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('Error on ajax:', jqXHR, textStatus, errorThrown);
        },
        async: true
    });
}

$(document).ready(function () {
    $("#file-upload-button").on('change', function () {
        //Get count of selected files
        var countFiles = $(this)[0].files.length;
        var imgPath = $(this)[0].value;
        var extn = imgPath.substring(imgPath.lastIndexOf('.') + 1).toLowerCase();
        var image_container = $("#image-container");
        //image_holder.empty();
        if (extn === "gif" || extn === "png" || extn === "jpg" || extn === "jpeg") {
            if (typeof (FileReader) !== "undefined") {
                //loop for each file selected for uploaded.
                for (var i = 0; i < countFiles; i++) {
                    var reader = new FileReader();
                    reader.onload = function(e) {
                        $("<img />",
                            {
                                "src": e.target.result,
                                "class": "admin-img-thumb img-added",
                                "click": setImage
                            }).appendTo(image_container);
                    };
                    image_container.show();
                    reader.readAsDataURL($(this)[0].files[i]);
                }
            } else {
                alert("Браузер не поддерживает загрузку картинок.");
            }
        } else {
            alert("Выберите изображения.");
        }
    });
});