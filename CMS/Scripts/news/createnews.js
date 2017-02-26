﻿/*
* 
*/
$(document).ready(function () {
    CKEDITOR.replace('txtcontent');
    jQuery.validator.addMethod("valueNotEquals", function (value, element, arg) {
        return arg != value;
    }, "Bạn chưa chọn giá trị nào.");
    jQuery.validator.addMethod("rgphone", function (value, element) {
        return this.optional(element) || /^(098|095|097|096|0169|0168|0167|0166|0165|0164|0163|0162|090|093|0122|0126|0128|0121|0120|091|094|0123|0124|0125|0127|0129|092|0188|0186|099|0199|086|088|089|087)[0-9]{7}$/.test(value);
    }, "Số điện thoại không đúng định dạng");
    jQuery.validator.addMethod('Selectcheck', function (value) {
        return (value != '0');
    }, "Bạn chưa chọn giá trị nào.");


    $("#txtprice").keyup(function () {
        $(this).val(ConvertCurrency(String(($(this).val().replace(/\./g, '')).replace(/-/g, ''))));
    });

    $("#frmnews").validate({
        ignore: [],
        rules: {
            title: {
                required: true,
                minlength: 10
            },
            CategoryId: {
                Selectcheck: true
            },
            DistricId: {
                Selectcheck: true
            },
            phone: {
                required: true,
                rgphone: true,
            },
            price: {
                required: true
            },
            pricetext: {
                required: true,
            },
            txtcontent: {
                required: function () {
                    CKEDITOR.instances.txtcontent.updateElement();
                }
            },
            hiddenRecaptcha: {
                required: function () {
                    if (grecaptcha.getResponse() == '') {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        },
        messages: {
            title: {
                required: "Tiêu đề không được để trống",
                minlength: "Tiêu đề lớn hơn 10 kí tự"
            },
            phone: {
                required: "Số điện thoại không được để trống",
                rgphone: "Số điện thoại chưa đúng định dạng",
            },
            price: {
                required: "Giá không được bỏ trống"
            },
            pricetext: {
                required: "Giá không được để trống",
            },
            CategoryId: {
                Selectcheck: "Bạn chưa chọn danh mục nào"
            },
            DistricId: {
                Selectcheck: "Bạn chưa chọn quận huyện nào"
            },
            txtcontent: {
                required: "Nội dung tin không được bỏ trống"
            },
            hiddenRecaptcha: {
                required: "Bạn chưa điền mã capcha"
            }
        },
        submitHandler: function () {

            var title = $.trim($("#txttitle").val());
            var cateId = $("#CategoryId").val();
            var districtId = $("#DistricId").val();
            var phone = $("#txtphone").val();
            var price = $("#txtprice").val().replace(/\./g, '');
            var pricetext = $("#txtpricetext").val();
            var content = CKEDITOR.instances['txtcontent'].getData();
            if ($.isNumeric(price)) {
                var data = {
                    title: title,
                    cateId: cateId,
                    districtId: districtId,
                    phone: phone,
                    price: price,
                    pricetext: pricetext,
                    content: content
                };
                $.post("/news/createNews", data, function(resp) {
                    if (resp != null) {
                        if (resp.type == 1) {
                            showmessage("success", "Tin đã được đăng thành công chờ quản trị viên duyệt!");
                            setTimeout(function() {
                                window.location.href = "/home/index";
                            }, 2000);
                        }
                    } else {
                        showmessage("error", "Hệ thống gặp sự cố trong quá trình update dữ liệu!");
                    }
                });
            } else {
                showmessage("error", "Giá tiền phải là số!");
                return false;
            }
            
        }
    });

    function ConvertCurrency(str) {
        return str.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    }
});
