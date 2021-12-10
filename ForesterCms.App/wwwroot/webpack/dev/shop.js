"use strict";
(self["webpackChunkds"] = self["webpackChunkds"] || []).push([["shop"],{

/***/ "./src/modules/shop/main.js":
/*!**********************************!*\
  !*** ./src/modules/shop/main.js ***!
  \**********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _product_scss__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./product.scss */ "./src/modules/shop/product.scss");
//import 'swiper/swiper.scss';
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
    }
  },
  keyboard: {
    enabled: true
  },
  navigation: {
    nextEl: ".swiper-button-next",
    prevEl: ".swiper-button-prev"
  },
  breakpoints: {
    // when window width is >= 320px
    320: {
      slidesPerView: "auto",
      spaceBetween: 0,
      freeMode: true
    },
    // when window width is >= 640px
    640: {
      slidesPerView: "auto",
      spaceBetween: 0,
      freeMode: true
    },
    1024: {
      slidesPerView: "auto",
      spaceBetween: 0,
      freeMode: true
    },
    1440: {
      slidesPerView: "auto",
      spaceBetween: 0,
      freeMode: true
    }
  }
});

/***/ }),

/***/ "./src/modules/shop/product.scss":
/*!***************************************!*\
  !*** ./src/modules/shop/product.scss ***!
  \***************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
// extracted by mini-css-extract-plugin


/***/ })

},
/******/ __webpack_require__ => { // webpackRuntimeModules
/******/ var __webpack_exec__ = (moduleId) => (__webpack_require__(__webpack_require__.s = moduleId))
/******/ __webpack_require__.O(0, ["vendors"], () => (__webpack_exec__("./src/modules/shop/main.js")));
/******/ var __webpack_exports__ = __webpack_require__.O();
/******/ }
]);
//# sourceMappingURL=shop.js.map