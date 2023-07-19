<script setup lang="ts">
import { ref } from 'vue'
import { AudioControlSource, YtAudio } from '../types'
import { apiFetch } from '../apiFetch';

let videoUrl = ref('')
const audio = ref<HTMLAudioElement>()
const sources = ref<AudioControlSource[]>()

async function go() {
    if (videoUrl?.value) {
        const videoId = parseVideoId(videoUrl?.value)
        const audioInfo = await apiFetch.get<YtAudio>(`http://localhost:8888/info/${videoId}`) 
        const formats: AudioControlSource[] = Object.entries(audioInfo.formats as Record<string, string>)
            .map(arr => { return { src: arr[1], mimeType: arr[0] } })
        
        sources.value = formats
    }
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
            <div class="col-12">
                <div class="input-group">
                    <input type="text" v-model="videoUrl" class="form-control form-control-lg" placeholder="Youtube Video Url" aria-label="Youtube Video Url">
                    <button type="button" @click="go" class="btn btn-primary">Go</button>
                </div>
            </div>
        </div>
        <div class="row" v-if="sources?.length">
            <div class="col-12 p-3">
                <audio controls ref="audio">
                    <template v-for="source in sources">
                        <source :src="source.src" :type="source.mimeType">
                    </template>
                    Your browser does not support the audio element.
                </audio>
            </div>
        </div>
    </div>
</template>