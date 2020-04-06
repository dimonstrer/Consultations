import VueRouter from "vue-router";
import Home from "@/components/pages/Home";
import PatientInfoFull from "@/components/pages/PatientInfoFull";
import NewPatient from "@/components/pages/NewPatient";
import EditPatient from "@/components/pages/EditPatient";

export default new VueRouter({
    routes: [
        {
            path: '/',
            name: 'home',
            component: Home
        },
        {
            path: '/addPatient',
            name: 'addPatient',
            component: NewPatient
        },
        {
            path: '/patients/:id',
            name: 'patientInfo',
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