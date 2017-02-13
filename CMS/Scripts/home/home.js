/*
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
                    var pageSize = parseInt($(".ddlpage").val());

                    var data = {
                        cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                        siteId: siteId, backdate: backdate,
                        minPrice: minPrice, maxPrice: maxPrice,
                        from: from, to: to, pageIndex: pageIndex, pageSize: pageSize
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
                            $('#datatable').attr("data-total", resp.TotalPage);
                            $('#datatable').attr("data-page", page);
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
            var pageSize = parseInt($(this).val());

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize
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

                }
                $.LoadingOverlay("hide");
            });
        });

        $("#check-all").checkAll();
        $(".mCustomScrollbar").mCustomScrollbar();
        $(document).on("click", ".checkboxItem", function () {
            var count = parseInt($('input:checkbox:checked').length);
            if ($(this).prop('checked')) {
                $(".btnsave, .btnhide, .btnreport").removeClass("disabled");
            } else {
                if (count < 1) {
                    $(".btnsave, .btnhide, .btnreport").addClass("disabled");
                }
            }
        });

        $(document).on("click", ".detail-item-list", function () {
            $.LoadingOverlay("show");
            $.get("/home/getnewsdetail", { Id: parseInt($(this).attr("data-id")) }, function (resp) {
                if (resp != null) {
                    $("#modaldetail").empty();
                    $("#modaldetail").html(resp.Content);
                    setTimeout(function () {
                        $("#newsdetail").modal("show");
                    }, 500);
                }
                $.LoadingOverlay("hide");
            });
        });

        $(document).on("click", ".btnclose", function () {
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".btnsubmit", function () {
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
            var pageSize = parseInt($(".ddlpage").val());

            var data = {
                cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                siteId: siteId, backdate: backdate,
                minPrice: minPrice, maxPrice: maxPrice,
                from: from, to: to, pageIndex: pageIndex, pageSize: pageSize
            };
            $.LoadingOverlay("show");
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
                }
                $.LoadingOverlay("hide");
            });
        });

        $(document).on("click", ".btnsave", function () {
            var selected = [];
            $('.checkboxItem:checked').each(function () {
                selected.push(parseInt($(this).attr('id')));
            });
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần lưu!");
            } else {
                $.post("/home/usersavenews", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được lưu thành công!");
                            }, 1200);
                        } else {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                    };
                });
            }
        });

        $(document).on("click", ".btnhide", function () {
            var selected = [];
            $('.checkboxItem:checked').each(function () {
                selected.push(parseInt($(this).attr('id')));
            });
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
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                    };
                });
            }
        });

        $(document).on("click", ".save-item-list", function () {
            var selected = [parseInt($(this).attr("data-id"))];
            if (selected.length == 0) {
                showmessage("error", "Bạn hãy chọn tin cần lưu!");
            } else {
                $.post("/home/usersavenews", { listNewsId: selected }, function (resp) {
                    if (resp != null) {
                        if (resp.Status == 1) {
                            LoadData();
                            setTimeout(function () {
                                showmessage("success", "Tin đã được lưu thành công!");
                            }, 1200);
                        } else {
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
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
                            showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                        }
                    };
                });
            }
            $("#newsdetail").modal("hide");
        });

        $(document).on("click", ".btnexport", function () {
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
            var url = "/home/exportexcel";
            location.href = decodeURIComponent(url + "?cateId=" + cateId + "&districtId=" + districtId + "&newTypeId=" + newTypeId + "&siteId=" + siteId + "&backdate=" + backdate + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&from=" + from + "&to=" + to + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
        });
        
        //Change search filter

        $(".cateId, .districtId, .newTypeId, .siteId, .ddlbackdate, .ddlprice, .txtFrom, .txtTo").change(function () {
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
        var pageIndex = parseInt($('#datatable').attr("data-page"));
        var pageSize = parseInt($(".ddlpage").val());

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
                pageSize: pageSize
            };
            $.LoadingOverlay("show");
            $.post("/home/loaddata", data, function(resp) {
                if (resp != null) {
                    $("#listnewstable tbody").html("");
                    $("#listnewstable tbody").html(resp.Content);
                    if (resp.TotalPage > 1) {
                        $(".page-home").show();
                        showPagination(resp.TotalPage);
                    } else {
                        $(".page-home").hide();
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
                var pageSize = parseInt($(".ddlpage").val());

                var data = {
                    cateId: cateId, districtId: districtId, newTypeId: newTypeId,
                    siteId: siteId, backdate: backdate,
                    minPrice: minPrice, maxPrice: maxPrice,
                    from: from, to: to, pageIndex: pageIndex, pageSize: pageSize
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
                        $('#datatable').attr("data-total", resp.TotalPage);
                        $('#datatable').attr("data-page", page);
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
});