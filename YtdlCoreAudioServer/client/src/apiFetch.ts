export const apiFetch = {
    get: <TResponse>(url: string) => request<TResponse>(url)
}

class FetchError extends Error {
   constructor(message: string) {
    super(message)

    this.name = "FetchError"
   }
}

async function request<TResponse>(url: string, config?: RequestInit): Promise<TResponse> {
    const response = await fetch(url, config)
    if (!response.ok) {
        const info = await response.json()
        throw new FetchError(info && info.reason ? info.reason : 'fetch error')
    }
    return await response.json()
}