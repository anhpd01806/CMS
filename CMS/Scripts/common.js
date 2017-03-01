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
$(document).ready(function () {
    $('#listnewstable').DataTable({
        sDom: 'rt',
        retrieve: true,
        bFilter: false,
        bInfo: false,
        searching: false,
        paging: false,
        aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }]
    });
    $('#check-all').parent().removeClass("sorting_asc");
});