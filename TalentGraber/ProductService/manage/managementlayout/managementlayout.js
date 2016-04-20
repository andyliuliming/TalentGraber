define(function (require) {
    var layout = {};

    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");

    var tmpl = require("text!manage/managementlayout/managementlayout.html");
    util.loadTmpl("layout-tmpl", tmpl);
    var css = require("text!manage/managementlayout/managementlayout.css");
    util.loadCss(css);

    function LayoutViewModel() {
        var self = this;
        this.userName = ko.observable("");
        this.LayoutTitle = ko.observable("");
        this.views = [];
        this.currView = ko.observable(null);
        this.path = ko.computed(function () {
            if (self.currView()) {
                var hash = self.currView().hash;
                var path = [];
                var items = hash.split("/");
                var currHash = "";
                for (var i = 0; i < items.length; i++) {
                    currHash += items[i];
                    path.push({
                        hash: currHash,
                        title: self.getView(currHash).title
                    });
                    if (i != items.length - 1) {
                        path[i].title += " >>";
                    }
                    currHash += "/";
                }
                return path;
            }
        });
    };

    LayoutViewModel.prototype.getUser = function () {
        var self = this;
        self.userName($.cookie("userName"));
    };

    LayoutViewModel.prototype.logout = function () {
        $.removeCookie("token");
        $.removeCookie("refreshToken");
        $.removeCookie("userName");
        util.checkAccess();
    };

    LayoutViewModel.prototype.getView = function (hash) {
        for (var i = 0; i < this.views.length; i++) {
            if (this.views[i].hash === hash) {
                return this.views[i];
            }
        }
        return null;
    };

    LayoutViewModel.prototype.updateView = function (hash) {
        var view = this.getView(hash);
        if (view) {
            var currView = this.currView();
            if (currView) {
                currView.model.cancel && currView.model.cancel();
            }
            view.model.refresh && view.model.refresh();
            this.currView(view);
        } else {
            throw "Unkown view: " + hash;
        }
    };

    layout.vm = new LayoutViewModel();

    layout.init = function () {
        console.log("Init layout");
        layout.vm.getUser();

        $(window).bind('hashchange', function (event) {
            layout.vm.updateView(util.getHash());
        });

        var hash = util.getHash();
        if (!hash) {
            hash = layout.vm.views[0].hash;
            window.location.hash = hash;
        }
        layout.vm.updateView(hash);
        ko.applyBindings(layout.vm);
    };

    layout.route = function (hash, title, tmpl, model, parent) {
        layout.vm.views.push({
            hash: hash,
            title: title,
            tmpl: tmpl,
            model: model,
            parent: parent,
        })
    };

    return layout;
});