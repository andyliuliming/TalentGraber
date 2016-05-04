(function () {
    require.config({
        baseUrl: '.',
        paths: {
            knockout: "./lib/knockout",
            komapping: "./lib/knockout.mapping-latest",
            text: "./lib/text",
            ztree: "lib/jquery.ztree.all-3.5",
            jquery: "./lib/jquery",
            jcookie: "./lib/jquery.cookie"
        },
        shim: {
            "jcookie": { "deps": ['jquery'] },
            "ztree": { "deps": ['jquery'] }
        }
    });

    require(["jquery",
        "common/shared/util",
        "jcookie",
        "ztree",
        "knockout",
        "komapping",
        "text",
        "manage/managementlayout/managementlayout",
        "manage/talentsmanagement/talentmanagement",
        "manage/githubrepo/githubrepomanagement",
        "manage/githubrepo/githubrepo-add",
        "manage/talentsmanagement/talent-detail",
        "manage/githubrepo/githubrepo-detail",
        "manage/githubaccount/githubaccountmanagement",
        "manage/githubaccount/githubaccount-add"
        ],
        function ($,
            util,
            jcookie,
            ztree,
            ko,
            komapping,
            text,
            managementlayout,
            talentmanagement,
            githubrepo,
            githubrepoadd,
            talentdetail,
            githubdetail,
            githubaccount,
            githubaccountadd) {

            util.checkAccess("/common/login/login.html");

            managementlayout.vm.LayoutTitle("Talent Management");
            managementlayout.route("githubrepomanagement", "Github Repo", githubrepo.tmplId, new githubrepo.GithubRepoListViewModel);
            managementlayout.route("github-detail", "Github Detail", githubdetail.tmplId, new githubdetail.GithubDetailViewModel, "githubrepomanagement");
            managementlayout.route("githubrepo-add", "Add Repo", githubrepoadd.tmplId, new githubrepoadd.GithubRepoAddViewModel(), "githubrepomanagement");
            managementlayout.route("talentmanagement", "Talents", talentmanagement.tmplId, new talentmanagement.TalentListViewModel);
            managementlayout.route("talent-detail", "Talent Detail", talentdetail.tmplId, new talentdetail.TalentDetailViewModel, "talentmanagement");

            managementlayout.route("githubaccountmanagement", "Github Account", githubaccount.tmplId, new githubaccount.GithubAccountListViewModel);
            managementlayout.route("githubaccount-add", "Add Github Account", githubaccountadd.tmplId, new githubaccountadd.GithubAccountAddViewModel(), "githubaccountmanagement");
            managementlayout.init();

        });
})()