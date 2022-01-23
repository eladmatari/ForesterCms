<script>
    import { prepareFieldsForm } from '../fields/fields-helper';

    export default {
        name: 'cms-branches-edit',
        data() {
            return {
                display: 'add',
                addNewBranchOptions: prepareFieldsForm({
                    fields: [
                        {
                            name: 'Name',
                            alias: 'name',
                            type: 'name',
                            mandatory: true
                        },
                        {
                            name: 'Alias',
                            alias: 'alias',
                            type: 'name'
                        }
                    ]
                })
            }
        },
        methods: {
            addNewBranch() {
                var self = this;

                self.addNewBranchOptions.validate();
                self.addNewBranchOptions.getModel((model) => {
                    console.log(model);
                });
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
                <cms-fields-form :options="addNewBranchOptions"></cms-fields-form>
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