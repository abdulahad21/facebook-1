// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// for post textarea button dissable by default

$(document).ready(function () {
    $('#postTextArea').on('input change', function () {
        if ($(this).val() != '') {
            $('#postTextButton').prop('disabled', false);
        } else {
            $('#postTextButton').prop('disabled', true);
        }
    });

    $('#editPostTextArea').on('input change', function () {
        if ($(this).val() != '') {
            $('#editPostTextButton').prop('disabled', false);
        } else {
            $('#editPostTextButton').prop('disabled', true);
        }
    });

});

//back to top
//Get the button
let backToTopButton = document.getElementById("btn-back-to-top");

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () {
    scrollFunction();
};

function scrollFunction() {
    if (
        document.body.scrollTop > 500 ||
        document.documentElement.scrollTop > 500
    ) {
        backToTopButton.style.display = "block";
    } else {
        backToTopButton.style.display = "none";
    }
}
// When the user clicks on the button, scroll to the top of the document
backToTopButton.addEventListener("click", backToTop);

function backToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}

//comments hide/visibe
function toggleComment(id) {
    var x = document.getElementById(id);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}