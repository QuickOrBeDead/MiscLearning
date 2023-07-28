export interface YtAudio {
    id?: string;
    url: string;
    formats?: Array<YtAudioFormat>;
    author: string;
    description?: string;
    title: string;
    images?: Array<YtAudioImage>;
    isLive?: boolean;
}

export interface YtAudioFormat {
  mimeType: string,
  audioBitrate: number,
  audioQuality: string,
  bitrate: number,
  contentLength: number,
  hasAudio: boolean,
  hasVideo: boolean,
  isLive: boolean,
  isHLS: boolean,
  itag: boolean,
  quality: number,
  url: string
}

export interface YtAudioImage {
  width: number;
  height: number;
  url: string;
}

export interface AudioContent {
  title: string;
  description?: string;
  author: string;
  width: number;
  image?: YtAudioImage;
  audioSources: Array<AudioControlSource>;
}

export interface AudioControlSource {
    src: string,
    mimeType: string
}