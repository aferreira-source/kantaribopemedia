// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.





 

$(document).ready(function () {


    $("#divulgacao").click(function () {
        
        var requestUrl = '/Mensagem/CadastroModal';
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