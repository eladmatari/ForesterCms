import './app.js';
import { setAppComponents } from './vue-helper.js';

const ContainerApp = {
    data() {
        return {

        }
    },
    created: function () {

    }
}

vueApp.set('container', '.vac-container', ContainerApp, (app, moduleName) => {
    const fileRegex = /^\.\/common\//i;
    const fileModuleRegex = moduleName ? new RegExp('^\.\/modules\/' + moduleName + '\/', 'i') : null;

    setAppComponents(app, (fileName) => {
        if (fileRegex.test(fileName))
            return true;

        if (fileModuleRegex == null || fileModuleRegex.test(fileName))
            return true;

        return false;
    });
});