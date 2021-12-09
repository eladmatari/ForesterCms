const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
let webpackConfig = require('./webpack.config.js');

webpackConfig.mode = 'production';
webpackConfig.output.path = path.resolve(__dirname, '..', 'wwwroot', 'webpack', 'prod');
webpackConfig.devtool = false;

module.exports = webpackConfig;