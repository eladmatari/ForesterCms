import './cms.scss';
// https://www.s-ings.com/projects/microns-icon-font/
import 'microns/fonts/microns.scss';
import '../../utils/cms/head.js'
import '../../utils/cms/app.js'
import Router from './router.js';
import dictionary from './dictionary';

// global components
import CmsBranchesNav from './branches/nav/nav.vue'
import CmsBranchesItems from './branches/nav/items.vue'
import CmsBranchesItem from './branches/nav/item.vue'

import './branches/main/fields/'

//import * as Vuex from 'vuex';



const CmsApp = {
    data() {
        return {
            branches: [],
            branchesTrees: [],
            mainBranch: null,
            lang: { // TODO: get from some datasource
                alias: 'heb',
                isRtl: true
            }
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
        },
        loadBranches: function (onLoaded) {
            var self = this;

            app.api.get('coreapi/branches').then(function (response) {
                try {

                    self.branches = Vue.reactive(response.data);

                    var branchesTrees = self.branches.filter(function (branch) {
                        return !branch.parentId;
                    });

                    var setBranchChildren = function (branch, counter, branchesTree) {
                        if (counter > 50) {
                            console.error(branch, 'setBranchChildren counter max');
                            return;
                        }

                        branch.children = self.branches.filter(function (currBranch) {
                            return currBranch.parentId === branch.objId;
                        }).map((currBranch) => {
                            currBranch.tree = branchesTree;
                            currBranch.isOpen = false;

                            return currBranch;
                        });

                        branch.children.map(function (currBranch) {
                            setBranchChildren(currBranch, counter + 1, branchesTree);
                        });
                    }

                    branchesTrees.sort(function (a, b) {
                        if (a.sort > b.sort)
                            return 1;

                        return -1;
                    });

                    branchesTrees.map(function (branchesTree) {
                        setBranchChildren(branchesTree, 0, branchesTree);
                    });

                    self.branchesTrees = Vue.reactive(branchesTrees);

                    var lsData = vueApp.apps.cms.getLocalStorage();
                    self.mainBranch = self.branchesTrees.filter(function (tree) {
                        return tree.objId == lsData.mainBranchId;
                    })[0] || self.branchesTrees[0];

                    if (typeof onLoaded == 'function')
                        onLoaded();
                }
                catch (e) {
                    console.error(e);
                }
            });
        },
        getLang: function (keyText) {
            if (dictionary[keyText] && dictionary[keyText][this.lang.alias])
                return dictionary[keyText][this.lang.alias];

            return keyText;
        }
    },
    created() {
        this.loadBranches();
    }
}

vueApp.addComponent(CmsBranchesNav, 'cms');
vueApp.addComponent(CmsBranchesItems, 'cms');
vueApp.addComponent(CmsBranchesItem, 'cms');


vueApp.set('cms', '#cms-container', CmsApp, function (app) {
    //app.component('cms-branches-nav', CmsBranchesNav);
    //app.component('cms-branches-items', CmsBranchesItems);
    //app.component('cms-branches-item', CmsBranchesItem);
    app.use(Router);
});

vueApp.load();