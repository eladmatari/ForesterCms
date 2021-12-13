import upperFirst from 'lodash/upperFirst'
import camelCase from 'lodash/camelCase'

export function setAppComponents(app, testFileName) {
    const requireComponent = require.context(
        // The relative path of the components folder
        '../../',
        // Whether or not to look in subfolders
        true,
        // The regular expression used to match base component filenames
        /.vue$/
    )

    requireComponent.keys().forEach(fileName => {
        // Get component config
        const componentConfig = requireComponent(fileName)
        if (!componentConfig.default) {
            console.log(fileName, 'must export default');
            return;
        }

        if (typeof testFileName == 'function' && !testFileName(fileName))
            return;

        // Get PascalCase name of component
        let componentName = upperFirst(
            camelCase(
                // Gets the file name regardless of folder depth
                fileName
                    .split('/')
                    .pop()
                    .replace(/\.\w+$/, '')
            )
        )

        if (componentConfig.default.name) {
            componentName = componentConfig.default.name;
        }

        app.component(
            componentName,
            componentConfig.default
        )
    })
}