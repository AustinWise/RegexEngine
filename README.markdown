RegexEngine, a regular expression compiler
-----------------------------------------------------------------

This program parses regular expressions (just concatenation, Kleene star, and alternation)
into an AST, transforms the AST into a NFA with λ-transitions, transforms the
NFA into a DFA using powerset construction, and finally transforms the DFA
into IL byte code for .NET.

License
-------

RegexEngine is licensed under the BSD license.

Areas for improvement
---------------------

 - Reduce the number of redundent states in the DFA.
 - Perhaps some optimizations, for example handleing regex'es that end with .* without scanning all the text.
