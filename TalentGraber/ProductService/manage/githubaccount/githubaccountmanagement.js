define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var list = require("common/listview/listview");
    var githubAccountItem = require("manage/githubaccount/githubaccount-item");

    var githubAccountModule = {};

    var tmplId = githubAccountModule.tmplId = "githubaccountmanagement-tmpl";
    var itemTmplId = githubAccountModule.itemTmplId = "githubaccount-item-tmpl";

    var tmpl = require("text!manage/githubaccount/githubaccountmanagement.html");
    util.loadTmpl(tmplId, tmpl);

    var itemTmpl = require("text!manage/githubaccount/githubaccount-item.html");
    util.loadTmpl(itemTmplId, itemTmpl);

    var css = require("text!manage/githubaccount/githubaccountmanagement.css");
    util.loadCss(css);

    githubAccountModule.GithubAccountListViewModel = function () {
        var self = this;
        self.tips = ko.observable("");
        this.listTmplId = list.tmplId;
        this.listViewModel = new list.ListViewModel({
            url: util.productEndpoint + "odata/GithubAccounts",
            headers: [{
                name: "Id"
            }, {
                name: "UserName"
            }, {
                name: "Password"
            }],
            tags: [{
                "name": "All",
                "filter": ""
            }],
            itemTmplId: itemTmplId,
            DataItemViewModel: githubAccountItem.GithubAccountItemViewModel
        });
    };

    githubAccountModule.GithubAccountListViewModel.prototype.addGithubAccount = function () {
        window.location.href = "#githubaccount-add";
    };

    githubAccountModule.GithubAccountListViewModel.prototype.refresh = function () {
        this.listViewModel.refresh();
    };

    githubAccountModule.GithubAccountListViewModel.prototype.cancel = function () {
        this.listViewModel.cancel();
    };

    return githubAccountModule;
});