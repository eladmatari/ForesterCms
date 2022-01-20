<script>
    export default {
        name: 'cms-branches-edit',
        data() {
            return {
                display: null,
            }
        },
        methods: {
            
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
            </div>
        </div>
        <div class="main">
            <div v-if="!display">

            </div>
            <div v-if="display == 'add'">
                add
            </div>
        </div>
    </div>
</template>

<style lang="scss">
    .cms-branches-edit {
        .main {
            padding: 10px;
        }
    }
</style>