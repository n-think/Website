import $ from "jquery";


module itemView {
    if (window.location.pathname.lastIndexOf("/ViewItem/") == 0) {
        $("img.img-thumb").on("click", setImage);
    }

    function setImage() {
        let img = $(this);
        let target = $("img#image-view").first();

        let newSrc = img.data("path");
        target.attr("src", newSrc);
        target.data("id", img.data("id"));
    }
}