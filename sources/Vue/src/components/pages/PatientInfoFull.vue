<template>
    <div>
        <router-link tag="button" class="btn btn-success" :to="{name:'editPatient', params:{id : pageInfo.patient.patientId}}">Редактировать</router-link>
        <button class="btn btn-danger" @click="deletePatient">Удалить</button>

        <div class="text-left mt-4">
            <dl class="row">
                <dt class="col-sm-3">ФИО</dt>
                <dd class="col-sm-9">{{FIO}}</dd>
                <dt class="col-sm-3">Пол</dt>
                <dd class="col-sm-9">{{pageInfo.patient.gender}}</dd>
                <dt class="col-sm-3">Дата рождения</dt>
                <dd class="col-sm-9">{{pageInfo.patient.birthDate | formatDateToYMD}}</dd>
                <dt class="col-sm-3">СНИЛС</dt>
                <dd class="col-sm-9">{{pageInfo.patient.pensionNumber}}</dd>
            </dl>
        </div>
        <div class="text-center">
        <button class="btn btn-success my-3" @click="isNewConsultation = true">Добавить новую консультацию</button>
        </div>
        <new-consultation-modal :id="id" v-if="isNewConsultation" @add="addedConsultation" @close="isNewConsultation=false"></new-consultation-modal>
        <edit-consultation-modal
                v-if="isEditConsultation"
                :consultation="consultationToEdit"
                @edited="editedConsultation"
                @close="isEditConsultation=false"
        ></edit-consultation-modal>
        <consultations-list
                :consultations="pageInfo.patient.consultations"
                @deleteConsultation="deleteConsultation"
                @editConsultation="editConsultation"
        ></consultations-list>

        <div class="text-center">
            <router-link
                    v-if="pageInfo.pageViewModel.hasPreviousPage"
                    tag="button"
                    class="btn btn-outline-dark"
                    :to="{name:'patientInfo', params:{id: pageInfo.patient.patientId, page : pageInfo.pageViewModel.pageNumber-1}}"
            >Назад</router-link>
            <router-link
                    v-if="pageInfo.pageViewModel.hasNextPage"
                    tag="button"
                    class="btn btn-outline-dark"
                    :to="{name:'patientInfo', params:{id: pageInfo.patient.patientId, page : pageInfo.pageViewModel.pageNumber+1}}"
            >Вперед</router-link>
        </div>
    </div>
</template>

<script>
    import ConsultationsList from "@/components/ConsultationsList";
    import NewConsultationModal from "@/components/pages/NewConsultationModal";
    import EditConsultationModal from "@/components/pages/EditConsultationModal";
    export default {
        components: {ConsultationsList, NewConsultationModal, EditConsultationModal},
        data() {
            return {
                id: this.$route.params['id'],
                pageInfo: {
                    patient: {},
                    pageViewModel: {}
                },
                isNewConsultation: false,
                isEditConsultation: false,
                consultationToEdit: {},
                page: Number
            }
        },
        computed: {
            FIO: function(){
                return (this.pageInfo.patient.firstName+' '+this.pageInfo.patient.lastName +
                    (this.pageInfo.patient.patronymic!=null ?' ' + this.pageInfo.patient.patronymic :""))
            }
        },
        watch: {
            $route (toR, fromR){
                this.id = toR.params['id'];
                this.page = toR.params['page'];
            }
        },
        created(){
            this.fetchPatient();
        },
        updated(){
            alert('UPDATED')
            this.fetchPatient()
        },
        methods: {
            async fetchPatient(){
                let vm = this;
                await fetch("https://localhost:44373/api/patient-management/patients/"+this.id+'?page='+this.page)
                    .then(response=>response.json())
                    .then(data =>vm.pageInfo=data);
            },
            async deletePatient(){
                let response = await fetch('https://localhost:44373/api/patient-management/patients/'+this.id, {
                        method: 'DELETE'});
                if(response.ok){
                    alert('Пациент успешно удален');
                    this.$router.push({name: 'home'});
                }
                else
                    alert('Произошла ошибка при удалении пациента')
            },
            async getConsultations(){
                let vm = this;
                await fetch("https://localhost:44373/api/consultation-management/patient/consultations/"+this.id)
                    .then(response=>response.json())
                    .then(data =>vm.pageInfo.patient.consultations=data);
            },
            async addedConsultation(){
                this.isNewConsultation = false;
                await this.getConsultations();
            },
            async deleteConsultation(id){
                let response = await fetch('https://localhost:44373/api/consultation-management/consultations/'+id, {
                    method: 'DELETE'});
                if(response.ok){
                    alert('Консультация успешно удалена');
                    await this.getConsultations();
                }
                else
                    alert('Произошла ошибка при удалении консультации')
            },
            editConsultation(consultation){
                this.isEditConsultation = true;
                this.consultationToEdit = consultation;
            },
            async editedConsultation(){
                this.isEditConsultation = false;
                await this.getConsultations();
            }
        }
    }
</script>