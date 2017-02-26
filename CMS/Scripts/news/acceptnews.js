$(document).ready(function () {

    var totalpage = parseInt($('#datatable').attr("data-total"));
    if (totalpage > 1) {
        var obj = $('#pagination').twbsPagination({
            totalPages: totalpage,
            visiblePages: 5,
            first: 'Trang đầu',
            prev: 'Trước',
            next: 'Tiếp',
            last: 'Trang cuối',
            onPageClick: function (event, page) {
                $.LoadingOverlay("show");
                var key = $.trim($(".txtsearchkey").val());
                var pageIndex = page;
                var pageSize = parseInt($(".ddlpage").val());

                var data = {
                    key : key, pageIndex : pageIndex, pageSize : pageSize
                };
                $.post("/news/loaddata", data, function (resp) {
                    if (resp != null) {
                        $("#listnewstable tbody").html("");
                        $("#listnewstable tbody").html(resp.Content);
                        if (resp.TotalPage > 1) {
                            $(".page-home").show();
                        } else {
                            $(".page-home").hide();
                        }
                        $(".fistrecord").html(((page - 1) * pageSize) + 1);
                        $(".endrecord").html((page * pageSize) <= resp.TotalRecord ? (page * pageSize) : resp.TotalRecord);
                        $(".totalrecord").html(resp.TotalRecord);
                        $('#datatable').attr("data-total", resp.TotalPage);
                        $('#datatable').attr("data-page", page);
                        $('#check-all').prop('checked', false);
                        $.LoadingOverlay("hide");
                    }
                });
            }
        });
    }

    $(document).on("click", ".btnsearch", function () {
        LoadData();
    });

    $('.txtsearchkey').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            LoadData();
        }
        event.stopPropagation();
    });

    $(document).on("click", ".btnsave", function () {
        if (!$(this).hasClass("disabled")) {
            var selected = [];
            $('.checkboxItem:checked').each(function () {
                selected.push($.trim($(this).attr('id')) + "-" + $.trim($(this).attr('data-userId')));
            });
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần duyệt!");
            } else {
                $.post("/news/activeordeleteNews", { newsId: selected, isDelete: false }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được lưu thành công!");
                            }, 1200);
                        }
                        if (resp.Status == 3) {
                            showmessage("error", "Hiện tài khoản ngày không đủ tiền để chi trả chi phí đăng bài viết!");
                        }
                        if (resp.Status == 4) {
                            showmessage("error", "Bạn chưa chọn tin để duyệt!");
                        }
                        if (resp.Status == 0) {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                        if (resp.Status == 2) {
                            showmessage("error", "Không tìm thấy dữ liệu cần duyệt!");
                        }
                    }
                    ;
                });
            }
        }
    });

    $(document).on("click", ".btnhide", function () {
        if (!$(this).hasClass("disabled")) {
            var selected = [];
            $('.checkboxItem:checked').each(function () {
                selected.push($.trim($(this).attr('id')) + "-" + $.trim($(this).attr('data-userId')));
            });
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần xóa!");
            } else {
                $.post("/news/activeordeleteNews", { newsId: selected, isDelete: true }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được xóa thành công!");
                            }, 1200);
                        }
                        if (resp.Status == 3) {
                            showmessage("error", "Hiện tài khoản ngày không đủ tiền để chi trả chi phí đăng bài viết!");
                        }
                        if (resp.Status == 4) {
                            showmessage("error", "Bạn chưa chọn tin để duyệt!");
                        }
                        if (resp.Status == 0) {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                        if (resp.Status == 2) {
                            showmessage("error", "Không tìm thấy dữ liệu cần duyệt!");
                        }
                    }
                    ;
                });
            }
        }
    });

    //$("#check-all").checkAll();

    $(document).on("click", ".checkboxItem", function () {
        $box = $(this);
        var group = ".checkboxItem";
        if ($box.is(":checked")) {
            $(group).prop("checked", false);
            $box.prop("checked", true);
            $(".btnsave, .btnhide, .btnreport, .btnspam").removeClass("disabled");
        } else {
            $(group).prop("checked", false);
            $(".btnsave, .btnhide, .btnreport, .btnspam").addClass("disabled");
        }        
    });

    $(document).on("change", ".ddlpage", function () {
        $.LoadingOverlay("show");
        var key = $.trim($(".txtsearchkey").val());
        var pageIndex = parseInt($('#datatable').attr("data-page"));
        var pageSize = parseInt($(".ddlpage").val());

        var data = {
            key: key, pageIndex: pageIndex, pageSize: pageSize
        };
        $.post("/news/loaddata", data, function (resp) {
            if (resp != null) {
                $("#listnewstable tbody").html("");
                $("#listnewstable tbody").html(resp.Content);
                if (resp.TotalPage > 1) {
                    $(".page-home").show();
                    showPagination(resp.TotalPage);
                } else {
                    $(".page-home").hide();
                }
                $(".fistrecord").html(((pageIndex - 1) * pageSize) + 1);
                $(".endrecord").html((pageIndex * pageSize) <= resp.TotalRecord ? (pageIndex * pageSize) : resp.TotalRecord);
                $(".totalrecord").html(resp.TotalRecord);
                $('#check-all').prop('checked', false);
            }
            $.LoadingOverlay("hide");
        });
    });

    function showPagination(pagesCounter) {
        $('#pagination').remove();
        $('.homepagging').html('<div class="pagination" id="pagination"></div>');
        $('#pagination').twbsPagination({
            totalPages: pagesCounter,
            visiblePages: 5,
            first: 'Trang đầu',
            prev: 'Trước',
            next: 'Tiếp',
            last: 'Trang cuối',
            onPageClick: function (event, page) {
                $.LoadingOverlay("show");
                var key = $.trim($(".txtsearchkey").val());
                var pageIndex = page;
                var pageSize = parseInt($(".ddlpage").val());

                var data = {
                    key: key, pageIndex: pageIndex, pageSize: pageSize
                };
                $.post("/news/loaddata", data, function (resp) {
                    if (resp != null) {
                        $("#listnewstable tbody").html("");
                        $("#listnewstable tbody").html(resp.Content);
                        if (resp.TotalPage > 1) {
                            $(".page-home").show();
                        } else {
                            $(".page-home").hide();
                        }
                        $(".fistrecord").html(((page - 1) * pageSize) + 1);
                        $(".endrecord").html((page * pageSize) <= resp.TotalRecord ? (page * pageSize) : resp.TotalRecord);
                        $(".totalrecord").html(resp.TotalRecord);
                        $('#datatable').attr("data-total", resp.TotalPage);
                        $('#datatable').attr("data-page", page);
                        $('#check-all').prop('checked', false);
                        $.LoadingOverlay("hide");
                    }
                });
            }
        });
    }

    function LoadData() {
        $.LoadingOverlay("show");
        var key = $.trim($(".txtsearchkey").val());
        var pageIndex = parseInt($('#datatable').attr("data-page"));
        var pageSize = parseInt($(".ddlpage").val());

        var data = {
            key: key, pageIndex: pageIndex, pageSize: pageSize
        };
        $.LoadingOverlay("show");
        $.post("/news/loaddata", data, function (resp) {
            if (resp != null) {
                $("#listnewstable tbody").html("");
                $("#listnewstable tbody").html(resp.Content);
                if (resp.TotalPage > 1) {
                    $(".page-home").show();
                    showPagination(resp.TotalPage);
                } else {
                    $(".page-home").hide();
                }
                $(".fistrecord").html(((pageIndex - 1) * pageSize) + 1);
                $(".endrecord").html((pageIndex * pageSize) <= resp.TotalRecord ? (pageIndex * pageSize) : resp.TotalRecord);
                $(".totalrecord").html(resp.TotalRecord);
                $('#check-all').prop('checked', false);
            }
            $.LoadingOverlay("hide");
        });
    }
});