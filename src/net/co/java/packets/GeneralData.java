package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.entity.Location;
import net.co.java.entity.Player;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.server.GameServerClient;
import net.co.java.server.Map;

/**
 * The General Data packet performs a variety of tasks for the client, these vary from
 * moving the client around the game, to ending an XP skill.
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 */
public class GeneralData implements PacketHandler {
	
	private final SubType subType;
	private final long timestamp;
	private long identity;
	
	private long dwParam;
	private int wParam1;
	private int wParam2;
	private int wParam3;
	
	private IncomingPacket ip;
	
	/**
	 * Construct an GeneralData packet filled with data from
	 * an Incoming packet
	 * @param ip
	 */
	public GeneralData(IncomingPacket ip) {
		this.timestamp = ip.readUnsignedInt(4);
		this.identity = ip.readUnsignedInt(8);
		this.subType = SubType.get(ip.readUnsignedShort(22));
		this.dwParam = ip.readUnsignedInt(12);
		this.wParam1 = ip.readUnsignedShort(16);
		this.wParam2 = ip.readUnsignedShort(18);
		this.wParam3 = ip.readUnsignedShort(20);
		this.ip = ip;
	}
	
	/**
	 * Construct a GeneralData packet with given data
	 * @param subType
	 * @param entity
	 */
	public GeneralData(SubType subType, Entity entity) {
		this.timestamp = System.currentTimeMillis() & 0xFFFFFFFF;
		this.subType = subType;
		this.identity = entity.getIdentity();
	}
	
	/**
	 * @return the identity
	 */
	public long getIdentity() {
		return identity;
	}

	/**
	 * @param identity the identity to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setIdentity(long identity) {
		this.identity = identity;
		return this;
	}

	/**
	 * @return the dwParam
	 */
	public long getDwParam() {
		return dwParam;
	}

	/**
	 * @param dwParam the dwParam to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setDwParam(long dwParam) {
		this.dwParam = dwParam;
		return this;
	}

	/**
	 * @return the wParam1
	 */
	public int getwParam1() {
		return wParam1;
	}

	/**
	 * @param wParam1 the wParam1 to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setwParam1(int wParam1) {
		this.wParam1 = wParam1;
		return this;
	}

	/**
	 * @return the wParam2
	 */
	public int getwParam2() {
		return wParam2;
	}

	/**
	 * @param wParam2 the wParam2 to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setwParam2(int wParam2) {
		this.wParam2 = wParam2;
		return this;
	}

	/**
	 * @return the wParam3
	 */
	public int getwParam3() {
		return wParam3;
	}

	/**
	 * @param wParam3 the wParam3 to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setwParam3(int wParam3) {
		this.wParam3 = wParam3;
		return this;
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "GeneralData [subType=" + subType + ", timestamp=" + timestamp
				+ ", identity=" + identity + ", dwParam=" + dwParam
				+ ", wParam1=" + wParam1 + ", wParam2=" + wParam2
				+ ", wParam3=" + wParam3 + "]";
	}

	@Override
	public void handle(GameServerClient client) {
		Player hero = client.getPlayer();
		switch(subType){
		case GET_SURROUNDINGS:
			hero.spawn();
			break;
		case JUMP:
			if(System.currentTimeMillis() - client.getPlayer().getLastMovement() > 550) 
			{
				hero.jump((int) (dwParam & 0xFFFF), (int) (dwParam >> 16), wParam3, ip);
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
				.setAttribute(UpdatePacket.Mode.XPCircle, 60l)
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
			String guildname = "Yakmelk"; 
			new PacketWriter(PacketType.STRING_PACKET, 11 + guildname.length())
				.putUnsignedInteger(10324) // Guild ID
				.putUnsignedByte(3) // TYPE: GuildName 
				.putUnsignedByte(1) // String count
				.putUnsignedByte(guildname.length()) // Str length
				.putString(guildname)
				.send(client);

			new PacketWriter(PacketType.GUILD_INFORMATION, 40)
				.putUnsignedInteger(10324) // Guild ID
				.putUnsignedInteger(1000000) // Donation
				.putUnsignedInteger(1250000) // Fund
				.putUnsignedInteger(134) // Members count
				.setOffset(20).putUnsignedByte(90) // position
				.putString("Jackpotsvr", 16) // leader
				.send(client);
			
			
			new MessagePacket(MessagePacket.SYSTEM, hero.getName(), "Guild bulletin!")
				.setMessageType(MessagePacket.MessageType.GUILDBULLETIN).build().send(client);;
			new GeneralData(SubType.CONFIRM_GUILD, hero).build().send(client);
			
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
										   oldLocation.yCord, wParam3)); 
			
			for(Player p : client.getPlayer().view.getPlayers())
			{
				if(client.getPlayer() == p)
					continue; 
				build().send(p.getClient());
			}
			break;
		}
		default:
			System.out.println("Unimplemented " + subType.toString());
			break;
		}
	}
	
	

	@Override
	public PacketWriter build() {
		return new PacketWriter(PacketType.GENERAL_DATA_PACKET, 0x18)
			.putUnsignedInteger(timestamp) // 4
			.putUnsignedInteger(identity) // 8
			.putUnsignedInteger(dwParam) // 12
			.putUnsignedShort(wParam1) // 16
			.putUnsignedShort(wParam2) // 18
			.putUnsignedShort(wParam3) // 20
			.putUnsignedShort(subType.type); //22
	}

	/**
	 * Enumeration for the subtypes
	 * @author Thomas Gmelig Meyling
	 */
	public static enum SubType {
		UNIMPLEMENTED		(0x00), // 0
		LOCATION 			(0x4A), // 74
		HOTKEYS	(75),
		CONFIRM_FRIENDS(76),
		CONFIRM_PROFS(77),
		CONFIRM_SPELLS(78),
		CHANGE_DIRECTION 	(0x4F), // 79
		CHANGE_ACTION(81),
		PORTAL 				(0x55), // 85
		TELEPORT(86),
		LEVEL(92),
		END_XP_LIST(93),
		REVIVE(94),
		DELETE_CHARACTER(95),
		CHANGE_PK_MODE		(0x60), // 96
		CONFIRM_GUILD		(97),
		BEGIN_MINE(99),
		TEAMLEADERLOCATION(101),
		ENTITY_SPAWN(102),
		BEGIN_XP(103),
		CompleteMapChange(104),
		TeamMateLoc(106),
		CorrectCords(108),
		UNLEARN_SPELL(109),
		UNLEARN_PROF(110),
		Shop(111),
		OpenShop(113),
		GET_SURROUNDINGS	(0x72), //114
		REMOTE_COMMANDS(116),
		END_TRANSFORM(118),
		END_FLY(120),
		PLAYSOUND(124),
		PickupCashEffect(121),
		Dialog(126),
		GuardJump(129),
		COMPLETE_LOGIN		(130),
		ENTITY_REMOVE		(132),
		JUMP				(0x85), // 133
		RemoveWeaponMesh(135),
		RemoveWeaponMesh2(136),
		PathFinding(162);
		
		private final int type;
		
		private SubType(int type) {
			this.type = type;
		}
		
		public static SubType get(int type) {
			for (SubType st : SubType.values()) {
				if (st.type == type) {
					return st;
				}
			}
			return UNIMPLEMENTED;
		}
	}
	
}
