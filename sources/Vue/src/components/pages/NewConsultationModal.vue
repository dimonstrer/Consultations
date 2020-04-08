<template>
    <div class="modal-mask">
        <div class="modal-wrapper">
            <div class="modal-container">
                <div>
                    <h3 class="text-center">Новая консультация</h3>
                    <form @submit.prevent="validateBeforeSubmit">
                        <div class="form-group">
                            <label for="day">Дата*</label>
                            <br>
                            <date-picker
                                    v-model="consultation.day"
                                    format="DD/MM/YYYY"
                                    :input-class="['form-control',{'is-invalid': $v.consultation.day.$error}]"
                                    id="day"
                                    name="day"
                                    @blur="$v.consultation.day.$touch()"
                            ></date-picker>
                            <div
                                    v-if="!$v.consultation.day.required"
                                    class="invalid-feedback"
                                    :style="[{'display' : dateErrorState}]"
                            >Не указана дата консультации</div>
                            <div
                                    v-if="!$v.consultation.day.between"
                                    class="invalid-feedback"
                                    :style="[{'display' : dateErrorState}]"
                            >Дата консультации должна быть в диапазоне от 01.01.1900 до {{currentDate}}</div>
                        </div>

                        <div class="form-group">
                            <label for="time">Время*</label>
                            <input
                                    v-model="consultation.time"
                                    id="time"
                                    name="time"
                                    type="time"
                                    class="form-control"
                                    :class="{'is-invalid': $v.consultation.time.$error}"
                                    @blur="$v.consultation.time.$touch()"
                            >
                            <div v-if="!$v.consultation.time.required" class="invalid-feedback">Не указано время консультации</div>
                        </div>

                        <div class="form-group">
                            <label for="symptoms">Симптомы</label>
                            <input
                                    v-model="consultation.symptoms"
                                    id="symptoms"
                                    name="symptoms"
                                    type="textarea"
                                    class="form-control"
                            >
                        </div>
                        <div class="text-center mt-2">
                            <input class="btn btn-success" type="submit" value="Добавить">
                            <input type="button" class="btn btn-danger" value="Закрыть" @click="$emit('close')">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import {between, required} from "vuelidate/lib/validators";
    import DatePicker from "vue2-datepicker";
    import moment from "moment";
    export default {
        props: ['id'],
        data(){
            return {
                consultation: {
                    patientId: +this.id,
                    day: {},
                    time: '',
                    symptoms: ''
                },
                currentDate: moment(new Date()).format('DD.MM.YYYY')
            }
        },
        watch: {
            $route (toR, fromR){
                this.id = toR.params['id'];
            }
        },
        computed: {
            dateErrorState() {
                return this.$v.consultation.day.$error ? 'block' : 'none';
            }
        },
        methods: {
            async validateBeforeSubmit() {
                this.$v.$touch()
                if (!this.$v.$invalid){
                    let parsedTime = this.consultation.time.split(':');
                    console.log(parsedTime);
                    this.consultation.time = new Date(new Date().setHours(parsedTime[0],parsedTime[1]));
                    let response = await fetch('https://localhost:44373/api/consultation-management/consultations/', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify(this.consultation)
                    });
                    let result = await response.json();
                    if(result.isSuccess) {
                        alert('Консультация успешно добавлена');
                        this.$emit('add');
                    }
                    else {
                        alert(result.errorMessage);
                    }
                }
            }
        },
        validations: {
            consultation: {
                day: {
                    required,
                    between: between(new Date().setFullYear(1899,11,31), new Date())
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