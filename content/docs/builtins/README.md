# Built-in functions and classes

The following functions and classes are automatically imported into your
default scope and can be called or used from anywhere.

## print

`print(value)`

Displays a message to the user. If the value passed in is a string, the string is shown
as-is. If the value is not a string, it is converted via default string conversion.

The exact behavior and manner that the string is displayed is dependent on the
specific CommonScript wrapper.

## tryParseInt

`tryParseInt(strValue)`

Converts the string value to an integer value. If such a conversion is impossible,
`null` is returned instead.

## tryParseFloat

`tryParseFloat(strValue)`

Converts the string value to an integer value. If such a conversion is impossible,
an `InvalidArgumentException` is thrown.

## floor

`floor(n)`

Converts the numeric value to an integer value by returning the integer value that
is less than or equal to the input value. Note that for positive numbers, this will
round *down* and for negative numbers, this will *increase* the magnitude of the number.

```javascript
print(floor(3.5)); // 3
print(floor(-3.5)); // -4
```

## getUnixTime

`getUnixTime()`

Returns the current unix timestamp as an integer.
This is the number of seconds since midnight on January 1st, 1970 UTC (epoch).

## getUnixTimeFloat

`getUnixTimeFloat()`

Returns the current unix timestamp as a float.
This is the number of seconds since midnight on January 1st, 1970 UTC (epoch)
including the fractional portion.

## delayInvoke

`delayInvoke(fn, sec)`

Invokes the given function in the given number of seconds. The number of seconds
can be a float. The function should not require any arguments.

The function will only be invoked once the event loop is free. This may cause
the function to run after more than the amount of time has passed.

## sleep

`sleep(sec)`

Pauses execution of the current task for the given number of seconds. This
frees the event loop and does not block the CPU.

## Exception

This is the base class that all exceptions extend from. Only instances of objects
that extend from Exception are allowed to be thrown.

### .message

A string message explaining the problem. This should be generally be human-readable.

### .trace

A list of strings representing the callstack at the time the exception was thrown.
This field is automatically populated by the `throw` keyword.

### constructor(msg = '')

The constructor of message optionally takes in a message value.

## FatalException

Extends Exception. FatalExceptions cannot be caught and will cause the termination
of the VM. This is reserved for un-recoverable problems such as the VM running out of
memory or stack overflows.

## FieldNotFoundException

Extends Exception. This is automatically thrown by the interpreter when you
access a field that does not exist on a value.

## InvalidArgumentException

Extends Exception. This is thrown by the interpreter when a built-in method or
function does not consume a proper argument. It is recommended to use this
exception when creating third-party consumed code.

## InvocationException

Extends Exception. This is thrown by the interpreter when you invoke a non-function type
like a function.

## KeyNotFoundException

Extends Exception. This is thrown by the interpreter when accessing a key on a dictionary
that does not exist.

## NotImplementedException

Extends Exception. This exception can be explicitly thrown by the user as a placeholder
for sections of code that have not been written.

## NullReferenceException

Extends Exception. This exception is automatically thrown by the interpreter when
null is used in a way that requires an actual value.

## OutOfRangeException

Extends Exception. This exception is automatically thrown by the interpreter when
a list or string is accessed at an index before or after the ends of the list or string.

## StackOverflowException

Extends FatalException. This exception is automatically thrown by the interpreter
when code has recursed too deep and additional stack frames cannot be allocated.

## TypeException

Extends Exception. Not currently used.

## ZeroDivisorException

Extends Exception. This exception is automatically thrown by the interpreter when
you divide a number by zero or use 0 as a modulus base.

## ImmutableDataException

Extends Exception. This exception occurs when attempting to modify data that is
immutable, such as assigning a value to a string's index.
