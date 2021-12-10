<script>
    export default {
        props: ['branch'],
        data() {
            return {
                branches: [],
                branchesTree: [],
                mainBranch: null
            }
        },
        methods: {

        },
        created: function () {
            
        }
    }
</script>

<template>
    <div>
        {{ branch.name }}
    </div>
</template>

<style>
    
</style>