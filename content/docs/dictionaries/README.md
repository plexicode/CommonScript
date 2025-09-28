# Dictionaries

Dictionaries are keyed-collection types. Dictionaries can hold any type of value
but the keys must either be integers or strings.

## Creation

Dictionaries can be created using JSON-like syntax with curly-braces:

```javascript
myDictionary = {
    "key1": 42,
    "key2": "bananas",
    "key3": ["nested", "collection"],
    "key4": true,
};
```

The keys of dictionaries are strings and require single or double quotes. A comma
separates each key-value-pair in the dictionary. The last item may optionally have
a comma after it as well.

## Single Item Access

Items in a dictionary can be accessed by using the key in square brackets after
a dictionary reference.

```javascript
countryLookup = {
    'ja': "Japan",
    'us': "United States",
    'ca': "Canada",
    'fr': "France",
    'ko': "Korea",
};
print(countryLookup['fr']); // Result: France
```

If a key is accessed but does not exist, a `KeyNotFoundException` is thrown. If
you are unsure whether a key is present and need to safely check, use the `.get`
method instead which takes a key and an optional fallback value.

```javascript
name = countryLookup.get(countryCode, "Unknown country");
```

## Modifying

After the dictionary has been created, it can be modified also using backet
syntax and the assignment operator.

```javascript
countryLookup['au'] = "Australia"; // add a new value
countryLookup['ko'] = "Republic of Korea"; // modify an existing value.
```

Note that dictionaries are reference types. Assigning or passing a dictionary somewhere
does not create a copy of the dictionary and is a reference to the same underlying
value.

```javascript
lookup = { "a": 4 };
newLookup = lookup;
print(lookup); // { "a": 4 }

lookup["b"] = 10;
print(lookup); // { "a": 4, "b": 10 }
print(newLookup); // { "a": 4, "b": 10 }
```

To create a shallow clone of a dictionary, use the `.clone()` method. This will
copy the keys and values directly to a new dictionary instance. It will not
clone the nested values, though.

```javascript
lookup = { "a": 4 };
newLookup = lookup.clone();
print(lookup); // { "a": 4 }

lookup["b"] = 10;
print(lookup); // { "a": 4, "b": 10 }
print(newLookup); // { "a": 4 }
```

## Batch Access

A list of all the keys in the dictionary can be accessed using the `.keys()`
method. Similarly, a list of all the values in the dictionary can be accessed
using the `.values()` method. These methods copy the keys or values into
a new list instance and contains no link to the original dictionary.

## Deletion

To delete items from a dictionary, use the `.remove(key)` method.

## Key iteration order

The order of the keys and values used by `.keys()`, `.values()`, and `.mapKvp()`
is generally the same as the order that the keys were inserted. The internal
key collection is appeneded to when a new key is added. However, when an item is
deleted, its position is swapped with the last item in the key collection. This
is to prevent the need for frequent capacity re-allocations in collections that
experience high churn.

If key iteration order is important, you should track it independently.

If no removals are performed on the dictionary, the key iteration order is guaranteed.

The iteration order of `.keys()`, `.values()`, and `.mapKvp()` will always be the same.

```javascript
items = {
    'A': "apple",
    'B': "ball",
    'C': "cat",
    'D': "doll",
};
print(items.keys()); // A, B, C, D
print(items.values()); // apple, ball, cat, doll
items['E'] = "elephant";
print(items.keys()); // A, B, C, D, E
items.remove('C'); // Removes cat, the last element (elephant) is swapped into its place
print(items.values()); // apple ball, elephant, doll
```

# Dictionary Methods

## clear

`dictionary.clear()`

Removes all items from this dictionary.

## clone

`dictionary.clone()`

Creates a new dictionary with the same keys and values as the original
dictionary. This is a shallow copy of the dictionary and is equivalent to the following code:

```javascript
dictCopy = {};
for (key : dictOriginal.keys()) {
    dictCopy[key] = dictOriginal[key];
}
```

## contains

`dictionary.contains(key)`

Returns a boolean indicating whether the dictionary contains the given key.

For checking a dictionary before attempting to access a key, do not use the
following pattern:

```javascript
if (lookup.contains(key)) {
    value = lookup[key];
    ...
}
```

This causes the interpreter to perform 2 separate key lookups. In such situations
where you are unsure if a key is present, use the `.get()` method instead.

## get

`dictionary.get(key, fallbackValue = null)`

Attempts to access the value at the given key and returns its value, if present.
If the value is not present, it will return a default value. This default value
is `null` if unspecified.

## keys

`dictionary.keys()`

Generates a list of all the keys in the dictionary. This list has no link to the
dictionary and will not update if the dictionary is updated. The order of the keys
is generally the same order as they were added, but not always.

> For more information on key iteration order, see the section titled *key iteration order*.

## mapKvp

`dictionary.mapKvp(fn)`

The provided function will be invoked on each key-value-pair in the dictionary.

The provided function must accept 2 parameters: one for the key and one for the value.

```javascript
function howManyDelawares(countryName, landArea) {
  delawareCount = landArea / 6450.0;
  return countryName + " is the size of " + delawareCount + " Delawares.";
}

function main(args) {
  countrySize = {
    "Singapore": 735.7,
    "Russia": 17125191,
    "Moldova": 33850,
    "Burundi": 27830,
  };
  report = countrySize.kvpMap(howManyDelawares);
  print(report.join('\n'));
}

...

countrySize = {
    "Singapore": 735.7,
    "Russia": 17125191,
    "Moldova": 33850,
    "Burundi": 27830,
};
report = countrySize.mapKvp(howManyDelwares);
print(report.join('\n'));
```

Result:
```
Singapore is the size of 0.11406201550387597 Delawares.
Russia is the size of 2655.0683720930233 Delawares.
Moldova is the size of 5.248062015503876 Delawares.
Burundi is the size of 4.314728682170543 Delawares.
```

The order in which the function is invoked on the key-value-pairs is mostly in
the order they were added to the dictionary, but not always.

> For more information on key iteration order, see the section titled *key iteration order*.

## remove

`dictionary.remove(key)`

Removes the given key and corresponding value from the dictionary.

The given value is the **key**.

## values

`dictionary.values()`

Generates a list of all the values in the dictionary. This list has no link to the
dictionary and will not update if the dictionary is updated. The order of the values
is generally the same order as they were added, but not always.

> For more information on key iteration order, see the section titled *key iteration order*.
