// sticky nav bar
window.onscroll = function () { stickyNavbar(); };
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