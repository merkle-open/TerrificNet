var Common = (function () {
    function Common() {
    }
    Common.parseQueryString = function (href) {
        var vars = {}, hash;
        var hashes = href.slice(href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars[hash[0]] = hash[1];
        }
        return vars;
    };
    return Common;
})();
