<script>
    export default {
        name: 'cms-field-name',
        props: ['field'],
        data() {
            return {
                
            }
        },
        methods: {
            validate(isBlur) {
                var field = this.$props.field;

                field.validate(isBlur);
            }
        },
        created: function () {
            var self = this;

            var field = this.$props.field;
            field.validate = function (isBlur) {
                if (isBlur)
                    field.isBlur = true;

                field.error = null;

                if (field.mandatory) {
                    if (!field.value) {
                        field.error = self.$root.getLang('Field is mandatory');
                        return false;
                    }
                }

                return true;
            }
        }
    }
</script>

<template>
    <div class="cms-field cms-field-name">
        <div>
            <input type="text" maxlength="50" 
                   v-model.trim="field.value"
                   v-on:blur="validate(true)"
                   v-on:keyup="validate()"/>
        </div>
        <div class="error" v-if="field.error && field.isBlur">
            {{ field.error }}
        </div>
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