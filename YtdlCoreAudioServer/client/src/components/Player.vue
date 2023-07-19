<script setup lang="ts">
import { onMounted } from 'vue'
import { YtAudio } from '../types'
import { apiFetch } from '../apiFetch';

onMounted(async () => {
    const audio = await apiFetch.get<YtAudio>("http://localhost:8888/info/QFgTPjYBmlU") 
    console.log(audio)
})

</script>
<template>

</template>