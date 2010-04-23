## What is Definitif Box?

Definitif Box is a complete solution for Rapid Web Application Development.

## Requirements:
1. Visual Studio 2010 and .Net Framework 4
2. Windows Installer XML tools (http://wix.sourceforge.net/)

## Custom types
Custom types can be used for serializing and deserializing using overrided explicit and implicit conversion methods.
For example:

    public class Foo
    {
        public Foo(double d)
        {
            this.X = d;
        }

        public double X { get; private set; }

        public static implicit operator Foo(double d)
        {
            return new Foo(d);
        }

        public static explicit operator double(Foo f)
        {
            return f.X;
        }
    }

To mark column as custom-casted member, 'as' attribute should be used:

    ["Foo" as double]
    public Foo Bar;
