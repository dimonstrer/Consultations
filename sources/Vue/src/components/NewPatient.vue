<template>
    <div>
        <h3 class="text-center">Новый пациент</h3>
        <form >
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
                <div v-if="!$v.patient.lastName.required" class="invalid-feedback">Не указана фамилия пациента</div>
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
                <input
                        class="form-control"
                        :class="{'is-invalid': $v.patient.birthDate.$error}"
                        type="text"
                        id="birthDate"
                        v-model="patient.birthDate"
                        @blur="$v.patient.birthDate.$touch()"
                >
                <div v-if="!$v.patient.birthDate.required" class="invalid-feedback">Не указана дата рождения пациента</div>
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
                <input
                        class="form-control"
                        :class="{'is-invalid': $v.patient.pensionNumber.$error}"
                        type="text"
                        id="pensionNumber"
                        v-model="patient.pensionNumber"
                        @blur="$v.patient.pensionNumber.$touch()"
                >
                <div v-if="!$v.patient.pensionNumber.required" class="invalid-feedback">Не указан СНИЛС пациента</div>
            </div>

            <input class="btn btn-success" type="submit" value="Создать">
        </form>

    </div>
</template>

<script>
    import {required} from "vuelidate/lib/validators";

    export default {
        data() {
            return {
                patient: {
                    firstName: '',
                    lastName: '',
                    patronymic: '',
                    birthDate: Date(),
                    gender: '',
                    pensionNumber: ''
                },
                genderOptions: ['Мужской','Женский']
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
                    required
                }
            }
        }
    }
</script>