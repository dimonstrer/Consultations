import VueRouter from "vue-router";
import Home from "@/components/pages/Home";
import PatientInfoFull from "@/components/pages/PatientInfoFull";
import NewPatient from "@/components/pages/NewPatient";
import EditPatient from "@/components/pages/EditPatient";
import NewConsultationModal from "@/components/pages/NewConsultationModal";

export default new VueRouter({
    routes: [
        {
            path: '/addPatient',
            name: 'addPatient',
            component: NewPatient
        },
        {
            path: '/patients/:id',
            name: 'patientInfo',
            component: PatientInfoFull,
            children: [
                {
                    path: 'addConsultation',
                    name: 'addConsultation',
                    component: NewConsultationModal
                }
            ]
        },
        {
            path: '/editPatient/:id',
            name: 'editPatient',
            component: EditPatient
        },
        {
            path: '/:page?',
            name: 'home',
            component: Home
        }
    ],
    mode: 'history'
})