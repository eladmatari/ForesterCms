//import 'swiper/swiper.scss';
import './product.scss';
//import Swiper from 'swiper';


var swiper = new Swiper({
    el: ".product_img_carousel",
    initialSlide: 0,
    slidesPerView: "auto",
    updateOnWindowResize: false,
    autoHeight: true,
    freeMode: true,
    setWrapperSize: true,
    slideToClickedSlide: true,
    grabCursor: true,
    iOSEdgeSwipeDetection: true,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
        renderBullet: function (index, className) {
            return '<span class="' + className + '"><img src="/images/product.png"/></span>';
        },
    },
    keyboard: {
        enabled: true,
    },

    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    breakpoints: {
        // when window width is >= 320px
        320: {
            slidesPerView: "auto",
            spaceBetween: 0,
            freeMode: true,
        },

        // when window width is >= 640px
        640: {
            slidesPerView: "auto",
            spaceBetween: 0,
            freeMode: true,
        },
        1024: {
            slidesPerView: "auto",
            spaceBetween: 0,
            freeMode: true,
        },
        1440: {
            slidesPerView: "auto",
            spaceBetween: 0,
            freeMode: true,
        },
    },
});




