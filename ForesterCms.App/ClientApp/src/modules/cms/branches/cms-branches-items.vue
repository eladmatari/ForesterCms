<script>
    import CmsBranchesItem from './cms-branches-item.vue'

    export default {
        props: ['branch'],
        data() {
            return {
                branches: [],
                branchesTree: [],
                mainBranch: null
            }
        },
        methods: {

        },
        created: function () {
            
        },
        components: {
            CmsBranchesItem
        }
    }
</script>

<template>
    <div>
        <cms-branches-item v-for="branchChild in branch.children" :key="branchChild.objId" :branch="branchChild"></cms-branches-item>
    </div>
</template>

<style>
    
</style>