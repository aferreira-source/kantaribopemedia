// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.





 

$(document).ready(function () {



 
    //$('#myTab li:last-child a').tab('show');
    //$('#minhaAba a').on('click', function (e) {      
    //    e.preventDefault()
    //    $(this).tab('show')
    //})


    $("#divulgacao").click(function () {
        
        var requestUrl = '/Postagens/Index';
        $.get(requestUrl)
           
            .done(function (responsedata) {
                console.log(requestUrl);

                $("#partialViewContent").html(responsedata);

                $('#myModal').modal('show')

            })
            .fail(function () {
                alert("error");
            })
            .always(function () {
                console.log("finished");
            });
    });
});