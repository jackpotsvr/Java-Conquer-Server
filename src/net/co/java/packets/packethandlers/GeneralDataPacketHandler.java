package net.co.java.packets.packethandlers;


import net.co.java.entity.Location;
import net.co.java.entity.Player;
import net.co.java.guild.GuildMember;
import net.co.java.packets.AbstractPacketHandler;
import net.co.java.packets.GeneralData;
import net.co.java.packets.GeneralDataPacket;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.Packet;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.String_Packet;
import net.co.java.packets.UpdatePacket;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.packets.Guild_Request_Packet.GuildRequestType;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.packets.String_Packet.StringPacketType;
import net.co.java.server.GameServerClient;
import net.co.java.server.Map;

public class GeneralDataPacketHandler extends AbstractPacketHandler {
	

	public GeneralDataPacketHandler(Packet packet) {
		super(packet);
	}

	@Override
	public void handle(GameServerClient client) {
		if(!(packet instanceof GeneralDataPacket)) 
			return; 
		
		GeneralDataPacket gpd = (GeneralDataPacket) packet; 
		
		Player hero = client.getPlayer();
		switch(gpd.getSubType()){
		case GET_SURROUNDINGS:
			hero.spawn();
			break;
		case JUMP:
			if(System.currentTimeMillis() - client.getPlayer().getLastMovement() > 550) 
			{
				hero.jump((int) (gpd.getDwParam() & 0xFFFF), (int) (gpd.getDwParam() >> 16), 
						gpd.getwParam3(), packet.getIncomingPacket());
				client.getPlayer().setLastMovement(System.currentTimeMillis());
			}
			else
			{
				new MessagePacket(MessagePacket.SYSTEM, client.getPlayer().getName(), "Sorry, you can't jump yet.")
					.setMessageType(MessageType.TOPLEFT)
					.setARGB(0xFFFF0000)
					.build().send(client);
				new GeneralData(SubType.LOCATION, hero)
					.setDwParam(hero.getLocation().getMap().getMapID())
					.setwParam1(hero.getLocation().getxCord())
					.setwParam2(hero.getLocation().getyCord())
					.build().send(client);
			}
			break;
		case LOCATION:
			{ // Update location
				Location location = hero.getLocation();
				
				new GeneralData(SubType.LOCATION, hero)
					.setDwParam(location.getMap().getMapID())
					.setwParam1(location.getxCord())
					.setwParam2(location.getyCord())
					.build().send(client);
				
				/*
				 * This packet is a bit unknown for now, but when sent, the
				 * map name is shown in the center of the screen and appears in the minimap
				 */
				new PacketWriter(PacketType.MAP_STATUS, 24)
					.putUnsignedInteger(System.currentTimeMillis() & 0xFFFFFFFF)
					.putUnsignedInteger(hero.getIdentity())
					.putUnsignedInteger(0xFFFFFFFFl)
					.putUnsignedShort(location.getxCord())
					.putUnsignedShort(location.getyCord())
					.setOffset(22).putUnsignedByte(0x68).send(client);
				
				// Retrieve map status TODO : Status from map
				// Sound plays
				new PacketWriter(PacketType.MAP_STATUS, 16)
					.putUnsignedInteger(location.getMap().getMapID()) // Map ID
					.putUnsignedInteger(location.getMap().getMapID()) // Map unique ID
					.putUnsignedInteger(0x0000) // Map status
				.send(client);
			}
		
			new UpdatePacket(hero)
				.setAttribute(UpdatePacket.Mode.RaiseFlag, hero.getFlags())
				.setAttribute(UpdatePacket.Mode.Stamina, (long) hero.getStamina())
				.setAttribute(UpdatePacket.Mode.HP, (long) hero.getHP())
				.setAttribute(UpdatePacket.Mode.Mana, (long) hero.getMana())
				.setAttribute(UpdatePacket.Mode.MaxHP, (long) hero.getMaxHP())
				.setAttribute(UpdatePacket.Mode.MaxMana, (long) hero.getMaxMana())
				.setAttribute(UpdatePacket.Mode.XPCircle, 0L)
				.setAttribute(UpdatePacket.Mode.Level, (long) hero.getLevel())
				.setAttribute(UpdatePacket.Mode.LocationPoint, 0l)
				.build().send(client);
			// Retrieve weather TODO : Weather from MAP
			new PacketWriter(PacketType.WEATHER_PACKET, 20)
				.putUnsignedByte(3) // Map effect
				.setOffset(8).putUnsignedInteger(30) // Intensity
				.putUnsignedInteger(75) // Direction
				.putUnsignedInteger(1) // colour
				.send(client);
			
			// Set PK Mode  TODO Retrieve from player
			new GeneralData(SubType.CHANGE_PK_MODE, hero).setDwParam(3).build().send(client);
			// TODO Start XP Circle
			
			// Send inventory, equips, skills and professions
			hero.inventory.send();
			hero.sendProficiencies();
			hero.sendSkills();
			new GeneralData(SubType.CONFIRM_PROFS, hero).build().send(client);
			new GeneralData(SubType.CONFIRM_SPELLS, hero).build().send(client);
			
			// Send friends and enemies
			new GeneralData(SubType.CONFIRM_FRIENDS, hero).build().send(client);
			
			// Send guilds
			GuildMember gm = client.getPlayer().getGuildMember();
			/** 
			new PacketWriter(PacketType.STRING_PACKET, 11 + 1 + gm.getGuild().getGuildName().length())
				.putUnsignedInteger(gm.getGuild().getUID()) // Guild  TODO
				.putUnsignedByte(3) // TYPE: GuildName 
				.putUnsignedByte(1) // String count
				.putUnsignedByte(gm.getGuild().getGuildName().length()) // Str length
				.putString(gm.getGuild().getGuildName())
				.send(client); */ 
			
			//new String_Packet(GuildRequestType.RequestName, gm.getGuild().getUID()).handle(client);
			
			new PacketWriter(PacketType.GUILD_INFORMATION, 40)
				.putUnsignedInteger(gm.getGuild().getUID()) // Guild ID
				.putUnsignedInteger(gm.getDonation()) // Donation
				.putUnsignedInteger(gm.getGuild().getFund()) // Fund
				.putUnsignedInteger(gm.getGuild().getMemberCount()) // Members count
				.setOffset(20).putUnsignedByte(gm.getRank().rank) // position
				.putString(gm.getGuild().getGuildLeaderName(), 16) // leader
				.send(client);
						
			new String_Packet(GuildRequestType.RequestName, gm.getGuild().getUID()).handle(client);
			
			new MessagePacket(MessagePacket.SYSTEM, hero.getName(), "Guild bulletin!")
				.setMessageType(MessagePacket.MessageType.GUILDBULLETIN).build().send(client);
			new GeneralData(SubType.CONFIRM_GUILD, hero).build().send(client);
			
			
			// TODO figure when to send this.. 
			new String_Packet(StringPacketType.EnemyGuild).handle(client);
			new String_Packet(StringPacketType.AllyGuild).handle(client);
			
			// Send animations
			/* 2NDMetempsychosis   for 2nd RB light of vigor
			 * letter1  - letter7 for King rank etc.
			 * coronet3 & coronet4 for unknown animations
			 */
			String animation = "letter7";
			new PacketWriter(PacketType.STRING_PACKET, 11 + animation.length())
				.putUnsignedInteger(hero.getIdentity())
				.putUnsignedByte(10) // Type
				.putUnsignedByte(1) // Str count
				.putUnsignedByte(animation.length()) // Str length
				.putString(animation)
				.send(client);
			
			// Send default messages
			new MessagePacket(MessagePacket.SYSTEM, hero.getName(), "Players online " + client.getGameServer().getAmountOfPlayers())
				.setMessageType(MessagePacket.MessageType.SYSTEM)
				.build().send(client);
			
			hero.inventory.send();

			new GeneralData(GeneralData.SubType.COMPLETE_LOGIN, hero).build().send(hero);
			
			break;
		case PORTAL:
			{ // Update Location
				Location location = new Location(Map.CentralPlain, 250, 180);
				client.getPlayer().setLocation(location);
				new GeneralData(SubType.LOCATION, hero)
					.setDwParam(location.getMap().getMapID())
					.setwParam1(location.getxCord())
					.setwParam2(location.getyCord())
					.build().send(client);
				break;
			}
		/** just resend the packet, like change direction, though in future you might want to set something
		 * for increased stamina regen.
		 */
		case CHANGE_ACTION:
		{
			for(Player p : client.getPlayer().view.getPlayers())
			{
				if(client.getPlayer() == p)
					continue; 
				build().send(p.getClient());
			}
			break;
		}
		case CHANGE_DIRECTION:
		{
			Location oldLocation = client.getPlayer().getLocation();
			client.getPlayer().setLocation(new Location(oldLocation.map, oldLocation.xCord, 
										   oldLocation.yCord, gpd.getwParam3())); 
			
			for(Player p : client.getPlayer().view.getPlayers())
			{
				if(client.getPlayer() == p)
					continue; 
				build().send(p.getClient());
			}
			break;
		}
		case END_XP_LIST:
		{
			client.getPlayer().setXPON(false);
			client.getPlayer().setXpRing(0);
			this.build().send(client);
			break;
		}
		
		default:
			System.out.println("Unimplemented " + gpd.getSubType().toString());
			break;
		}
		
	}

}
