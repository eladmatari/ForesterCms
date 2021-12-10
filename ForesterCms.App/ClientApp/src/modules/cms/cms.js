import './cms.scss';
import '../../utils/cms/head.js'
import '../../utils/cms/app.js'
import CmsBranchesNav from './branches/cms-branches-nav.vue'
import * as VueRouter from 'vue-router';

const SomeTest = { template: '<div>Test</div>' }
const DefaultMain = { template: '<div>DefaultMain</div>' }

const routes = [
    { path: '/ForesterCms/sometesturl', component: SomeTest },
    { path: '/:pathMatch(.*)', component: DefaultMain },
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
    },
    components: {
        CmsBranchesNav
    }
}

vueApp.set('cms', '#cms-container', CmsApp, function (app) {
    app.use(router);
});
