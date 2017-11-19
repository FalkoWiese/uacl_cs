# Unified Architecture Convenience Layer (UACL)

## Project websites?
 - **Code repository:** 
    - **.NET**: https://gitlab.com/falko.wiese/uacl_cs

 - Git Branches:
     - `master`: tested and production-ready, has the latest API and the latest 
        tested features.
     - `*`: development branches, only.

## What?

 - OPC UA (OPC (https://opcfoundation.org/) **Unified Architecture** - 
   https://de.wikipedia.org/wiki/OPC_Unified_Architecture)) is the current 
   standard for secure, reliable and scalable industrial communication.

 - The Unified Automation SDK from Unified Automation GmbH (https://www.unified-automation.com/) 
   is a closed source software toolkit, what - first of all - implements the OPC Foundation 
   communication standard.

 - The Unified Architecture Convenience Layer (UACL) is a software framework to simplify 
   the development of **OPC UA** applications on **Microsoft Windows** (in **C#**), for 
   Client/Server Applications!

 In short, the UACL is **OPC UA made easy**!


## Why?
   
 - The intent of UACL is to give fast an easy access to the OPC UA technology.

 - In practice, the UACL is a framework (or "toolkit") that helps you to create advanced OPC UA 
   applications with minimal effort. Essentially, it takes care of some *technical* aspects so 
   that you can concentrate on the *working* aspects of your software application.
   
 - Some very concise and easy to understand examples come along with the UACL, itself.
 
**Intuitive platform driven API:**
All code from the Software Developers Kit is wrapped into some convenient namespaces and 
classes, and for sure, .NET assemblies. You can write your business code as usual, the
only thing you've to do is to mark your business classes as so called **Remote Objects** via
an annotation and to register it on the supplied **UA Server** object at runtime. Furthermore
you've to mark variables, and methods as **Remote Variables**, and **Remote Methods**, 
respectively. The UACL will generate an UA Server interface for you. I think, it's a really
magic behavior, isn't it?

What ever, if you don't want to dive deep inside of **OPC UA**, you can let UACL do the 
annoying things for you.


## How?
Technically we used the language features from .NET - "Annotations", respectively. The
framework comes with a bunch of well implemented stuff - e. g. helper classes like *RemoteObject*
and *ServerSideUaProxy* on *Client* and *Server* side, respectively.


## Dependencies?
The UACL is for all **Microsoft Windows** platforms where we have a working implementation
of the .NET Common Language Runtime. It's based on the commercial C# OPC UA 
Software Developers Kit from Unified Automation. A demo version for the SDK can be downloaded 
from their website for free: http://www.unified-automation.com.


## Documentation?
The library consists of three modules:

 - *UaclUtils*
 - *UaclServer*
 - *UaclClient*

There exist some class diagrams. One for every library. Please have look to the source tree. My intention was, to show
them right here...

 - ![UaclUtils Class Diagram](https://gitlab.com/falko.wiese/uacl_cs/blob/master/UaclUtils.png)
 - ![UaclServer Class Diagram](https://gitlab.com/falko.wiese/uacl_cs/blob/master/UaclServer.png)
 - ![UaclClient Class Diagram](https://gitlab.com/falko.wiese/uacl_cs/blob/master/UaclClient.png)
 

## Examples?
You will find full featured client and server application examples at the bitbucket.org 
repository. There you can find best practices and idioms for the usage of **UACL**.

### How to create an *UA Server*?
    var server = new InternalServer("localhost", 48030, "ServerConsole");
Yup, that's it, really. The only thing you've to consider is maybe, that the so called *Application Name*
attribute is part of your *Browsing or Display Name* at the *Server Interface*. Clients should be aware of it.

#### Register a Server Object
```c#
    server.CreateClient<BusinessLogic>();
```
That way, we add a server side UA Object Node with the path *ServerConsole.BusinessLogic*. To access to that
specific node from *Client Side*, you can ignore the application name.

#### What Preparations we've to do with the *BusinessLogic* Class?
```c#
    [UaObject]
    public class BusinessLogic : ServerSideUaProxy
    {
        [UaclInsertState]
        [UaMethod]
        public int GetInteger(object state, string value)
        {...}

        [UaMethod]
        public bool CalculateJob(string name, int state)
        {...}

        [UaMethod("JobStates")]
        public string States()
        {...}
        
        [UaVariable]
        public string BoState
        {...}
        
        [UaVariable]
        public int IntBoState
        {...}
    }
```
You have to annotate your business class with *UaObject*. With the registering above on a server you
will see an instance of *BusinessLogic* on your - maybe with the *UaExpert* (an UA Standard Client)
browsed - *UA Server Interface*. Further you will see three *UA Method Nodes* and two *UA Property Nodes*.
The only things you have to do to get these nodes, are the annotations of methods with *UaMethod* or
properties with *UaProperty*, respectively.

Maybe you notice the *UaclInsertState* annotation of one method. That way we extend the argument list
of so annotated methods dynamically with a *Server State Object*.

The inheritance to *ServerSideUaProxy* is 
not necessary, but I would really recomend it! If it is not possible to extend the helper class, you
have to implement some of the code from *ServerSideUaProxy* in your class.

#### How to get a running/working *UA Server*?
```c#
    server.Start();
    while (true)
    {
        Thread.Sleep(100);
    }
```
That's the only thing, you have to do. I suggest some decoration with a bit convenience or security code,
but yep, that's it. You will find the whole example at the [ServerConsole](https://bitbucket.org/falko_wiese/concept_laser/src/ecb7966318dccd989711185ac0e9900381776ee6/ServerConsole/?at=master)
example.

### How to create an *UA Client*?
```c#
    var obj = new RemoteObject("localhost", 48030, "BusinessLogic");
```
That's an *UA Client* to the above created *UA Server Object Node*.
```c#
    obj.Connect();
```
Now we have a connection to the *UA Object Node* created.

#### How to read/write a *UA Property*?
```c#
    var s0 = obj.Read<string>("BoState");
```
To read the *string* result from the *BusinessLogic.BoState* property and store it to *s0*, we can
do it like so.
```c#
    const string s1 = "THE NEW JOB STATE!";
    obj.Write("BoState", s1);
```
And to write the value of *s1* to the same *Remote UA Property*, we have to execute these two lines.
I think, that's easy. I would recommend to deal with your *UA Server* in an object-oriented manner. Means,
manipulate your *Server Objects* while execution of *Remoted Methods* and give the internal state of your
*Remote Objects* with changed *Properties* to outside. So, we should *monitor* these properties. 

#### How to monitor a *UA Property*?
```c#
    obj.Monitor(
        "BoState",
        v => { Logger.Info($"Received value from {obj.Name}.BoState ... '{v}'."); });
```
Yup, it's as easy as the stuff above. The given (really simple) logging *Client Side Callback* will executed,
if we notice an *Value Change* on *Server Side*.

#### How to call a *UA Method*?
```c#
    const string value = "Moin from *UA Client* ...";
    var bytes = obj.Invoke<byte[]>("GetBytes", value);
```
What's missing, yes, call a method. We can see in the example above, how to define the right *UA Method*, I
want to call. For that we need the name 'GetBytes', the right number of arguments 'value' from right types 'byte[]'.
If all the things are correct, the execution of a method call will terminate successfully.

#### How to disconnect from an *UA Server*?
```c#
    obj.Disconnect();
```
If my work is done on *Client Side*, it's a good idea to disconnect from the *UA Server*.

## Status?
I would say, the implementation is in a **GOOD STATE** -- or -- **PRODUCTION READY**.

## Installation?
If you want to use *fabric* (http://www.fabfile.org/installing.html) as build system, 
you need Python (https://www.python.org/downloads/windows/) - for sure.

For .NET you don't have to install any additional stuff. We install external 
libraries with the NuGet package manager, if necessary. 

## Who?
 - **Author**: Falko Wiese
 - **Contact**: http://www.wieseweb.com
 - **Organization**: wieSE Software Engineering (Germany)

## License?
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as
published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.