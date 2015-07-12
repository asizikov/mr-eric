mr-eric
=======
Create and initialize private auto-property context action for ReSharper.


![Demo](https://raw.githubusercontent.com/asizikov/mr-eric/master/Content/context_action_demo.gif)

The plugin was inspired by the discussion about Auto Properties in C# language.
http://stackoverflow.com/questions/3310186/are-there-any-reasons-to-use-private-properties-in-c

```
The primary usage of this in my code is lazy initialization, as others have mentioned.

Another reason for private properties over fields is that private properties are much, much easier to debug than private fields.
I frequently want to know things like "this field is getting set unexpectedly; who is the first caller that sets this field?"
and it is way easier if you can just put a breakpoint on the setter and hit go. You can put logging in there. 
You can put performance metrics in there. You can put in consistency checks that run in the debug build.

Basically, it comes down to : code is far more powerful than data. 
Any technique that lets me write the code I need is a good one. 
Fields don't let you write code in them, properties do.
(c) Eric Lippert
```

How to install
===

https://resharper-plugins.jetbrains.com/packages/MrEric/


