var CleanWebpackPlugin = require('clean-webpack-plugin');
var distFolder = __dirname + "/wwwroot/dist";
module.exports = {


    entry: {
        site: "./Scripts/site/index.ts",        
        admin: "./Scripts/admin/index.ts"
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
        }
    },
    devtool: "source-map",

    resolve: {
        // Add '.ts' and '.tsx' as resolvable extensions.
        extensions: [".ts", ".tsx", ".js", ".json"]
    },

    module: {
        rules: [
            // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
            {
                test: /\.tsx?$/,
                loader: "awesome-typescript-loader",
                exclude: /node_modules/
            },

            // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
            {
                enforce: "pre",
                test: /\.js$/,
                loader: "source-map-loader"
            }
        ]
    },

    plugins: [
        new CleanWebpackPlugin([distFolder])
    ]
};

