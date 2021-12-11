<script>
    export default {
        name: 'cms-branches-item',
        props: ['branch'],
        data() {
            return {
                
            }
        },
        methods: {
            getItemsLink() {
                let branch = this.$props.branch;

                return '/ForesterCms/branch/items/?b=' + branch.objId;
            },
            getEditLink() {
                let branch = this.$props.branch;

                return '/ForesterCms/branch/edit/?b=' + branch.objId;
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
        <div class="item">
            <button class="mu mu-i-left cms-expand"
                    @click="toggleOpen()"
                    v-bind:class="{ open: branch.isOpen }">
            </button>
            <router-link :to="getItemsLink()">{{ branch.name }}</router-link>
            <router-link :to="getEditLink()" class="mu mu-opts-v options"></router-link>
        </div>
        <div class="item-children" v-if="branch.children.length && branch.isOpen">
            <cms-branches-items :branch="branch"></cms-branches-items>
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

        .item {
            position: relative;

            .options {
                background: none;
                border: none;
                position: absolute;
                top: 0;
                left: 0;
                font-size: 13px;
                opacity: 0;
                cursor: pointer;
                width: 20px;
                text-align: left;
                text-decoration: none;

                &:hover {
                    color: gray;
                }
            }

            &:hover {
                .options {
                    opacity: 1;
                }
            }
        }
    }

    button.cms-expand {
        background: none;
        border: none;
        font-size: 15px;
        cursor: pointer;

        &:hover {
            color: gray;
        }

        &.open {
            transform: rotate(-90deg);
        }
    }
</style>