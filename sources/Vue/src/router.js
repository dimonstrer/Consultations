import VueRouter from "vue-router";
import Home from "@/components/Home";
import PatientInfo from "@/components/PatientInfoTable";
import PatientInfoFull from "@/components/PatientInfoFull";

export default new VueRouter({
    routes: [
        {
            path: '/',
            component: Home
        },
        {
            path: '/patients/:id',
            component: PatientInfoFull
        }
    ],
    mode: 'history'
})