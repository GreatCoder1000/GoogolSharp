# Bug Log

## Letter 38 bug
How is letter 38 being created?

Example. `TestSixTetratedToSeven` expects approximately `10^(10^(10^(10^(2*10^36305))))` or some close number indistinguishable from that.

Actual: `[38]8.08711910247802756579543652589025944[38]`

Let's trace through where they could be created.

It calls `Tetration(Six, Seven, analytical: false)`.

`ValidateTetrationInputs` does NOTHING with particularly these inputs.

`TryTetrationFastPath` doesn't do anything for numbers as large as Seven.

`useAnalytical` is false.

`integerHeight` should be `Seven`. After debugging, yes.
`fractionalHeight` should be `Zero`.
delegates to `PowerTower(Six, Eight, Zero)`

...
BUG FOUND!!! `LetterGToLetterJ` returns less-than-2 number. Added +2

Now the scale is closer, but still too much to be deemed "inaccuracy".

```
6^^7 seems to be 10^^(1.004*10^10)
```
