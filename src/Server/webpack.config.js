const path          = require('path');
const webpack       = require('webpack');
const autoprefixer  = require('autoprefixer');

module.exports = function (env) {
    env = env || {};
    var isProduction = env.NODE_ENV === 'production';

    var commonConfig = {
        module: {
            rules: [
                {
                    test: /\.(woff2?|ttf|otf|eot|svg)$/,
                    loader: 'file-loader',
                    exclude: /node_modules/,
                    options: {
                        name: './fonts/[name].[ext]'
                    }
                },
                {
                    test: /\.(png|jpg|gif)$/,
                    loader: 'file-loader',
                    exclude: /node_modules/,
                    options: {
                        name: './img/[name].[ext]'
                    }
                },
                {
                    test: /\.scss$/,
                    use: [
                        {
                            loader: 'file-loader',
                            options: {
                                name: './css/[name].css'
                            }
                        },
                        { loader: 'extract-loader' },
                        {
                            loader: 'css-loader',
                            options: {
                                url: false
                            }
                        },
                        {
                            loader: 'postcss-loader',
                            options: {
                                postcssOptions: {
                                    plugins: () => [autoprefixer()]
                                }
                            }
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                implementation: require('sass'),
                                webpackImporter: false,
                                sassOptions: {
                                    includePaths: ['./node_modules']
                                }
                            }
                        },
                    ]
                },
            ]
        }
    }

    var appConfig = Object.assign({}, commonConfig, {
        name: 'app',
        entry: './Assets/styles/app.scss',
        output: {
            path: path.join(__dirname, 'wwwroot'),
            filename: './js/app-bundle.js'
        },
    });

    var siteConfig = Object.assign({}, commonConfig, {
        name: 'site',
        entry: './Assets/styles/site.scss',
        output: {
            path: path.join(__dirname, 'wwwroot'),
            filename: './js/site-bundle.js'
        },
    });

    return [appConfig, siteConfig];
};