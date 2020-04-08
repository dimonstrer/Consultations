<template>
    <div class="modal-mask">
        <div class="modal-wrapper">
            <div class="modal-container">
                <div>
                    <h3 class="text-center">Изменение консультации</h3>
                    <form @submit.prevent="validateBeforeSubmit">
                        <div class="form-group">
                            <label for="day">Дата*</label>
                            <br>
                            <date-picker
                                    v-model="editedConsultation.day"
                                    format="DD/MM/YYYY"
                                    :input-class="['form-control',{'is-invalid': $v.editedConsultation.day.$error}]"
                                    id="day"
                                    name="day"
                                    @blur="$v.editedConsultation.day.$touch()"
                            ></date-picker>
                            <div
                                    v-if="!$v.editedConsultation.day.required"
                                    class="invalid-feedback"
                                    :style="[{'display' : dateErrorState}]"
                            >Не указана дата консультации</div>
                        </div>

                        <div class="form-group">
                            <label for="time">Время*</label>
                            <input
                                    v-model="editedConsultation.time"
                                    id="time"
                                    name="time"
                                    type="time"
                                    class="form-control"
                                    :class="{'is-invalid': $v.editedConsultation.time.$error}"
                                    @blur="$v.editedConsultation.time.$touch()"
                            >
                            <div v-if="!$v.editedConsultation.time.required" class="invalid-feedback">Не указано время консультации</div>
                        </div>

                        <div class="form-group">
                            <label for="symptoms">Симптомы</label>
                            <input
                                    v-model="editedConsultation.symptoms"
                                    id="symptoms"
                                    name="symptoms"
                                    type="textarea"
                                    class="form-control"
                            >
                        </div>
                        <div class="text-center mt-2">
                            <input class="btn btn-success" type="submit" value="Изменить">
                            <input type="button" class="btn btn-danger" value="Закрыть" @click="$emit('close')">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import {required} from "vuelidate/lib/validators";
    import DatePicker from "vue2-datepicker";
    export default {
        props: ['consultation'],
        data(){
            return {
                editedConsultation: {}
            }
        },
        computed: {
            dateErrorState() {
                return this.$v.consultation.day.$error ? 'block' : 'none';
            }
        },
        created(){
            this.editedConsultation = Object.assign({},this.consultation)
            console.log(this.editedConsultation)
            this.editedConsultation.day = new Date(this.editedConsultation.day);
            this.editedConsultation.time = new Date(this.editedConsultation.time+'Z');
            let rawHours = this.editedConsultation.time.getHours();
            let rawMinutes = this.editedConsultation.time.getMinutes();
            let hours = (rawHours<10 ? '0'+rawHours: rawHours);
            let minutes = (rawMinutes<10 ? '0'+rawMinutes: rawMinutes);
            this.editedConsultation.time = hours+':'+minutes;
        },
        methods: {
            async validateBeforeSubmit() {
                this.$v.$touch()
                if (!this.$v.$invalid){
                    let parsedTime = this.editedConsultation.time.split(':');
                    console.log(parsedTime);
                    this.editedConsultation.time = new Date(new Date().setHours(parsedTime[0],parsedTime[1]));
                    let response = await fetch('https://localhost:44373/api/consultation-management/consultations/'+this.consultation.consultationId, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify(this.editedConsultation)
                    });
                    let result = await response.json();
                    if(result.isSuccess) {
                        alert('Консультация успешно изменена');
                        this.$emit('edited');
                    }
                    else {
                        alert(result.errorMessage);
                    }
                }
            }
        },
        validations: {
            editedConsultation: {
                day: {
                    required
                },
                time: {
                    required
                }
            }
        },
        components: {
            DatePicker
        }
    }
</script>

<style>
    .modal-mask {
        position: fixed;
        z-index: 1;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, 0.5);
        display: table;
        transition: opacity 0.3s ease;
    }

    .modal-wrapper {
        display: table-cell;
        vertical-align: middle;
    }

    .modal-container {
        width: 600px;
        height: auto;
        margin: 0px auto;
        padding: 20px 30px;
        background-color: #fff;
        border-radius: 2px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.33);
        transition: all 0.3s ease;
        font-family: Helvetica, Arial, sans-serif;
    }
</style>