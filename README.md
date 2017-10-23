Unified Architecture Convenience Layer (UACL)
================================================================================

Project websites?
---------------------------
 - **Code repository:** 
    - **.NET**: https://bitbucket.org/falko_wiese/concept_laser

 - Git Branches:
     - `master`: tested and production-ready, has the latest API and the latest 
        tested features.
     - `*`: development branches, only.

What?
-------------------------------------------------------------------------------

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


Why?
-------------------------------------------------------------------------------
   
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


How?
-------------------------------------------------------------------------------
Technically we used the language features from .NET - "Annotations", respectively. The
framework comes with a bunch of well implemented stuff - e. g. helper classes like *RemoteObject*
and *ServerSideUaProxy* on *Client* and *Server* side, respectively.


Dependencies?
-------------------------------------------------------------------------------
The UACL is for all **Microsoft Windows** platforms where we have a working implementation
of the .NET Common Language Runtime. It's based on the commercial C# OPC UA 
Software Developers Kit from Unified Automation. A demo version for the SDK can be downloaded 
from their website for free: http://www.unified-automation.com.


Examples?
-------------------------------------------------------------------------------
You will find full featured client and server application examples at the bitbucket.org 
repository. There you can find best practices and idioms for the usage of **UACL**.


Documentation?
-------------------------------------------------------------------------------
Hmm ... Please read the code of the examples. ;)


Status?
-------------------------------------------------------------------------------
I would say, the implementation is in a **GOOD STATE**.


Installation?
-------------------------------------------------------------------------------
If you want to use *fabric* (http://www.fabfile.org/installing.html) as build system, 
you need Python (https://www.python.org/downloads/windows/) - for sure.

For .NET you don't have to install any additional stuff. We install external 
libraries with the NuGet package manager, if necessary. 


Who?
-------------------------------------------------------------------------------
 - **Author**: Falko Wiese
 - **Contact**: `f****.w****@mail.de` (replace the asterisks)
 - **Organization**: wieSE Software Engineering (Germany)
 - **Project website**:
    - https://bitbucket.org/falko_wiese/concept_laser


License?
-------------------------------------------------------------------------------
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


