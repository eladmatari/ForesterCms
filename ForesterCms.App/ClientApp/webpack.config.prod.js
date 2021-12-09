const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
let webpackConfig = require('./webpack.config.js');
const hjson = require('hjson');

webpackConfig.mode = 'production';
webpackConfig.output.path = path.resolve(__dirname, '..', 'wwwroot', 'webpack', 'prod');
webpackConfig.devtool = false;

webpackConfig.plugins.forEach(function (val, ind, arr) {
    if (val.definitions && val.definitions.__VUE_OPTIONS_API__ !== undefined) {
        val.definitions.__VUE_OPTIONS_API__ = false;
        val.definitions.__VUE_PROD_DEVTOOLS__ = false;
    }
});

//console.log(hjson.stringify(webpackConfig.plugins));

module.exports = webpackConfig;