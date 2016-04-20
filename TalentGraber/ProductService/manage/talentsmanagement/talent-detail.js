define(function (require) {
    var $ = require("jquery");
    var jcookie = require("jcookie");
    var ko = require('knockout');
    var util = require("common/shared/util");
    var komapping = require('komapping');
    var css = require("text!manage/talentsmanagement/talent-detail.css")
    util.loadCss(css);
    var talentDetail = {};

    var tmplId = talentDetail.tmplId = "talent-detail-tmpl";
    var tmpl = require("text!manage/talentsmanagement/talent-detail.html");
    util.loadTmpl(tmplId, tmpl);

    talentDetail.TalentDetailViewModel = function () {
        var self = this;
        self.loading = ko.observable(true);
        self.error = ko.observable("");
        self.contributions = ko.observableArray([]);

        self.loading(false);
    };

    talentDetail.TalentDetailViewModel.prototype.refresh = function () {
        var self = this;
        
        var Id = util.getQuery("Id");
        util.ajax({
            url: util.productEndpoint + "/odata/ContributerToRepoes?$filter=(TalentCandidateId eq " + Id+")",
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
    return talentDetail;
})