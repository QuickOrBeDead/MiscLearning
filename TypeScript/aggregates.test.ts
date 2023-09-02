import { expect, test } from '@jest/globals'
import './aggregates'

interface OrderItem {
    name: string

    price: number
}

/* sum */
test('sum number array', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.sum(x => x)).toBe(15)
})

test('sum number empty array', () => {
    const arr = []
    expect(arr.sum(x => x)).toBe(0)
})

test('sum OrderItem array', () => {
    const arr: OrderItem[] = [{
        name: 'Product 1',
        price: 30.49
    }, {
        name: 'Product 2',
        price: 99.99
    }, {
        name: 'Product 3',
        price: 9.99
    }]

    expect(arr.sum(x => x.price)).toBe(140.47)
})

test('sum empty OrderItem array', () => {
    const arr: OrderItem[] = []

    expect(arr.sum(x => x.price)).toBe(0)
})


/* count */
test('count number array', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.count()).toBe(5)
})

test('count number empty array', () => {
    const arr = []
    expect(arr.count()).toBe(0)
})

test('count OrderItem array', () => {
    const arr: OrderItem[] = [{
        name: 'Product 1',
        price: 30.49
    }, {
        name: 'Product 2',
        price: 99.99
    }, {
        name: 'Product 3',
        price: 9.99
    }]

    expect(arr.count()).toBe(3)
})

test('count empty OrderItem array', () => {
    const arr: OrderItem[] = []

    expect(arr.count()).toBe(0)
})


/* avg */
test('avg number array', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.avg(x => x)).toBe(3)
})

test('avg number empty array', () => {
    const arr = []
    expect(arr.avg(x => x)).toBe(0)
})

test('avg OrderItem array', () => {
    const arr: OrderItem[] = [{
        name: 'Product 1',
        price: 30.49
    }, {
        name: 'Product 2',
        price: 99.99
    }]

    expect(arr.avg(x => x.price)).toBe(65.24)
})

test('avg empty OrderItem array', () => {
    const arr: OrderItem[] = []

    expect(arr.avg(x => x.price)).toBe(0)
})


/* min */
test('min number array', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.min(x => x)).toBe(1)
})

test('min number empty array', () => {
    const arr = []
    expect(arr.min(x => x)).toBe(Infinity)
})

test('min OrderItem array', () => {
    const arr: OrderItem[] = [{
        name: 'Product 1',
        price: 30.49
    }, {
        name: 'Product 2',
        price: 99.99
    }, {
        name: 'Product 3',
        price: 9.99
    }]

    expect(arr.min(x => x.price)).toBe(9.99)
})

test('min empty OrderItem array', () => {
    const arr: OrderItem[] = []

    expect(arr.min(x => x.price)).toBe(Infinity)
})




/* max */
test('max number array', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.max(x => x)).toBe(5)
})

test('max number empty array', () => {
    const arr = []
    expect(arr.max(x => x)).toBe(-Infinity)
})

test('max OrderItem array', () => {
    const arr: OrderItem[] = [{
        name: 'Product 1',
        price: 30.49
    }, {
        name: 'Product 2',
        price: 99.99
    }, {
        name: 'Product 3',
        price: 9.99
    }]

    expect(arr.max(x => x.price)).toBe(99.99)
})

test('max empty OrderItem array', () => {
    const arr: OrderItem[] = []

    expect(arr.max(x => x.price)).toBe(-Infinity)
})
