const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    mode: 'development',
    entry: {
        bot: './src/bot/index.ts',
        spectator: './src/spectator/index.ts'
    },
    output: {
        filename: '[name].js',
        publicPath: "/dist/",
        path: path.resolve(__dirname, 'dist'),
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                loader: 'ts-loader',
                exclude: /node_modules/,
            },
        ]
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js"],
        alias: {
            common: path.resolve(__dirname, 'src/common/'),
            bot: path.resolve(__dirname, 'src/bot/'),
            spectator: path.resolve(__dirname, 'src/spectator/')
        }
    },
    plugins: [
        new HtmlWebpackPlugin({
            chunks: ['bot'],
            template: './src/bot/index.html',
            filename: 'bot.html',
            publicPath: '.'
        }),
        new HtmlWebpackPlugin({
            chunks: ['spectator'],
            template: './src/spectator/index.html',
            filename: 'spectator.html'
        })
    ],
    devServer: {
        contentBase: path.join(__dirname, 'dist')
    }
};