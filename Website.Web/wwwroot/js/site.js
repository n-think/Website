window.onscroll = function () { stickyNavbar() };

var navbar = document.getElementById("navbar");
var sticky = navbar.offsetTop;

function stickyNavbar() {
    if (window.pageYOffset >= sticky) {
        navbar.classList.add("navbar-fixed-top");
    }
    else {
        navbar.classList.remove("navbar-fixed-top");
    }
}
