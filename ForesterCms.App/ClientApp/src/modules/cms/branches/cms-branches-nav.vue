<script>
    import CmsBranchesItems from './cms-branches-items.vue'

    export default {
        data() {
            return {
                branches: [],
                branchesTrees: [],
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

                    var lsData = vueApp.cms.getLocalStorage();
                    self.mainBranch = self.branchesTrees.filter(function (tree) {
                        return tree.objId == lsData.mainBranchId;
                    })[0] || self.branchesTrees[0];
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
                <option v-for="branchesTree in branchesTrees" :key="branchesTree.objId" :value="branchesTree">
                    {{ branchesTree.name }}
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