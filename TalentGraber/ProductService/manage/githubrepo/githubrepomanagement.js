define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var list = require("common/listview/listview");
    var githubRepoItem = require("manage/githubrepo/githubrepo-item");

    var githubRepoModule = {};

    var tmplId = githubRepoModule.tmplId = "githubrepomanagement-tmpl";
    var itemTmplId = githubRepoModule.itemTmplId = "githubrepo-item-tmpl";

    var tmpl = require("text!manage/githubrepo/githubrepomanagement.html");
    util.loadTmpl(tmplId, tmpl);

    var itemTmpl = require("text!manage/githubrepo/githubrepo-item.html");
    util.loadTmpl(itemTmplId, itemTmpl);

    var css = require("text!manage/githubrepo/githubrepomanagement.css");
    util.loadCss(css);

    githubRepoModule.GithubRepoListViewModel = function () {
        var self = this;
        self.tips = ko.observable("");
        this.listTmplId = list.tmplId;
        this.listViewModel = new list.ListViewModel({
            url: util.productEndpoint + "odata/GithubRepoes",
            headers: [{
                name: "Id"
            }, {
                name: "Url"
            }, {
                name: "Details"
            }],
            tags: [{
                "name": "All",
                "filter": ""
            }],
            itemTmplId: itemTmplId,
            DataItemViewModel: githubRepoItem.GithubRepoItemViewModel
        });
    };

    githubRepoModule.GithubRepoListViewModel.prototype.addGithubRepo = function () {
        window.location.href = "#githubrepo-add";
    };

    githubRepoModule.GithubRepoListViewModel.prototype.refresh = function () {
        this.listViewModel.refresh();
    };

    githubRepoModule.GithubRepoListViewModel.prototype.cancel = function () {
        this.listViewModel.cancel();
    };

    return githubRepoModule;
});