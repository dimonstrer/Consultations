<template>
    <div>
        <router-link tag="button" class="btn btn-success" :to="{name:'editPatient', params:{id : patient.patientId}}">Редактировать</router-link>
        <button class="btn btn-danger" @click="deletePatient">Удалить</button>

        <div class="text-left mt-4">
            <dl class="row">
                <dt class="col-sm-3">ФИО</dt>
                <dd class="col-sm-9">{{FIO}}</dd>
                <dt class="col-sm-3">Пол</dt>
                <dd class="col-sm-9">{{patient.gender}}</dd>
                <dt class="col-sm-3">Дата рождения</dt>
                <dd class="col-sm-9">{{patient.birthDate | formatDateToYMD}}</dd>
                <dt class="col-sm-3">СНИЛС</dt>
                <dd class="col-sm-9">{{patient.pensionNumber}}</dd>
            </dl>
        </div>
        <div class="text-center">
        <button class="btn btn-success my-3" @click="newConsultation = true">Добавить новую консультацию</button>
        </div>
        <new-consultation-modal :id="id" v-if="newConsultation" @add="addedConsultation" @close="newConsultation=false"></new-consultation-modal>
        <consultations-list
                :consultations="patient.consultations"
                @deleteConsultation="deleteConsultation"
        ></consultations-list>
    </div>
</template>

<script>
    import ConsultationsList from "@/components/ConsultationsList";
    import NewConsultationModal from "@/components/pages/NewConsultationModal";
    export default {
        components: {ConsultationsList, NewConsultationModal},
        data() {
            return {
                id: this.$route.params['id'],
                patient: {},
                newConsultation: false
            }
        },
        computed: {
            FIO: function(){
                return (this.patient.firstName+' '+this.patient.lastName +
                    (this.patient.patronymic!=null ?' ' + this.patient.patronymic :""))
            }
        },
        watch: {
            $route (toR, fromR){
                this.id = toR.params['id'];
            }
        },
        created(){
            this.fetchPatient();
        },
        methods: {
            async fetchPatient(){
                let vm = this;
                await fetch("https://localhost:44373/api/patient-management/patients/"+this.id)
                    .then(response=>response.json())
                    .then(data =>vm.patient=data);
            },
            editPatient(){
                console.dir(this.patient)
                alert('edit');
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
                    .then(data =>vm.patient.consultations=data);
            },
            async addedConsultation(){
                this.newConsultation = false;
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
            }
        }
    }
</script>