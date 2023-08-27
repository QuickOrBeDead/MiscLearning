const { app, BrowserWindow, BrowserView } = require('electron')

const createWindow = () => {
  const win = new BrowserWindow({
    width: 1324,
    height: 992
  })

  win.setMenu(null)
  win.loadFile('index.html')

  win.webContents.on('did-finish-load', () => {
    const view = new BrowserView()
    view.webContents.loadFile("/home/boraa/Documents/Projects/Certificates/az204/0001_00001.html")
    
    const bound = win.getBounds()
    const height = 55
    view.setBounds({ x: 0, y: height, width: bound.width, height: bound.height - height });
    
    win.addBrowserView(view)
  })
}

app.whenReady().then(() => {
  createWindow()
})

process.on('SIGINT', function() {
  console.log( "\nShutting down from SIGINT\n" )

  process.exit(0)
})

