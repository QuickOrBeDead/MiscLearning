const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld(
  'api', {
    next: async () => await ipcRenderer.invoke('next'),
    prev: async () => await ipcRenderer.invoke('prev')
  }
)