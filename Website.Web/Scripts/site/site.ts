import Popper from "popper.js"
import $ from "jquery"

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

module instantSearch{    
    //let popper = new Popper(document.querySelector(""),document.querySelector(""))
}