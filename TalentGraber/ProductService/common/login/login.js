(function () {

    require.config({
        baseUrl: '..',
        paths: {
            jquery: "../lib/jquery",
            jcookie: "../lib/jquery.cookie",
            knockout: "../lib/knockout",
            text: "../lib/text"
        }
    });

    require(["jquery",
        "knockout",
        "text",
        "../../common/shared/util"], function ($, ko, text, util) {
            //TODO this is not a full returnurl.
            var returnurl = $.cookie("returnuri");
            function LoginViewModel() {
                var self = this;
                this.username = ko.observable("").extend({ required: "Please type in username" });
                this.password = ko.observable("").extend({ required: "Please type in password" });
                this.rememberMe = ko.observable(false);
                this.error = ko.observable();
                this.enableSubmit = ko.observable(true);
            }

            LoginViewModel.prototype.login = function (self, event) {
                //Trigger validateion
                self.username.valueHasMutated()
                self.password.valueHasMutated();
                if (self.username.hasError() || self.password.hasError()) {
                    return;
                }

                self.enableSubmit(false);
                util.ajax({
                    url: util.productEndpoint + "/odata/UserTokens",
                    type: "POST",
                    data: JSON.stringify({
                        UserName: self.username(),
                        Password: self.password()
                    }),
                    success: function (data) {
                        self.enableSubmit(true);
                        if (data && data.Token) {
                            $.cookie("token", data.Token, { path: '/', expires: 1 });
                            $.cookie("userName", data.UserName, { path: '/', expires: 1 });
                            window.location.href = returnurl || "/";
                        }
                        else {
                            throw "Server returns an invalid token";
                        }
                    },
                    error: function (e) {
                        self.enableSubmit(true);
                        console.log(e);
                        if (e.status == 401 || e.status == 404) {
                            self.error("Username or password wrong.");
                        } else {
                            self.error("one error occured.");
                        }
                    }
                });
                return false;
            }
            var vm = new LoginViewModel();
            ko.applyBindings(vm);
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    vm.login(vm);
                }
            });
        });
})()