# Strings

Strings represent text content and are logical sequences of characters.

## Creation

String literals can be created by using either single quotation marks or double quotation marks.

```javascript
string1 = "This is a string.";
string2 = 'This is also a string.';
```

Both of these syntaxes are equivalent.

## Immutability

Like many languages, strings are immutable. When operations are performed on a string
that "change" it, a new string instance is created.

```javascript
msg1 = "hello";
msg2 = msg1.upper();
print(msg2); // Result: HELLO
print(msg1); // Result: hello

msg1 += "!"; // (reference to original string instance is lost)
print(msg1); // Result: hello!
```

## Concatention and string building

Strings can be joined together using the `+` operator or multiplied with the `*` operator.

```javascript
original = "x";
duplicated = original + 'y'; // result: "xy"
longChain = original * 10; // result: "xxxxxxxxxx"
```

When a non-string value is combined with a string using the `+` operator, the other
value is automatically converted to a string.

```javascript
num = 42;
msg = "The answer is " + num; // Result: "The answer is 42"
```

## Characters

Individual characters can be accessed using square bracket notation. The resulting value of
a character access is another string of length 0.

As with most programming languages, all character access is 0-indexed.
That is, the first character is 0, not 1.

```javascript
msg = "Hello, World!";
chr = msg[4];
print(chr); // Result: o
print(chr[0]); // Result: o
```

There is no distinction between characters and strings in the type system.

## Escape Sequences

To include a character in a string that would cause disruption to the string syntax
(such as including a double-quote character in a string surrounded by double-quotes),
precede the character with a backslash `\`. The collection of escape sequences is
fairly consistent with most common programming languages:

```javascript
msg = 'How\'s it going?';
response = "I am doing \"well\".\nHow about \"you\"?";
```

| Sequence | Character inserted |
| :-: | :-- |
| `\'` | Single quote `'` |
| `\"` | Double quote `"` |
| `\\` | Backslash `\` |
| `\n` | New line |
| `\r` | Carriage return |
| `\t` | Tab |
| `\0` | Null terminator |

## Slicing

You can extract substrings from a string instance using slice syntax, similar to Python.
This negates the need for an explicit subString method.

```javascript
alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
print("I know my " + alphabet[:3] + "'s!"); // Result: I know my ABC's!
print(alphabet[::-1] + ", officer"); // Result: ZYXWVUTSRQPONMLKJIHGFEDCBA, officer
print("I thought " + alphabet[11:16] + " was one letter"); // I thought LMNOP was one letter
```

Read more about how to use [slicing](../slicing).

## Character internal encoding

Each string character represents a single unicode character. This is functionally
equivalent to a UTF-32 representation (as opposed to UTF-16 or encoding-free
byte-based representations common in other languages). This ensures that arbitrary
character access is _mostly_ safe.

**Examples from various languages:**

```javascript
// CommonScript: true Unicode
msg = "🐝";
print(msg.length); // Result: 1
print(msg[0]); // Result: 🐝
```

```javascript
// JavaScript: UCS-2 (UTF-16)
let msg = "🐝";
console.log(msg.length); // Result: 2
console.log(msg[0]); // Result: <mojibake>
```

```php
// PHP: encoding-free byte sequences from presumed-UTF-8 source code.
$msg = "🐝";
echo strlen($msg); // Result: 4
echo $msg[0]; // Result: <byte: 240>
```

Even though strings in CommonScript are Unicode-safe,
caution still needs to be used for compound unicode sequences as glyphs and
ligatures are dependent on the font and rendering environment that you're viewing the
output in and are not represented in unicode.

```javascript
msg = '🧑‍🚒';
print(msg[0:2]); // Result: 🧑
print(msg[3:]); // Result: 🚒

msg = '🇫🇷';
print(msg[0]); // Result: <country code: F>
print(msg[1]); // Result: <country code: R>
```

## Performance impliciations of internal implementation

Internally, string instances are represented in one of two ways and can toggle
between these two modes.

- An array of unicode characters
- String builder (A binary tree node of two strings combined together)

When strings are used in a way that incurs a time-complexity performance penalty in
one mode but not the other, the string is converted.

For example, when adding suffixes to the end of a string inside a loop, this
usually incurs a performance penalty in most languages:

```javascript
lyrics = '';
for (i = 99; i >= 1; i--) {
    verse = i + " bottles of beer on the wall, " + i + " bottles of beer!\n"
        + "Take one down, pass it around, " + (i - 1) + " bottles of beer on the wall.\n";
    lyrics += verse;
}
print(lyrics);
```

In most languages this would be an expensive operation as each resulting string would need to be copied
to a new character buffer every time `+` is used. However CommonScript generates a string instance
for each `+` operation based on a string builder that refers to the two strings being joined such
that the actual `+` operation itself is constant-time instead of linear-time.

Once the lyrics are printed after the end of the loop, the string is then flattened in linear-time.

# String Methods

Strings have various built-in methods. Note that strings are **immutable** and any
method that appears to modify a string actually returns a new instance of a string with
the resulting output.

## getCodePoint

`string.getCodePoint(index)`

Returns the unicode code point (character code) at the given index as an integer. This value is
a logical codepoint as an integer type and therefore has no endian-ness.

Note that the internal character encoding is unicode character sequences and therefore partial
character or surrogates are not a concern.

```javascript
msg = "Hello";
print(msg.getCodePoint(1)); // 102 (ASCII code for 'e')
msg = "Hello 😀";
lastCharIndex = msg.length - 1;
print(msg.getCodePoint(lastCharIndex)); // 128512
```

## lower

`string.lower()`

Returns a new string that is a lower-case version of the original string.

```javascript
msg = "Hello, World!";
print(msg.lower()); // hello, world!
```

## replace

`string.replace(strToFind, strToReplace)`

Creates a new string where all occurences of `strToFind` are replaced with `strToReplace`.

The complexity of this method is O(string length * (strToFind + strToReplace))

This operation is case-sensitive.

```javascript
msg = "Cat Dog Bird";
newMsg = msg.replace("d", "b");
print(msg); // Cat Dog Birb <-- Note that the D in Dog was not replaced.
```

## split

`string.split(separator)`

Creates a new list instance containing the strings that were delimited by
the given separator.

```javascript
msg = "Flopsy, Mopsy, Cottontail";
rabbits = msg.split(', ');
print(rabbits); // ["Fopsy", "Mopsy", "Cottontail"]
```

Note that there is no default value for the separator. A separator argument
must be provided.

## toUnicodePoints

`string.toUnicodePoints()`

Creates a new list instance containing all the unicode character points of the string.

```javascript
msg = "Hello 😀";
print(msg.toUnicodePoints()); // Result: [72, 102, 108, 108, 111, 32, 128512]
```

## trim

`string.trim()`

Creates a string with the whitespace characters at the beginning and end of the string
removed.

```javascript
msg = "  \tTo whom it may concern... \n";
trimmed = msg.trim();
edges = "|" + trimmed + "|";
print(edges); // Result: |To whom it may concern...|
```

## upper

`string.upper()`

Returns a new string that is an upper-case version of the original string.

```javascript
msg = "Hello, World!";
print(msg.lower()); // HELLO, WORLD!
```
