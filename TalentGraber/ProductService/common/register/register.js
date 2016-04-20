(function () {
    var endpoint = "";
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
        "shared/util"], function ($, ko, text, util) {
        var RegisterViewModel = function () {
            var self = this;
            this.username = ko.observable().extend({ required: "请输入用户名" });
            this.phone = ko.observable().extend({ required: "请输入手机号" });
            this.verifyCode = ko.observable().extend({ required: "请输入验证码" });
            this.password = ko.observable().extend({ required: "请输入密码" });
            this.confirm = ko.observable().extend({ required: "请输入确认密码", match: { ref: this.password, msg: "两次输入的密码不一致" } });
            this.error = ko.observable();
            this.fetchBtnText = ko.observable("获取");
            this.enableFetch = ko.observable(true);
            this.enableSubmit = ko.observable(true);
        }

        RegisterViewModel.prototype.getVerifyCode = function (self, event) {
            self.phone.valueHasMutated();
            if (self.phone.hasError()) {
                return;
            }
            self.enableFetch(false);
            util.countdown(60, function (count) {
                if (count > 1) {
                    self.fetchBtnText("获取(" + count + ")");
                }
            }, function () {
                self.enableFetch(true);
                self.fetchBtnText("获取");
            });

            util.ajax({
                url: util.productEndpoint + "/odata/VerificationCodes",
                type: "POST",
                data: JSON.stringify({
                    "Cellphone": self.phone()
                }),
                success: function (data) {
                    if (!data) {
                        self.error("无法获得验证码,请稍后再试");
                        console.log("Server returns invalid data.");
                        console.log(data);
                    }
                },
                error: function (e) {
                    console.log("Failed to get verification code: " + e.status);
                    self.error("无法获得验证码,请稍后再试");
                }
            });
        };

        RegisterViewModel.prototype.register = function (self, event) {
            //Trigger validation
            self.username.valueHasMutated();
            self.phone.valueHasMutated();
            self.verifyCode.valueHasMutated();
            self.password.valueHasMutated();
            self.confirm.valueHasMutated();

            if (self.username.hasError() || self.phone.hasError() || self.verifyCode.hasError() ||
                self.password.hasError() || self.confirm.hasError()) {
                return;
            }

            self.enableSubmit(false);
            //TODO: add mock
            util.ajax({
                url: util.productEndpoint + "/odata/Users",
                type: "POST",
                headers: {
                    AuthorizationType: "VerificationCode",
                    Authorization: self.verifyCode()
                },
                data: JSON.stringify({
                    UserName: self.username(),
                    Cellphone: self.phone(),
                    Password: self.password()
                }),
                success: function (data) {
                    self.enableSubmit(true);
                    if (!data) {
                        self.error("注册失败,请稍后再试");
                        console.log("Server returns invalid data.");
                        console.log(data);
                    }
                    //TODO fetch token for user and pass thru return url
                    window.location.href = "/common/login/login.html";
                },
                error: function (e) {
                    self.enableSubmit(true);
                    if (e.status == 401) {
                        console.log("Wrong verification code: " + e.status);
                        self.error("验证码错误");
                    } else {
                        self.error("注册失败,请稍后再试");
                    }
                }
            });
            return false;
        }
        var vm = new RegisterViewModel();
        ko.applyBindings(vm);
        $(document).keypress(function (e) {
            if (e.which == 13) {
                vm.register(vm);
            }
        });
        $('.form').submit(function (event) { event.preventDefault(); });
    });
})()