<template>
    <div>
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
                v-for="patient in pageInfo.patients"
                :patient="patient"
                :key="patient.PensionNumber"
                @deletePatient="deletePatient">
            </tr>
            </tbody>
        </table>
        <router-link
                v-if="pageInfo.pageViewModel.hasPreviousPage"
                tag="button"
                class="btn btn-outline-dark"
                :to="{name:'home', params:{page : pageInfo.pageViewModel.pageNumber-1}}"
        >Назад</router-link>
        <router-link
                v-if="pageInfo.pageViewModel.hasNextPage"
                tag="button"
                class="btn btn-outline-dark"
                :to="{name:'home', params:{page : pageInfo.pageViewModel.pageNumber+1}}"
        >Вперед</router-link>
    </div>
</template>

<script>
    import PatientInfoTableRow from "@/components/PatientInfoTableRow";
    export default {
        data() {
            return {
                pageInfo: {
                    patients: [],
                    pageViewModel: {}
                },
                page: this.$route.params['page'],
            }
        },
        watch: {
            $route (toR, fromR){
                this.page = toR.params['page'];
                this.fetchPatients()
            }
        },
        created() {
            this.fetchPatients();
        },
        methods: {
            async fetchPatients() {
                let vm = this;
                console.log(this)
                await fetch("https://localhost:44373/api/patient-management/patients"+'?page='+this.page)
                    .then(response=>response.json())
                    .then(data =>vm.pageInfo=data);
                // this.pageInfo.pageViewModel.hasPreviousPage = this.pageInfo.pageViewModel.hasPreviousPage == 'true';
                // this.pageInfo.pageViewModel.hasNextPage = this.pageInfo.pageViewModel.hasNextPage == 'true';
                console.log(this.pageInfo)

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