const fs = require('fs');
const hjson = require('hjson');
const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');
const filesHelper = require('./utils/files-helper');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const ModulesListPlugin = require('./utils/modules-list-plugin')
const Webpack = require('webpack');

let entry = {};

filesHelper.getDirInfos(path.join(__dirname, 'src', 'modules')).forEach((dir) => {

    var moduleEntry = {
        import: [],
        dependOn: 'common'
    }

    let indexFilePath = dir.path + '\\index.js';
    if (fs.existsSync(indexFilePath)) {
        moduleEntry.import.push(indexFilePath);
        entry[dir.name] = moduleEntry;
    }
});

entry.common = [path.join(__dirname, 'src', 'common', 'index.js')];

module.exports = {
    mode: "development",
    entry: entry,
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, '..', 'wwwroot', 'webpack', 'dev'),
        clean: true
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
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.css$/,
                use: [
                    'vue-style-loader',
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
                    'vue-style-loader',
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
                    },
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
        new VueLoaderPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css'
        }),
        new ModulesListPlugin(),
        new Webpack.DefinePlugin({ __VUE_OPTIONS_API__: true, __VUE_PROD_DEVTOOLS__: true }), // to remove warn in browser console: runtime-core.esm-bundler.js:3607 Feature flags __VUE_OPTIONS_API__, __VUE_PROD_DEVTOOLS__ are not explici
    ],
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm-bundler.js'
        }
    },
    devtool: 'source-map'
};