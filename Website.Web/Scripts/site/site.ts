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
        } else {
            navbar.classList.remove("fixed-top");
        }
    }
}

module instantSearch {
    let searchResults = $("#instant-search-results");
    let searchInput = $("#nav-search");
    let searchForm = $("#nav-search-form");
    let p: Popper;
    let resultsXHR: JQuery.jqXHR = null;
    let resultId = 0;
    let keyupTimeout;

    searchInput.on("keyup", searchTimeout);
    $(document).on('click touchend', hideInstantSearch);
    searchForm.on("focusin", showInstantSearch);

    class liveSearchItem {
        id: number;
        name: string;
        description: string;
        price: number;
        imageThumb: string;
    }

    function searchTimeout() {
        clearTimeout(keyupTimeout);
        keyupTimeout = setTimeout(search,300);        
    }
    
    function search() {
        let searchString = searchInput.val().toString();
        if (searchString.length < 2)
            return;

        if (p === undefined) {
            // @ts-ignore        
            p = new Popper(searchInput, searchResults);
        }
        searchResults.show();

        let id = ++resultId;
        resultsXHR = $.ajax({
                type: "get",
                url: `/api/public/instantSearch/${searchString}`,
                beforeSend: function () {
                    if (resultsXHR != null)
                        resultsXHR.abort();
                },
                success: function (response) {                   
                    if (id != resultId)
                        return;
                    resultsXHR = null;

                    let items : liveSearchItem[] = response;

                    searchResults.empty();
                    if (items === undefined || items.length == 0) {
                        searchResults.append(`<span>Ничего не найдено.</span>`)
                    }
                    else {
                        items.forEach((item)=>{
                        searchResults.append(
                            `
<a href="/ViewItem/1" class="list-group-item list-group-item-action d-flex flex-row">
 <div class="d-flex flex-grow-1 flex-column" style="max-width:80%">
   <div class="d-flex flex-grow-1 flex-column" >
    <div class="h5 mb-1">${item.name}</div>  
    <p class="mb-1 text-truncate">${item.description}</p>
   </div>
   <div class="h6">${item.price} ¤</div> 
  </div>
 <img class="rounded ml-auto instant-search-image" src="${item.imageThumb}">
</a>
`
                        ); 
                    });
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //console.log("live search error")
                }
            }
        );
    }

    function hideInstantSearch(e) {
        if (p === undefined) {
            return;
        }

        let target = $(e.target);
        if (searchResults.is(':visible') &&
            (target.is(searchInput) ||
                //@ts-ignore
                searchForm.has(target).length > 0)) {
            return;
        } else {            
            searchResults.hide();
        }
    }

    function showInstantSearch(e) {        
        if (p !== undefined)
            searchResults.show();
    }

}