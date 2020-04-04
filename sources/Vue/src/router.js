import VueRouter from "vue-router";
import Home from "@/components/Home";
import PatientInfoFull from "@/components/PatientInfoFull";
import NewPatient from "@/components/NewPatient";
import EditPatient from "@/components/EditPatient";

export default new VueRouter({
    routes: [
        {
            path: '/',
            component: Home
        },
        {
            path: '/addPatient',
            component: NewPatient
        },
        {
            path: '/patients/:id',
            component: PatientInfoFull
        },
        {
            path: '/editPatient/:id',
            name: 'editPatient',
            component: EditPatient
        }
    ],
    mode: 'history'
})