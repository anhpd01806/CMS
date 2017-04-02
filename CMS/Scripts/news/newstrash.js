/*
* Create 11/02/2017 Nghianh
*/
$(function () {
    $(document).ready(function () {
        var totalpage = parseInt($('#datatable').attr("data-total"));
        var totalrecord = parseInt($('#datatable').attr("data-totalrecord"));

        if (totalrecord > 0) {
            $('#listnewstable').DataTable({
                sDom: 'rt',
                retrieve: true,
                bFilter: false,
                bInfo: false,
                searching: false,
                paging: false,
                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }, null, { "bSortable": false }]
            });
            $('#check-all').parent().removeClass("sorting_asc");
        }
        if (totalpage > 1) {
            var obj = $('#pagination').twbsPagination({
                totalPages: totalpage,
                visiblePages: 5,
                first: 'Trang đầu',
                prev: 'Trước',
                next: 'Tiếp',
                last: 'Trang cuối',
                onPageClick: function (event, page) {
                    var curentpage = parseInt($('#datatable').attr("data-page"));
                    if (curentpage != page) {
                        $.LoadingOverlay("show");
                        var cateId = parseInt($(".cateId").val());
                        var districtId = parseInt($(".districtId").val());
                        var newTypeId = parseInt($(".newTypeId").val());
                        var siteId = parseInt($(".siteId").val());
                        var backdate = parseInt($(".ddlbackdate").val());
                        var minPrice = parseFloat(checkminprice($(".ddlprice").val()));
                        var maxPrice = parseFloat(checkmaxprice($(".ddlprice").val()));
                        var from = $(".txtFrom").val();
                        var to = $(".txtTo").val();
                        var pageIndex = page;
                        var pageSize = 20;
                        if (typeof $(".ddlpage").val() != "undefined") {
                            pageSize = parseInt($(".ddlpage").val());
                        }
                        var isrepeat = $('#chkIsrepeatNews').prop('checked') ? 1 : 0;
                        var key = $.trim($(".txtsearchkey").val());

                        var NameOrder = "";
                        var descending = false;

                        $("#listnewstable th").each(function () {
                            if ($(this).hasClass("order_desc") || $(this).hasClass("order_asc")) {
                                NameOrder = $(this).attr("data-name");
                                if ($(this).hasClass("order_desc")) {
                                    descending = true;
                                }
                            }
                        });

                        var data = {
                            cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                            siteId: siteId, backdate: backdate,
                            minPrice: minPrice, maxPrice: maxPrice,
                            from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key, NameOrder: NameOrder, descending: descending
                        };
                        $.post("/newstrash/loaddata", data, function (resp) {
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
                                if (resp.TotalRecord > 0) {
                                    $('#listnewstable').DataTable({
                                        sDom: 'rt',
                                        retrieve: true,
                                        bFilter: false,
                                        bInfo: false,
                                        searching: false,
                                        paging: false,
                                        aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }, null, { "bSortable": false }]
                                    });
                                    $('#check-all').parent().removeClass("sorting_asc");
                                }
                                $.LoadingOverlay("hide");
                            }
                        });
                    }
                }
            });
        }

        $('.date-picker').datepicker({
            dateFormat: 'dd-mm-yy',
            autoclose: true,
            todayHighlight: true
        });

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

        $(".cateId, .districtId, .siteId, .newTypeId, .ddlbackdate, .ddlprice, .txtFrom, .txtTo, #chkIsrepeatNews").change(function () {
            LoadData();
        });

        $(document).on("change", "#chkIsrepeatNews", function () {
            if ($(this).prop('checked')) {
                $(this).val(1);
            } else {
                $(this).val(0);
            }
        });

        $(document).on("click", ".checkboxItem", function () {
            var count = parseInt($('input:checkbox:checked').length);
            if ($(this).prop('checked')) {
                $(".btnsave, .btnhide, .btnreport, .btnspam, .btndelete").removeClass("disabled");
            } else {
                if (count < 1) {
                    $(".btnsave, .btnhide, .btnreport, .btnspam, .btndelete").addClass("disabled");
                }
            }
        });

        $(document).on("change", ".ddlpage", function () {
            $.LoadingOverlay("show");
            var cateId = parseInt($(".cateId").val());
            var districtId = parseInt($(".districtId").val());
            var newTypeId = parseInt($(".newTypeId").val());
            var siteId = parseInt($(".siteId").val());
            var backdate = parseInt($(".ddlbackdate").val());
            var minPrice = parseFloat(checkminprice($(".ddlprice").val()));
            var maxPrice = parseFloat(checkmaxprice($(".ddlprice").val()));
            var from = $(".txtFrom").val();
            var to = $(".txtTo").val();
            var pageIndex = 1;
            var pageSize = 20;
            if (typeof $(".ddlpage").val() != "undefined") {
                pageSize = parseInt($(".ddlpage").val());
            }
            var isrepeat = $('#chkIsrepeatNews').prop('checked') ? 1 : 0;
            var key = $.trim($(".txtsearchkey").val());

            var NameOrder = "";
            var descending = false;

            $("#listnewstable th").each(function () {
                if ($(this).hasClass("order_desc") || $(this).hasClass("order_asc")) {
                    NameOrder = $(this).attr("data-name");
                    if ($(this).hasClass("order_desc")) {
                        descending = true;
                    }
                }
            });

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key, NameOrder: NameOrder, descending: descending
            };
            $.post("/newstrash/loaddata", data, function (resp) {
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
                    if (resp.TotalRecord > 0) {
                        $('#listnewstable').DataTable({
                            sDom: 'rt',
                            retrieve: true,
                            bFilter: false,
                            bInfo: false,
                            searching: false,
                            paging: false,
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }, null, { "bSortable": false }]
                        });
                        $('#check-all').parent().removeClass("sorting_asc");
                    }
                }
                $.LoadingOverlay("hide");
            });
        });

        $(document).on("click", ".lbltitle", function () {
            $(this).parents("tr").attr("style", "color: #bf983b;");
            $(this).parents("tr").find(".label-info").html("Đã xem");
            $(this).parents("tr").find(".label-info").addClass("label-warning");
            $(this).parents("tr").find(".label-info").addClass("arrowed-right");
            $(this).parents("tr").find(".label-info").addClass("arrowed-in");
            $(this).parents("tr").find(".label-warning").removeClass("label-info");
            $(this).parents("tr").find(".label-warning").removeClass("arrowed");
            $.LoadingOverlay("show");
            $.get("/newstrash/getnewsdetail", { Id: parseInt($(this).attr("data-id")) }, function (resp) {
                if (resp != null) {
                    if (resp.Pay == 1 && resp.Content != "") {
                        if (resp.Pay == 1 && resp.Content != "") {
                            $("#modaldetail").empty();
                            $("#modaldetail").html(resp.Content);
                            $.LoadingOverlay("hide");
                            setTimeout(function () {
                                $("#newsdetail").modal("show");
                            }, 500);
                        } else {
                            window.location.href = '/Payment/RegisterPackage';
                        }
                    } else {
                        window.location.href = '/Payment/RegisterPackage';
                    }
                }
            });
        });

        $("#check-all").checkAll();

        $(document).on("click", ".btnexport", function () {
            var selected = "";
            $('.checkboxItem:checked').each(function () {
                selected += $(this).attr('id') + ",";
            });
            selected = selected.slice(0, -1);
            if (selected.length == 0) {
                showboxcomfirm("Thông báo", "Bạn chưa chọn tin nào! Bạn có muốn xuất hết danh sách tin tức đang được hiển thị không?");
            } else {
                var url = "/newstrash/exportexcelv2";
                location.href = decodeURIComponent(url + "?listNewsId=" + selected);
                $('#check-all').prop('checked', false);
            }
        });

        $(document).on("click", ".btnclose", function () {
            $("#newsdetail").modal("hide");
            $("#newsedit").modal("hide");
        });

        $(document).on("click", ".btnsave", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần khôi phục!");
                } else {
                    $.post("/newstrash/restorenews", { listNewsId: selected }, function (resp) {
                        if (resp != null) {
                            if (resp.Status == 1) {
                                LoadData();
                                setTimeout(function () {
                                    showmessage("success", "Tin đã được khôi phục thành công!");
                                }, 1200);
                            } else {
                                showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
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
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần xóa!");
                } else {
                    bootbox.confirm({
                        title: "Thông báo",
                        message: "Những tin này sẽ không được hiển thị trong hệ thống nữa! Bạn có chắc chắn muốn xóa không?",
                        buttons: {
                            cancel: {
                                label: '<i class="fa fa-times"></i> Đóng'
                            },
                            confirm: {
                                label: '<i class="fa fa-check"></i> Đồng ý'
                            }
                        },
                        callback: function (result) {
                            if (result) {
                                $.post("/newstrash/deletenews", { listNewsId: selected }, function (resp) {
                                    if (resp != null) {
                                        if (resp.Status == 1) {
                                            LoadData();
                                            setTimeout(function () {
                                                showmessage("success", "Tin đã được xóa khỏi hệ thống thành công!");
                                            }, 1200);

                                        } else {
                                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                                        }
                                    }
                                    ;
                                });
                            }
                        }
                    });
                }
            }
        });

        $(document).on("click", ".active-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần khôi phục!");
            } else {
                $.post("/newstrash/restorenews", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được khôi phục thành công!");
                            }, 1200);
                        } else {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                    }
                    ;
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".delete-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần xóa!");
            } else {
                bootbox.confirm({
                    title: "Thông báo",
                    message: "Tin này sẽ không được hiển thị trong hệ thống nữa! Bạn có chắc chắn muốn xóa không?",
                    buttons: {
                        cancel: {
                            label: '<i class="fa fa-times"></i> Đóng'
                        },
                        confirm: {
                            label: '<i class="fa fa-check"></i> Đồng ý'
                        }
                    },
                    callback: function (result) {
                        if (result) {
                            $.post("/newstrash/deletenews", { listNewsId: selected }, function (resp) {
                                if (resp != null) {
                                    if (resp.Status == 1) {
                                        LoadData();
                                        setTimeout(function () {
                                            showmessage("success", "Tin đã được xóa khỏi hệ thống thành công!");
                                        }, 1200);

                                    } else {
                                        showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                                    }
                                }
                                ;
                            });
                        }
                    }
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on('shown.bs.tab', 'a[data-toggle="tab"]', function (e) {
            $('#image-gallery').lightSlider({
                gallery: true,
                item: 1,
                thumbItem: 9,
                slideMargin: 0,
                speed: 500,
                auto: true,
                loop: true,
                onSliderLoad: function () {
                    $('#image-gallery').removeClass('cS-hidden');
                }
            });
            $(".mCustomScrollbar").mCustomScrollbar();
        });

        $(document).on("shown.bs.modal", function () {
            $(".mCustomScrollbar").mCustomScrollbar();
        });

        $(document).on("click", "#listnewstable th", function () {
            if (!$(this).hasClass("order")) {
                if ($(this).hasClass("order_desc")) {
                    $("#listnewstable th").removeClass("order_desc");
                    $(this).addClass("order_asc");
                } else {
                    if ($(this).hasClass("order_asc")) {
                        $("#listnewstable th").removeClass("order_asc");
                        $(this).addClass("order_desc");
                    } else {
                        $(this).addClass("order_asc");
                    }
                }
                LoadData();
            }
        });

        $(document).on("click", ".edit-item-list_adm", function () {
            var id = $(this).attr("data-id");
            $.LoadingOverlay("show");
            $.post("/news/GetNewsDetailForEdit", { newsId: id }, function (resp) {
                if (resp != null) {
                    if (typeof resp.Content == "undefined") {
                        showmessage("error", "Bạn không có quyền hoặc server đang bận. Vui lòng thử lại sau!");
                        $.LoadingOverlay("hide");
                        return;
                    }
                    $("#modaledit").empty();
                    $("#modaledit").html(resp.Content);
                    $(".btneditnews").attr("data-id", id);
                    setTimeout(function () {
                        $("#newsedit").modal("show");
                    }, 500);
                    $.LoadingOverlay("hide");
                    return;
                } else {
                    showmessage("error", "Bạn không có quyền hoặc server đang bận. Vui lòng thử lại sau!");
                    $.LoadingOverlay("hide");
                    return;
                };
                showmessage("error", "Bạn không có quyền hoặc server đang bận. Vui lòng thử lại sau!");
                $.LoadingOverlay("hide");
            });
        });
    });

    function LoadData() {
        var cateId = parseInt($(".cateId").val());
        var districtId = parseInt($(".districtId").val());
        var newTypeId = parseInt($(".newTypeId").val());
        var siteId = parseInt($(".siteId").val());
        var backdate = parseInt($(".ddlbackdate").val());
        var minPrice = parseFloat(checkminprice($(".ddlprice").val()));
        var maxPrice = parseFloat(checkmaxprice($(".ddlprice").val()));
        var from = $(".txtFrom").val();
        var to = $(".txtTo").val();
        var pageIndex = 1;
        var pageSize = 20;
        if (typeof $(".ddlpage").val() != "undefined") {
            pageSize = parseInt($(".ddlpage").val());
        }
        var isrepeat = $('#chkIsrepeatNews').prop('checked') ? 1 : 0;
        var key = $.trim($(".txtsearchkey").val());

        var datefrom = new Date(from.split('-')[1] + "/" + from.split('-')[0] + "/" + from.split('-')[2]);
        var dateto = new Date(to.split('-')[1] + "/" + to.split('-')[0] + "/" + to.split('-')[2]);

        if (datefrom.getTime() > dateto.getTime()) {
            showmessage("error", "Ngày bắt đầu không được lớn hơn ngày kết thúc!");
        } else {

            var NameOrder = "";
            var descending = false;

            $("#listnewstable th").each(function () {
                if ($(this).hasClass("order_desc") || $(this).hasClass("order_asc")) {
                    NameOrder = $(this).attr("data-name");
                    if ($(this).hasClass("order_desc")) {
                        descending = true;
                    }
                }
            });

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key, NameOrder: NameOrder, descending: descending
            };
            $.LoadingOverlay("show");
            $.post("/newstrash/loaddata", data, function (resp) {
                if (resp != null) {
                    $("#listnewstable tbody").html("");
                    $("#listnewstable tbody").html(resp.Content);
                    if (resp.TotalPage > 1) {
                        if (typeof $(".page-home").val() != "undefined") {
                            $(".page-home").show();
                        } else {
                            var html = '<div class="row page-home lo-paging"><div class="col-xs-12 col-md-4"><div class="dataTables_length"><label>Hiển thị <select class="ddlpage"><option value="20">20</option><option value="50">50</option><option value="100">100</option><option value="150">150</option><option value="200">200</option></select> tin</label></div></div><div class="col-xs-12 col-md-8 lo-paging-0"><div class="dataTables_paginate homepagging "><div class="pagination" id="pagination"></div></div></div></div>';
                            $(".pagecus").append(html);
                        }
                        showPagination(resp.TotalPage);
                    } else {
                        $(".page-home").hide();
                    }
                    $(".fistrecord").html(((pageIndex - 1) * pageSize) + 1);
                    $(".endrecord").html((pageIndex * pageSize) <= resp.TotalRecord ? (pageIndex * pageSize) : resp.TotalRecord);
                    $(".totalrecord").html(resp.TotalRecord);
                    $('#check-all').prop('checked', false);
                    if (resp.TotalRecord > 0) {
                        $('#listnewstable').DataTable({
                            sDom: 'rt',
                            retrieve: true,
                            bFilter: false,
                            bInfo: false,
                            searching: false,
                            paging: false,
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }, null, { "bSortable": false }]
                        });
                        $('#check-all').parent().removeClass("sorting_asc");
                    }
                }
                $.LoadingOverlay("hide");
            });
            $("#check-all").checkAll();
        }
    }

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
                var curentpage = parseInt($('#datatable').attr("data-page"));
                if (curentpage != page) {
                    $.LoadingOverlay("show");
                    var cateId = parseInt($(".cateId").val());
                    var districtId = parseInt($(".districtId").val());
                    var newTypeId = parseInt($(".newTypeId").val());
                    var siteId = parseInt($(".siteId").val());
                    var backdate = parseInt($(".ddlbackdate").val());
                    var minPrice = parseFloat(checkminprice($(".ddlprice").val()));
                    var maxPrice = parseFloat(checkmaxprice($(".ddlprice").val()));
                    var from = $(".txtFrom").val();
                    var to = $(".txtTo").val();
                    var pageIndex = page;
                    var pageSize = 20;
                    if (typeof $(".ddlpage").val() != "undefined") {
                        pageSize = parseInt($(".ddlpage").val());
                    }
                    var isrepeat = $('#chkIsrepeatNews').prop('checked') ? 1 : 0;
                    var key = $.trim($(".txtsearchkey").val());

                    var NameOrder = "";
                    var descending = false;

                    $("#listnewstable th").each(function () {
                        if ($(this).hasClass("order_desc") || $(this).hasClass("order_asc")) {
                            NameOrder = $(this).attr("data-name");
                            if ($(this).hasClass("order_desc")) {
                                descending = true;
                            }
                        }
                    });

                    var data = {
                        cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                        siteId: siteId, backdate: backdate,
                        minPrice: minPrice, maxPrice: maxPrice,
                        from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key, NameOrder: NameOrder, descending: descending
                    };
                    $.post("/newstrash/loaddata", data, function (resp) {
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
                            if (resp.TotalRecord > 0) {
                                $('#listnewstable').DataTable({
                                    sDom: 'rt',
                                    retrieve: true,
                                    bFilter: false,
                                    bInfo: false,
                                    searching: false,
                                    paging: false,
                                    aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }, null, { "bSortable": false }]
                                });
                                $('#check-all').parent().removeClass("sorting_asc");
                            }
                            $.LoadingOverlay("hide");
                        }
                    });
                }
            }
        });
    }

    function checkminprice(strprice) {
        if (strprice == '' || strprice.length == 0) {
            return "0";
        }
        var price = strprice.split('+')[0];
        return price;
    }

    function checkmaxprice(strprice) {
        if (strprice == '' || strprice.length == 0) {
            return "-1";
        }
        var price = strprice.split('+')[1];
        return price;
    }

    function showboxcomfirm(title, message) {
        bootbox.confirm({
            title: title,
            message: message,
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Đóng'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Đồng ý'
                }
            },
            callback: function (result) {
                if (result) {
                    var cateId = parseInt($(".cateId").val());
                    var districtId = parseInt($(".districtId").val());
                    var newTypeId = parseInt($(".newTypeId").val());
                    var siteId = parseInt($(".siteId").val());
                    var backdate = parseInt($(".ddlbackdate").val());
                    var minPrice = parseFloat(checkminprice($(".ddlprice").val()));
                    var maxPrice = parseFloat(checkmaxprice($(".ddlprice").val()));
                    var from = $(".txtFrom").val();
                    var to = $(".txtTo").val();
                    var pageIndex = parseInt($('#datatable').attr("data-page"));
                    var pageSize = parseInt($(".ddlpage").val());
                    var isrepeat = $('#chkIsrepeatNews').prop('checked') ? 1 : 0;
                    var key = $.trim($(".txtsearchkey").val());

                    var NameOrder = "";
                    var descending = false;

                    $("#listnewstable th").each(function () {
                        if ($(this).hasClass("order_desc") || $(this).hasClass("order_asc")) {
                            NameOrder = $(this).attr("data-name");
                            if ($(this).hasClass("order_desc")) {
                                descending = true;
                            }
                        }
                    });

                    var url = "/home/exportexcel";
                    location.href = decodeURIComponent(url + "?cateId=" + cateId + "&districtId=" + districtId + "&newTypeId=" + newTypeId + "&siteId=" + siteId + "&backdate=" + backdate + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&from=" + from + "&to=" + to + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&IsRepeat=" + isrepeat + "&key=" + key + "&NameOrder=" + NameOrder + "&descending=" + descending);
                    $('#check-all').prop('checked', false);
                }
            }
        });
    }

});