Lock Pick
=====================

Lock Pick unlocks items.

Description
-----------

Lock Pick unlocks items locked by the Sitecore user when he/she logouts, and after an elapsed period of idle time.

Installation
------------

Install /Packages/Lock Pick-1.0.0.0.zip in Sitecore.

Do I need to do anything else after installing the package?
-----------------------------------------------------------

Nope.  The module should be ready to go after installing the package.

Installed Assets
----------------

The installation package installs the following:

Items
-----

The following items are installed into the master database:
 
* /sitecore/system/Tasks/Commands/UnlockIdleUserItemsCommand
* /sitecore/system/Tasks/Schedules/UnlockIdleUserItemsSchedule

Files
-----

* ~/bin/LockPick.dll
* ~/App_Config/Include/LockPick.config
 
What versions of Sitecore will this work on?
--------------------------------------------

I have tested this on Sitecore 6.4.1, 6.5 and 6.6.

What version of .NET is required?
---------------------------------

.NET 4.0

How do I use this?
------------------

This blog [article](http://sitecorejunkie.com/2013/09/24/unlock-sitecore-users-items-during-logout/) discusses the logout pipeline processor that unlocks items locked by the user during logout.

This blog [article](http://sitecorejunkie.com/2013/09/28/periodically-unlock-items-of-idle-users-in-sitecore/) discusses the scheduled task that periodically unlocks items by idle users.