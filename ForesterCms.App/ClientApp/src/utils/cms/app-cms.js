

app.api.getCms = function (url, qsData) {
    return app.api.get(url, qsData);
}

app.api.postCms = function (url, qsData, data) {
    return app.api.post(url, qsData, data);
}