({
    //appDir: "./js",
    baseUrl: ".",
    name: "management-main",
    out: "management-main-built.js",
    //modules: [
    //    {
    //        name: 'main'
    //    }
    //],
    optimizeAllPluginResources: true,
    removeCombined: false,

    //baseUrl: "./js",
    //name: "main",
    //out:"main-built.js",
    ////   dir: './dist',
    //optimizeAllPluginResources: true,
    //removeCombined: false,

    paths: {
        jquery: "lib/jquery",
        jcookie: "lib/jquery.cookie",
        knockout: "lib/knockout",
        komapping: "lib/knockout.mapping-latest",
        text: "lib/text",
        ztree: "lib/jquery.ztree.all-3.5"
    }
})
