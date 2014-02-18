package net.co.java.item;

import java.util.HashMap;

import net.co.java.item.ItemPrototype.EquipmentPrototype;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;

/**
 * Create a unique item instance
 * @author Jan-Willem Gmelig Meyling
 *
 */
public class ItemInstance {
	
	public final Long uniqueIdentifier;
	
	public final ItemPrototype itemPrototype;
	
	/**
	 * Constructor for a unique item instance
	 * @param UID
	 * @param ip
	 */
	public ItemInstance(Long UID, ItemPrototype ip) {
		this.uniqueIdentifier = UID;
		this.itemPrototype = ip;
		// Add the item to the Map
		synchronized(ITEM_INSTANCES) {
			ITEM_INSTANCES.put(UID, this);
		}
	}
	
	/**
	 * Prepare an Item Information Packet for this ItemInstance
	 * @param mode
	 * @param position
	 * @return Item Information Packet
	 */
	public PacketWriter send(int mode, int position) {
		return new PacketWriter(PacketType.ITEM_INFORMATION_PACKET, 36)
		.putUnsignedInteger(uniqueIdentifier)
		.putUnsignedInteger(itemPrototype.identifier)
		.setOffset(16)
		.putUnsignedShort(mode)
		.putUnsignedShort(position);
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Object#hashCode()
	 */
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime
				* result
				+ ((uniqueIdentifier == null) ? 0 : uniqueIdentifier.hashCode());
		return result;
	}

	/* (non-Javadoc)
	 * @see java.lang.Object#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		ItemInstance other = (ItemInstance) obj;
		if (uniqueIdentifier == null) {
			if (other.uniqueIdentifier != null)
				return false;
		} else if (!uniqueIdentifier.equals(other.uniqueIdentifier))
			return false;
		return true;
	}
	
	

	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "ItemInstance [uniqueIdentifier=" + uniqueIdentifier
				+ ", itemPrototype=" + itemPrototype + "]";
	}

	private static HashMap<Long, ItemInstance> ITEM_INSTANCES = new HashMap<Long, ItemInstance>();
	
	public ItemInstance get(long id) {
		synchronized(ITEM_INSTANCES) {
			return ITEM_INSTANCES.get(id);
		}
	}

	/**
	 * A unique equipment instance
	 * @author Jan-Willem Gmelig Meyling
	 *
	 */
	public static class EquipmentInstance extends ItemInstance {
		
		public final EquipmentPrototype equipmentPrototype;
	
		// These attributes are declared volatile
		// because their values are not final, and a
		// thread may change the value. The other threads
		// must always use the newest version of these
		// attributes and should not cache the value.
		public volatile int dura;
		public volatile int firstSocket;
		public volatile int secondSocket;
		public volatile int plus;
		public volatile int bless;
		public volatile int enchant;
		
		/**
		 * Create a new EquipmentInstance
		 * @param UID
		 * @param ip
		 */
		public EquipmentInstance(Long UID, EquipmentPrototype ip) {
			super(UID, ip);
			this.equipmentPrototype = ip;
		}
	
		/**
		 * @param dura the dura to set
		 */
		public void setDura(int dura) {
			this.dura = dura;
		}
	
		/**
		 * @param firstSocket the firstSocket to set
		 */
		public void setFirstSocket(int firstSocket) {
			this.firstSocket = firstSocket;
		}
	
		/**
		 * @param secondSocket the secondSocket to set
		 */
		public void setSecondSocket(int secondSocket) {
			this.secondSocket = secondSocket;
		}
	
		/**
		 * @param plus the plus to set
		 */
		public void setPlus(int plus) {
			this.plus = plus;
		}
	
		/**
		 * @param bless the bless to set
		 */
		public void setBless(int bless) {
			this.bless = bless;
		}
	
		/**
		 * @param enchant the enchant to set
		 */
		public void setEnchant(int enchant) {
			this.enchant = enchant;
		}
		
		@Override
		public PacketWriter send(int mode, int position) {
			return super.send(mode, position)
				.setOffset(12)
				.putUnsignedShort(dura)
				.putUnsignedShort(equipmentPrototype.maxDura)
				.setOffset(24)
				.putUnsignedByte(firstSocket)
				.putUnsignedByte(secondSocket)
				.setOffset(28)
				.putUnsignedByte(plus)
				.putUnsignedByte(bless)
				.putUnsignedByte(enchant);
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		@Override
		public String toString() {
			return "EquipmentInstance [equipmentPrototype="
					+ equipmentPrototype + ", dura=" + dura + ", firstSocket="
					+ firstSocket + ", secondSocket=" + secondSocket
					+ ", plus=" + plus + ", bless=" + bless + ", enchant="
					+ enchant + ", uniqueIdentifier=" + uniqueIdentifier
					+ ", itemPrototype=" + itemPrototype + "]";
		}
		
	}
	
}
