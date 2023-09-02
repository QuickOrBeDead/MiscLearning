import { expect, test } from '@jest/globals'
import './aggregates'


test('sum number array with map function', () => {
    const arr = [1, 2, 3, 4, 5]
    expect(arr.sum(x => x)).toBe(15)
})

test('sum number array without map function', () => {
    const arr = [10, 11, 12]
    expect(arr.sum()).toBe(33)
})

test('sum number empty array with map function', () => {
    const arr = []
    expect(arr.sum(x => x)).toBe(0)
})

test('sum number empty array without map function', () => {
    const arr = []
    expect(arr.sum()).toBe(0)
})