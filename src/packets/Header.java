package packets;

import conquerServer.ByteConversion;

public class Header{ // header of packet (Always first 4 bytes). 
	private short packetSize; 
	private PacketType type;
	
	public Header() { /* default ctor */ }
	
	/**
	 * 
	 * @param data, insert your packet here, to retreive it's header.
	 */
	
	public Header(byte[] data){
		setHeader(data);
	}
	
	/**
	 * 
	 * @param data, insert your packet here, if you didn't do with constructor.
	 */
	public void setHeader(byte[] data){
		byte[] temp = new byte[2];
		
		System.arraycopy(data, 0, temp, 0, 2);
		packetSize = ByteConversion.bytesToShort(temp);
		
		System.arraycopy(data, 0, temp, 2, 2);
		type.value = ByteConversion.bytesToShort(temp); 
	}
	/* ACCESORS */ 
	
	public void setPacketSize(short value){
		packetSize = value;
	}
	
	public void setType(PacketType value){
		type = value;
	}
	
	public void setType(short value){
		type.value = value;	
	}
	
	public short getPacketSize(){
		return packetSize; 
	}
	
	public PacketType getType(){
		return type;
	}
}