(function () {

    require.config({
        baseUrl: '/',
        paths: {
            jquery: "lib/jquery",
            jcookie: "lib/jquery.cookie",
            ko: "lib/knockout",
            text: "lib/text"
        }
    });

    require(["jquery",
        "ko",
        "text",
        "common/shared/util"],
        function ($, ko, text, util) {
            var ChangePassViewModel = function () {
                var self = this;
                this.phone = ko.observable().extend({ required: "请输入手机号" });
                this.verifyCode = ko.observable().extend({ required: "请输入验证码" });
                this.password = ko.observable().extend({ required: "请输入密码" });
                this.confirm = ko.observable().extend({ required: "请输入确认密码", match: { ref: this.password, msg: "两次输入的密码不一致" } });
                this.error = ko.observable();
                this.fetchBtnText = ko.observable("获取");
                this.enableFetch = ko.observable(true);
                this.enableSubmit = ko.observable(true);
            };

            ChangePassViewModel.prototype.getVerifyCode = function (self) {
                //TODO: add mockup
                self.phone.valueHasMutated();
                if (self.phone.hasError()) {
                    return
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
            ChangePassViewModel.prototype.changePass = function (self, event) {
                //Trigger validation
                self.phone.valueHasMutated();
                self.verifyCode.valueHasMutated();
                self.password.valueHasMutated();
                self.confirm.valueHasMutated();

                if (self.phone.hasError() || self.verifyCode.hasError() ||
                    self.password.hasError() || self.confirm.hasError()) {
                    return;
                }

                self.enableSubmit(false);
                //TODO: add mock
                util.ajax({
                    url: util.productEndpoint + "/odata/Users(-1)",
                    type: "PATCH",
                    headers: {
                        AuthorizationType: "VerificationCode",
                        Authorization: self.verifyCode()
                    },
                    data: JSON.stringify({
                        Cellphone: self.phone(),
                        Password: self.password()
                    }),
                    success: function (data) {
                        self.enableSubmit(true);
                        if (!data) {
                            self.error("修改密码失败,请稍后再试");
                            console.log("Server returns invalid data.");
                            console.log(data);
                        }
                        //TODO fetch token for user and pass thru return url
                        window.location.href = "/common/login/login.html";
                    },
                    error: function (e) {
                        self.enableSubmit(true);
                        console.log("Failed to verification code: " + e.status);
                        self.error("修改密码失败,请稍后再试");
                    }
                });
                return false;
            }
            var vm = new ChangePassViewModel();
            ko.applyBindings(vm);
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    vm.changePass(vm);
                }
            });
        });

})()