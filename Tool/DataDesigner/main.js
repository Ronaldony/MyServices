const { app, BrowserWindow, ipcMain } = require('electron/main')
const path = require('node:path')

const createWindow = () => {
    const win = new BrowserWindow({
        width: 1300,
        height: 800,
        // attach preload.js.
        webPreferences: {
          preload: path.join(__dirname, 'preload.js')
        }
    })
    
  ipcMain.on('set-title', (event, title) => {
    const webContents = event.sender
    const win = BrowserWindow.fromWebContents(webContents)
    win.setTitle(title)
  })

    win.loadFile('index.html')
}

// Ready.
app.whenReady().then(() => {
    // Create window.
    createWindow()

    // Activate window.
    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) createWindow()
    })
})

// entirely quit.
app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit()
})