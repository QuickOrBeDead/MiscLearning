export interface Config {
    ytAudioServer: string;
}

export const config: Config = {
    ytAudioServer: import.meta.env.VITE_YT_AUDIO_SERVER
}