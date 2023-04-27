$(document).ready(function () {
    $('#all-ajax').click(function () {
        // Send Ajax request
        $.ajax({
            url: '/my-url', // URL to send the request to
            success: function (result) {
                // Handle successful response from the server
                console.log(result);
            },
            error: function (xhr, status, error) {
                // Handle error response from the server
                console.log(error);
            }
        });

        // Prevent form submission or link click
        return false;
    });
});