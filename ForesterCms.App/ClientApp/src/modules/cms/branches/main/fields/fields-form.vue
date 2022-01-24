<script>

    export default {
        name: 'cms-fields-form',
        props: ['options'],
        data() {
            return {
                
            }
        },
        methods: {

        },
        created: function () {
            let self = this;

            self.$props.options.validate =() => {
                let isValid = !self.options.fields.filter((field) => {
                    return !field.validate(true);
                }).length;

                return isValid;
            }
        }
    }
</script>

<template>
    <div class="cms-fields-form">
        <form novalidate>
            <table>
                <tr v-for="(field, index) in options.fields" :key="index">
                    <td>
                        {{ $root.getLang(field.name) }}:
                    </td>
                    <td>
                        <keep-alive>
                            <component :is="'cms-field-' + field.type" :field="field"></component>
                        </keep-alive>                      
                    </td>
                </tr>
            </table>
        </form>
    </div>
</template>

<style lang="scss">
    .cms-fields-form {
        form {
            table {
                tr td:first-child {
                    width: 100px;
                }

                td {
                    vertical-align: text-bottom;
                }
            }
        }

        .error {
            color: #fb4545;
            font-size: 14px;
            margin: 4px 0;
        }
    }
</style>