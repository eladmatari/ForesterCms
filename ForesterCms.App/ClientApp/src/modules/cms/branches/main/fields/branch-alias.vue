<script>
    export default {
        name: 'cms-field-alias',
        props: ['field', 'model'],
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

            var model = this.$props.model;
            var field = this.$props.field;
            field.validate = function (isBlur) {
                if (isBlur)
                    field.isBlur = true;

                field.error = null;

                if (field.mandatory) {
                    if (!model[field.alias]) {
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
    <div class="cms-field cms-field-alias">
        <div>
            <input type="text" maxlength="50"
                   v-model.trim="model[field.alias]"
                   v-on:blur="validate(true)"
                   v-on:keyup="validate()" />
        </div>
        <div class="error" v-if="field.error && field.isBlur">
            {{ field.error }}
        </div>
    </div>
</template>

<style lang="scss">
    .rtl {
        .cms-field-alias {
            .error {
                margin-left: 0;
                margin-right: 10px;
            }
        }
    }

    .cms-field-name {
        input {
            height: 30px;
            line-height: 30px;
            border: 1px solid gray;
            border-radius: 5px;
            padding: 0 7px;
        }

        display: flex;
        align-items: center;

        .error {
            margin-left: 10px;
        }
    }
</style>