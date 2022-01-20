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
        
    }
</style>