// CSS bundle builder
module.exports = (ctx) => {
    const args = process.argv.slice(2);
    const isProduction = args[args.indexOf('--environment') + 1] != 'development';

    const plugins = [
        // Combining imports (@import)
        // require('postcss-import')(),

        // Auto-prefixes
        require('autoprefixer')({
            overrideBrowserslist: ['last 2 versions', '> 1%', 'IE 11']
        })
    ];

    // Minification
    if (isProduction) {
        plugins.push(
            require('cssnano')({
                preset: 'default'
            })
        );
    }

    return {
        plugins
    };
};