/*
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
    $("#frmnews").validate({
            ignore: [],
            debug: false,
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
                    required: true,
                    number: true
                },
                pricetext: {
                    required: true,
                }
            },
            messages: {
                title: {
                    required: "Tiêu đề không được để trống",
                    minlength: "Tiêu đề lớn hơn 10 kí tự"
                },
                CategoryId: {
                    Selectcheck: "Bạn chưa chọn danh mục nào"
                },
                DistricId: {
                    Selectcheck: "Bạn chưa chọn quận huyện nào"
                },
                phone: {
                    required: "Số điện thoại không được để trống",
                    rgphone: "Số điện thoại chưa đúng định dạng",
                },
                price: {
                    required: "Giá không được bỏ trống",
                    number: "Giá phải là số"
                },
                pricetext: {
                    required: "Giá không được để trống",
                }
            },
            submitHandler: function () {
                alert(1);
            }
        });
});
