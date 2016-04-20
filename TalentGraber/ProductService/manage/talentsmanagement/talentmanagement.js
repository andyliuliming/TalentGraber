define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var list = require("common/listview/listview");
    var talentItem = require("manage/talentsmanagement/talent-item");

    var talentModule = {};

    var tmplId = talentModule.tmplId = "talentmanagement-tmpl";
    var itemTmplId = talentModule.itemTmplId = "talent-item-tmpl";

    var tmpl = require("text!manage/talentsmanagement/talentmanagement.html");
    util.loadTmpl(tmplId, tmpl);

    var itemTmpl = require("text!manage/talentsmanagement/talent-item.html");
    util.loadTmpl(itemTmplId, itemTmpl);

    var css = require("text!manage/talentsmanagement/talentmanagement.css");
    util.loadCss(css);

    talentModule.TalentListViewModel = function () {
        var self = this;
        self.tips = ko.observable("");
        this.listTmplId = list.tmplId;
        this.listViewModel = new list.ListViewModel({
            url: util.productEndpoint + "odata/TalentCandidates",
            headers: [{
                name: "Id"
            }, {
                name: "Name"
            }, {
                name: "Company"
            }, {
                name: "Location"
            }, {
                name: "TotalCommits"
            }, {
                name: "Details"
            }],
            tags: [{
                "name": "All",
                "filter": ""
            }],
            itemTmplId: itemTmplId,
            DataItemViewModel: talentItem.TalentItemViewModel
        });
    };

    talentModule.TalentListViewModel.prototype.refresh = function () {
        this.listViewModel.refresh();
    };

    talentModule.TalentListViewModel.prototype.cancel = function () {
        this.listViewModel.cancel();
    };

    return talentModule;
});