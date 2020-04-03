<template>
    <div>
        <table class="table">
            <caption>Консультации пациента</caption>
            <thead class="thead-light">
            <tr>
                <th width="15%">Дата посещения</th>
                <th width="20%">Время посещения</th>
                <th width="50%">Симптомы</th>
                <th></th>
            </tr>
            </thead>
            <tbody>
            <tr v-for="cons in consultations" :key="cons.consultationId">
                <td>{{cons.day | formatDateToYMD}}</td>
                <td>{{cons.time | formatDateToHH}}</td>
                <td>{{cons.symptoms}}</td>
            </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
    export  default {
        props: ['consultations']
    }
</script>