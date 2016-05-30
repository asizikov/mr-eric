mr-eric
=======
Create and initialize private auto-property context action for ReSharper.

Build statuses

| master | feature |
|----|---|
|[![Build status](https://ci.appveyor.com/api/projects/status/0oi2pep5gv5m1t2o/branch/master?svg=true)](https://ci.appveyor.com/project/asizikov/mr-eric/branch/master) |[![Build status](https://ci.appveyor.com/api/projects/status/0oi2pep5gv5m1t2o?svg=true)](https://ci.appveyor.com/project/asizikov/mr-eric) | 



## Features: 

Create private and private read-only property quick fix:

<img src="https://raw.githubusercontent.com/asizikov/mr-eric/master/Content/context_action_demo.gif" width="600">

Live templates:

<img src="https://raw.githubusercontent.com/asizikov/mr-eric/master/Content/LiveTemplates.gif" width="600">

## Why

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
ReSharper 8.2 (Not supported anymore):
https://resharper-plugins.jetbrains.com/packages/MrEric/

Latest ReSharper:
https://resharper-plugins.jetbrains.com/packages/Sizikov.MrEric/


