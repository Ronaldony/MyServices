const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const url = require('url');
const path = require('path');

let mainWindow;

function createMainWindow() {
  mainWindow = new BrowserWindow({
    title: 'My buddy app',
    width: 1700,
    height: 800,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: true,
      preload: path.join(__dirname, 'preload.js'),
    },
  });

  mainWindow.webContents.openDevTools();

  const startUrl = url.format({
    pathname: path.join(__dirname, './app/build/index.html'),
    protocol: 'file',
  });

  mainWindow.loadURL('http://localhost:3000');
  // mainWindow.loadURL(startUrl);
}

// execute server.
app.whenReady().then(() => {
  createMainWindow();

  // select directory
  ipcMain.handle('select-directory', async () => {
    const result = await dialog.showOpenDialog({
      properties: ['openDirectory'],
    })

    // return folder path. 
    if (!result.canceled && result.filePaths.length > 0)
      return result.filePaths[0]

    // no selection.
    return null;
  })
});