<template>
    <tr>
        <td>
            <patient-link :id="patient.patientId">
                {{ FIO }}
            </patient-link>
        </td>
        <td>{{ patient.pensionNumber }}</td>
        <td>{{ patient.birthDate }}</td>
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