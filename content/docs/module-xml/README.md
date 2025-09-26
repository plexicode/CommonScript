# Built-in `xml` module

TODO: fill this in

## parseXml

`parseXml(s)`

## XmlNode

Abstract base class of XmlElement and XmlText.

## XmlElement

Extends XmlNode

- `name`
- `attributes`
- `children`

## XmlText

Extends XmlNode
- `value`

## XmlParseException

Extends Exception
- `line`
- `col`
- `err`
