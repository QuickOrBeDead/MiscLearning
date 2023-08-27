const { app, BrowserWindow, BrowserView, ipcMain } = require('electron')
const fs = require('fs')
const path = require('path')

let _view
let _folder = '/home/boraa/Documents/Projects/Certificates/az204'
let _files = []
let _fileIndex = 0


const createWindow = () => {
  const win = new BrowserWindow({
    width: 1300,
    height: 900,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    }
  })

  win.setMenu(null)
  win.loadFile('index.html')

  win.webContents.on('did-finish-load', () => {
    _view = new BrowserView()
    resizeView(_view)
    
    win.addBrowserView(_view)

    loadFiles(_folder)
  })

  win.on('resize', () => {
    win.getBrowserViews().forEach((view) => {
      resizeView(view)
    })
  })

  function resizeView(view) {
    const bound = win.getBounds();
    const height = 55
    view.setBounds({ x: 0, y: height, width: bound.width, height: bound.height - height })
  }
}

const viewFile = function() {
  const file = _files[_fileIndex]

  if (file) {
    _view.webContents.loadFile(file)
  }
}

const loadFiles = function(folder) {
  _files = []
  _fileIndex = 0

  if (!folder || !fs.existsSync(folder)) {
    return
  }

  var items = fs.readdirSync(folder)
  items.sort()

  for (let i = 0; i < items.length; i++) {
      const filename = path.join(folder, items[i]);
      const stat = fs.lstatSync(filename);
      if (!stat.isDirectory() && /\.html$/.test(filename)) {
        _files.push(filename)
      }
  }

  if (_files.length) {
    viewFile()
  }
}

ipcMain.handle('prev', e => {
  _fileIndex--
  if (_fileIndex < 0) {
    _fileIndex = 0
  }

  viewFile()
})

ipcMain.handle('next', e => {
  _fileIndex++
  if (_fileIndex === _files.length) {
    _fileIndex = _files.length - 1
  }

  if (_fileIndex < 0) {
    _fileIndex = 0
  }

  viewFile()
})

app.whenReady().then(() => {
  createWindow()
})

process.on('SIGINT', function() {
  console.log( "\nShutting down from SIGINT\n" )

  process.exit(0)
})

