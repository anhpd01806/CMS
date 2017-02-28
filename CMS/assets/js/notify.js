$(document).ready(function () {
    var userId = $('#user-id').val();
    var isUser = $('#isUser').val();
    var isAdmin = isUser == 0 ? true : false;
    //connect to notify server
    var socket = io.connect('http://ozo.vn:8088');
    socket.emit('join-room', { userId, isAdmin });
    //show notify
    socket.on('show-notify', function (obj) {
        //span avata
        var spanAvt = "<span class='blue'>" + obj.name + ":</span>";
        //span message title
        var spanMsgTit = "<span class='msg-title'>" + spanAvt + obj.title + "</span>";
        //span time
        var spanTime = "<span>" + obj.time + "</span>";
        //span message time
        var spanMsgTime = "<span class='msg-time'> <i class='ace-icon fa fa-clock-o'></i>" + spanTime + "</span>";
        //span message body
        var spanMsgBody = "<span class='msg-body'>" + spanMsgTit + spanMsgTime + "</span>";
        //tag image
        var tagImg = "<img src='" + obj.avatar + "' class='msg-photo' alt='avatar' />";
        //tag a
        var tagA = "<div data-id='" + obj.id + "' data-type='" + obj.type + "'>" + tagImg + spanMsgBody + "</div>";
        //tag li
        var tagLi = "<li style='padding-top:10px;padding-bottom: 15px;cursor: pointer;border-bottom: 1px solid #E4ECF3' onclick='return DetailNotify(" + obj.id + "," + obj.type + ");' id='li-noti-" + obj.id + "'>" + tagA + "</li>";
        $('#ul-notify').prepend(tagLi);
        //update count notify
        $('#count-notify').attr('data-badge', parseInt(typeof ($('#count-notify').data('badge')) === "undefined" ? 0 : $('#count-notify').data('badge')) + 1);
        $.ajax({
            type: "GET",
            url: "/Notice/UpdateNotifySesion",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {

            },
            error: function (response) {
                alert('Có lỗi xẩy ra khi lấy dữ liệu trong database !');
            }
        });
    });

    $('#btn-accept-acount').click(function () {
        //close modal notify account
        $('#md-notify-account').modal('hide');
        var notifyId = $('#btn-accept-acount').data('id');
        //get notify information
        $.ajax({
            type: "GET",
            url: "/Notice/AcceptAccount",
            data: { Id: notifyId, UserId: $('#btn-accept-acount').data('userid') },
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result) {
                    alert('Duyệt thành công .');
                } else {
                    alert('Duyệt thất bại .');
                }

            },
            error: function (response) {
                alert('Có lỗi xẩy ra khi lấy dữ liệu trong database !');
            }
        });
    });
});

//detail notify account function
function DetailNotify(id, type) {
    //remove notify
    $('#li-noti-' + id).remove();
    //update count notify
    if ($('#count-notify').data('badge') === 1) {
        $('#count-notify').removeAttr('data-badge');
    } else {
        $('#count-notify').attr('data-badge', parseInt($('#count-notify').data('badge')) - 1);
    }
    //update status notify set viewflag = true
    $.ajax({
        type: "GET",
        url: "/Notice/UpdateNotify",
        data: { Id: id },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

        },
        error: function (response) {

        }
    });
    if (type == 2) {
        window.location = '/ReportNews/Index';
        /*$.ajax({
            type: "GET",
            url: "/Notice/GetNotify",
            data: { Id: id },
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result) {
                    //show modal notify payment account
                    $('#tit-notify-news').text(result.UserName +' báo tin ' + result.Desription + ' là tin mô giới .');
                    $('#md-notify-report-news').modal('show');
                }

            },
            error: function (response) {
                alert('Có lỗi xẩy ra khi lấy dữ liệu trong database !');
            }
        });*/
    }
    if (type == 3) {

        $.ajax({
            type: "GET",
            url: "/Notice/GetNotify",
            data: { Id: id },
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result) {
                    //show modal notify payment account
                    $('#tit-notify').text(result.Title);
                    $('#md-notify-payment').modal('show');
                }

            },
            error: function (response) {
                alert('Có lỗi xẩy ra khi lấy dữ liệu trong database !');
            }
        });

    }
    if (type == 1) {
        //get notify information
        $.ajax({
            type: "GET",
            url: "/Notice/GetNotify",
            data: { Id: id },
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result) {

                    //set date for modal
                    //$('#nt-user-name').text(result.Account);
                    //$('#nt-full-name').text(result.FullName);
                    //$('#nt-gender').text(result.Gender ? 'Nam' : 'Nữ');
                    //$('#nt-phone').text(result.Phone);
                    //$('#nt-email').text(result.Email);
                    //$('#btn-accept-acount').attr('data-id', result.Id);
                    //$('#btn-accept-acount').attr('data-userid', result.Userid);
                    window.location = '/Customer/Edit/' + result.Userid;
                    //show modal notify create account
                    //$('#md-notify-account').modal('show');
                }

            },
            error: function (response) {
                alert('Có lỗi xẩy ra khi lấy dữ liệu trong database !');
            }
        });
    }
};

