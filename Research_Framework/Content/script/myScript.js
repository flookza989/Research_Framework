$(function () {
    function setupAutocomplete(selector, permission) {
        $(selector).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: "AddReserch.aspx/GetEmployeeName",
                    data: JSON.stringify({ empName: request.term, permission: permission }),
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return {
                                label: item,
                                value: item
                            };
                        }));
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        console.log(xhr.responseText);
                    }
                });
            },
            minLength: 1
        });
    }

    // Setting up autocomplete for teacher
    setupAutocomplete("#ContentPlaceHolder1_Tb_teacher", "teacher");

    // Setting up autocomplete for student
    setupAutocomplete("#ContentPlaceHolder1_Tb_student", "student");
});


function activateButton(button) {
    // Remove 'active' class from all buttons
    $(".sidebar-button").removeClass("active");

    // Add 'active' class to the clicked button
    $(button).addClass("active");
}