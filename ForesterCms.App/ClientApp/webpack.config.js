const fs = require('fs');
const hjson = require('hjson');
const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');
const filesHelper = require('./utils/files-helper');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const Webpack = require('webpack');

let entry = {};

filesHelper.getDirInfos(path.join(__dirname, 'src', 'modules')).forEach((dir) => {

    var moduleEntry = {
        import: [],
        dependOn: 'common'
    }

    filesHelper.getFileInfos(dir.path, true, (f) => {
        return f.isDirectory || f.extension == 'js';
    }).forEach((f) => {
        moduleEntry.import.push(f.path);
    });

    if (moduleEntry.import.length > 0) {
        entry[dir.name] = moduleEntry;
    }

});

entry.common = [];

filesHelper.getFileInfos(path.join(__dirname, 'src', 'common'), true, (f) => {
    return f.isDirectory || f.extension == 'js' || f.extension == 'vue';
}).forEach((f) => {
    entry.common.push(f.path);
});

module.exports = {
    mode: "development",
    entry: entry,
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'webpack', 'dev'),
        //ecmaVersion: 5
    },
    optimization: {
        splitChunks: {
            chunks: 'all',
            name: (module, chunks, cacheGroupKey) => {
                return 'vendors';
            }
        },
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            },
            {
                test: /\.m?js$/,
                //exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                        }
                    }
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                        }
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: true,
                        }
                    }
                ]
            },
            {
                test: /\.(png|svg|jpg|gif)$/,
                type: 'asset/resource'
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                type: 'asset/resource'
            },
        ]
    },
    plugins: [
        new CleanWebpackPlugin(),
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css',
            //path: path.resolve(__dirname, '..', 'wwwroot', 'webpack', 'prod')
        }),
        new Webpack.DefinePlugin({ __VUE_OPTIONS_API__: true, __VUE_PROD_DEVTOOLS__: true }), // to remove warn in browser console: runtime-core.esm-bundler.js:3607 Feature flags __VUE_OPTIONS_API__, __VUE_PROD_DEVTOOLS__ are not explici
    ],
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm-bundler.js'
        }
    },
    devtool: 'source-map'
};