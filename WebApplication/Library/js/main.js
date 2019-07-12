//Function Load Dictionary
$(document).ready(function () {
    loadDictionary();
});
function loadDictionary() {
    $.ajax({
        type: "GET",
        url: "https://cntttest.vanlanguni.edu.vn:18081/SpeakOut/Vocabularies/getVocabularies",
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response != "Không tìm thấy từ điển") {
                var allList = '';
                $.each(response, function (i, item) {
                    item.VnWord = item.VnWord.replace(/<br\s*\/?>/gi, ' ');
                    let wsRegex = /^\s*\s*$/; // Change this line
                    let result = item.VnWord.replace(wsRegex, ''); // Change this line
                    var res = result.split(";");
                    console.log(result + ' ' + item.EngWord + ' ' + item.Spelling);
                    allList += item.EngWord + ' ' + item.Spelling + '<br>' + ' ' + res[0] + '<br>';
                });
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