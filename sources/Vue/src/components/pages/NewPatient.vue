<template>
    <div>
        <h3 class="text-center">Новый пациент</h3>
        <form @submit.prevent="validateBeforeSubmit">
            <div class="form-group">
                <label for="firstName">Имя*</label>
                <input
                        class="form-control"
                        :class="{'is-invalid': $v.patient.firstName.$error}"
                        type="text"
                        id="firstName"
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
                        v-model="patient.lastName"
                        @blur="$v.patient.lastName.$touch()"
                >
                <div v-if="!$v.patient.lastName.required"
                     class="invalid-feedback">Не указана фамилия пациента</div>
            </div>

            <div class="form-group">
                <label for="patronymic">Отчество (если присутствует)</label>
                <input
                        class="form-control"
                        type="text"
                        id="patronymic"
                        v-model="patient.patronymic"
                        @blur="$v.patient.patronymic.$touch()"
                >
            </div>

            <div class="form-group">
                <label for="birthDate">Дата рождения*</label>
                <br>

                <date-picker
                    v-model="patient.birthDate"
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
                        v-model="patient.gender"
                        @blur="$v.patient.gender.$touch()"
                >
                    <option selected="selected" disabled="disabled">Выберите пол</option>
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
                        v-model="patient.pensionNumber"
                        @blur="$v.patient.pensionNumber.$touch()"
                ></masked-input>
                <div v-if="!$v.patient.pensionNumber.required" class="invalid-feedback">Не указан СНИЛС пациента</div>
                <div v-if="!$v.patient.pensionNumber.isValidPension" class="invalid-feedback">Неверно введен СНИЛС</div>
            </div>

            <input class="btn btn-success" type="submit" value="Создать">
        </form>
    </div>
</template>

<script>
    import {required} from "vuelidate/lib/validators";
    import DatePicker from 'vue2-datepicker';
    import 'vue2-datepicker/index.css';
    import 'vue2-datepicker/locale/ru'
    import MaskedInput from 'vue-text-mask'

    export default {
        data() {
            return {
                patient: {
                    firstName: '',
                    lastName: '',
                    patronymic: '',
                    birthDate: {},
                    gender: '',
                    pensionNumber: ''
                },
                genderOptions: ['Мужской','Женский']
            }
        },
        computed: {
            dateErrorState() {
                return this.$v.patient.birthDate.$error ? 'block' : 'none';
            }
        },
        methods:{
            async validateBeforeSubmit() {
                this.$v.$touch()
                if (!this.$v.$invalid){
                    let response = await fetch('https://localhost:44373/api/patient-management/patients/', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json;charset=utf-8'
                        },
                        body: JSON.stringify(this.patient)
                    });
                    let result = await response.json();
                    if(result.ok) {
                        alert('Пациент успешно добавлен');
                        this.$router.push('home')
                    }
                    else {
                        alert(result.errorMessage);
                    }
                }
            },
            checkHash(sum, checkSum) {
                if (sum < 100)
                    return sum == +checkSum;
                if (sum == 100 || sum == 101)
                    return checkSum == "00";
                if (sum > 100)
                    return this.checkHash(sum % 101, checkSum);
            }
        },
        components: {
            DatePicker,
            MaskedInput
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
