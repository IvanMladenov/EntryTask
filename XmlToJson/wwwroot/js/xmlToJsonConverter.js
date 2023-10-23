$(document).ready(function () {
    const apiUrl = '/Convert/Convert';
    const fileInput = $('#file-input');
    const nameInput = $("#filename-input");
    const uploadButton = $('#upload-button');
    const loader = $('#loader');

    let isFileInputValid = false;
    let isNameInputValid = false;

    uploadButton.prop("disabled", true);

    nameInput.on('input', function () {
        const filename = nameInput.val();
        const isValid = /^[a-zA-Z0-9-]+$/.test(filename) && filename.length > 1;

        if (isValid) {
            nameInput.removeClass("is-invalid").addClass("is-valid");
            isNameInputValid = true;
        } else {
            nameInput.removeClass("is-valid").addClass("is-invalid");
            isNameInputValid = false;
        }

        toggleUploadButtonState();
    });

    fileInput.on('input', function () {
        if (fileInput.get(0).files.length === 0) {
            fileInput.removeClass("is-valid").addClass("is-invalid");
            isFileInputValid = false;
        } else {
            fileInput.removeClass("is-invalid").addClass("is-valid");
            isFileInputValid = true;
        }

        toggleUploadButtonState();
    });

    function toggleUploadButtonState() {
        if (isFileInputValid && isNameInputValid) {
            uploadButton.prop("disabled", false);
        } else {
            uploadButton.prop("disabled", true);
        }
    }

    function showError(message) {
        $("#error-container").text(message);
        $("#error-container").show();
    };

    function showSuccess(message) {
        $("#success-container").text(message);
        $("#success-container").show();
    };

    $("#upload-form").submit(function (event) {
        event.preventDefault();

        const fileInput = $("#file-input")[0].files[0];
        const filename = $("#filename-input").val();

        if (!fileInput) {
            alert("Please select an XML file.");
            return;
        }

        loader.show();
        uploadButton.prop("disabled", true);

        const formData = new FormData();
        formData.append("xmlFile", fileInput);
        formData.append("filename", filename);

        $.ajax({
            url: apiUrl,
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                showSuccess(data.message);

                loader.hide();
                uploadButton.prop("disabled", true);

                $("#success-container").fadeOut(3000, function () {
                    $('#file-input').val('').addClass("is-invalid");
                    nameInput.val('').addClass("is-invalid");
                    isFileInputValid = isNameInputValid = false;
                });
            },
            error: function (xhr, status, error) {
                loader.hide();
                uploadButton.prop("disabled", false);

                showError(xhr.responseJSON.error);
            }
        });
    });
});
