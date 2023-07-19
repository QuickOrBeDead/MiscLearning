export interface YtAudio {
    id?: string;
    url: string;
    formats?: Record<string, string>;
    author: string;
    description?: string;
    title: string;
    images?: Array<{
      width: number;
      height: number;
      url: string;
    }>;
}

export interface AudioControlSource {
    src: string,
    mimeType: string
}