module.exports = api => {
    api.cache(true);

    return {
        plugins: ["transform-remove-strict-mode"],
        presets: [
            //[
            //    "@babel/preset-env",
            //    {
            //        useBuiltIns: "entry",
            //        targets: { ie: "11" }
            //    }
            //],
            ["vue"]
        ]
    }
}