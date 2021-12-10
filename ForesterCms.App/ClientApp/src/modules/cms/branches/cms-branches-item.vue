<script>
    export default {
        props: ['branch'],
        data() {
            return {
                
            }
        },
        methods: {
            getBranchLink() {
                let branch = this.$props.branch;

                return `/ForesterCms/?t=` + branch.tree.objId + '&b=' + branch.objId;
            }
        },
        created: function () {
            
        }
    }
</script>

<template>
    <div class="cms-branches-item">
        <router-link :to="getBranchLink()">{{ branch.name }}</router-link>
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