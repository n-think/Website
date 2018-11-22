import "jquery"
import "jquery-validation"
import "jquery-validation-unobtrusive"
import "jquery-validation/dist/localization/messages_ru"
import "popper.js"

module stickyNavBar {
    window.onscroll = function () {
        stickyNavbar();
    };
    const navbar = document.getElementById("navbar");
    const sticky = navbar.offsetTop;

    function stickyNavbar() {
        if (/*window.innerWidth >= 768 && */window.pageYOffset >= sticky) {
            navbar.classList.add("fixed-top");
        }
        else {
            navbar.classList.remove("fixed-top");
        }
    }
}