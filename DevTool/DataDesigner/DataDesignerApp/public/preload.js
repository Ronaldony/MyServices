const { contextBridge, ipcRenderer } = require('electron');

// electronAPI
contextBridge.exposeInMainWorld('electronAPI', {
  selectDirectory: () => ipcRenderer.invoke('select-directory'),
  setupApiBase: () => ipcRenderer.invoke('setup-api-base'),
})

