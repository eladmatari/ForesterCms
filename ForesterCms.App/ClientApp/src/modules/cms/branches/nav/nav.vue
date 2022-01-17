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
            <select v-model="$root.mainBranch" v-on:change="onTreeBranchChange()">
                <option v-for="branchesTree in branchesTrees" :key="branchesTree.objId" :value="branchesTree">
                    {{ branchesTree.name }}
                </option>
            </select>
        </div>
        <div>
            <cms-branches-items :branch="$root.mainBranch" v-if="$root.mainBranch"></cms-branches-items>
        </div>
    </div>
</template>

<style>
    
</style>