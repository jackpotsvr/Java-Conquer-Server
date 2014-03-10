Java-Conquer-Server
===================
This repository will be about an Conquer Online server emulator programmed in Java. The repository is
maintained by Thomas and Jan-Willem Gmelig Meyling, but is open for public contributions and a ElitePVPers
thread has been opened since March 2014.

![screenshot](https://f.cloud.github.com/assets/3469492/2370007/b94dc976-a7f4-11e3-8288-3b444aef0ce9.jpg)

Our goal is to hold on to the native Java libraries as much as possible, currently only leaning on
Simple XML for configuration persistency and a PostgreSQL JDBC-connector. The Java Archives for these
libraries and other project files are included in this repository. Furthermore, we document our
code with Javadoc and Github Wiki's. This is to improve the overall code quality as well as to keep
contribution to the repository easy accessible and informative.

The current version of the server has the following features:
* Login and account creation
* Character information and other start up packets
* Spawn me to other players, retrieve surrounding entities when walking or jumping around
* Monster spawning
* Magic skill proficiency and animation

We're currently working on the attack skills, damage calculation and investigating on how we will
implement the AI for NPC's and Monsters. We're thinking about languages like LISP or LUA to take care
of the AI.

Our goal is to keep as close as possible to the original Conquer. We intend to implement all varities
of item upgrades, stay to the original skills and their damage calculation / chances. We see a view
improvements though:
* Better balance between the various professions
* Higher loot chances; higher item qualities and socketed items
* Higher gem mining chance
* More quests / kill quests
* More interaction with NPC's (for example quest teaming or a more interactive tutorial)

Building and contribution
-----
Fork the Github repository, load the project into you're Java IDE. We are using Eclipse with EGit.
A tutorial on EGit can be found here: https://wiki.eclipse.org/EGit/User_Guide . Commited changes
can be merged in to the main repository by requesting a merge commit.

For more documentation on how several functions could be implemented, you can go to the following
places:
* [ConquerWiki.com](http://conquerwiki.com/wiki/Main_Page) - A site containing all Packet Structures
* [spirited-fang](https://spirited-fang.wikispaces.com/) - A site which may be useful for enumerations
* [ElitePVPers.com](http://www.elitepvpers.com/forum/co2-pserver-discussions-questions/) - A forum with a lot of useful posts

Running the server
-----
The client first connects to the Authentication Server. There it retrieves the server IP and port number.
Then the client connects to the Game Server. The Game Server creates a thread for every active player.
This thread then handles all incoming packets from the client, and sends responses and other outgoing
packets. Before reading and writing packets, they are decrypted and encrypted in the cipher.

All data for the entities is stored in a so called Model. There are currently two implementations for
the model interface: one using Mock data (the Mock model) and one using data stored in a
database (the PostgreSQL model). The Mock model allows to develop and test new functionality, without
having to alter the database model first. For "production" a database model should be a better fit
instead, because compared to the Mock model, the database model is persistent. Other implementations
for the Model interface, like a SQLite or Java serialization based model are not planned, but
contributors are free to implement them.

A model can be bound in the `config.xml` file which is loaded at initialization:
```xml
<server gameport="5816" authport="9958">
   <model class="net.co.java.model.Mock"/>
</server>
```

The database model can be initialized with only a few changes:
```xml
<server gameport="5816" authport="9958">
   <model class="net.co.java.model.PostgreSQL" host="jdbc:postgresql://host:port/database" username="username" password="password"/>
</server>
```

*Currently we only support a one-to-one binding of Authentication server and Game server. We're
intending to make the configuration such that additional Game Servers listening on different IP addresses
and/or port numbers can be initialized on this instance and/or bound to the Authentication server.*

Getting the client
-----
This version uses the Conquer Online 5017 client. Binaries for that version can be downloaded [here](http://www.elitepvpers.com/forum/co2-pserver-guides-releases/1672967-downloads-popular-clients-list.html). Do not forget to run the client with the `blacknull` parameter and that local IP adresses can only be accessed after a HEX edit.
