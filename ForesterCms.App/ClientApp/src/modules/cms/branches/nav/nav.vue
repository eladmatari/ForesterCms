<script>
    export default {
        name: 'cms-branches-nav',
        data() {
            return {
                
            }
        },
        computed: {
            branchesTrees() {
                return this.$root.branchesTrees;
            },
            mainBranch() {
                return this.$root.mainBranch;
            }
        },
        methods: {
            onTreeBranchChange() {
                var lsData = this.$root.getLocalStorage();
                lsData.mainBranchId = this.$root.mainBranch.objId;
                this.$root.setLocalStorage(lsData);
            }
        },
        created: function () {
            
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