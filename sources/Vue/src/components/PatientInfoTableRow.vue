<template>
    <tr>
        <td>
            <patient-link :id="patient.patientId">
                {{ FIO }}
            </patient-link>
        </td>
        <td>{{patient.gender}}</td>
        <td>{{ patient.birthDate | formatDateToYMD}}</td>
        <td>{{ patient.pensionNumber }}</td>
        <td>
            <router-link :to="'editPatient/'+patient.patientId">
                <svg class="bi bi-pencil text-success" width="30" height="30" viewBox="0 0 20 20" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                    <path fill-rule="evenodd" d="M13.293 3.293a1 1 0 011.414 0l2 2a1 1 0 010 1.414l-9 9a1 1 0 01-.39.242l-3 1a1 1 0 01-1.266-1.265l1-3a1 1 0 01.242-.391l9-9zM14 4l2 2-9 9-3 1 1-3 9-9z" clip-rule="evenodd" />
                    <path fill-rule="evenodd" d="M14.146 8.354l-2.5-2.5.708-.708 2.5 2.5-.708.708zM5 12v.5a.5.5 0 00.5.5H6v.5a.5.5 0 00.5.5H7v.5a.5.5 0 00.5.5H8v-1.5a.5.5 0 00-.5-.5H7v-.5a.5.5 0 00-.5-.5H5z" clip-rule="evenodd" />
                </svg>
            </router-link>
            <svg class="bi bi-x-square-fill text-danger" width="30" height="30" viewBox="0 0 20 20" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                <path fill-rule="evenodd" d="M4 2a2 2 0 00-2 2v12a2 2 0 002 2h12a2 2 0 002-2V4a2 2 0 00-2-2H4zm3.354 4.646L10 9.293l2.646-2.647a.5.5 0 01.708.708L10.707 10l2.647 2.646a.5.5 0 01-.708.708L10 10.707l-2.646 2.647a.5.5 0 01-.708-.708L9.293 10 6.646 7.354a.5.5 0 11.708-.708z" clip-rule="evenodd" />
            </svg>
        </td>
    </tr>
</template>

<script>
    import PatientLink from "@/components/PatientLink";
    export default {
        props: ['patient'],
        computed: {
            FIO: function(){
                return (this.patient.firstName+' '+this.patient.lastName +
                    (this.patient.patronymic!=null ?' ' + this.patient.patronymic :""))
            },
        },
        components: {
            PatientLink
        }
    }
</script>

<style scoped>

</style>