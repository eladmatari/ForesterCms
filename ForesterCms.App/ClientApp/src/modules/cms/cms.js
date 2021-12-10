import './cms.scss';
import '../../utils/cms/head.js'
import '../../utils/cms/app.js'
import CmsBranchesNav from './branches/cms-branches-nav.vue'
import * as VueRouter from 'vue-router';

const SomeTest = { template: '<div>Test</div>' }
const DefaultMain = { template: '<div>DefaultMain</div>' }

// 2. Define some routes
// Each route should map to a component.
// We'll talk about nested routes later.
const routes = [
    { path: '/ForesterCms/sometesturl', component: SomeTest },
    { path: '/:pathMatch(.*)', component: DefaultMain },
]

// 3. Create the router instance and pass the `routes` option
// You can pass in additional options here, but let's
// keep it simple for now.
const router = VueRouter.createRouter({
    // 4. Provide the history implementation to use. We are using the hash history for simplicity here.
    history: VueRouter.createWebHistory(),
    routes, // short for `routes: routes`
})



const CmsApp = {
    data() {
        return {
            counter: 0
        }
    },
    computed: {
        isRtl: function () {
            return true;
        }
    },
    components: {
        CmsBranchesNav
    }
}

vueApp.set('cms', '#cms-container', CmsApp, function (app) {
    app.use(router);
});
