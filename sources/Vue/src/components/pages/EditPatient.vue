<template>
    <div>
        <h3 class="text-center">Редактирование данных пациента</h3>
        <form @submit.prevent="validateBeforeSubmit">
            <div class="form-group">
                <label for="firstName">Имя</label>
                <input
                        class="form-control"
                        :class="{'is-invalid': $v.patient.firstName.$error}"
                        id="firstName"
                        type="text"
                        :disabled="isDisabled"
                        v-model="patient.firstName"
                        @blur="$v.patient.firstName.$touch()"
                >
                <div v-if="!$v.patient.firstName.required" class="invalid-feedback">Не указано имя пациента</div>
            </div>
            <div class="form-group">
                <label for="lastName">Фамилия*</label>
                <input
                        class="form-control"
                        :class="{'is-invalid': $v.patient.lastName.$error}"
                        type="text"
                        id="lastName"
                        :disabled="isDisabled"
                        v-model="patient.lastName"
                        @blur="$v.patient.lastName.$touch()"
                >
                <div v-if="!$v.patient.lastName.required" class="invalid-feedback">Не указана фамилия пациента</div>
            </div>
            <div class="form-group">
                <label for="patronymic">Отчество (если присутствует)</label>
                <input
                        class="form-control"
                        type="text"
                        id="patronymic"
                        :disabled="isDisabled"
                        v-model="patient.patronymic"
                        @blur="$v.patient.patronymic.$touch()"
                >
            </div>
            <div class="form-group">
                <label for="birthDate">Дата рождения*</label>
                <br>
                <date-picker
                        v-model="patient.birthDate"
                        :disabled="isDisabled"
                        format="DD/MM/YYYY"
                        :input-class="['form-control',{'is-invalid': $v.patient.birthDate.$error}]"
                        id="birthDate"
                        @blur="$v.patient.birthDate.$touch()"
                ></date-picker>
                <span :class="{'is-invalid': $v.patient.birthDate.$error}"></span>
                <div
                        v-if="!$v.patient.birthDate.required"
                        class="invalid-feedback"
                        :style="[{'display' : dateErrorState}]"
                >Не указана дата рождения пациента</div>
            </div>
            <div class="form-group">
                <label for="gender">Пол*</label>
                <select
                        class="form-control"
                        :class="{'is-invalid': $v.patient.gender.$error}"
                        type="text"
                        id="gender"
                        :disabled="isDisabled"
                        v-model="patient.gender"
                        @blur="$v.patient.gender.$touch()"
                >
                    <option v-for="opt in genderOptions" :key="opt">{{opt}}</option>
                </select>
                <div v-if="!$v.patient.gender.required" class="invalid-feedback">Не указан пол пациента</div>
            </div>
            <div class="form-group">
                <label for="pensionNumber">СНИЛС*</label>
                <masked-input
                        :mask="[/\d/,/\d/,/\d/,'-',/\d/,/\d/,/\d/,'-',/\d/,/\d/,/\d/,' ',/\d/,/\d/]"
                        class="form-control"
                        :class="{'is-invalid': $v.patient.pensionNumber.$error}"
                        type="text"
                        id="pensionNumber"
                        :disabled="isDisabled"
                        v-model="patient.pensionNumber"
                        @blur="$v.patient.pensionNumber.$touch()"
                ></masked-input>
                <div v-if="!$v.patient.pensionNumber.required" class="invalid-feedback">Не указан СНИЛС пациента</div>
                <div v-if="!$v.patient.pensionNumber.isValidPension" class="invalid-feedback">Неверно введен СНИЛС</div>
            </div>
            <input
                    :disabled="isDisabled"
                    class="btn btn-success"
                    type="submit"
                    value="Изменить"
            >
        </form>
    </div>
</template>
<script>
    import {required} from "vuelidate/lib/validators";
    import DatePicker from "vue2-datepicker";
    import moment from "moment";
    import MaskedInput from 'vue-text-mask'


    export default {
        data() {
            return {
                id: this.$route.params['id'],
                patient: {
                    firstName: '',
                    lastName: '',
                    patronymic: '',
                    birthDate: {},
                    gender: '',
                    pensionNumber: ''
                },
                disabled: true,
                genderOptions: ['Мужской','Женский']
            }
        },
        computed: {
            isDisabled() {
                return this.disabled;
            },
            dateErrorState() {
                return this.$v.patient.birthDate.$error ? 'block' : 'none';
            }
        },
        watch: {
            $route (toR, fromR){
                this.id = toR.params['id'];
            }
        },
        created(){
            this.getPatient();
        },
        components: {
            DatePicker,
            MaskedInput
        },
        methods: {
            async getPatient() {
                let vm = this;
                await fetch("https://localhost:44373/api/patient-management/patients/" + this.id)
                    .then(response => response.json())
                    .then(data => vm.patient = data);
                this.patient.birthDate = new Date(this.patient.birthDate);
                this.disabled = false;
            },
            checkHash(sum, checkSum) {
                if (sum < 100)
                    return sum == +checkSum;
                if (sum == 100 || sum == 101)
                    return checkSum == "00";
                if (sum > 100)
                    return this.checkHash(sum % 101, checkSum);
            },
            async validateBeforeSubmit() {
                this.$v.$touch()
                if (!this.$v.$invalid){
                    let response = await fetch('https://localhost:44373/api/patient-management/patients/'+this.id, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify(this.patient)
                    });
                    let result = await response.json();
                    if(result.ok) {
                        alert('Пациент успешно изменен');
                        this.$router.push('home')
                    }
                    else {
                        alert(result.errorMessage);
                    }
                }
            }
        },
        validations: {
            patient: {
                firstName: {
                    required
                },
                lastName: {
                    required
                },
                birthDate: {
                    required
                },
                gender: {
                    required
                },
                pensionNumber: {
                    required,
                    isValidPension(pension) {
                        let parsed = pension.split('-').join('').split(' ').join('');
                        let checkSum = parsed.slice(9, 11);
                        let sum = 0;
                        for (let i = 0, j = 9; i < parsed.length - 2; i++ , j--)
                            sum += +parsed[i] * j;
                        return this.checkHash(sum, checkSum);
                    }
                }
            }
        }
    }
</script>