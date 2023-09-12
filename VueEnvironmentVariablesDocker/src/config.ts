export interface Config {
    variable1: string
    variable2: string
}

export const config: Config = {
    variable1: import.meta.env.VITE_VARIABLE1,
    variable2: import.meta.env.VITE_VARIABLE2
}