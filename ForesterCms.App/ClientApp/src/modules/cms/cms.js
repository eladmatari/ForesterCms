import './cms.scss';
import '../../utils/cms/head.js'
import '../../utils/cms/app.js'
import CmsBranchesNav from './branches/cms-branches-nav.vue'

const CmsApp = {
    data() {
        return {
            counter: 0
        }
    },
    computed: {
        isRtl: function () {
            return true;
        }
    },
    components: {
        CmsBranchesNav
    }
}

vueApp.set('cms', '#cms-container', CmsApp);