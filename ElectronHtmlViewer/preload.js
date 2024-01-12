const { contextBridge, ipcRenderer } = require('electron')

contextBridge.exposeInMainWorld(
  'api', {
    next: async () => await ipcRenderer.invoke('next'),
    prev: async () => await ipcRenderer.invoke('prev')
  }
)

ipcRenderer.on('fileUpdated', (event, data) => {
  document.querySelector('#files-current').innerHTML = data.current
  document.querySelector('#files-total').innerHTML = data.total
})