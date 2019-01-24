import $ from "jquery";

module admin {
    // read client height and submit max items per page
    
    //load from anchors
    $("a.read-height").on("click", function () {
        let itemsPerPage = getItemCountFromHeight();
        let anchor = $(this);
        anchor.attr("href", anchor.attr("href") + "?&c=" + itemsPerPage);
    });

    //form submit
    $("form#admin-search-form").on("submit", function () {
        let itemsPerPage = getItemCountFromHeight();
        $(this).append("<input type=\"hidden\" name=\"c\" value=\"" + itemsPerPage + "\"/>");
    });


    function getItemCountFromHeight() {
        if (window.innerWidth < 768)
            return 5;
        const clientHeight = window.innerHeight;
        const value = Math.round((clientHeight - 400) / 65);
        return value < 5 ? 5 : value;
    }
}