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
    <div class="cms-branches-item">
        <router-link :to="'/ForesterCms/branch/' + branch.objId">{{ branch.name }}</router-link>
    </div>
</template>

<style lang="scss" scoped>
    .cms-branches-item {
        margin-bottom: 5px;

        a {
            text-decoration: none;
            color: inherit;
            /*cursor: default;*/
        }

        a:hover {
            text-decoration: underline;
        }
    }
</style>