define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/talentsmanagement/talent-item.css")
    util.loadCss(css);
    var talentItem = {};

    talentItem.TalentItemViewModel = function (data) {
        var self = this;
        self.error = ko.observable("");
        
        self.talentCandidate = komapping.fromJS(data);
    };


    return talentItem;
})