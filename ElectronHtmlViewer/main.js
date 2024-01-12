const { app, BrowserWindow, BrowserView, ipcMain } = require('electron')
const fs = require('fs')
const path = require('path')

let _win
let _view
let _folder = '/home/boraa/Documents/AZ-400-Learning'
let _files = []
let _fileIndex = 0


const createWindow = () => {
  const win = _win = new BrowserWindow({
    width: 1500,
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

  _win.webContents.send('fileUpdated', { current: _fileIndex + 1, total: _files ? _files.length : 0 })
}

const loadFiles = function(folder) {
  _files = []
  _fileIndex = 0


  const addFiles = function(dir) {
    if (!dir || !fs.existsSync(dir)) {
      return
    }
  
    const items = fs.readdirSync(dir)
    items.sort()

    const dirs = []

    for (let i = 0; i < items.length; i++) {
      const filename = path.join(dir, items[i])
      const stat = fs.lstatSync(filename)

      if (stat.isDirectory()) {
        dirs.push(filename)
      } else if (/\.html$/.test(filename)) {
        _files.push(filename)
      }
    }

    dirs.sort()

    for (let i = 0; i < dirs.length; i++) {
      addFiles(dirs[i]) 
    }
  }

  addFiles(folder)

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

