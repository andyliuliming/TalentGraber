define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/githubaccount/githubaccount-add.css")
    util.loadCss(css);
    var githubAccountAdd = {};

    var tmplId = githubAccountAdd.tmplId = "githubaccount-add-tmpl";
    var tmpl = require("text!manage/githubaccount/githubaccount-add.html");
    util.loadTmpl(tmplId, tmpl);

    githubAccountAdd.GithubAccountAddViewModel = function () {
        var self = this;
        self.loading = ko.observable(false);
        self.error = ko.observable("");
        self.enableSave = ko.observable(false);
        self.error = ko.observable("");
        self.githubAccount = komapping.fromJS({
            Id: 0,
            UserName: "",
            Password: ""
        });

        self.enableSave(true);
    };

    githubAccountAdd.GithubAccountAddViewModel.prototype.saveGithubAccount = function () {
        console.dir("save github account");
        var self = this;
        self.enableSave(false);
        var githubAccountObj = komapping.toJS(self.githubAccount);
        util.ajax({
            url: util.productEndpoint + "odata/GithubAccounts",
            type: "POST",
            data: JSON.stringify(githubAccountObj),
            success: function (data) {
                self.enableSave(true);
                self.error("Add sucessfually.");
            },
            error: function (e) {
                self.enableSave(true);
                self.error("Add failed, probably the github account exists already.");
                console.log("Failed to submit github account code: " + e.status);
            }
        });
    };
    return githubAccountAdd;
})