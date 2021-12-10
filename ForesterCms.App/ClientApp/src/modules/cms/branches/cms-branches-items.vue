<script>
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
            
        }
    }
</script>

<template>
    <div class="cms-branches-items">
        <cms-branches-item v-for="branchChild in branch.children" :key="branchChild.objId" :branch="branchChild"></cms-branches-item>
    </div>
</template>

<style scoped>
    .cms-branches-items { padding: 10px 7px; }
</style>