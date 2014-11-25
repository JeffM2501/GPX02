GPX02
=====
A 2d tank game to give the masses (30 people ) what they want.

Licensing
=====
The main codebase is licensed under the Mozilla Public License (MPL) and Copyright 2014 Jeffery Myers. Some artwork and assets are from other sources and are restricted by there own licenses and copyright holders as marked out in individual license.txt files (usually LGPL, CC BY-NC-ND 3.0, etc..)

Some parts of the system use code or assets provided with the Unity3d system and are provided free of use but are copyright Unity Technologies.


Unity
=====
The system is built with Unity3d (http://unity3d.com/) with targets for Windows, MacOSX, and Linux. Building requires the use of the Unity3d IDE. This IDE is available for Windows and MacOSX. The system is designed to build with ether the free (as in beer) or paid versions of Unity3d. Please note that while many aspects of Unity3d are open source, the IDE and it's associated code is NOT.

If you want to develop on Linux you will need to use Wine, that's all there is to it. Both Windows and MacOSX IDEs can build Linux binary packages.


Coding
=====
The code for the system is done in C# and uses ether the .net or mono runtimes on end user systems. The Unity IDE packages up the run-times that are needed on each platform so there are no end user dependencies.


TODO
====
* Chat System
* Spawns
  1. Client Sends request
  2. Server sends response with location
  3. Client spawns network object at location
  4. Server links object to player and verifies location
* Movement
  * Should be automatic
  * Mouse
  * Keyboard
  * Touch?
  * Sticks?
* Shots
  1. Client Fires local shot and sends fire message with local ID
  2. Server creates network shot and sends back local ID to global ID mapping
  3. Client removes local shot and lets global shot do it's thing
* Deaths
  * Server side detection
  * Death message kills a players local shots because the real ones are still live on the server
* Powerups
  * Entirely server side
  * Synced Object Flags or just messages
* Authentication
  * Use BZID or Google?
    * BZID
      * Known
      * Simple
      * Lame
    * Google
      * Unknown
      * Probably complicated
      * Samples exist
      * No forum based registration
* Maps
  * Object Placement Format
  * Unity Level Loads
