import * as Vue from 'vue'
import * as Counter from './counter.vue'

const appVue = {
    data() {
        return {
            message: 'Hello Vue!!!'
        }
    },
    methods: {
        reverseMessage: function () {
            this.message = this.message.split('').reverse().join('');
        }
    },
    components: [
        Counter
    ]
}

window.vueApp = Vue.createApp(appVue).mount('#app')