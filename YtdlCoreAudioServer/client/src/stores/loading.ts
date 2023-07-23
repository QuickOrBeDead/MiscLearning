import { defineStore } from 'pinia'

export const useLoadingStore = defineStore('loading', {
  state: () => {
    return { loading: false }
  },
  actions: {
    enable() {
        this.loading = true
    },
    disable() {
        this.loading = false
    }
  }
})