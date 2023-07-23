export interface Config {
    ytAudioServerPort: number;
}

export const config: Config = {
    ytAudioServerPort: import.meta.env.VITE_YT_AUDIO_SERVER_PORT
}