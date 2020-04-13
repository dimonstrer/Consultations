<template>
    <div class="text-center">
        <h1 class="display-4">Список пациентов</h1>
        <router-link to="/addPatient" tag="button" class="btn btn-success my-3">Добавить пациента</router-link>
        <patient-info-table></patient-info-table>
    </div>
</template>

<script>
    import PatientInfoTable from "@/components/PatientInfoTable";

    export default  {
        components: {
            PatientInfoTable
        }
    }
</script>