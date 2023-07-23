<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useLoadingStore } from '../stores/loading'

const { loading } = storeToRefs(useLoadingStore())

</script>
<template>
    <div v-if="loading" class="overlay-wrapper">
        <div class="overlay">
            <div class="d-flex justify-content-center">
                <div class="spinner-border" role="status"></div>
            </div>
        </div>
    </div>
</template>