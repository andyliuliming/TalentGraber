define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/githubrepo/githubrepo-add.css")
    util.loadCss(css);
    var githubRepoAdd = {};

    var tmplId = githubRepoAdd.tmplId = "githubrepo-add-tmpl";
    var tmpl = require("text!manage/githubrepo/githubrepo-add.html");
    util.loadTmpl(tmplId, tmpl);

    githubRepoAdd.GithubRepoAddViewModel = function () {
        var self = this;
        self.loading = ko.observable(false);
        self.error = ko.observable("");
        self.enableSave = ko.observable(false);
        self.error = ko.observable("");
        self.githubRepo = komapping.fromJS({
            Id: 0,
            Url: ""
        });

        self.enableSave(true);
    };

    githubRepoAdd.GithubRepoAddViewModel.prototype.saveGithubRepo = function () {
        console.dir("save github repo");
        var self = this;
        self.enableSave(false);
        var githubRepoObj = komapping.toJS(self.githubRepo);
        util.ajax({
            url: util.productEndpoint + "odata/GithubRepoes",
            type: "POST",
            data: JSON.stringify(githubRepoObj),
            success: function (data) {
                self.enableSave(true);
                self.error("Add sucessfually.");
            },
            error: function (e) {
                self.enableSave(true);
                self.error("Add failed, probably the github exists already.");
                console.log("Failed to submit github repo code: " + e.status);
            }
        });
    };
    return githubRepoAdd;
})