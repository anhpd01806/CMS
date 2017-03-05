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
                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, null]
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
                                    aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, null]
                                });
                                $('#check-all').parent().removeClass("sorting_asc");
                            }
                            $.LoadingOverlay("hide");
                        }
                    });
                }
            });
        }

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

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key
            };
            $.post("/home/loaddata", data, function (resp) {
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, null]
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

        $(".cateId, .districtId, .newTypeId, .siteId, .ddlbackdate, .ddlprice, .txtFrom, .txtTo, #chkIsrepeatNews").change(function () {
            LoadData();
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
                            aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, null]
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

                var data = {
                    cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                    siteId: siteId, backdate: backdate,
                    minPrice: minPrice, maxPrice: maxPrice,
                    from: from, to: to, pageIndex: pageIndex, pageSize: pageSize, IsRepeat: isrepeat, key: key
                };
                $.post("/home/loaddata", data, function (resp) {
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
                                aoColumns: [{ "bSortable": false, "aTargets": 'no-sort' }, null, { "bSortable": false }, null, null, { "bSortable": false }, { "bSortable": false }, { "bSortable": false }, null]
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