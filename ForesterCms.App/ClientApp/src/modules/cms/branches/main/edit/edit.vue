<script>
    import { prepareFieldsForm } from '../fields/fields-helper';

    var branchFields = [
        {
            name: 'Name',
            alias: 'name',
            type: 'name',
            mandatory: true
        },
        {
            name: 'Alias',
            alias: 'alias',
            type: 'name',
            mandatory: true
        }
    ]


    export default {
        name: 'cms-branches-edit',
        data() {
            return {
                display: null,
                addNewBranchOptions: null,
            }
        },
        methods: {
            showAddNewBranch() {

                this.addNewBranchOptions = Vue.reactive(prepareFieldsForm({
                    fields: branchFields
                }));

                this.display = 'add';
            },
            addNewBranch() {
                var self = this;

                let isValid = self.addNewBranchOptions.validate();
                if (!isValid)
                    return;

                var model = self.addNewBranchOptions.model;
                model.parentId = self.$root.currentBranch.objId;
                model.lcid = self.$root.currentBranch.lcid;
                model.entityInfoId = self.$root.currentBranch.entityInfoId;

                app.showLoader();

                app.api.postCms('coreapi/addorupdatebranch/', null, model).then(function (response) {
                    app.hideLoader();
                    console.log(response.data);
                })

            }
        },
        created: function () {
            this.showAddNewBranch();
        }
    }
</script>

<template>
    <div class="cms-branches-edit">
        <div class="top-options">
            <div v-if="!display">
                <button v-on:click="showAddNewBranch()">
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
                <h3>
                    {{ $root.getLang('Add new branch') }}
                </h3>
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