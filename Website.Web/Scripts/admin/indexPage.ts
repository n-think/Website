import $ from "jquery";

module indexPage {
    if (window.location.pathname==="/Admin") {
        loadIndexEventHandlers()
    }

    function loadIndexEventHandlers() {
        //edit buttons
        $("#create-users-form").on("submit", disableButton);
        $("#create-items-form").on("submit", disableButton);
        $("#reset-db-form").on("submit", disableButton);
    }

    function disableButton() {
        $("input[type='submit']", this)
            .val("Подождите...")
            .attr('disabled', 'disabled');

        return true;
    }
}
