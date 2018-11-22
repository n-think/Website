var OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");
var merge = require('webpack-merge');
var common = require('./webpack.common.js');

module.exports = merge(common, {
    mode: 'production',
    plugins: [
        new OptimizeCSSAssetsPlugin(), 
    ]
});