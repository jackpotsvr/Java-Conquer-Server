package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.packets.packethandlers.GeneralDataPacketHandler;
import net.co.java.packets.serialization.*;

/**
 * The General Data packet performs a variety of tasks for the client, these vary from
 * moving the client around the game, to ending an XP skill.
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 */
@Type(type = PacketType.GENERAL_DATA_PACKET)
@Bidirectional(handler = GeneralDataPacketHandler.class)
@PacketLength(length = 24)
public class GeneralDataPacket extends Packet {
	
	/**
	 * Construct an GeneralData packet filled with data from
	 * an Incoming packet
	 * @param ip
	 */
	public GeneralDataPacket(IncomingPacket ip) {
		super(ip);
	}


    /**
	 * Construct a GeneralData packet with given data
	 * @param location
	 * @param entity
	 */
	public GeneralDataPacket(net.co.java.packets.GeneralData.SubType location, Entity entity) {
		super(null);
		this.setType(PacketType.GENERAL_DATA_PACKET);
		this.timestamp = System.currentTimeMillis() & 0xFFFFFFFF;
		this.subType = location;
		this.identity = entity.getIdentity();
	}

	@PacketValue(type = PacketValueType.ENUM_VALUE)
	@Offset(22)
	private SubType subType;
	
	@PacketValue(type = PacketValueType.UNSIGNED_INT)
	@Offset(value = 4)
	private long timestamp;
	
	@PacketValue(type = PacketValueType.UNSIGNED_INT)
	@Offset(value = 8)
	private long identity;
	
	@PacketValue(type = PacketValueType.UNSIGNED_INT)
	@Offset(value = 12)
	private long dwParam;
	
	@PacketValue(type = PacketValueType.UNSIGNED_SHORT)
	@Offset(value = 16)
	private int wParam1;
	
	@PacketValue(type = PacketValueType.UNSIGNED_SHORT)
	@Offset(value = 18)
	private int wParam2;
	
	@PacketValue(type = PacketValueType.UNSIGNED_SHORT)
	@Offset(value = 20)
	private int wParam3;


		
	public SubType getSubType() {
		return subType;
	}
	
	public GeneralDataPacket setSubType(SubType subType) {
		this.subType = subType;
		return this;
	}
	
	public long getTimestamp() {
		return timestamp;
	}

	public GeneralDataPacket setTimestamp(long timestamp) {
		this.timestamp = timestamp;
		return this;
	}
	
	public long getIdentity() {
		return identity;
	}

	public GeneralDataPacket setIdentity(long identity) {
		this.identity = identity;
		return this;
	}

	public long getDwParam() {
		return dwParam;
	}

	public GeneralDataPacket setDwParam(long dwParam) {
		this.dwParam = dwParam;
		return this;
	}

	public int getwParam1() {
		return wParam1;
	}

	public GeneralDataPacket setwParam1(int wParam1) {
		this.wParam1 = wParam1;
		return this;
	}

	public int getwParam2() {
		return wParam2;
	}

	public GeneralDataPacket setwParam2(int wParam2) {
		this.wParam2 = wParam2;
		return this;
	}

	public int getwParam3() {
		return wParam3;
	}

	public GeneralDataPacket setwParam3(int wParam3) {
		this.wParam3 = wParam3;
		return this;
	}

//	/**
//	 * Enumeration for the subtypes
//	 * @author Thomas Gmelig Meyling
//	 */
//	public static enum SubType {
//		UNIMPLEMENTED		(0x00), // 0
//		LOCATION 			(0x4A), // 74
//		HOTKEYS	(75),
//		CONFIRM_FRIENDS(76),
//		CONFIRM_PROFS(77),
//		CONFIRM_SPELLS(78),
//		CHANGE_DIRECTION 	(0x4F), // 79
//		CHANGE_ACTION(81),
//		PORTAL 				(0x55), // 85
//		TELEPORT(86),
//		LEVEL(92),
//		END_XP_LIST(93),
//		REVIVE(94),
//		DELETE_CHARACTER(95),
//		CHANGE_PK_MODE		(0x60), // 96
//		CONFIRM_GUILD		(97),
//		BEGIN_MINE(99),
//		TEAMLEADERLOCATION(101),
//		ENTITY_SPAWN(102),
//		BEGIN_XP(103),
//		CompleteMapChange(104),
//		TeamMateLoc(106),
//		CorrectCords(108),
//		UNLEARN_SPELL(109),
//		UNLEARN_PROF(110),
//		Shop(111),
//		OpenShop(113),
//		GET_SURROUNDINGS	(0x72), //114
//		REMOTE_COMMANDS(116),
//		END_TRANSFORM(118),
//		END_FLY(120),
//		PLAYSOUND(124),
//		PickupCashEffect(121),
//		Dialog(126),
//		GuardJump(129),
//		COMPLETE_LOGIN		(130),
//		ENTITY_REMOVE		(132),
//		JUMP				(0x85), // 133
//		RemoveWeaponMesh(135),
//		RemoveWeaponMesh2(136),
//		PathFinding(162);
//		
//		private final int type;
//		
//		private SubType(int type) {
//			this.type = type;
//		}
//		
//		public static SubType valueOf(int type) {
//			for (SubType st : SubType.values()) {
//				if (st.type == type) {
//					return st;
//				}
//			}
//			return UNIMPLEMENTED;
//		}
//	}
}
