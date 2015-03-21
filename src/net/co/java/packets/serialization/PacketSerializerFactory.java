package net.co.java.packets.serialization;

import net.co.java.packets.GeneralDataPacket;
import net.co.java.packets.Packet;
import net.co.java.packets.PacketType;


public enum PacketSerializerFactory {
	GENERAL_DATA_PACKET_SERIALIZER(PacketType.GENERAL_DATA_PACKET) {
		public PacketSerializer<GeneralDataPacket> getInstance(Packet packet) {
				PacketSerializer<GeneralDataPacket> serializer = 
						new PacketSerializer<GeneralDataPacket>(packet){};
				/* The extra brackets are important for generic type determination. */ 
				return serializer;
		}
	};
	
	private PacketType type; 
	
	private PacketSerializerFactory(PacketType type) {
		this.type = type; 
	}
	
	public abstract PacketSerializer<?> getInstance(Packet packet);
	
	/** 
	 * @param type The PacketType of the packet you want to be deserialized. 
	 * @return instance of the factory that can instantiate the deserializer with getInstance(). 
	 */
	public static PacketSerializerFactory valueOf(PacketType type) {
		for ( PacketSerializerFactory pt : PacketSerializerFactory.values() ) 
			if ( pt.type == type )
				return pt;
		return null; 
	}
}
