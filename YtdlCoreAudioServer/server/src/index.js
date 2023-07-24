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
          const formats = [];
          info.formats
            .filter(file => file.mimeType && file.mimeType.startsWith('audio'))
            .map(file => {
              if (file.url) {
                formats.push({
                  mimeType: file.mimeType.split(';')[0],
                  audioBitrate: file.audioBitrate,
                  audioQuality: file.audioQuality,
                  bitrate: file.bitrate,
                  contentLength: file.contentLength,
                  hasAudio: file.hasAudio,
                  hasVideo: file.hasVideo,
                  isLive: file.isLive,
                  isHLS: file.isHLS,
                  itag: file.itag,
                  quality: file.quality,
                  url: file.url
                });
              }
            });
  
          res.send({
            formats,
            author: info.videoDetails.author.name,
            title: info.videoDetails.title,
            description: info.videoDetails.description,
            images: info.player_response.videoDetails.thumbnail.thumbnails,
            duration: info.videoDetails.lengthSeconds
          });
        })
        .catch(e => {
          console.log(e);

          if (
            (e.constructor && e.constructor.name === 'UnrecoverableError') ||
            (e.errno === -3001))
          {
            return res.status(500).send({
              reason: e.message
            });
          }
          
          return res.status(400).send({
            url: '',
            author: '',
            title: ''
          });
        });
    } catch (e) {
      res.status(500).send({
        error: 'unexpected server error',
        reason: ''
      });
    }
  });
