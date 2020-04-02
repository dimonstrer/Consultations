<template>
    <div id="text-center">
        <h1 class="display-4">Список пациентов</h1>
        <table id="patients" class="table table-striped">
            <tbody>
                <tr is="patient-info"
                    v-for="patient in patientsList"
                    :patient="patient"
                    :key="patient.PensionNumber">

                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
    import PatientInfo from "@/components/PatientInfoTable";

    export default  {
        data() {
            return {
                patientsList: []
            }
        },
        created() {
            this.fetchPatients();
        },
        methods: {
            fetchPatients() {
                let vm = this;
                console.log(this)
                fetch("https://localhost:44373/api/patient-management/patients")
                    .then(response=>response.json())
                    .then(data =>vm.patientsList=data);
            }
        },
        components: {
            PatientInfo
        }
    }
</script>