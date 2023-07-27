<script setup lang="ts">
import { ref } from 'vue'
import { AudioContent, AudioControlSource, YtAudio, YtAudioFormat, YtAudioImage } from '../types'
import { apiFetch } from '../apiFetch'
import { config } from '../config'
import { useLoadingStore } from '../stores/loading'
import miniToastr from 'mini-toastr'
import { getErrorMessage } from '../common'

const loading = useLoadingStore()

let videoUrl = ref('')
const audio = ref<HTMLAudioElement>()
const audioInfo = ref<AudioContent>()

enum AudioQuality {
  Low = 1,
  High
}

async function go() {
    if (videoUrl?.value) {
        try {
            loading.enable()

            const videoId = parseVideoId(videoUrl?.value)
            if (!videoId) {
                miniToastr.warn('Invalid Youtube Video Url', 'Warning')
                return
            }
            
            const ytAudioInfo = await apiFetch.get<YtAudio>(`${window.location.protocol}//${window.location.hostname}:${config.ytAudioServerPort}/info/${videoId}`) 
            const image = chooseImage(ytAudioInfo.images)

            audioInfo.value = {
                title: ytAudioInfo.title,
                description: ytAudioInfo.description,
                author: ytAudioInfo.author,
                image: image,
                audioSources: chooseFormats(ytAudioInfo, AudioQuality.Low),
                width: chooseWidth(image)
            }

            audio.value?.load()

            if (navigator && navigator.mediaSession) {
                const artwork: MediaImage[] = []

                if (image) {
                    artwork.push({
                        src: image.url,
                        sizes: `${image.height}x${image.width}`,
                        type: 'image/png'
                    })
                }

                navigator.mediaSession.metadata = new MediaMetadata({
                    title: ytAudioInfo.title,
                    artist: ytAudioInfo.author,
                    album: 'Youtube Audio Player',
                    artwork: artwork
                })

                navigator.mediaSession.setActionHandler('play', () => {
                    audio.value?.play();
                })
            }

            loading.disable()
        } catch (err) {
            loading.disable()

            miniToastr.error(getErrorMessage(err), 'Error')
        } finally {
            loading.disable()
        }
    } else {
        miniToastr.warn('Youtube Video Url is required', 'Warning')
    }
}

function chooseWidth(image: YtAudioImage | undefined): number {
    if (!image || image.width < 300) {
        return 300
    }

    return image.width
}

function chooseImage(images: Array<YtAudioImage> | undefined) {
    if (!images) {
        return undefined
    }

    if (images.length > 3) {
        return images[3]
    }

    if (images.length > 2) {
        return images[2]
    }

    if (images.length > 1) {
        return images[1]
    }

    if (images.length > 0) {
        return images[0]
    }

    return undefined
}

function chooseFormats(ytAudioInfo: YtAudio, quality: AudioQuality): AudioControlSource[] {
    if (ytAudioInfo.formats) {
        return chooseByQuality(groupByFormats(ytAudioInfo.formats), quality);
    }
    
    return [];
}

function chooseByQuality(formats: Record<string, YtAudioFormat[]>, quality: AudioQuality): AudioControlSource[] {
    const result: AudioControlSource[] = [];
    for (const format in formats) {
        let values = formats[format];
        if (values) {
            values = values.sort((a, b) => (a.bitrate - b.bitrate) * (quality == AudioQuality.High ? -1 : 1))

            if (values.length) {
                result.push({ mimeType: format, src: values[0].url })
            }
        }
    }

    return result
}

function groupByFormats(formats: Array<YtAudioFormat>): Record<string, Array<YtAudioFormat>> {
    if (formats.length === 1) {
        const result: Record<string, Array<YtAudioFormat>> = { }
        result[formats[0].mimeType] = [formats[0]]
        return result
    }

    let firstFormat = true;
    return formats.reduce((acc, format) => {
        const { mimeType } = format
        const group = firstFormat ? {} : acc as any
        firstFormat = false
        group[mimeType] = group[mimeType] ?? []
        group[mimeType].push(format)
        return group
    }) as any as Record<string, Array<YtAudioFormat>>
}

function parseVideoId(url: string = ''): string {
  if (!url) {
    return ''
  }

  try {
    const re = /(https?:\/\/)?(((m|www)\.)?(youtube(-nocookie)?|youtube.googleapis)\.com.*(v\/|v=|vi=|vi\/|e\/|embed\/|user\/.*\/u\/\d+\/)|youtu\.be\/)([_0-9a-z-]+)/i
    
    const matches =url.match(re)
    const id = matches && matches.length > 8 ? matches[8] : ''
    return typeof id === 'string' ? id : ''
  } catch (e) {
    return ''
  }
}
</script>
<template>
    <div class="container">
        <div class="row">
            <div class="col-12" style="margin: 0; padding: 0;">
                <div class="input-group" v-on:keyup.enter="go">
                    <input type="text" v-model="videoUrl" class="form-control form-control-lg" placeholder="Youtube Video Url" aria-label="Youtube Video Url">
                    <button type="button" @click="go" class="btn btn-primary">Go</button>
                </div>
            </div>
        </div>
        <div class="row justify-content-center" v-if="audioInfo?.audioSources?.length">
            <div class="col-12 mt-3" style="margin: 0; padding: 0;">
                <div class="card text-center" :style="{width: audioInfo.width + 'px'}">
                    <img v-if="audioInfo?.image" :src="audioInfo.image.url" alt="..." :width="audioInfo.image.width" :height="audioInfo.image.height">
                    <div class="card-body">
                        <audio controls ref="audio">
                            <template v-for="source in audioInfo.audioSources">
                                <source :src="source.src" :type="source.mimeType">
                            </template>
                            Your browser does not support the audio element.
                        </audio>
                        <h5 class="card-title">{{ audioInfo?.title }}</h5>
                        <p class="card-text">{{ audioInfo?.description }}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>