define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/githubaccount/githubaccount-item.css")
    util.loadCss(css);
    var githubAccountItem = {};

    githubAccountItem.GithubAccountItemViewModel = function (data) {
        var self = this;
        self.error = ko.observable("");
        self.githubAccount = komapping.fromJS(data);
    };

    return githubAccountItem;
})