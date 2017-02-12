/*
* Creat 11/02/2017
*/
function showmessage(status, message) {
    $.gritter.add({
        title: "Thông báo",
        text: message,
        class_name: 'gritter-' + status + ' gritter-light'
    });
}
