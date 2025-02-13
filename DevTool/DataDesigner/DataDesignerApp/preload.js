const { contextBridge, ipcRenderer } = require('electron');

// electronAPI
contextBridge.exposeInMainWorld('electronAPI', {
  selectDirectory: () => ipcRenderer.invoke('select-directory')
})