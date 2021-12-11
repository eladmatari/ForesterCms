import './cms.scss';
// https://www.s-ings.com/projects/microns-icon-font/
import 'microns/fonts/microns.scss';
import '../../utils/cms/head.js'
import '../../utils/cms/app.js'
import CmsBranchesNav from './branches/nav/nav.vue'
import CmsBranchesItems from './branches/nav/items.vue'
import CmsBranchesItem from './branches/nav/item.vue'
import * as VueRouter from 'vue-router';

import CmsBranchesEdit from './branches/main/edit/edit.vue';
const SomeTest = { template: '<div>Test</div>' }
const DefaultMain = { template: '<div>DefaultMain</div>' }

const routes = [
    { path: '/ForesterCms/sometesturl', component: SomeTest },
    { path: '/ForesterCms/branch/edit', component: CmsBranchesEdit },
    { path: '/:pathMatch(.*)', component: DefaultMain }
]

const router = VueRouter.createRouter({
    history: VueRouter.createWebHistory(),
    routes,
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
    methods: {
        getLocalStorage: function () {
            var lsData = localStorageService.getItem('fcms');
            if (!lsData) {
                lsData = {};
                this.setLocalStorage(lsData);
            }

            return lsData;
        },
        setLocalStorage: function (lsData) {
            localStorageService.setItem('fcms', lsData);
        }
    }
}

vueApp.set('cms', '#cms-container', CmsApp, function (app) {
    app.component('cms-branches-nav', CmsBranchesNav);
    app.component('cms-branches-items', CmsBranchesItems);
    app.component('cms-branches-item', CmsBranchesItem);
    app.use(router);
});
