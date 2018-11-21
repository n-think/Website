import $ from "jquery";
import "bootstrap";
import "bootstrap-select";
import "bootstrap-select/dist/js/i18n/defaults-ru_RU"

// read client height and submit max items per page
//first load from anchors
$("a.read-height").on("click", function () {
    let itemsPerPage = getItemCountFromHeight();
    let anchor = $(this);
    anchor.attr("href", anchor.attr("href") + "?&c=" + itemsPerPage);
});

//submit from search
$("form#admin-search-form").on("submit", function () {
    let itemsPerPage = getItemCountFromHeight();
    $(this).append("<input type=\"hidden\" name=\"c\" value=\"" + itemsPerPage + "\"/>");
});

//submit form from selector
$("select.admin-selector").on("change", () => {
    $("form#admin-search-form").trigger("submit");
});

function getItemCountFromHeight() {
    if (window.innerWidth < 768)
        return 5;
    const clientHeight = window.innerHeight;
    const value = Math.round((clientHeight - 400) / 65);
    return value < 5 ? 5 : value;
}