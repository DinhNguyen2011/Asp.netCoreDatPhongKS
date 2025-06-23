$(document).ready(function () {
    $("#hinhAnh").change(function () {
        var input = this;
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $("#imagePreview").attr("src", e.target.result).show();
            };
            reader.readAsDataURL(input.files[0]);
        } else {
            $("#imagePreview").hide();
        }
    });
});