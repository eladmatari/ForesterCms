const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

let getFileHash = function (fileFullPath) {
    const fileBuffer = fs.readFileSync(fileFullPath);
    const hashSum = crypto.createHash('sha256');
    hashSum.update(fileBuffer);

    const hex = hashSum.digest('hex');
    return hex;
}

class ModulesListPlugin {
    apply(compiler) {
        compiler.hooks.done.tap(
            'Modules List Plugin',
            (
                stats /* stats is passed as an argument when done hook is tapped.  */
            ) => {
                var modules = {};
                Object.keys(stats.compilation.assets).map((fileName) => {
                    if (!/(.js$)|(.css$)/i.test(fileName))
                        return;

                    let fileNameArr = fileName.split('.');
                    let fileExtension = fileNameArr[fileNameArr.length - 1].toLowerCase();

                    let fileFullPath = path.join(stats.compilation.outputOptions.path, fileName);
                    let filehash = getFileHash(fileFullPath);

                    let moduleName = fileNameArr.slice(0, fileNameArr.length - 1).join('.');
                    let currModule = modules[moduleName];
                    if (!currModule)
                        currModule = modules[moduleName] = { js: null, css: null };

                    currModule[fileExtension] = fileName + '?v=' + filehash;
                });

                fs.writeFileSync(path.join(stats.compilation.outputOptions.path, "modules.json"), JSON.stringify(modules), 'utf8', function (err) {
                    if (err)
                        console.error('Modules List Plugin', err);

                });
            }
        );
    }
}

module.exports = ModulesListPlugin;