import * as VueRouter from 'vue-router';

import CmsBranchesEdit from './branches/main/edit/edit.vue';
import CmsBranchesObjects from './branches/main/edit/objects.vue'
const SomeTest = { template: '<div>Test</div>' }
const DefaultMain = { template: '<div>DefaultMain</div>' }

const routes = [
    { path: '/ForesterCms/sometesturl', component: SomeTest },
    { path: '/ForesterCms/branch/edit', component: CmsBranchesEdit },
    { path: '/ForesterCms/branch/objects', component: CmsBranchesObjects },
    { path: '/:pathMatch(.*)', component: DefaultMain }
]

const router = VueRouter.createRouter({
    history: VueRouter.createWebHistory(),
    routes,
})

router.afterEach((to, from, failure) => {
    datasource.queryString = app.api.parseQueryString();

    if (datasource.queryString.b) {
        vueApp.apps.cms.currentBranch = vueApp.apps.cms.branches.filter((branch) => {
            return datasource.queryString.b == branch.objId;
        })[0] || null;
    }
    else {
        vueApp.apps.cms.currentBranch = null;
    }

    $('body').trigger('branchChange');
})

export default router;