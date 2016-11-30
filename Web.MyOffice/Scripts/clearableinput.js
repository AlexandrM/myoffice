jQuery(function ($) {

    function tog(v) { return v ? 'addClass' : 'removeClass'; }
    $(document)
        .on('input', '.clearable', function () {
            $(this)[tog(this.value)]('x');
        }).on('mousemove', '.x', function (e) {
            $(this)[tog(this.offsetWidth - 18 < e.clientX - this.getBoundingClientRect().left)]('onX');
        }).on('mousemove', '.clearable', function (e) {
            $(this).addClass("x");
        }).on('mouseleave', '.clearable', function (e) {
            $(this).removeClass("x");
        }).on('click', '.onX', function () {
            $(this).keyup();
            $(this).removeClass('x onX').val('').change();
        });


});