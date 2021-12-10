<script>
    import CmsBranchesItems from './cms-branches-items.vue'

    export default {
        data() {
            return {
                branches: [],
                branchesTree: [],
                mainBranch: null
            }
        },
        methods: {
            onTreeBranchChange() {
                var lsData = vueApp.cms.getLocalStorage();
                lsData.mainBranchId = this.mainBranch.objId;
                vueApp.cms.setLocalStorage(lsData);
            }
        },
        created: function () {
            var self = this;

            app.api.get('coreapi/branches').then(function (response) {
                try {
                    
                    self.branches = Vue.reactive(response.data);

                    var branchesTree = self.branches.filter(function (branch) {
                        return !branch.parentId;
                    });

                    var setBranchChildren = function (branch, counter) {
                        if (counter > 50) {
                            console.error(branch, 'setBranchChildren counter max');
                            return;
                        }

                        branch.children = self.branches.filter(function (currBranch) {
                            return currBranch.parentId === branch.objId;
                        });

                        branch.children.map(function (currBranch) {
                            setBranchChildren(currBranch, counter + 1);
                        });
                    }

                    branchesTree.sort(function (a, b) {
                        if (a.sort > b.sort)
                            return 1;

                        return -1;
                    });

                    branchesTree.map(function (branch) {
                        setBranchChildren(branch, 0);
                    });

                    self.branchesTree = Vue.reactive(branchesTree);

                    var lsData = vueApp.cms.getLocalStorage();
                    self.mainBranch = self.branchesTree.filter(function (tree) {
                        return tree.objId == lsData.mainBranchId;
                    })[0] || self.branchesTree[0];
                }
                catch (e) {
                    console.error(e);
                }
            });
        },
        components: {
            CmsBranchesItems
        }
    }
</script>

<template>
    <div>
        <div>
            <select v-model="mainBranch" v-on:change="onTreeBranchChange()">
                <option v-for="branch in branchesTree" :key="branch.objId" :value="branch">
                    {{ branch.name }}
                </option>
            </select>
        </div>
        <div>
            <cms-branches-items :branch="mainBranch" v-if="mainBranch"></cms-branches-items>
        </div>
    </div>
</template>

<style>
    
</style>