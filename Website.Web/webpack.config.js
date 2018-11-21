var CleanWebpackPlugin = require('clean-webpack-plugin');
var distFolder = __dirname + "/wwwroot/dist";
var webpack = require('webpack');
module.exports = {

    entry: {
        site: [ 
            "./Scripts/site/index.ts"
        ],
        admin: "./Scripts/admin/index.ts"
    },
    output: {
        filename: "[name]-bundle.js",
        path: distFolder
    },
    // watch: true,
    optimization: {
        splitChunks: {
            chunks: "all",
            minSize: 0,
            name: "common"
        }
    },
    devtool: "source-map",

    resolve: {
        // Add '.ts' and '.tsx' as resolvable extensions.
        extensions: [".ts", ".tsx", ".js", ".json"]
    },

    module: {
        rules: [           
            {
                test: /\.tsx?$/,
                loader: "awesome-typescript-loader",
                exclude: /node_modules/
            },
            
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            },

            // {
            //     test: /\.css$/,
            //     use: ['style-loader', 'css-loader', 'postcss-loader'],
            // },
            // {
            //     test: /\.scss$/,
            //     use: ['style-loader', 'css-loader', 'postcss-loader', 'sass-loader'],
            // },
            // {
            //     test: /\.woff2?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
            //     use: 'url-loader?limit=10000',
            // },
            // {
            //     test: /\.(ttf|eot|svg)(\?[\s\S]+)?$/,
            //     use: 'file-loader',
            // },
        ]
    },  

    plugins: [
        new CleanWebpackPlugin([distFolder]),
        new webpack.ProvidePlugin({ // inject ES5 modules as global vars
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
            // Tether: "tether",
            // "window.Tether": "tether",
            // Popper: ['popper.js', 'default'],
            // Alert: "exports-loader?Alert!bootstrap/js/dist/alert",
            // Button: "exports-loader?Button!bootstrap/js/dist/button",
            // Carousel: "exports-loader?Carousel!bootstrap/js/dist/carousel",
            // Collapse: "exports-loader?Collapse!bootstrap/js/dist/collapse",
            // Dropdown: "exports-loader?Dropdown!bootstrap/js/dist/dropdown",
            // Modal: "exports-loader?Modal!bootstrap/js/dist/modal",
            // Popover: "exports-loader?Popover!bootstrap/js/dist/popover",
            // Scrollspy: "exports-loader?Scrollspy!bootstrap/js/dist/scrollspy",
            // Tab: "exports-loader?Tab!bootstrap/js/dist/tab",
            // Tooltip: "exports-loader?Tooltip!bootstrap/js/dist/tooltip",
            // Util: "exports-loader?Util!bootstrap/js/dist/util"
        })
    ]
};

