define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");

    var list = {};
    var tmplId = list.tmplId = "listview-tmpl";

    var tmpl = require("text!common/listview/listview.html");
    util.loadTmpl(list.tmplId, tmpl);

    var css = require("text!common/listview/listview.css");
    util.loadCss(css);

    list.ListViewModel = function (option) {
        var self = this;
        this.url = option.url;
        this.searchurl = util.productEndpoint + "/api/Search";
        this.headers = option.headers;
        this.itemTmplId = option.itemTmplId || "";
        this.tags = option.tags || [];
        this.DataItemViewModel = option.DataItemViewModel || Object;
        this.itemList = ko.observableArray([]);

        if (option.top == undefined) {
            this.top = 100;
        } else {
            this.top = option.top;
        }
        if (option.skip = undefined) {
            this.skip = 0;
            this.skip = option.skip;
        }
        this.filter = ko.observable(option.filter);
        this.searchFieldText = ko.observable("");

        this.loading = ko.observable(false);
        this.error = ko.observable("");
        this.xhr = null;

        this.filter.subscribe(function () {
            self.Jump();
        });

    };

    list.ListViewModel.prototype.enterKeyPressed = function () {
        var self = this;
        self.Jump();
    };

    list.ListViewModel.prototype.Jump = function () {
        var self = this;
        //TODO if there any not same, do the real jump.
        var currentQueryObject = self.getQueryObjectFromModel();

        var queryStrInUrl = window.location.hash;
        var newHash = "#" + util.getHash() + currentQueryObject.queryString;
        if (queryStrInUrl != newHash) {
            window.location.href = newHash;
        }
    };

    list.ListViewModel.prototype.getQueryObjectFromModel = function () {
        var self = this;
        var queryObject = { "url": self.url, "queryString": "" }
        var queryStr = "?";
        var fullTextSearch = self.searchFieldText();
        if (fullTextSearch != undefined && fullTextSearch != "") {
            queryObject.url = self.searchurl;
        }
        queryStr += "$top=" + this.top;
        queryStr += "&$skip=" + this.skip;
        var currentFilter = self.filter();
        if (currentFilter != undefined && currentFilter != "") {
            queryStr += "&$filter=" + currentFilter;
        }

        queryStr += "&$fullTextQuery=" + fullTextSearch;
        queryObject.queryString = queryStr;
        return queryObject;
    };

    list.ListViewModel.prototype.parseFromQueryStr = function () {
        var self = this;
        var top = (util.getQuery("$top") || 10);
        self.top = parseInt(top);
        var skip = (util.getQuery("$skip") || 0);
        self.skip = parseInt(skip);
        var fullTextQuery = util.getQuery("$fullTextQuery");
        self.searchFieldText(fullTextQuery);
        self.filter(encodeURI(util.getQuery("$filter")));
    }

    list.ListViewModel.prototype.search = function () {
        console.dir("search");
        var self = this;
        self.skip = 0;
        self.Jump();
    }

    list.ListViewModel.prototype.refresh = function () {
        console.dir("refresh");
        var self = this;
        this.loading(true);

        // parse the parameters in the url first.
        self.parseFromQueryStr();
        var currentFilter = util.getQuery("$filter");
        currentFilter = decodeURI(currentFilter);
        var filterCreteria = currentFilter.split("and");
        console.dir(filterCreteria);
        var queryStrings = [];
        for (var filterItemStr in filterCreteria) {
            var filterItemArr = filterItemStr.split(" ");
            var queryString = {
                "DefaultField": filterItemArr[0],
                "Operation": filterItemArr[1],
                "Query": filterItemArr[2]
            };
            queryStrings.push(queryString);
        }
        var queryObject = this.getQueryObjectFromModel();

        var fullTextQuery = util.getQuery("$fullTextQuery");
        if (fullTextQuery != undefined && fullTextQuery != "") {
            var currentHash = util.getHash();
            var searchQueryBody = {
                "ObjectType": "",
                "FullText": self.searchFieldText(),
                "From": self.skip,
                "Size": self.top,
                "QueryStrings": queryStrings
            };

            switch (currentHash) {
                case "talentmanagement":
                    searchQueryBody.ObjectType = "TalentCandidate";
                    break;
                default:
                    break;
            }

            util.ajax({
                url: queryObject.url,
                type: "POST",
                data: JSON.stringify(searchQueryBody),
                success: function (data) {
                    self.loading(false);
                    self.itemList.removeAll();
                    if (data) {
                        for (var i = 0; i < data.length; i++) {
                            self.itemList.push(new self.DataItemViewModel(data[i]));
                        }
                    } else {
                        for (var i = 0; i < data.length; i++) {
                            self.itemList.push(new self.DataItemViewModel(data[i]));
                        }
                    }
                    console.dir(data);
                },
                error: function (e) {
                    if (e.status !== 0) {
                        self.loading(false);
                        console.log(e);
                        self.error("服务暂不可用, 请稍后再试");
                    }
                }
            });

        } else {
            util.ajax({
                url: queryObject.url + queryObject.queryString,
                type: "GET",
                success: function (data) {
                    self.loading(false);
                    if (!data) {
                        console.log(data);
                        throw "Service returned invalid data";
                    }
                    self.itemList.removeAll();
                    if (data.value) {
                        for (var i = 0; i < data.value.length; i++) {
                            self.itemList.push(new self.DataItemViewModel(data.value[i]));
                        }
                    } else {
                        for (var i = 0; i < data.length; i++) {
                            self.itemList.push(new self.DataItemViewModel(data[i]));
                        }
                    }
                },
                error: function (e) {
                    if (e.status !== 0) {
                        self.loading(false);
                        console.log(e);
                        self.error("服务暂不可用, 请稍后再试");
                    }
                }
            });
        }
    };

    list.ListViewModel.prototype.nextPage = function (data, event) {
        this.skip += this.top;
        var queryObject = this.getQueryObjectFromModel();
        window.location.href = "#" + util.getHash() + queryObject.queryString;
    }

    list.ListViewModel.prototype.previousPage = function (data, event) {
        this.skip -= this.top;
        var queryObject = this.getQueryObjectFromModel();
        window.location.href = "#" + util.getHash() + queryObject.queryString;
    }

    list.ListViewModel.prototype.cancel = function () {

    }
    return list;
});