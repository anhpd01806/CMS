﻿/*
* Create 11/02/2017 Nghianh
*/
$(function () {
    $(document).ready(function () {
        $('.widget-container-col').sortable({
            connectWith: '.widget-container-col',
            items: '> .widget-box',
            handle: ace.vars['touch'] ? '.widget-title' : false,
            cancel: '.fullscreen',
            opacity: 0.8,
            revert: true,
            forceHelperSize: true,
            placeholder: 'widget-placeholder',
            forcePlaceholderSize: true,
            tolerance: 'pointer',
            start: function (event, ui) {
                ui.item.parent().css({ 'min-height': ui.item.height() });
            },
            update: function (event, ui) {
                ui.item.parent({ 'min-height': '' });
                var widget_order = {};
                $('.widget-container-col').each(function () {
                    var container_id = $(this).attr('id');
                    widget_order[container_id] = [];


                    $(this).find('> .widget-box').each(function () {
                        var widget_id = $(this).attr('id');
                        widget_order[container_id].push(widget_id);
                    });
                });

                ace.data.set('demo', 'widget-order', widget_order, null, true);
            }
        });

        $('.date-picker').datepicker({
            dateFormat: 'dd-mm-yy',
            autoclose: true,
            todayHighlight: true
        });

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
                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }]
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
                        var newTypeId = 0;
                        var siteId = parseInt($(".siteId").val());
                        var backdate = parseInt($(".ddlbackdate").val());
                        var minPrice = -1;
                        var maxPrice = -1;
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
                        $.post("/newssave/loaddata", data, function(resp) {
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
                                        aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }]
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

        $(document).on("change", ".ddlpage", function () {
            $.LoadingOverlay("show");
            var cateId = parseInt($(".cateId").val());
            var districtId = parseInt($(".districtId").val());
            var newTypeId = 0;
            var siteId = parseInt($(".siteId").val());
            var backdate = parseInt($(".ddlbackdate").val());
            var minPrice = -1;
            var maxPrice = -1;
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
            $.post("/newssave/loaddata", data, function (resp) {
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }]
                        });
                        $('#check-all').parent().removeClass("sorting_asc");
                    }
                }
                $.LoadingOverlay("hide");
            });
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

        $(document).on("change", "#chkIsrepeatNews", function () {
            if ($(this).prop('checked')) {
                $(this).val(1);
            } else {
                $(this).val(0);
            }
        });

        $("#check-all").checkAll();

        $(document).on("click", ".checkboxItem", function () {
            var count = parseInt($('input:checkbox:checked').length);
            if ($(this).prop('checked')) {
                $(".btnremove, .btnhide, .btnreport, .btnspam, .btndelete").removeClass("disabled");
            } else {
                if (count < 1) {
                    $(".btnremove, .btnhide, .btnreport, .btnspam, .btndelete").addClass("disabled");
                }
            }
        });

        $(document).on("click", ".detail-item-list", function () {
            $(this).parents("tr").attr("style", "color: #bf983b;");
            $(this).parents("tr").find(".label-info").html("Đã xem");
            $(this).parents("tr").find(".label-info").addClass("label-warning");
            $(this).parents("tr").find(".label-info").addClass("arrowed-right");
            $(this).parents("tr").find(".label-info").addClass("arrowed-in");
            $(this).parents("tr").find(".label-warning").removeClass("label-info");
            $(this).parents("tr").find(".label-warning").removeClass("arrowed");
            $.LoadingOverlay("show");
            $.get("/newssave/getnewsdetail", { Id: parseInt($(this).attr("data-id")) }, function (resp) {
                if (resp != null) {
                    if (resp.Pay == 1 && resp.Content != "") {
                        $("#modaldetail").empty();
                        $("#modaldetail").html(resp.Content);
                        setTimeout(function () {
                            $("#newsdetail").modal("show");
                        }, 500);
                    } else {
                        window.location.href = '/Payment/RegisterPackage';
                    }
                }
                $.LoadingOverlay("hide");
            });
        });

        $(document).on("click", ".btnsamenews", function () {
            $("#newsdetail").modal("hide");
            var id = $(this).attr("data-id");
            setTimeout(function () {
                $.LoadingOverlay("show");
                $.get("/home/getnewsdetail", { Id: parseInt(id) }, function (resp) {
                    if (resp != null) {
                        if (resp.Pay == 1 && resp.Content != "") {
                            $("#modaldetail").empty();
                            $("#modaldetail").html(resp.Content);
                            setTimeout(function () {
                                $("#newsdetail").modal("show");
                            }, 500);
                        } else {
                            window.location.href = '/Payment/RegisterPackage';
                        }
                    }
                    $.LoadingOverlay("hide");
                });
            }, 200);
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
            $.get("/newssave/getnewsdetail", { Id: parseInt($(this).attr("data-id")) }, function (resp) {
                if (resp != null) {
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
                }
            });
        });

        $(document).on("click", ".btnclose", function () {
            $("#newsdetail").modal("hide");
            $("#newsedit").modal("hide");
        });

        $(document).on("click", ".btnremove", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function() {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần xóa khỏi danh sách lưu!");
                } else {
                    $.post("/newssave/userremovenewssave", { listNewsId: selected }, function(resp) {
                        if (resp != null) {
                            if (resp.Status == 1) {
                                LoadData();
                                setTimeout(function() {
                                    showmessage("success", "Tin đã được xóa thành công!");
                                }, 1200);
                            } else {
                                showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
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
                $('.checkboxItem:checked').each(function() {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần ẩn!");
                } else {
                    $.post("/home/userhidenews", { listNewsId: selected }, function(resp) {
                        if (resp != null) {
                            if (resp.Status == 1) {
                                LoadData();
                                setTimeout(function() {
                                    showmessage("success", "Tin đã được ẩn thành công!");
                                }, 1200);

                            } else {
                                showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                            }
                        }
                        ;
                    });
                }
            }
        });

        $(document).on("click", ".btndelete", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần xóa!");
                } else {
                    $.post("/home/delete", { listNewsId: selected }, function (resp) {
                        if (resp != null) {
                            if (resp.Status == 1) {
                                LoadData();
                                setTimeout(function () {
                                    showmessage("success", "Tin đã được xóa thành công!");
                                }, 1200);

                            } else {
                                if (resp.Status == 2) {
                                    showmessage("error", "Bạn chưa chọn tin nào để xóa!");
                                } else {
                                    showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                }
                            }
                        }
                        ;
                    });
                }
            }
        });
        
        $(document).on("click", ".delete-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần xóa!");
            } else {
                bootbox.confirm({
                    title: "Thông báo",
                    message: "Bạn có chắc muốn xóa này không?",
                    buttons: {
                        confirm: {
                            label: '<i class="fa fa-check"></i> Đồng ý',
                            className: 'btn-sm margin-right-10 btn-primary'
                        },
                        cancel: {
                            label: '<i class="fa fa-times"></i> Đóng',
                            className: 'btn-sm pull-right btn-inverse'
                        },
                    },
                    callback: function (result) {
                        if (result) {
                            $.post("/home/delete", { listNewsId: selected }, function (resp) {
                                if (resp != null) {
                                    if (resp.Status == 1) {
                                        LoadData();
                                        setTimeout(function () {
                                            showmessage("success", "Tin đã được xóa thành công!");
                                        }, 1200);

                                    } else {
                                        if (resp.Status == 2) {
                                            showmessage("error", "Bạn chưa chọn tin nào để xóa!");
                                        } else {
                                            showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                        }
                                    }
                                }
                                ;
                            });
                            $("#newsdetail").modal("hide");
                        }
                    }
                });
            }
        });
        
        /*Nút báo tin chính chủ*/
        $(document).on("click", ".btncc", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần báo chính chủ!");
                } else {
                    bootbox.confirm({
                        title: "Thông báo",
                        message: "Bạn có chắc muốn báo chính chủ tin này không?",
                        buttons: {
                            confirm: {
                                label: '<i class="fa fa-check"></i> Đồng ý',
                                className: 'btn-sm margin-right-10 btn-primary'
                            },
                            cancel: {
                                label: '<i class="fa fa-times"></i> Đóng',
                                className: 'btn-sm pull-right btn-inverse'
                            },
                        },
                        callback: function (result) {
                            if (result) {
                                $.post("/home/newsforuser", { listNewsId: selected }, function (resp) {
                                    if (resp != null) {
                                        if (resp.Status == 1) {
                                            LoadData();
                                            setTimeout(function () {
                                                showmessage("success", "Tin đã được báo thành công!");
                                            }, 800);
                                        } else {
                                            showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
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

        /*Báo chính chủ*/
        $(document).on("click", ".iscc-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin chính chủ!");
            } else {
                bootbox.confirm({
                    title: "Thông báo",
                    message: "Bạn có chắc muốn báo chính chủ tin này không?",
                    buttons: {
                        confirm: {
                            label: '<i class="fa fa-check"></i> Đồng ý',
                            className: 'btn-sm margin-right-10 btn-primary'
                        },
                        cancel: {
                            label: '<i class="fa fa-times"></i> Đóng',
                            className: 'btn-sm pull-right btn-inverse'
                        },
                    },
                    callback: function (result) {
                        if (result) {
                            $.post("/home/newsforuser", { listNewsId: selected }, function (resp) {
                                if (resp != null) {
                                    if (resp.Status == 1) {
                                        LoadData();
                                        setTimeout(function () {
                                            showmessage("success", "Tin đã được báo thành công!");
                                        }, 800);
                                    } else {
                                        showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                    }
                                }
                                ;
                            });
                            $("#newsdetail").modal("hide");
                        }
                    }
                });
            }
        });

        /*hủy tin chính chủ*/
        $(document).on("click", ".rmiscc-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin hủy chính chủ!");
            } else {
                bootbox.confirm({
                    title: "Thông báo",
                    message: "Bạn có chắc muốn hủy chính chủ tin này không?",
                    buttons: {
                        confirm: {
                            label: '<i class="fa fa-check"></i> Đồng ý',
                            className: 'btn-sm margin-right-10 btn-primary'
                        },
                        cancel: {
                            label: '<i class="fa fa-times"></i> Đóng',
                            className: 'btn-sm pull-right btn-inverse'
                        },
                    },
                    callback: function (result) {
                        if (result) {
                            $.post("/home/Removenewsforuser", { listNewsId: selected }, function (resp) {
                                if (resp != null) {
                                    if (resp.Status == 1) {
                                        LoadData();
                                        setTimeout(function () {
                                            showmessage("success", "Tin đã được bỏ thành công!");
                                        }, 800);
                                    } else {
                                        showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                    }
                                }
                                ;
                            });
                            $("#newsdetail").modal("hide");
                        }
                    }
                });
            }
        });

        $(document).on("click", ".remove-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin xóa khỏi danh sách đã lưu!");
            } else {
                $.post("/newssave/userremovenewssave", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được xóa thành công!");
                            }, 1200);
                        } else {
                            showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                        }
                    };
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".hide-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần ẩn!");
            } else {
                $.post("/home/userhidenews", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được ẩn thành công!");
                            }, 1200);

                        } else {
                            showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                        }
                    };
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".spam-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần ẩn!");
            } else {
                $.post("/home/newsspam", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin môi giới đã được cho vào danh sách đen!");
                            }, 1200);

                        } else {
                            if (resp.Status == 2) {
                                showmessage("success", "Tin không được cho vào danh sách đen! Số điện thoại đang bị trống!");
                            } else {
                                if (resp.Status == 3) {
                                    showmessage("success", "Không tìm thấy tin cần cho chặn!");
                                } else {
                                    showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                }
                            }
                        }
                    };
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".report-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần ẩn!");
            } else {
                bootbox.confirm({
                    title: "Thông báo",
                    message: "Bạn có chắc chắn muốn báo môi giới những tin này không?",
                    buttons: {
                        confirm: {
                            label: '<i class="fa fa-check"></i> Đồng ý',
                            className: 'btn-sm margin-right-10 btn-primary'
                        },
                        cancel: {
                            label: '<i class="fa fa-times"></i> Đóng',
                            className: 'btn-sm pull-right btn-inverse'
                        },
                    },
                    callback: function (result) {
                        if (result) {
                            $.post("/home/reportnews", { listNewsId: selected }, function (resp) {
                                if (resp != null) {
                                    LoadData();
                                    setTimeout(function () {
                                        showmessage("success", "Tin môi giới đã được báo cáo thành công!");
                                    }, 1200);
                                    //send notify to admin
                                    var socket = io.connect('http://ozo.vn:8010');
                                    $.each(resp, function (i, value) {
                                        var notify = {};
                                        notify.title = value.Title;
                                        //notify id
                                        notify.id = value.Id;
                                        notify.type = value.Type;
                                        //user name
                                        notify.name = value.UserName;
                                        //link image avatar
                                        notify.avatar = "/assets/avatars/avatar.png";
                                        notify.time = moment(value.DateSend).format('DD/MM/YYYY hh:mm:ss');;
                                        socket.emit('send-to-admin', notify);
                                    });
                                }
                                else {
                                    showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                }
                                ;
                            });
                        }
                    }
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".btnexport", function () {
            var selected = "";
            $('.checkboxItem:checked').each(function () {
                selected += $(this).attr('id') + ",";
            });
            selected = selected.slice(0, -1);
            if (selected.length == 0) {
                showboxcomfirm("Thông báo", "Bạn chưa chọn tin nào! Bạn có muốn xuất hết danh sách tin tức đang được hiển thị không?");
            } else {
                var url = "/home/exportexcelv2";
                location.href = decodeURIComponent(url + "?listNewsId=" + selected);
                $('#check-all').prop('checked', false);
            }
        });

        $(document).on("click", ".btnreport", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần ẩn!");
                } else {
                    $.post("/home/reportnews", { listNewsId: selected }, function (resp) {
                        bootbox.confirm({
                            title: "Thông báo",
                            message: "Bạn có chắc chắn muốn báo môi giới những tin này không?",
                            buttons: {
                                confirm: {
                                    label: '<i class="fa fa-check"></i> Đồng ý',
                                    className: 'btn-sm margin-right-10 btn-primary'
                                },
                                cancel: {
                                    label: '<i class="fa fa-times"></i> Đóng',
                                    className: 'btn-sm pull-right btn-inverse'
                                },
                            },
                            callback: function (result) {
                                if (result) {
                                    $.post("/home/reportnews", { listNewsId: selected }, function (resp) {
                                        if (resp != null) {
                                            LoadData();
                                            setTimeout(function () {
                                                showmessage("success", "Tin môi giới đã được báo cáo thành công!");
                                            }, 1200);
                                            //send notify to admin
                                            var socket = io.connect('http://ozo.vn:8010');
                                            $.each(resp, function (i, value) {
                                                var notify = {};
                                                notify.title = value.Title;
                                                //notify id
                                                notify.id = value.Id;
                                                notify.type = value.Type;
                                                //user name
                                                notify.name = value.UserName;
                                                //link image avatar
                                                notify.avatar = "/assets/avatars/avatar.png";
                                                notify.time = moment(value.DateSend).format('DD/MM/YYYY hh:mm:ss');;
                                                socket.emit('send-to-admin', notify);
                                            });
                                        }
                                        else {
                                            showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                        }
                                        ;
                                    });
                                }
                            }
                        });
                    });
                }
            }
        });

        $(document).on("click", ".btnspam", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần ẩn!");
                } else {
                    $.post("/home/newsspam", { listNewsId: selected }, function (resp) {
                        if (resp != null) {
                            if (resp.Status == 1) {
                                LoadData();
                                setTimeout(function () {
                                    showmessage("success", "Tin môi giới đã được cho vào danh sách đen!");
                                }, 1200);

                            } else {
                                if (resp.Status == 2) {
                                    showmessage("success", "Tin không được cho vào danh sách đen! Số điện thoại đang bị trống!");
                                } else {
                                    if (resp.Status == 3) {
                                        showmessage("success", "Không tìm thấy tin cần cho chặn!");
                                    } else {
                                        showmessage("error", "Bạn không có quyền hoặc server đang bận. vui long thử lại sau!");
                                    }
                                }
                            }
                        };
                    });
                }
            }
        });

        //Change search filter

        $(".cateId, .districtId, .siteId, .newTypeId, .ddlbackdate, .ddlprice, .txtFrom, .txtTo, #chkIsrepeatNews").change(function () {
            LoadData();
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
            //a Hai comment 
            //LoadData();
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
        
        $(document).on("click", ".btnedit-item-list", function () {
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
        var newTypeId = 0;
        var siteId = parseInt($(".siteId").val());
        var backdate = parseInt($(".ddlbackdate").val());
        var minPrice = -1;
        var maxPrice = -1;
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
            $.post("/newssave/loaddata", data, function (resp) {
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }]
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
                    var newTypeId = 0;
                    var siteId = parseInt($(".siteId").val());
                    var backdate = parseInt($(".ddlbackdate").val());
                    var minPrice = -1;
                    var maxPrice = -1;
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
                    $.post("/newssave/loaddata", data, function (resp) {
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
                            if (resp.TotalRecord > 0) {
                                $('#listnewstable').DataTable({
                                    sDom: 'rt',
                                    retrieve: true,
                                    bFilter: false,
                                    bInfo: false,
                                    searching: false,
                                    paging: false,
                                    aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, { "bSortable": false }, { "bSortable": false }, null, null, { "bSortable": false }, null, { "bSortable": false }]
                                });
                                $('#check-all').parent().removeClass("sorting_asc");
                            }
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
                confirm: {
                    label: '<i class="fa fa-check"></i> Đồng ý',
                    className: 'btn-sm margin-right-10 btn-primary'
                },
                cancel: {
                    label: '<i class="fa fa-times"></i> Đóng',
                    className: 'btn-sm pull-right btn-inverse'
                },
            },
            callback: function (result) {
                
                if (result) {
                    var cateId = parseInt($(".cateId").val());
                    var districtId = parseInt($(".districtId").val());
                    var newTypeId = 0;
                    var siteId = parseInt($(".siteId").val());
                    var backdate = parseInt($(".ddlbackdate").val());
                    var minPrice = -1;
                    var maxPrice = -1;
                    var from = $(".txtFrom").val();
                    var to = $(".txtTo").val();
                    var pageIndex = parseInt($('#datatable').attr("data-page"));
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

                    var url = "/newssave/exportexcel";
                    location.href = decodeURIComponent(url + "?cateId=" + cateId + "&districtId=" + districtId + "&newTypeId=" + newTypeId + "&siteId=" + siteId + "&backdate=" + backdate + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&from=" + from + "&to=" + to + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize + "&IsRepeat=" + isrepeat + "&key=" + key + "&NameOrder=" + NameOrder + "&descending=" + descending);
                    $('#check-all').prop('checked', false);
                }
            }
        });
    }
});