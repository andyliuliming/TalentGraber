define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/githubrepo/githubrepo-item.css")
    util.loadCss(css);
    var githubRepoItem = {};

    githubRepoItem.GithubRepoItemViewModel = function (data) {
        var self = this;
        self.error = ko.observable("");
        self.githubRepo = komapping.fromJS(data);
    };

    return githubRepoItem;
})