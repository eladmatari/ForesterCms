(function () {

    window.$ready = function (check, onReady, count, maxCount, timeout) {
        if (check()) {
            onReady();
            return;
        }

        maxCount = maxCount || 100;
        count = count || 0;
        timeout = timeout || 50;
        if (maxCount > 0 && count >= maxCount)
            throw '$ready failed ' + onReady.toString();

        setTimeout(function () {
            $ready(check, onReady, count + 1, maxCount, timeout);
        }, timeout);
    }

})();