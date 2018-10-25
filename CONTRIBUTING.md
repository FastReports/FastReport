# Contributors Guide

The following is a set of guidelines on contributing to FastReport Open Source project. 
We are fully accepting any positive change into the project that will make FastReport faster, more maintainable, and anything that increases the convenience of using it.
Any contribution requirement may be modified, and full compliance with this document does not guarantee approval of the contribution to the project.

## Where to Start

If you want to work on something, look in the [Issues](https://github.com/FastReports/FastReport/issues) first. Both to see that your contribution isn't already being worked on by someone else, as well as to make sure that it is an appropriate contribution before diving into it.

## Creating New Issues

If you want to make something and there isn't an issue on it, post one. Specify a detailed description of the issue or requested feature.
For bug reports describe the expected behavior and the actual behavior. Then you need to provide  code example that reproduces the issue. 
Also you can specify any relevant exception messages and stack traces.
You should subscribe to notifications for the created issue in case there are any follow up questions.

## Coding Conventions

### Code Formatting

- You should use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces, where each brace begins on a new line. 
- You should use four spaces of indentation without tabs. 
- If the string is longer than 100 characters, it is recommended to move part of the code to the next line. 
- If the length of the string is more than 128 characters you should split the string (or add the comment why the length of the line is longer 128 characters).

### Commenting

- Comments should be placed on a separate line, not at the end of a line of code.
- The text of the comment should begin with a capital letter.
- Comment text should be ended with a dot.
- A single space should be inserted between the comment delimiter (//) and the comment text.
- Comments should be presented on each cycle and branch.

### Naming

- You need to use UpperCamelCase for the name of Properties, Classes, Interfaces, Methods, Structures, Namespaces, Events.
- You need to use lowerCamelCase for variable names.
- You need to use UPPER_CASE for the name of the constants.
- Object names should characterize the element. You cannot use single letter names or abbreviations.
- All names must be longer than 2 characters (excluding variable loop iterators).
- When declaring a variable and when explicitly converting a type, you need to use the type name with a small letter.

### Working with Strings

- You should avoid using string interpolation (strings with $).
- You need to use the plus sign for concatenation of short lines.
- You need to use StringBuilder to work with large amounts of text or when working with strings in loops.

### Loops Requirements

- You should avoid using break, continue and goto in loops.
- Do not use loops nesting greater than 2. If deeper nesting is necessary, then a part of the code must be put into a separate function.

### try-catch Requirements

- You need to use "using" instead of "try-finally", where possible. If "finally" has only one dispose statement then it is necessary to convert to "using".
- Using try-catch should not affect application performance.

### Fields and Properties Requirements

- You can not use automatically implemented properties.
- You can not use short implementation of properties.

### Syntax Limitations

- You cannot use implicitly typed local variables (var keyword).
- You should avoid LINQ.
- You should avoid lambda.

## Testing

The unit tests for a package should be put in a project FastReport.Tests.OpenSource. Each new class or public property should be tested.

## Submitting Pull Requests

Once you have completed your contribution feel free to submit a pull request. We will pull down your branch and test it to confirm that the change works and if any further changes need to happen before being merged in. 

## Resources

[FastReport Open Source](https://github.com/FastReports/FastReport "Click for visiting the FastReport Open Source GitHub")
