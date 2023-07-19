const express = require('express');
const cors = require('cors');
const ytdl = require('ytdl-core');

const port = process.env.PORT || 8080;

const app = express();

app.use(cors());

app.listen(port, () => {
    console.log(`Server Works !!! At port ${port}`);
});

app.get('/info/:videoID/', (req, res) => {
    try {
      const id = (req?.params?.videoID || '').replace(/[^A-Za-z0-9_\-]/g, '');
      ytdl
        .getInfo(id)
        .then(info => {
          const formats = {};
          info.formats
            .filter(file => file.mimeType.startsWith('audio'))
            .map(file => {
              formats[file.mimeType.split(';')[0]] = file.url;
            });
  
          res.send({
            url: ytdl.chooseFormat(info.formats, { filter: 'audioonly' }).url,
            formats,
            author: info.videoDetails.author.name,
            title: info.videoDetails.title,
            description: info.videoDetails.description,
            images: info.player_response.videoDetails.thumbnail.thumbnails,
          });
        })
        .catch(e => {
          console.log(e);
          return res.status(400).send({
            url: '',
            author: '',
            title: '',
          });
        });
    } catch (e) {
      res.status(500).send({
        error: 'unexpected server error',
        reason: '',
      });
    }
  });
