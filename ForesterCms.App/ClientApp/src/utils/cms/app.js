import * as Vue from 'vue';
import * as jQuery from 'jquery';

window.$ = window.jQuery = jQuery;

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

window.vueApp = {
    set: function (key, selector, options) {

        if (['set', 'options', 'copy'].indexOf(key) > -1)
            throw key + ' is not allowed';

        if (options)
            this.options[key] = $.extend({}, options);
        else
            options = $.extend({}, this.options[key]);

        var vueContexts = [];

        $(selector).each(function (index, element) {
            element = $(element);
            if (element.is('[vued]'))
                return;

            if (datasource.env !== 3 || datasource.queryString.debug === '1') {
                try {
                    //setVueDebugInfo(element);
                }
                catch (e) {
                    console.error(e);
                }
            }

            element.attr('vued', '');
            //options.el = element[0];
            //var vueContext = new Vue(options);
            debugger
            var vueContext = Vue.createApp(options).mount(element[0]);
            vueContexts.push(vueContext);
        });

        if (!vueApp[key]) {
            vueApp[key] = [];
        }

        if (!Array.isArray(vueApp[key]))
            vueApp[key] = [vueApp[key]];

        vueContexts.map(function (i) {
            vueApp[key].push(i);
        })

        if (vueApp[key].length === 1)
            vueApp[key] = vueApp[key][0];
    },
    copy: function (obj) {
        return JSON.parse(JSON.stringify(obj))
    },
    options: {}
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
    //url = url +
    //    '?bId=' + (datasource.bid || '') +
    //    '&nsId=' + (datasource.nsid || '') +
    //    '&eId=' + (datasource.eid || '') +
    //    '&lcId=' + (datasource.lcid || '');



    //if (datasource.queryString.preview)
    //    url += '&preview=' + datasource.queryString.preview;

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