import * as Vue from 'vue';
import * as jQuery from 'jquery';
import * as axios from 'axios';

window.$ = window.jQuery = jQuery;
window.axios = axios;
window.Vue = Vue;

if (!window.datasource)
    window.datasource = {};

if (!window.app)
    window.app = {};

window.localStorageService = {
    getItem: function (key) {
        var jsonData = localStorage.getItem(key);

        if (jsonData) {
            try {
                var storageData = JSON.parse(jsonData)

                if (!storageData.expirationDate || storageData.expirationDate > (new Date()))
                    return storageData.data;
            }
            catch (e) {
                console.error(e);
            }
        }

        return null;
    },
    setItem: function (key, data, expirationDate) {
        var jsonData = JSON.stringify({
            data: data,
            expirationDate: expirationDate
        });

        localStorage.setItem(key, jsonData);
    }
}

var setVueDebugInfo = function (element) {
    element.find('*').each(function () {
        var childElement = $(this);
        var isTemplate = this.tagName === 'TEMPLATE';
        var comment = '<!-- ';

        $.each(this.attributes, function () {
            if (this.specified &&
                (this.name.indexOf('v-') === 0 || this.name.charAt(0) === ':')) {

                if (['v-else', 'v-else-if'].indexOf(this.name) !== -1)
                    return;

                var newName = 'debug-' + this.name;
                if (!isTemplate)
                    childElement.attr(newName, this.value);
                else
                    comment += newName + '="' + this.value + '" ';
            }
        });

        if (isTemplate) {
            comment += '-->'
            $(comment).insertBefore(childElement);
        }

    });
}

var getDs = function (element) {
    let vappDs = null;
    try {
        let vappDsStr = element.attr('vapp-ds');
        if (vappDsStr) {
            var txt = document.createElement("textarea");
            txt.innerHTML = vappDsStr;
            vappDs = JSON.parse(txt.value);
            return vappDs;
        }
    }
    catch (e) {
        console.error('vapp-ds', selector, e);
        return null;
    }
}

let vueAppPrivate = {
    components: {},
    isBusy: false,
    modules: null,
    loadedModules: {},
    meta: [],
    load: function () {
        var self = this;

        Object.keys(self.meta).map(function (key) {

            let options = self.meta[key].options;
            let selector = self.meta[key].selector;
            let onCreateApp = self.meta[key].onCreateApp;

            var vueAppsData = [];

            $(selector).each(function (index, element) {
                element = $(element);
                if (element.is('[vapp]'))
                    return;

                if (datasource.env !== 3 || datasource.queryString.debug === '1') {
                    try {
                        //setVueDebugInfo(element);
                    }
                    catch (e) {
                        console.error(e);
                    }
                }

                element.attr('vapp', '');
                let vappDs = getDs(element);
                let moduleInfo = null;
                let moduleName = element.attr('vapp-module');

                var onModuleAssetsReady = function () {
                    let app = Vue.createApp($.extend({}, options), { datasource: vappDs });
                    onCreateApp(app, moduleName);

                    var vueContext = app.mount(element[0]);
                    let appName = element.attr('vapp-name') ?? key;

                    vueAppsData.push({
                        vueContext,
                        appName
                    });
                }
                // $('script[src^="/webpack/common.js"]')
                if (moduleName) {
                    moduleInfo = self.modules[moduleName];
                    if (!moduleInfo)
                        console.error('moduleInfo is missing for: ' + moduleName);

                    if (!self.loadedModules[moduleInfo.js]) {
                        $('head').append('<script src="' + vueApp.clientPath + moduleInfo.js + '"></script>');
                        self.loadedModules[moduleInfo.js] = true;
                    }

                    if (moduleInfo.css && !self.loadedModules[moduleInfo.css]) {
                        $('head').append('<link href="' + vueApp.clientPath + moduleInfo.css + '" rel="stylesheet" type="text/css">');
                        self.loadedModules[moduleInfo.css] = true;
                    }

                    $ready(() => {
                        return self.components[moduleName];
                    }, onModuleAssetsReady)
                }
                else {
                    onModuleAssetsReady();
                }

            });

            vueAppsData.map((i) => {
                if (!vueApp.apps[i.appName]) {
                    vueApp.apps[i.appName] = [];
                }

                if (!Array.isArray(vueApp.apps[i.appName]))
                    vueApp.apps[i.appName] = [vueApp.apps[i.appName]];

                vueApp.apps[i.appName].push(i.vueContext);
            })

            Object.keys(vueApp.apps).map((key) => {
                if (vueApp.apps[key].length === 1)
                    vueApp.apps[key] = vueApp.apps[key][0];
            });
        });
    },
}

