// read client height and submit max items per page
$("a.read-height").click(function () {
    var itemsPerPage = getItemCountFromHeight();
    var anchor = $(this);
    anchor.attr("href", anchor.attr("href") + "?&c=" + itemsPerPage);
}); //first load from anchors
$("form#admin-search-form").submit(function () {
    var itemsPerPage = getItemCountFromHeight();
    $(this).append("<input type=\"hidden\" name=\"c\" value=\"" + itemsPerPage + "\"/>");
}); //submit from search
$("select.admin-selector").change(function () {
    var form = $("form#admin-search-form");
    form.submit();
}); //submit form from selector
function getItemCountFromHeight() {
    if (window.innerWidth < 768)
        return 5;
    var clientHeight = window.innerHeight;
    var value = Math.round((clientHeight - 400) / 65);
    return value < 5 ? 5 : value;
}
//# sourceMappingURL=admin.js.map