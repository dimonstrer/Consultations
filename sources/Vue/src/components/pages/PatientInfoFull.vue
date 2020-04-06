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

        <consultations-list :consultations="patient.consultations"></consultations-list>
    </div>
</template>

<script>
    import ConsultationsList from "@/components/ConsultationsList";
    export default {
        components: {ConsultationsList},
        data() {
            return {
                id: this.$route.params['id'],
                patient: {}
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
            deletePatient(){
                alert('delete');
            }
        }
    }
</script>