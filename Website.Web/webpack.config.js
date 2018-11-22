var distFolder = __dirname + "/wwwroot/dist";
var MiniCssExtractPlugin = require("mini-css-extract-plugin");
var CleanWebpackPlugin = require("clean-webpack-plugin");
var OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = {
    entry: {
        site: [
            "./Scripts/site/_index.ts"
        ],
        admin: "./Scripts/admin/_index.ts"
    },

    output: {
        filename: "[name]-bundle.js",
        path: distFolder
    },

    optimization: {
        splitChunks: {
            chunks: "all",
            minSize: 0,
            name: "common"
        },
    },
    devtool: "source-map",
    
    resolve: {
        extensions: [".ts", ".tsx", ".js", ".json"]
    },

    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: "awesome-typescript-loader",
            },
            {
                test: /\.js$/,
                loader: "source-map-loader",
                enforce: "pre",
            },
            //css in js
            // {
            //     test: /\.css$/,
            //     use: ['style-loader', 'css-loader'],
            // },
            //css in file
            { 
                test: /\.css$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                        // options: {
                        //     // you can specify a publicPath here
                        //     // by default it use publicPath in webpackOptions.output
                        //     publicPath: '../'
                        // }
                    },
                    "css-loader"
                ]
            },
            {
                test: /\.woff2?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
                use: 'url-loader?limit=10000',
            },
            {
                test: /\.(ttf|eot|svg)(\?[\s\S]+)?$/,
                use: 'file-loader',
            },
        ]
    },

    plugins: [
        new CleanWebpackPlugin([distFolder]),
        new MiniCssExtractPlugin(),
        new OptimizeCSSAssetsPlugin(),
    ]
};

