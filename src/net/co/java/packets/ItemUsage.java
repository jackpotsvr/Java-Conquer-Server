package net.co.java.packets;

import net.co.java.entity.Location;
import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.model.AccessException;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.server.GameServerClient;
import net.co.java.server.Map;

/**
 * Item usage packet
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 */
public class ItemUsage implements PacketHandler {

	private final long identity;
	private final long parameter;
	private final Mode mode;
	private final long timestamp;
	
	public ItemUsage(long identity, long parameter, Mode mode) {
		this(identity, parameter, mode, System.currentTimeMillis() & 0xFFFFFFFF);
	}
	
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
		mode = Mode.valueOf(ip.readUnsignedInt(12));
		timestamp = ip.readUnsignedInt(16);
		if ( mode == Mode.EquipItem ) {
			System.out.println(ip.toString());
			System.out.println(this.toString());
		}
			
	}

	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "ItemUsage [identity=" + identity + ", parameter=" + parameter
				+ ", mode=" + mode + ", timestamp=" + timestamp + "]";
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
	
	@Override
	public PacketWriter build() {
		return new PacketWriter(PacketType.ITEM_USAGE_PACKET, 28)
		.putUnsignedInteger(identity)
		.putUnsignedInteger(parameter)
		.putUnsignedInteger(mode.index)
		.putUnsignedInteger(timestamp);
	}

	@Override
	public void handle(GameServerClient client) {
		switch(mode){
		case Ping:
			new ItemUsage(client.getIdentity(), 0, mode, timestamp).build().send(client);
			break;
		case EquipItem:
			try {
				ItemInstance item = client.getModel().getItemInstance(identity);
				if(item instanceof EquipmentInstance)
				{
					client.getPlayer().inventory.equip((int) parameter, (EquipmentInstance) client.getModel().getItemInstance(identity));
				
					for(Player p : client.getPlayer().view.getPlayers())
					{
						if(client.getPlayer() == p)
							continue; 
						new SpawnPacket(client.getPlayer()).send(p.getClient());
					}
				}
					
				if(parameter == 0) // parameter 0 is for no equips, for instance usage of TwinCity gates and Windscrolls
				{
					switch (client.getModel().getItemInstance(identity).itemPrototype.identifier.intValue()) // should get item_sid from identity before switch.
					{
						case 1060020: // // tc gate 1002, 439, 383
							client.getPlayer().setLocation(new Location(Map.CentralPlain, 439, 383)); 
							new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
							break;
						case 1060021: //desert
							client.getPlayer().setLocation(new Location(Map.Desert, 491, 647)); // 491, 
							new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
							break;
						case 1060022: // ape
							client.getPlayer().setLocation(new Location(Map.ApeMoutain, 567, 563));
							new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
							break;
						case 1060023: // castle
							client.getPlayer().setLocation(new Location(Map.PhoenixCastle, 190, 262));
							new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
							break;
						case 1060024: //bi
							client.getPlayer().setLocation(new Location(Map.BirdIsland, 716, 572));
							new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
							break;
					
					}
				}
			} catch (AccessException e) {} /* ClassCastException is for when people send item equip packets for no equipment items*/
			break;
		case UnEquipItem:
			client.getPlayer().inventory.unequip((int) parameter);
			break;
		case AddVendingItem:
		case BuyItem:
		case BuyVendingItem:
		case DepositWarehouseMoney:
		case DropGold:
		case ParticleEffect:
		case RemoveEquipment:
		case RemoveInventory:
		case RemoveVendingItem:
		case RepairItem:
		case SellItem:
		case SetEquipPosition:
		case ShowVendingList:
		case ShowWarehouseMoney:
			
		case UpdateArrowCount:
		case UpdateDurability:
		case UpgradeDragonball:
		case UpgradeMeteor:
		case WithdrawWarehouseMoney:
		default:
			System.out.println("Unimplemented ItemUsage/" + mode + " with parameter " + parameter);
			break;
		}
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
		
		public static Mode valueOf(long index) {
			for (Mode m : Mode.values()) {
				if (m.index == index) {
					return m;
				}
			}
			return null;
		}
	}   
}
