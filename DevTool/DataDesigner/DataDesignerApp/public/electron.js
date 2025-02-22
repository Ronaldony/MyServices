const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const url = require('url');
const path = require('path');

let mainWindow;

// createMainWindow.
function createMainWindow() {
  // Browse window.
  mainWindow = new BrowserWindow({
    title: 'DataDesigner',
    width: 1700,
    height: 800,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: true,
      preload: path.join(__dirname, 'preload.js'),
    },
  });

  const startUrl = url.format({
    pathname: path.join(__dirname, '../build/index.html'),
    protocol: 'file',
  });
  
  if (process.env.NODE_ENV === 'development') {
    // development.
    mainWindow.webContents.openDevTools();
    mainWindow.loadURL('http://localhost:3000');
  } 
  else {
    // Production
    mainWindow.loadURL('http://localhost:3000');
    // mainWindow.loadURL(startUrl);
  }
}

// execute server.
app.whenReady().then(() => {
  createMainWindow();

  // select directory
  ipcMain.handle('select-directory', async () => {
    const result = await dialog.showOpenDialog({
      properties: ['openDirectory'],
    });

    // return folder path.
    if (!result.canceled && result.filePaths.length > 0)
      return result.filePaths[0];

    // no selection.
    return null;
  });
  
  // select directory
  ipcMain.handle('setup-api-base', async () => {
    let spawn = require("child_process").spawn;

    let bat = spawn("cmd.exe", [
      ""
    ]);

    return 'http://localhost:7000';
  });
});

// Mac인 경우 모든 창이 닫히면 앱 종료.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
