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
	
	public static enum Mode {
		/**
		 * The default mode is used to send inventory items
		 */
		DEFAULT(1),
		
		/**
		 * The trade mode is used for trade views
		 */
		TRADE(2),
		
		UPDATE(3),
		
		/**
		 * The view mode is used when inspecting someone
		 */
		VIEW(4);
		
		public final int value;
		Mode(int value) { this.value = value; }
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

	public class ItemInformationPacket extends PacketWriter {
	
		public ItemInformationPacket(ItemInstance.Mode mode, EquipmentSlot slot) {
			super(PacketType.ITEM_INFORMATION_PACKET, 36);
			this.putUnsignedInteger(uniqueIdentifier);
			this.putUnsignedInteger(itemPrototype.identifier);
			this.setOffset(16);
			this.putUnsignedShort(mode.value);
			this.putUnsignedByte(slot.value);
			
			if(ItemInstance.this instanceof EquipmentInstance) {
				EquipmentInstance equipment = (EquipmentInstance) ItemInstance.this;
				this.setOffset(12);
				this.putUnsignedShort(equipment.dura);
				this.putUnsignedShort(equipment.equipmentPrototype.maxDura);
				this.setOffset(24);
				this.putUnsignedByte(equipment.firstSocket.value);
				this.putUnsignedByte(equipment.secondSocket.value);
				this.setOffset(28);
				this.putUnsignedByte(equipment.plus);
				this.putUnsignedByte(equipment.bless);
				this.putUnsignedByte(equipment.enchant);
			}
		}
	}

	private static HashMap<Long, ItemInstance> ITEM_INSTANCES = new HashMap<Long, ItemInstance>();
	
	public static ItemInstance get(long id) {
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
		public volatile Socket firstSocket = Socket.None;
		public volatile Socket secondSocket = Socket.None;
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
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setDura(int dura) {
			this.dura = dura;
			return this;
		}
	
		/**
		 * @param firstSocket the firstSocket to set
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setFirstSocket(Socket firstSocket) {
			this.firstSocket = firstSocket;
			return this;
		}
	
		/**
		 * @param secondSocket the secondSocket to set
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setSecondSocket(Socket secondSocket) {
			this.secondSocket = secondSocket;
			return this;
		}
	
		/**
		 * @param plus the plus to set
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setPlus(int plus) {
			this.plus = plus;
			return this;
		}
	
		/**
		 * @param bless the bless to set
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setBless(int bless) {
			this.bless = bless;
			return this;
		}
	
		/**
		 * @param enchant the enchant to set
		 * @return this EquipmentInstance - builder pattern
		 */
		public EquipmentInstance setEnchant(int enchant) {
			this.enchant = enchant;
			return this;
		}
		
		public static EquipmentInstance get(long id) {
			ItemInstance item = ItemInstance.get(id);
			if ( item instanceof EquipmentInstance)
				return (EquipmentInstance) item;
			return null;
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

		public static enum Socket {
			None(0),
			Empty(4),
			NormalPhoenix(1),
			RefinedPhoenix(2),
			SuperPhoenix(3),
			NormalDragon(11),
			RefinedDragon(12),
			SuperDragon(13),
			NormalFury(21),
			RefinedFury(22),
			SuperFury(23),
			NormalRainbowGem(31),
			RefinedRainbowGem(32),
			SuperRainbowGem(33),
			NormalKylinGem(41),
			RefinedKylinGem(42),
			SuperKylinGem(43),
			NormalVioletGem(51),
			RefinedVioletGem(52),
			SuperVioletGem(53),
			NormalMoonGem(61),
			RefinedMoonGem(62),
			SuperMoonGem(63),
			NormalTortoiseGem(71),
			RefinedTortoiseGem(72),
			SuperTortoiseGem(73);
			
			final int value;
			Socket(int value) { this.value = value; }
			
			public static Socket valueOf(int value) {
				for ( Socket s : Socket.values() )
					if ( s.value == value )
						return s;
				return null;
			}
		}
		
	}
	
}
