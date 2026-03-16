# Internal Letter System

Internally, `Arithmonym` uses a letter-based system. For simplification of explanation, we will limit the range to ≥1.

The Idea comes from PsiCubed2's Universal Canonical Form of Letter Notation. <sup>[[1]]([link](https://googology.miraheze.org/wiki/Introduction_to_PsiCubed%27s_letter_notation))</sup><sup>[[2]]([link](https://googology.fandom.com/wiki/User_blog:PsiCubed2/My_Letter_Notation))</sup><sup>[[3]](https://googology.fandom.com/wiki/User_blog:PsiCubed2/Letter_Notation_Part_II)</sup><sup>[[4]](https://googology.fandom.com/wiki/User_blog:PsiCubed2/Letter_Notation_Part_III)</sup>

The differences are that we have 4 extra letters in the start, A, B, C, and D to interpolate between 1 and 100, and avoid having too many edge cases to debug, and also that G and H are skipped because in that case we don't have the H10 to J4 edge case, which would require special handling and normalization. Above P10, the system is actually not formalized quite yet, but since most of arithmetic kinda "breaks down" way before that point, at least before more googological operators are added, that's no problem.

Here is the full letter table, and what each of it does (note a letter is like a function here, if it was hard to understand. E is the first one and it does `10^` to the number)

**Range assumes operand is in range 2 to 10, which is the case in Arithmonym.*

| Letter | Internal Number | Mnemonic | Range* |
|-|-|-|-|
|**A**|1|--|1 to 2|
|**B**|2|--|2 to 4|
|**C**|3|--|4 to 20|
|**D**|4|--|20 to 100|
|**E**|5|--|100 to 10<sup>10</sup>|
|**F**|6|--|10<sup>10</sup> to 10↑↑10|
|**J**|7|--|10↑↑10 to 10↑↑↑↑↑↑↑↑↑↑10|
|**K**|8|Ca**k**e (Graham's Number representation resembles a birthday cake)|10↑↑↑↑↑↑↑↑↑↑10 to `{10,10,1,2}` (googological notation: BEAF)
|**L**|9|**L**ots of Cakes 😃|`{10,10,1,2}` to `{10,10,2,2}`
|**M**|10 or `0x0A`|Cake **M**aker (not really, it makes make makers that make make makers, etc.)|`{10,10,2,2}` to `{10,10,10,2}`
|**N**|11 or `0x0B`|TBD|`{10,10,10,2}` to `{10,10,10,10}`
|**P**|12 or `0x0C`|**P**olynomial ω in the fast-growing hierarchy|`{10,10,10,10}` to `{10,10,10,10,10,10,10,10,10,10,10,10}` w/ 12 10s [NOTE: past P to Q level, it is NOT well-defined] |
|**Q**|13 or `0x0D`|**Q**uest (see Hercules and the Hydra / Cedric and the worm)|`{10,10,10,10,10,10,10,10,10,10,10,10}` w/ 12 10s to approximately `{10,12(((((1)1)1)1)1)2}` or approximately `{12,5 (1~2) 2}` or approximately f<sub>ε<sub>0</sub></sub>(10) in the fast-growing hierarchy|
|**R**|14 or `0x0E`|Lowercase r resembles Γ from FGH|f<sub>ε<sub>0</sub></sub>(10) to f<sub>Γ<sub>0</sub></sub>(10)
|**S**|15 or `0x0F`|**S**mall Veblen Ordinal|f<sub>Γ<sub>0</sub></sub>(10) to f<sub>SVO</sub>(10)|
|**T**|16 or `0x10`|**T**ree since most TREE(n) where n>2 and n is not "too large" to escape this function is T2-T3 range|f<sub>SVO</sub></sub>(10) to f<sub>LVO</sub>(10)|
|**V**|17 or `0x11`|*TBD*|f<sub>LVO</sub>(10) to f<sub>BHO</sub>(10)|
|**W**|18 or `0x12`|*TBD*|f<sub>BHO</sub>(10) to f<sub>Buchholz' Ordinal</sub>(10)|

## Ultra large 100 percent undefined numbers

X3 ~ s(10,10{1,,1{1,,1,,2}2}2) in SAN? (limit of BAN)

Cyrillic B is `BusyBeaver(Tetration(10, x))` but elsewhere noted to be a very rough approximation.
