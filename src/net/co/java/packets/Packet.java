package net.co.java.packets;

import net.co.java.packets.serialization.PacketValue;
import net.co.java.packets.serialization.PacketValueType;

public abstract class Packet {
	private IncomingPacket ip; 
	
	public PacketHeader header; 
	
	public Packet(IncomingPacket ip) { 
		this.ip = ip; 
		if(ip == null) { 
			header = new PacketHeader(); 
		}
	}
	
	public PacketType getType() {
        return (ip == null) ? header.type : ip.getPacketType();
    };


	public void setType(PacketType type) { 
		header.type = type; 
	}
	
	public int getLength() { 
		return header.length; 
	}
	
	public void setLength(int length) { 
		header.length = length; 
	}
	
	public IncomingPacket getIncomingPacket() { 
		return ip; 
	}
	
	public static final class PacketHeader { 
		
		@PacketValue(type = PacketValueType.ENUM_VALUE)
		private PacketType type; 
		
		@PacketValue(type = PacketValueType.UNSIGNED_SHORT)
		private int length; 
		
		public PacketType getType() {
			return this.type; 
		}
		
		public void setType(PacketType type) { 
			this.type = type; 
		}
		
		public int getLength() { 
			return this.length; 
		}
		
		public void setLength(int length) { 
			this.length = length; 
		}
	}
}
