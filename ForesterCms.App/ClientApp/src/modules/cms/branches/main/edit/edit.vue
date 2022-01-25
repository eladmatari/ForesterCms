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

    var branchCopyFields = ['objId', 'entityInfoId', 'lcid', 'objId', 'parentId', 'sort', 'status']


    export default {
        name: 'cms-branches-edit',
        data() {
            return {
                display: null,
                addNewBranchOptions: null,
                editBranchOptions: null
            }
        },
        methods: {
            showEditBranch() {
                this.editBranchOptions = Vue.reactive(prepareFieldsForm(branchFields, this.$root.currentBranch, branchCopyFields));

                this.display = null;
            },  
            showAddNewBranch() {

                this.addNewBranchOptions = Vue.reactive(prepareFieldsForm(branchFields));

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

                self.$root.refreshBranches();
            },
            updateBranch() {
                var self = this;

                let isValid = self.editBranchOptions.validate();
                if (!isValid)
                    return;

                var model = self.editBranchOptions.model;

                app.showLoader();

                app.api.postCms('coreapi/addorupdatebranch/', null, model).then(function (response) {
                    app.hideLoader();
                    console.log(response.data);
                })

                self.$root.refreshBranches();
            }
        },
        created: function () {
            var self = this;
            self.showEditBranch();
            $('body').on('branchChange', function () {
                self.showEditBranch();
            });
        }
    }
</script>

<template>
    <div class="cms-branches-edit">
        <div class="top-options">
            <h1>
                {{ $root.currentBranch.objId }} - {{ $root.currentBranch.name }}
            </h1>
            <div v-if="!display">
                <button v-on:click="showAddNewBranch()">
                    {{ $root.getLang('Add New Branch') }}
                </button>
                <button v-on:click="updateBranch()">
                    {{ $root.getLang('Save') }}
                </button>
            </div>
            <div v-if="display == 'add'">
                <button v-on:click="showEditBranch()">
                    {{ $root.getLang('Back') }}
                </button>
                <button v-on:click="addNewBranch()">
                    {{ $root.getLang('Save') }}
                </button>
            </div>
        </div>
        <div class="main">
            <div v-if="!display">
                <h3>
                    {{ $root.getLang('Edit branch') }}
                </h3>
                <cms-fields-form :options="editBranchOptions"></cms-fields-form>
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
                h1 {
                   /* margin-right: 0;
                    margin-left: 10px;*/
                }

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
            display: flex;
            justify-content: space-between;

            h1 {
                font-size: 20px;
                margin: 0;
                /*margin-right: 10px;*/
            }

            button + button {
                margin-left: 10px;
            }
        }
    }
</style>