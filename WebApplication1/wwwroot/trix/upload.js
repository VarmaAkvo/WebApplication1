(function () {
    var HOST = "/TrixUpload"
    //禁止上传大小大于2M的图片
    addEventListener("trix-file-accept", function (event) {
        const fileSizeLimit = 2097152
        var fileSize = event.file.size
        if (fileSize > fileSizeLimit) {
            event.preventDefault()
            alert(`The file's size is bigger than ${fileSizeLimit}B.`)
        }
    })
    //上传图片
    addEventListener("trix-attachment-add", function (event) {
        if (event.attachment.file) {
            uploadFileAttachment(event.attachment)
        }
    })

    function uploadFileAttachment(attachment) {
        uploadFile(attachment.file, setProgress, setAttributes)

        function setProgress(progress) {
            attachment.setUploadProgress(progress)
        }

        function setAttributes(attributes) {
            attachment.setAttributes(attributes)
        }
    }

    function uploadFile(file, progressCallback, successCallback) {
        var formData = createFormData(file)
        var xhr = new XMLHttpRequest()

        xhr.open("POST", HOST, true)

        xhr.upload.addEventListener("progress", function (event) {
            var progress = event.loaded / event.total * 100
            progressCallback(progress)
        })

        xhr.addEventListener("load", function (event) {
            var result = JSON.parse(this.response)
            if (result.successed) {
                var attributes = {
                    url: result.url
                }
                document.getElementById("attachmentPaths").value += result.path + ';'
                successCallback(attributes)
            }
            else {
                alert(result.errorMessage)
            }
        })
        var token = document.getElementsByName("__RequestVerificationToken")[0].value
        xhr.setRequestHeader("RequestVerificationToken", token)
        xhr.send(formData)
    }

    function createFormData(file) {
        var data = new FormData()
        data.append("Content-Type", file.type)
        data.append("file", file)
        return data
    }
    //移除图片时删除远端存放的图片
    addEventListener("trix-attachment-remove", function (event)
    {
        var url = event.attachment.attachment.previewURL
        var data = new FormData()
        data.append("url", url)

        var xhr = new XMLHttpRequest()
        xhr.open("Post", HOST + "?handler=RemoveAttachment", true)
        xhr.addEventListener("load", function (event) {
            console.log(`${url} has been remove.`)
        })
        var token = document.getElementsByName("__RequestVerificationToken")[0].value
        xhr.setRequestHeader("RequestVerificationToken", token)
        xhr.send(data)
    })
})();