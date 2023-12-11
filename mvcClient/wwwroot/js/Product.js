
        $(".summernote").summernote({
            height: 300,
            minHeight: null,
            maxHeight: null,
            focus: true
        });

        var myDropzone = new Dropzone("#myDropzone", {
            url: "/Upload/UploadFiles",
            paramName: "file",
            maxFiles: 10,
            maxFilesize: 100, // mb
            addRemoveLinks: true,
            success: function (file, response) {
                file.upload.newFileName = response.newFileName;
                console.log(response);
            },
            error: function (file, response) {
                alert(response);
                myDropzone.removeFile(file);
                console.log(response);
            },
            removedfile: function (file) {
                $.ajax({
                    url: "/Upload/DeleteFile/" + encodeURIComponent(file.upload.newFileName),
                    type: "DELETE",
                    success: function (response) {
                        console.log(response);
                    },
                    error: function (response) {
                        console.log(response);
                    }
                });
                var _ref;
                return (_ref = file.previewElement) != null ? _ref.parentNode.removeChild(file.previewElement) : void 0;
            }
        });

        $("#myForm").submit(function (e) {
            e.preventDefault();

            for (var i = 0; i < myDropzone.files.length; i++) {
                var file = myDropzone.files[i];
                $("#myDropzone").append("<input type='hidden' name='files' value='" + file.upload.newFileName + "' />");
            }

            $.ajax({
                url: "/Product/Create",
                type: "POST",
                data: $("#myForm").serialize(),
                success: function (response) {
                    myDropzone.removeAllFiles();
                    location.href = response;
                    console.log(response);
                },
                error: function (response) {
                    console.log(response);
                }
            });
        });


