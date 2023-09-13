<script setup lang="ts">
import { config } from '../config'

defineProps<{ msg: string }>()

</script>

<template>
  <h1>{{ msg }}</h1>

  <div class="card">  
    <p>VITE_VARIABLE1: {{ config.variable1 }} </p>
    <p>VITE_VARIABLE2: {{ config.variable2 }} </p>
  </div>
</template>

<style scoped>
.read-the-docs {
  color: #888;
}
</style>
