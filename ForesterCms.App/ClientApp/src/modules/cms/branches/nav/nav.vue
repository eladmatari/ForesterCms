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
            },
            getEditLink() {
                if (!this.$root.mainBranch)
                    return '';

                let branch = this.$root.mainBranch;

                return '/ForesterCms/branch/edit/?b=' + branch.objId;
            }
        },
        created: function () {

        }
    }
</script>

<template>
    <div class="nav">
        <div class="nav-roots">
            <select v-model="$root.mainBranch" v-on:change="onTreeBranchChange()">
                <option v-for="branchesTree in branchesTrees" :key="branchesTree.objId" :value="branchesTree">
                    {{ branchesTree.name }}
                </option>
            </select>
            <router-link :to="getEditLink()" class="mu mu-opts-v"></router-link>
        </div>
        <div>
            <cms-branches-items :branch="$root.mainBranch" v-if="$root.mainBranch"></cms-branches-items>
        </div>
    </div>
</template>

<style lang="scss">
    .rtl {
        .nav {
            .nav-roots {
                a {
                    
                }
            }
        }
    }

    .nav {
        .nav-roots {
            display: flex;

            a {
                display: block;
                width: 20px;
                display: flex;
                justify-content: center;
                align-items: center;
                border: 1px solid #ccc;
                border-left: none;
                border-right: none;
                text-decoration: none;
                color: transparent;
            }



            &:hover {
                a {
                    color: #273849;

                    &:hover {
                        color: gray;
                    }
                }
            }
        }
    }
</style>