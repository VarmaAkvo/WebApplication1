(function () {
    bindReplyToBtn()
    bindReplyNavBtn()

    function bindReplyToBtn() {
        let rbtns = document.getElementsByName("replyToButton")
        for (let i = 0; i < rbtns.length; i++) {
            let btn = rbtns[i]
            btn.onclick = () => {
                let commentId = btn.getAttribute("data-comment-id")
                let replyName = btn.getAttribute("data-reply-name")
                let editor = document.getElementById(`reply${commentId}`)
                let trixEditor = document.querySelector(`trix-editor[input="x${commentId}"]`)
                editor.classList.remove("d-none")
                trixEditor.value = ""
                trixEditor.focus()
                trixEditor.editor.setSelectedRange([0, 0])
                trixEditor.editor.insertString(`Reply to @${replyName} :`)
                editor.scrollIntoView()
            }
        }
    }
    function bindReplyNavBtn() {
        let rnbtns = document.getElementsByName("replyNavBtn")
        for (let i = 0; i < rnbtns.length; i++) {
            let btn = rnbtns[i]
            btn.onclick = () => {
                let commentId = btn.getAttribute("data-comment-id")
                let pageIndex = btn.getAttribute("data-page-index")
                let replyArea = document.getElementById(`repliesOf${commentId}`)
                let url = `/Posts/Details?handler=Replies&commentId=${commentId}&pageIndex=${pageIndex}`
                let xhr = new XMLHttpRequest()
                xhr.open("Get", url)
                xhr.addEventListener("load", (event) => {
                    replyArea.innerHTML = event.target.response
                    bindReplyToBtn()
                    bindReplyNavBtn()
                })
                xhr.send()
            }
        }
    }
    
})();