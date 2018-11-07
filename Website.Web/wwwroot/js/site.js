window.onscroll = function () { stickyNavbar(); };
var navbar = document.getElementById("navbar");
var sticky = navbar.offsetTop;
function stickyNavbar() {
    if (window.pageYOffset >= sticky) {
        navbar.classList.add("fixed-top");
    }
    else {
        navbar.classList.remove("fixed-top");
    }
}
//# sourceMappingURL=site.js.map