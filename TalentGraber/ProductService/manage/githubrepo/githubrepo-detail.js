define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/githubrepo/githubrepo-detail.css")
    util.loadCss(css);
    var githubRepoDetail = {};

    var tmplId = githubRepoDetail.tmplId = "githubrepo-detail-tmpl";
    var tmpl = require("text!manage/githubrepo/githubrepo-detail.html");
    util.loadTmpl(tmplId, tmpl);

    githubRepoDetail.GithubDetailViewModel = function () {
        var self = this;
        self.loading = ko.observable(true);
        self.error = ko.observable("");
        self.contributions = ko.observableArray([]);

        self.loading(false);
    };

    githubRepoDetail.GithubDetailViewModel.prototype.refresh = function () {
        var self = this;
        var Id = util.getQuery("Id");
        util.ajax({
            url: util.productEndpoint + "/odata/ContributerToRepoes?$filter=(RepoId eq " + Id + ")",
            type: "GET",
            success: function (contributions) {
                self.contributions.removeAll();
                contributions.value.forEach(function (contribution) {
                    self.contributions.push(contribution);
                });
                self.loading(false);
            },
            error: function (e) {
                console.log(e);
                self.error("could not get the contribution information.")
                self.loading(false);
            }
        });
    };
    return githubRepoDetail;
})