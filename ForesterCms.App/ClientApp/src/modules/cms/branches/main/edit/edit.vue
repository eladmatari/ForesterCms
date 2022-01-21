<script>
    export default {
        name: 'cms-branches-edit',
        data() {
            return {
                display: 'add',
                newBranch: {
                    name: ''
                }
            }
        },
        methods: {
            addNewBranch() {
                var self = this;

                console.log(1);
                
            }
        },
        created: function () {
            
        }
    }
</script>

<template>
    <div class="cms-branches-edit">
        <div class="top-options">
            <div v-if="!display">
                <button v-on:click="display = 'add'">
                    {{ $root.getLang('Add New Branch') }}
                </button>
            </div>
            <div v-if="display == 'add'">
                <button v-on:click="display = null">
                    {{ $root.getLang('Back') }}
                </button>
                <button v-on:click="addNewBranch()">
                    {{ $root.getLang('Save') }}
                </button>
            </div>
        </div>
        <div class="main">
            <div v-if="!display">

            </div>
            <div v-if="display == 'add'">
                <form novalidate>
                    <table>
                        <tr>
                            <td>
                                {{ $root.getLang('Name') }}:
                            </td>
                            <td>
                                <cms-field-name :model="newBranch" :field="'name'" ref="field"></cms-field-name>
                            </td>
                        </tr>
                    </table>
                </form>
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    .rtl {
        .cms-branches-edit {
            .top-options {
                button + button {
                    margin-right: 10px;
                    margin-left: 0;
                    
                }
            }
        }
    }

    .cms-branches-edit {
        .main {
            padding: 10px;
        }

        .top-options {
            button + button {
                margin-left: 10px;
            }
        }
    }
</style>