(function () {
    Vue.component('cms-branches-nav', {
        data: function () {
            return {
                branches: [],
                branchesTree: [],
                mainBranch: null
            }
        },
        template: `<div>
        <div>
            <select v-model="mainBranch">
                <option v-for="branch in branchesTree" v-bind:value="branch">
                    {{ branch.name }}
                </option>
            </select>
        </div>
</div>`,
        methods: {

        },
        created: function () {
            var self = this;

            app.api.get('coreapi/branches').then(function (response) {
                try {
                    self.branches = Vue.observable(response.data);

                    var branchesTree = self.branches.filter(function (branch) {
                        return !branch.parentId;
                    });

                    var setBranchChildren = function (branch, counter) {
                        if (counter > 50) {
                            console.error(branch, 'setBranchChildren counter max');
                            return;
                        }

                        branch.children = self.branches.filter(function (currBranch) {
                            return currBranch.parentId === branch.id;
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

                    self.branchesTree = Vue.observable(branchesTree);
                    self.mainBranch = self.branchesTree[0];
                }
                catch (e) {
                    console.error(e);
                }
            });
        }
    })

})();