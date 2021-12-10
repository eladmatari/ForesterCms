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
            },
            toggleOpen() {
                let branch = this.$props.branch;
                branch.isOpen = !branch.isOpen;
            }
        },
        created: function () {
            
        }
    }
</script>

<template>
    <div class="cms-branches-item">
        <button class="mu mu-i-left cms-expand" 
                @click="toggleOpen()"
                v-bind:class="{ open: branch.isOpen }">
        </button>
        <router-link :to="getBranchLink()">{{ branch.name }}</router-link>
        <div>
            <cms-branches-items :branch="branch" v-if="branch.isOpen"></cms-branches-items>
        </div>
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

    button.cms-expand {
        background: none;
        border: none;
        font-size: 15px;
        cursor: pointer;

        &:hover{
            color: gray;
        }
    }
</style>