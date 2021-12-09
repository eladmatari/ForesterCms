const fs = require('fs');
const path = require('path');

let getFileExtension = (fileName) => {
    if (!fileName)
        return '';

    var fileArr = fileName.split('.');
    if (fileArr.length <= 1)
        return '';

    return fileArr[fileArr.length - 1].toLowerCase();
}

let getFileSystemInfos = (dirPath, isIncludeSubDirectories, testMethod) => {
    let fileSystemInfos = [];

    fs.readdirSync(dirPath).forEach(file => {
        let fileSystemInfo = {}

        fileSystemInfo.name = file;
        fileSystemInfo.path = path.join(dirPath, file);
        fileSystemInfo.isDirectory = fs.lstatSync(fileSystemInfo.path).isDirectory();
        fileSystemInfo.extension = fileSystemInfo.isDirectory ? null : getFileExtension(file);

        if (testMethod && !testMethod(fileSystemInfo))
            return;

        if (fileSystemInfo.isDirectory && isIncludeSubDirectories) {
            fileSystemInfo.extension = null;
            fileSystemInfo.files = getFileSystemInfos(fileSystemInfo.path, true, testMethod);
        }

        fileSystemInfos.push(fileSystemInfo);
    });

    return fileSystemInfos;
}

let toFlatList = function (fileSystemInfos, list) {
    list = list || [];

    fileSystemInfos.forEach((f) => {
        list.push(f);

        if (f.files) {
            toFlatList(f.files, list);
        }
    });

    return list;
}

let getFileInfos = (dirPath, isIncludeSubDirectories, testMethod) => {
    var items = getFileSystemInfos(dirPath, isIncludeSubDirectories, testMethod);
    items = toFlatList(items).filter((i) => {
        return !i.isDirectory;
    }).map((i) => {
        i.relativePath = i.path.substr(dirPath.length + 1);

        let path = i.path;
        if (path.charAt(path.length - 1) == '\\')
            path = path.substr(0, path.length - 1);

        let pathArr = i.path.split('\\');
        pathArr.pop();
        i.dirPath = pathArr.join('\\');

        return i;
    });

    return items;
}

let getDirInfos = (dirPath, isIncludeSubDirectories, testMethod) => {
    var items = getFileSystemInfos(dirPath, isIncludeSubDirectories, testMethod);
    items = toFlatList(items).map((i) => {
        i.relativePath = i.path.substr(dirPath.length + 1);
        return i;
    }).filter((i) => {
        return i.isDirectory;
    });

    return items;
}

let test = () => {

}

//test();

exports.getFileExtension = getFileExtension;
exports.getFileSystemInfos = getFileSystemInfos;
exports.getFileInfos = getFileInfos;
exports.getDirInfos = getDirInfos;