# The History of `Arithmonym`

## 2024

This is when GreatCoder1000 started endeavours in the googology programming world.

A GitHub gist from this time can be found [here](https://gist.github.com/GreatCoder1000/cea019f976b8df0e647a87ff4482b74d)

## 2025

This is when GreatCoder1000 experimented in multiple languages. Middle of this year is when GreatCoder1000 heard about .NET and C# and started developing some predecessors to `Arithmonym`.

A C program exists somewhere on my device. It backfired *slightly* because I additionally put a challenge that I will use 1960's conventions like Hungarian Notation. Here's an excerpt:

```c
// --snip-- ...Lots of comments and includes and macros...

typedef struct
{
    QWORD qwSlog2_hi64;
    QWORD qwSlog2_lo64;
    QWORD qwSlog2_lo128;
    DWORD dwSlog2_lo160;
    WORD   wSlog2_lo176;
    BYTE   ySlog2_lo184;
    BYTE   yflags;
} _dclnf6;

/**
 * SECTION 2 - FUNCTION PROTOTYPES
 */

// Get/Set high part of superlogarithm.
QWORD getSlog2Hi(_dclnf6 *pstValue);
void setSlog2Hi(_dclnf6 *pstValue, QWORD qwValue);

// Get/Set flags (currently only negative and reciprocal)
bool        getFlagIsNegative(_dclnf6 *pstValue);
void        setFlagIsNegative(_dclnf6 *pstValue, bool bValue);
bool        getFlagIsReciprocal(_dclnf6 *pstValue);
void        setFlagIsReciprocal(_dclnf6 *pstValue, bool bValue);

// Get Approximate base-2 superlogarithm (float, double, long double)
// Do NOT use this for stuff that does not involve these types.
float       getApproximateSlog2InFloat(_dclnf6 *pstValue);
double      getApproximateSlog2InDouble(_dclnf6 *pstValue);
long double getApproximateSlog2InLongDouble(_dclnf6 *pstValue);

// Extract float/double/long double
float       getFloatRepresentation(_dclnf6 *pstValue);
double      getDoubleRepresentation(_dclnf6 *pstValue);
long double getLongDoubleRepresentation(_dclnf6 *pstValue);

// Set a _dclnf6 to argument 2.
void        setFloatRepresentation(_dclnf6 *pstValue, float fValue);
void        setDoubleRepresentation(_dclnf6 *pstValue, double dValue);
void        setLongDoubleRepresentation(_dclnf6 *pstValue, long double ldValue);

// Simple unary (monadic) math operations
_dclnf6 dclnf6_abs(_dclnf6 stValue);
_dclnf6 dclnf6_neg(_dclnf6 stValue);
_dclnf6 dclnf6_recip(_dclnf6 stValue);

// --snip-- ... Lots of lines ... 
```

## 2026

Now `Arithmonym` is out and working!

## List of predecessors and prototypes of `Arithmonym`

* Hyperoperation engine in Scratch -- virtually nonexistent type-- it goes directly to string from inputs...
* DCLNF6 in C (hungarian notation) -- Range upto 10^^x
* TetraNum in C/Python -- Range upto 10^^x (Don't remember exactly what x is though.. some number like 10^308, i suppose...)
* AstroNum in C# -- Range upto 10^10^308
* TetraNum in C# -- Range upto 10^^10^308
* GodgahNum in C# -- Lovely Idea but got terribly broken also its over 2 kilobytes per number, and most of the time its wasted. Using List instead of arrays would be better.
* Arithmonym in C# -- All the qualities!