export {}

declare global {
    interface Array<T> {
        sum(mapFn?: (v: T) => number): number

        count(): number
    }
}

const sumReduceFn = (accumulator: number, currentValue: number): number => accumulator + currentValue

Array.prototype.sum = function<T>(mapFn: (v: T) => number): number {
    return (this as Array<T>).map(v => mapFn(v)).reduce(sumReduceFn, 0)
}

Array.prototype.sum = function<T extends number>(): number {
    return (this as Array<T>).reduce(sumReduceFn, 0)
}

Array.prototype.count = function<T>(): number {
    return (this as Array<T>).length
}