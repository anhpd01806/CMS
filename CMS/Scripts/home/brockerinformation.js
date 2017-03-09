$(function () {
    $(document).ready(function () {
        $('.date-picker').datepicker({
            dateFormat: 'dd-mm-yy',
            autoclose: true,
            todayHighlight: true
        });

        $(document).on("change", "#chkIsrepeatNews", function () {
            if ($(this).prop('checked')) {
                $(this).val(1);
            } else {
                $(this).val(0);
            }
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
                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, null]
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
                onPageClick: function(event, page) {
                    var curentpage = parseInt($('#datatable').attr("data-page"));
                    if (curentpage != page) {
                        $.LoadingOverlay("show");
                        var cateId = parseInt($(".cateId").val());
                        var districtId = parseInt($(".districtId").val());
                        var newTypeId = 0;
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

                        var data = {
                            cateId: cateId,
                            districtId: districtId,
                            newTypeId: newTypeId,
                            siteId: siteId,
                            backdate: backdate,
                            minPrice: minPrice,
                            maxPrice: maxPrice,
                            from: from,
                            to: to,
                            pageIndex: pageIndex,
                            pageSize: pageSize,
                            IsRepeat: isrepeat,
                            key: key
                        };
                        $.post("/brokersinformation/loaddata", data, function(resp) {
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
                                        aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, null]
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

        $(document).on("click", ".lbltitle", function () {
            $(this).parents("tr").attr("style", "color: #bf983b;");
            $(this).parents("tr").find(".label-info").html("Đã xem");
            $(this).parents("tr").find(".label-info").addClass("label-warning");
            $(this).parents("tr").find(".label-info").addClass("arrowed-right");
            $(this).parents("tr").find(".label-info").addClass("arrowed-in");
            $(this).parents("tr").find(".label-warning").removeClass("label-info");
            $(this).parents("tr").find(".label-warning").removeClass("arrowed");
            //$.LoadingOverlay("show");
            $.get("/brokersinformation/getnewsdetail", { Id: parseInt($(this).attr("data-id")) }, function (resp) {
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
                //$.LoadingOverlay("hide");
            });
        });

        $(document).on("change", ".ddlpage", function () {
            $.LoadingOverlay("show");
            var cateId = parseInt($(".cateId").val());
            var districtId = parseInt($(".districtId").val());
            var newTypeId = 0;
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

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key
            };
            $.post("/brokersinformation/loaddata", data, function (resp) {
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, null]
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

        $("#check-all").checkAll();

        $(document).on("click", ".btnclose", function () {
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

        $(".cateId, .districtId, .siteId, .ddlbackdate, .ddlprice, .txtFrom, .txtTo, #chkIsrepeatNews").change(function () {
            LoadData();
        });

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

        $(document).on("click", ".btnhide", function () {
            if (!$(this).hasClass("disabled")) {
                var selected = [];
                $('.checkboxItem:checked').each(function () {
                    selected.push(parseInt($(this).attr('id')));
                });
                if (selected.length == 0) {
                    showmessage("error", "Bạn hãy chọn tin cần khôi phục!");
                } else {
                    $.post("/BrokersInformation/DeleteBlacklist", { listNewsId: selected }, function (resp) {
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

        $(document).on("click", ".hide-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần khôi phục!");
            } else {
                $.post("/BrokersInformation/DeleteBlacklist", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được khôi phục thành công!");
                            }, 1200);

                        } else {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                    };
                });
            }
            $("#newsdetail").modal("hide");
        });
    });

    function LoadData() {
        var cateId = parseInt($(".cateId").val());
        var districtId = parseInt($(".districtId").val());
        var newTypeId = 0;
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

            var data = {
                cateId: cateId,
                districtId: districtId,
                newTypeId: newTypeId,
                siteId: siteId,
                backdate: backdate,
                minPrice: minPrice,
                maxPrice: maxPrice,
                from: from,
                to: to,
                pageIndex: pageIndex,
                pageSize: pageSize,
                IsRepeat: isrepeat,
                key: key
            };
            $.LoadingOverlay("show");
            $.post("/brokersinformation/loaddata", data, function (resp) {
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, null]
                        });
                        $('#check-all').parent().removeClass("sorting_asc");
                    }
                }
                $.LoadingOverlay("hide");
            });
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
                $.LoadingOverlay("show");
                var cateId = parseInt($(".cateId").val());
                var districtId = parseInt($(".districtId").val());
                var newTypeId = 0;
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

                var data = {
                    cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                    siteId: siteId, backdate: backdate,
                    minPrice: minPrice, maxPrice: maxPrice,
                    from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key
                };
                $.post("/brokersinformation/loaddata", data, function (resp) {
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
                                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, null]
                            });
                            $('#check-all').parent().removeClass("sorting_asc");
                        }
                        $.LoadingOverlay("hide");
                    }
                });
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
                    label: '<i class="fa fa-check"></i> Đồng ý'
                },
                cancel: {
                    label: '<i class="fa fa-times"></i> Đóng'
                },
            },
            callback: function (result) {
                if (result) {
                    
                }
            }
        });
    }
});