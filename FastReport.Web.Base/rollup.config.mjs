// JS bundle builder
import { nodeResolve } from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import terser from '@rollup/plugin-terser';
import del from 'rollup-plugin-delete';
import replace from '@rollup/plugin-replace';
import { defineConfig } from 'rollup';
import path from 'path';

const args = process.argv.slice(2);
const isProduction = args[args.indexOf('--environment') + 1] != 'development';
const isOpenSource = args[args.indexOf('--environment') + 1] == 'opensource';
const outPutPath = args[args.indexOf('--output-path') + 1].replace('"', '');
const outputFile = path.join(outPutPath, 'wwwroot', 'js', 'webreport-script.bundle.min.js');

export default defineConfig([
    {
        // main bundle
        input: 'wwwroot/js/main-scripts.js',
        output: {
            file: outputFile,
            format: 'iife',
            name: 'WebReportScript',
            sourcemap: !isProduction
        },
        plugins: [

            del({ targets: outputFile, force: true }),
            del({ targets: `${outputFile}.min`, force: true }),
            replace({
                preventAssignment: true,
                values: {
                    '__EXPORT_SETTINGS__': !isOpenSource ? './ExportScripts/export-settings.js' : './ExportScripts/export-settings-open-source.js',
                }
            }),
            nodeResolve({ browser: true }),
            commonjs(),

            // Minification
            isProduction && terser({
                compress: {
                    drop_console: true,
                    drop_debugger: true
                }
            })
        ].filter(Boolean),
        onwarn(warning, warn) {
            if (warning.code === 'THIS_IS_UNDEFINED') return;
            warn(warning);
        }
    },
    {
        input: 'wwwroot/js/printscript.js',
        output: {
            file: path.join(outPutPath, 'wwwroot', 'js', 'printscript.min.js'),
            format: 'iife',
            name: 'WebReportExportScript',
            sourcemap: !isProduction
        },
        plugins: [
            nodeResolve({ browser: true }),
            commonjs(),
            isProduction && terser()
        ].filter(Boolean)
    },
    {
        input: 'wwwroot/js/split.js',
        output: {
            file: path.join(outPutPath, 'wwwroot', 'js', 'split.min.js'),
            format: 'iife',
            name: 'Split',
            sourcemap: false
        },
        plugins: [
           terser()
        ].filter(Boolean)
    }
]);