//Function Load Dictionary
$(document).ready(function () {
    loadDictionary();
});
function loadDictionary() {
    $.ajax({
        type: "GET",
        url: homeURL,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response != "Không tìm thấy từ điển của bạn") {
                var allList = '<ul class="tags">';
                $.each(response, function (i, item) {
                    item.VnWord = item.VnWord.replace(',', '</br>');
                    allList += '<li><a href="#" class="tag"><details><summary>' + item.EngWord
                        + '</summary><div class="meanTag"><p><em>' + item.Spelling +'</em></br>'+item.VnWord+'</p></div></details></a></li>';
                });
                allList += "<ul>";
                $('#wordLibrary').html(allList);
            }
            
        },
        error: function (xhr, status, error) {
            alert('Result: ' + status + ' ' + error + ' ' + xhr.status + ' ' + xhr.statusText)
        }
    });
}
function myFunction() {
    var x = document.getElementById("myTopnav");
    if (x.className === "navBar") {
        x.className += " responsive";
    } else {
        x.className = "navBar";
    }
}
