package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.server.Server.Map;
import net.co.java.server.Server.GameServer.Client;

/**
 * The General Data packet performs a variety of tasks for the client, these vary from
 * moving the client around the game, to ending an XP skill.
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 */
public class GeneralData implements PacketHandler {
	
	private SubType subType;
	private final long timestamp;
	private long identity;
	
	private int[] shorts;
	private int shortValue;
	private long intValue;
	private IncomingPacket ip;

	/**
	 * Construct an empty GeneralData packet
	 */
	public GeneralData() {
		this.timestamp = System.currentTimeMillis();
	}
	
	/**
	 * Construct an GeneralData packet filled with data from
	 * an Incoming packet
	 * @param ip
	 */
	public GeneralData(IncomingPacket ip) {
		this.timestamp = ip.readUnsignedInt(4);
		this.identity = ip.readUnsignedInt(8);
		this.subType = SubType.get(ip.readUnsignedShort(22));
		this.shortValue =  ip.readUnsignedShort(16);
		this.intValue = ip.readUnsignedInt(12);
		this.shorts = new int[] {
			ip.readUnsignedShort(12),
			ip.readUnsignedShort(14),
			ip.readUnsignedShort(16)
		};
		this.ip = ip;
	}

	/**
	 * Construct a GeneralData packet with given data
	 * @param subType
	 * @param identity
	 * @param shortValue
	 * @param intValue
	 */
	public GeneralData(SubType subType, long identity, int shortValue, long intValue) {
		this();
		setSubType(subType);
		setIdentity(identity);
		setShortValue(shortValue);
		setIntValue(intValue);	
	}
	
	/**
	 * Construct a GeneralData packet with given data
	 * @param subType
	 * @param identity
	 * @param shorts
	 */
	public GeneralData(SubType subType, long identity, int[] shorts) {
		this();
		setSubType(subType);
		setIdentity(identity);
		setShorts(shorts);
	}
	
	/**
	 * Set the short value
	 * @param shortValue
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setShortValue(int shortValue) {
		this.shortValue = shortValue;
		return this;
	}

	/**
	 * @return the short value
	 */
	public int getShortValue() {
		return shortValue;
	}

	/**
	 * Set the integer value
	 * @param intValue
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setIntValue(long intValue) {
		this.intValue = intValue;
		return this;
	}

	/**
	 * @return the integer value
	 */
	public long getIntValue() {
		return intValue;
	}

	/**
	 * @return the shorts
	 */
	public int[] getShorts() {
		return shorts;
	}

	/**
	 * Set the shorts
	 * @param shorts
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setShorts(int[] shorts) {
		this.shorts = shorts;
		return this;
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
	 * @return the subType
	 */
	public SubType getSubType() {
		return subType;
	}

	/**
	 * @param subType the subType to set
	 * @return this GeneralData instance (builder pattern)
	 */
	public GeneralData setSubType(SubType subType) {
		this.subType = subType;
		return this;
	}

	@Override
	public void handle(Client client) {
		switch(subType){
		case GET_SURROUNDINGS:
			for (Entity e : client.getPlayer().getSurroundings())
				e.spawn().send(client);
			break;
		case JUMP:
			client.getPlayer().jump(shorts[0], shorts[1], ip);
			break;
		case LOCATION:
			client.getPlayer().retrieveLocation().build().send(client);
			break;
		case PORTAL:
			System.out.printf("GENERAL DATA: %s , %s ,%s", shorts[0], shorts[1], shorts[2]);
			client.getPlayer().setLocation(Map.CentralPlain.new Location(250, 180), null);
			client.getPlayer().retrieveLocation().build().send(client);
			break;
		default:
			System.out.println("Unimplemented " + subType.toString());
			break;
		}
	}

	@Override
	public PacketWriter build() {
		return new PacketWriter(PacketType.GENERAL_DATA_PACKET, 0x18)
		.putUnsignedInteger(timestamp)
		.putUnsignedInteger(identity)
		.setOffset(12)
		.putUnsignedShort(shorts[0])
		.setOffset(16)
		.putUnsignedShort(shorts[1])
		.putUnsignedShort(shorts[2])
		.setOffset(22)
		.putUnsignedShort(subType.type);
	}

	/**
	 * Enumeration for the subtypes
	 * @author Thomas Gmelig Meyling
	 */
	public static enum SubType {
		UNIMPLEMENTED		(0x00), // 0
		LOCATION 			(0x4A), // 74 
		CHANGE_DIRECTION 	(0x4F), // 79
		PORTAL 				(0x55), // 85
		GET_SURROUNDINGS	(0x72), //114
		ENTITY_REMOVE		(0x84), // 132
		JUMP				(0x85); //133
		
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
