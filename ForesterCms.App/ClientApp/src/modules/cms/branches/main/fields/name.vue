<script>
    export default {
        name: 'cms-field-name',
        props: ['model', 'field'],
        data() {
            return {
                
            }
        },
        methods: {
            addNewBranch() {
                var self = this;

                debugger
            }
        },
        created: function () {
            
        }
    }
</script>

<template>
    <div class="cms-field cms-field-name">
        <input type="text" maxlength="50" v-model="model[field]" />
    </div>
</template>

<style lang="scss">
    .cms-field-name {
        input {
            height: 30px;
            line-height: 30px;
            border: 1px solid gray;
            border-radius: 5px;
            padding: 0 7px;
        }
    }
</style>