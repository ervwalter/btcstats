/// <reference path="jquery-1.5.1.js" />
/// <reference path="knockout-1.3.0beta.js" />

viewModel = {
    pool: ko.observable(window.location.search.substring(1)),
    key: ko.observable(""),
    statsKey: ko.observable("")
};

viewModel.keyLabel = ko.dependentObservable(function () {
    var pool = viewModel.pool();
    if (pool === "eligius" || pool == "p2pool") {
        return "Payment Address";
    } else {
        return "API Key";
    }
});

viewModel.url = ko.dependentObservable(function () {
    var statsKey = viewModel.statsKey();
    if (statsKey !== "") {
        return "http://btcstats.net/sig/" + statsKey;
    } else {
        return "";
    }
});

viewModel.avatarUrl = ko.dependentObservable(function () {
    var statsKey = viewModel.statsKey();
    if (statsKey !== "") {
        return "http://btcstats.net/avatar/" + statsKey;
    } else {
        return "";
    }
});

viewModel.bbCode = ko.dependentObservable(function () {

    var pool = viewModel.pool();
    var image = viewModel.url();
    var link;

    if (pool === "eligius") {
        link = "http://eligius.st";
    } else if (pool === "p2pool") {
        link = "http://p2pool.info";
    } else if (pool === "ozcoin") {
        link = "https://ozcoin.net/";
    } else if (pool === "emc") {
        link = "https://eclipsemc.com/";
    } else if (pool === "slush") {
        link = "https://mining.bitcoin.cz/";
    } else {
        return "";
    }

    return "[url=" + link + "][img]" + image + "[/img][/url]";

});

viewModel.generate = function () {
    var pool = viewModel.pool();
    var key = viewModel.key();
    if (pool !== "" && key !== "") {
        var getUrl = "/sig/getkey/" + pool + "/" + key;
        $.get(getUrl, null, function (result) {
            if (result !== "") {
                viewModel.statsKey(result);
            }
        }, "text");
    }
};

$(function () {
    viewModel.pool.subscribe(function () {
        viewModel.key("");
        viewModel.statsKey("");
    });
    ko.applyBindings(viewModel);
});

