<template>
    <router-link :to="'/patients/'+id"><slot></slot></router-link>
</template>

<script>
    export default {
        props: ['id']
    }
</script>