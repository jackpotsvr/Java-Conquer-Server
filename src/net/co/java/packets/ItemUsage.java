package net.co.java.packets;

import net.co.java.server.Server.GameServer.Client;

/**
 * Item usage packet
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 */
public class ItemUsage {

	private final long identity;
	private final long parameter;
	private final Mode mode;
	private final long timestamp;
	
	/**
	 * Create a new ItemUsage packet based on parameters
	 * @param identity
	 * @param parameter
	 * @param mode
	 */
	public ItemUsage(long identity, long parameter, Mode mode, long timestamp) {
		this.identity = identity;
		this.parameter = parameter;
		this.mode = mode;
		this.timestamp = timestamp;
	}
	
	/**
	 * Create a new ItemUsage packet based on an Incomming packet
	 * @param ip
	 */
	public ItemUsage(IncomingPacket ip) {
		identity = ip.readUnsignedInt(4);
		parameter = ip.readUnsignedInt(8);
		mode = Mode.getMode(ip.readUnsignedInt(12));
		timestamp = ip.readUnsignedInt(16);
	}

	/**
	 * @return the parameter
	 */
	public long getParameter() {
		return parameter;
	}

	/**
	 * @return the identity
	 */
	public long getIdentity() {
		return identity;
	}

	/**
	 * @return the mode
	 */
	public Mode getMode() {
		return mode;
	}

	/**
	 * @return the timestamp
	 */
	public long getTimestamp() {
		return timestamp;
	}
	
	/**
	 * @return a PacketWriter based on this ItemUsage packet
	 */
	public PacketWriter build() {
		return new PacketWriter(PacketType.ITEM_USAGE_PACKET, 28)
		.putUnsignedInteger(identity)
		.putUnsignedInteger(parameter)
		.putUnsignedInteger(mode.index)
		.putUnsignedInteger(timestamp);
	}
	
	public void handle(Client client) {
		switch(mode){
		case AddVendingItem:
			break;
		case BuyItem:
			break;
		case BuyVendingItem:
			break;
		case DepositWarehouseMoney:
			break;
		case DropGold:
			break;
		case EquipItem:
			break;
		case ParticleEffect:
			break;
		case Ping:
			new ItemUsage(client.getIdentity(), 0, mode, timestamp).build().send(client);
			break;
		case RemoveEquipment:
			break;
		case RemoveInventory:
			break;
		case RemoveVendingItem:
			break;
		case RepairItem:
			break;
		case SellItem:
			break;
		case SetEquipPosition:
			break;
		case ShowVendingList:
			break;
		case ShowWarehouseMoney:
			break;
		case UnEquipItem:
			break;
		case UpdateArrowCount:
			break;
		case UpdateDurability:
			break;
		case UpgradeDragonball:
			break;
		case UpgradeMeteor:
			break;
		case WithdrawWarehouseMoney:
			break;
		default:
			break;
		}
	}
	
	public static void handle(ItemUsage packet, Client client) {
		packet.handle(client);
	}
	
	public static void handle(IncomingPacket packet, Client client) {
		new ItemUsage(packet).handle(client);
	}

	/**
	 * Enumeration of Item usage modes
	 * @author Thomas Gmelig Meyling
	 *
	 */
	public static enum Mode {
		BuyItem(1), SellItem(2), RemoveInventory(3), EquipItem(4), SetEquipPosition(
		5), UnEquipItem(6), ShowWarehouseMoney(9), DepositWarehouseMoney(
		10), WithdrawWarehouseMoney(11), DropGold(12), RepairItem(14), UpdateDurability(
		17), RemoveEquipment(18), UpgradeDragonball(19), UpgradeMeteor(
		20), ShowVendingList(21), AddVendingItem(22), RemoveVendingItem(
		23), BuyVendingItem(24), UpdateArrowCount(25), ParticleEffect(
		26), Ping(27);
		
		private final int index; 
		
		private Mode(int index){
			this.index = index; 
		}
		
		public static Mode getMode(long index) {
			for (Mode m : Mode.values()) {
				if (m.index == index) {
					return m;
				}
			}
			return null;
		}
	}   

}
