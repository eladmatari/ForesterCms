(self["webpackChunkds"] = self["webpackChunkds"] || []).push([["yes"],{

/***/ "./src/modules/yes/main.js":
/*!*********************************!*\
  !*** ./src/modules/yes/main.js ***!
  \*********************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _products_scss__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./products.scss */ "./src/modules/yes/products.scss");

$(document).ready(function () {//alert(1);
});
console.log('yes');

/***/ }),

/***/ "./src/modules/yes/products.js":
/*!*************************************!*\
  !*** ./src/modules/yes/products.js ***!
  \*************************************/
/***/ (() => {

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
    }); // Close  DropDown Sort on any click

    $('html').click(function () {
      $(".tools_sec .sort .sort_items").removeClass("is-open");
    });
    $('.tools_sec .sort .title').click(function (event) {
      event.stopPropagation();
    }); // Close  DropDown Sort on any click

    $(".filters_box .title_box").on("click", function () {
      if ($(this).hasClass("is-open")) {
        $(this).removeClass("is-open");
        $(this).next("div").slideUp();
      } else {
        $(this).addClass("is-open");
        $(this).next("div").slideDown();
      }
    });
  } //POPUPS


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

/***/ }),

/***/ "./src/modules/yes/products.scss":
/*!***************************************!*\
  !*** ./src/modules/yes/products.scss ***!
  \***************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
// extracted by mini-css-extract-plugin


/***/ })

},
/******/ __webpack_require__ => { // webpackRuntimeModules
/******/ var __webpack_exec__ = (moduleId) => (__webpack_require__(__webpack_require__.s = moduleId))
/******/ __webpack_require__.O(0, ["vendors"], () => (__webpack_exec__("./src/modules/yes/main.js"), __webpack_exec__("./src/modules/yes/products.js")));
/******/ var __webpack_exports__ = __webpack_require__.O();
/******/ }
]);
//# sourceMappingURL=yes.js.map