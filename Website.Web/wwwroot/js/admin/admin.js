$("a.read-height").click(function () {
    var itemsPerPage = getItemCountFromHeight();
    var anchor = $(this);
    anchor.attr("href", anchor.attr("href") + "?&c=" + itemsPerPage);
});
$("form#admin-search-form").submit(function () {
    var itemsPerPage = getItemCountFromHeight();
    $(this).append("<input type=\"hidden\" name=\"c\" value=\"" + itemsPerPage + "\"/>");
});
$("select.admin-selector").change(function () {
    var form = $("form#admin-search-form");
    form.submit();
});
function getItemCountFromHeight() {
    if (window.innerWidth < 768)
        return 5;
    var clientHeight = window.innerHeight;
    var value = Math.round((clientHeight - 400) / 65);
    return value < 5 ? 5 : value;
}
var enums;
(function (enums) {
    var DtoState;
    (function (DtoState) {
        DtoState["Unchanged"] = "unchanged";
        DtoState["Added"] = "added";
        DtoState["Deleted"] = "deleted";
        DtoState["Modified"] = "modified";
    })(DtoState = enums.DtoState || (enums.DtoState = {}));
})(enums || (enums = {}));