window.vueApp = {
    addComponent: function (component) {
        let file = component.__file;
        let fileArr = file.split('/');
        let moduleName = fileArr[1];
        if (moduleName == 'modules')
            moduleName = fileArr[2];

        let moduleComponents = vueAppPrivate.components[moduleName];
        if (!moduleComponents)
            moduleComponents = vueAppPrivate.components[moduleName] = [];

        if (!component.name) {
            console.error('component name is missing', component)
            return;
        }

        moduleComponents.push(component);
    },
    clientPath: datasource.clientPath || '/webpack/',
    apps: null,
    load: function () {
        var self = this;
        if (!this.apps)
            this.apps = {};

        if (!vueAppPrivate.modules) {
            if (vueAppPrivate.isBusy)
                return;

            vueAppPrivate.isBusy = true;

            axios.get(self.clientPath + 'modules.json?v=' + (new Date() * 1)).then((response) => {
                vueAppPrivate.isBusy = false;
                vueAppPrivate.modules = response.data;
                self.load();
            }).catch((e) => {
                vueAppPrivate.isBusy = false;
            })

            return;
        }

        vueAppPrivate.load();
    },
    reset: function () {
        this.apps = {};
    },
    set: function (key, selector, options, onCreateAppMethod) {
        if (['set', 'options', 'copy'].indexOf(key) > -1)
            throw key + ' is not allowed';

        if (vueAppPrivate.meta[key])
            throw key + ' is already defined';

        let addComponents = function (componentsDict, app, moduleName) {
            if (!moduleName)
                return;

            let commonComponents = vueAppPrivate.components[moduleName];
            if (commonComponents) {
                commonComponents.map((component) => {
                    if (componentsDict[component.name]) {
                        console.error('component name already exists', component.name, moduleName)
                        return;
                    }

                    componentsDict[component.name] = true;
                    app.component(
                        component.name,
                        component
                    )
                });
            }
        }

        let onCreateApp = function (app, moduleName) {

            let componentsDict = {};

            addComponents(componentsDict, app, 'common');
            addComponents(componentsDict, app, moduleName);

            onCreateAppMethod(app);
        }


        vueAppPrivate.meta[key] = {
            key,
            selector,
            options,
            onCreateApp
        };
    },
    copy: function (obj) {
        return JSON.parse(JSON.stringify(obj))
    }
};

app.api = {};
app.api.createQueryString = function (qsData) {
    var qs = '';

    for (var field in qsData) {
        if (qsData[field]) {
            if (qs)
                qs += '&';

            qs += field + '=' + encodeURIComponent(qsData[field]);
        }
    }

    return qs;
}
app.api.parseQueryString = function (url) {

    url = url || location.href;
    var queryString = {};

    var urlArr = url.split('?');
    if (urlArr.length > 1) {
        var qsArr = urlArr[1].split('#')[0].split('&');

        for (var i = 0; i < qsArr.length; i++) {
            var paramArr = qsArr[i].split('=');

            queryString[paramArr[0].toLowerCase()] = paramArr[1] ? decodeURIComponent(paramArr[1]) : paramArr[1];
        }
    }

    return queryString;
}

datasource.queryString = app.api.parseQueryString();

app.api.getQueryChar = function (url) {
    if (url.indexOf('?') > -1)
        return '&';

    return '?';
}

var getUrl = function (url, qsData, disableBrowserCache) {
    if (disableBrowserCache)
        url += app.api.getQueryChar(url) + 't=' + (+new Date());

    if (qsData) {
        url += app.api.getQueryChar(url) + app.api.createQueryString(qsData);
    }

    return url;
}

app.api.get = function (url, qsData) {
    url = getUrl(url, qsData, true);

    return axios.get(url);
};

app.api.post = function (url, qsData, data) {
    url = getUrl(url, qsData);

    return axios.post(url, data);
};

app.showLoader = function () {
    if ($('#loader').length === 0)
        $('body').append('<div id="loader" role="alert"></div>');

    $('#loader').show();
};

app.hideLoader = function () {
    $('#loader').hide();
};

app.showLoaderTop = function () {
    try {
        top.app.showLoader();
    } catch (e) {
        app.showLoader();
    }
};

app.hideLoaderTop = function () {
    try {
        top.app.hideLoader();
    } catch (e) {
        app.hideLoader();
    }
};