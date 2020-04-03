import Vue from 'vue'
import VueRouter from "vue-router";
import router from "@/router";
import App from "@/App";
import moment from "moment";

Vue.use(VueRouter);

Vue.filter('formatDateToYMD', function (value) {
  return moment(value).format('MM/DD/YYYY')
});
Vue.filter('formatDateToHH', function (value) {
  return moment(value).format('HH:mm')
});


Vue.config.productionTip = false;

new Vue({
  render: h => h(App),
  router
}).$mount('#app')