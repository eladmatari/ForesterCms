
$(document).ready(function () {
    if (screen.width <= 768) {
        $(".tools_sec .filter .title").on("click", function () {
            $(".main_sec .filters_box").addClass("is-open");
        });

        $(".btn_mobile_filter").on("click", function () {
            $(".main_sec .filters_box").removeClass("is-open");
        });

        $(".tools_sec .sort .title").on("click", function () {
            $(".tools_sec .sort .sort_items").addClass("is-open");
        });

        // Close  DropDown Sort on any click
        $('html').click(function () {
            $(".tools_sec .sort .sort_items").removeClass("is-open");
        });
        $('.tools_sec .sort .title').click(function (event) {
            event.stopPropagation();
        });
        // Close  DropDown Sort on any click

        $(".filters_box .title_box").on("click", function () {
            if ($(this).hasClass("is-open")) {
                $(this).removeClass("is-open");
                $(this).next("div").slideUp();
            }
            else {
                $(this).addClass("is-open");
                $(this).next("div").slideDown();
            }
        });




    }


    //POPUPS
    $(".btn_more_info").click(function (e) {
        e.preventDefault();

        $(".popup").css("display", "flex");
        $("body").addClass("overlay_on");
    });
    $("#closePop").click(function () {
        $(".popup").css("display", "none");
        $("body").removeClass("overlay_on");
    });


});


