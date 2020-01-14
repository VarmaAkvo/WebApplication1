(function (){
    var images = document.querySelectorAll("figure>img")
    images.forEach(function (item) {
        item.classList.add("img-fluid")
        console.log(item)
    })
})();
