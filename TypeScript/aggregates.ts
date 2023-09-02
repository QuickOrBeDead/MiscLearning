export {}

declare global {
    interface Array<T> {
        sum(mapFn: (v: T) => number): number

        avg(mapFn: (v: T) => number): number

        count(): number
    }
}

Array.prototype.sum = function<T>(mapFn: (v: T) => number): number {
    return (this as Array<T>).map(v => mapFn(v)).reduce((accumulator: number, currentValue: number): number => accumulator + currentValue, 0)
}

Array.prototype.count = function<T>(): number {
    return (this as Array<T>).length
}

Array.prototype.avg = function<T>(mapFn: (v: T) => number): number {
    const arr = this as Array<T>
    const count = arr.count()
    return count === 0 ? 0 : arr.sum(mapFn) / count
}
