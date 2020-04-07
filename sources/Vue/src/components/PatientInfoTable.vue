<template>
    <table class="table table-striped">
        <thead>
        <tr>
            <th width="50%">ФИО</th>
            <th width="10%">Пол</th>
            <th width="15%">Дата рождения</th>
            <th width="15%">СНИЛС</th>
            <th width="10%"></th>
        </tr>
        </thead>
        <tbody>
        <tr is="patient-info-table-row"
            v-for="patient in patientsList"
            :patient="patient"
            :key="patient.PensionNumber"
            @deletePatient="deletePatient">
        </tr>
        </tbody>
    </table>
</template>

<script>
    import PatientInfoTableRow from "@/components/PatientInfoTableRow";
    export default {
        data() {
            return {
                patientsList: []
            }
        },
        created() {
            this.fetchPatients();
        },
        methods: {
            async fetchPatients() {
                let vm = this;
                console.log(this)
                await fetch("https://localhost:44373/api/patient-management/patients")
                    .then(response=>response.json())
                    .then(data =>vm.patientsList=data);
            },
            async deletePatient(id){
                let response = await fetch('https://localhost:44373/api/patient-management/patients/'+id, {
                    method: 'DELETE'});
                if(response.ok){
                    alert('Пациент успешно удален');
                    await this.fetchPatients();
                }
                else
                    alert('Произошла ошибка при удалении пациента')
            },
        },
        components: {
            PatientInfoTableRow
        }
    }
</script>