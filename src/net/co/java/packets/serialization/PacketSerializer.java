package net.co.java.packets.serialization;

import java.lang.reflect.Field;

import net.co.java.packets.Packet;
import net.co.java.packets.PacketWriter;

public class PacketSerializer {
	protected int totalStringLength = 0;
    protected int currentStringLength = 0;
	protected final Packet packet;
	protected Class<? extends Packet> clasz;
	protected PacketWriter pw = null; 
	
	public PacketSerializer(Packet packet) { 
		this.packet = packet;
	
		try {
			clasz = Packets.getInstance().getPacketClass(packet.getType());
			setTotalStringLength(packet);
		} catch (IllegalArgumentException e) {
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			e.printStackTrace();
		}
	}
	
	/** 
	 * @param packet
	 * @throws IllegalArgumentException
	 * @throws IllegalAccessException
	 */
	protected void setTotalStringLength(Packet packet) throws IllegalArgumentException, IllegalAccessException {
		for(Field field : clasz.getDeclaredFields()) {
			if(field.isAnnotationPresent(PacketValue.class)) {
                field.setAccessible(true);
				PacketValue value = field.getAnnotation(PacketValue.class);
				switch(value.type()) {
					case STRING_WITH_LENGTH:
						totalStringLength += ((String) field.get(packet)).length();
					default:
						break;
				}
			}
		}
	}
	
	public PacketWriter serialize() { 
		int length = 0; 
		
		if(clasz.isAnnotationPresent(PacketLength.class)) {
			PacketLength packetLength = clasz.getAnnotation(PacketLength.class);
			length = packetLength.length();
		}
		
		pw = new PacketWriter(packet.getType(), length + totalStringLength);
		
		for(Field field : clasz.getDeclaredFields()) {
			if(field.isAnnotationPresent(PacketValue.class)) {
				if(field.isAnnotationPresent(Offset.class)){	
					PacketValue value = field.getAnnotation(PacketValue.class);
					Offset offset = field.getAnnotation(Offset.class);					
					field.setAccessible(true);
					try {
						switch(value.type()) { 
							case ENUM_VALUE:
								setEnum(packet, field, value, offset);
								break;
							case STRING: 
								pw.setOffset(offset.value());
								//setString(packet, field, value, offset); 
								pw.putString((String) field.get(packet), 16);
								break;
							case STRING_WITH_LENGTH: 
								setStringWithLength(packet, field, value, offset);
							case UNSIGNED_BYTE:
							case BYTE:
							case UNSIGNED_SHORT: 
							case SHORT:
							case UNSIGNED_INT:
							case INT:
							default:
								setPrimitive(packet, field, value, offset);
								break;
						}	
					} catch (IllegalArgumentException e) {
						e.printStackTrace();
					} catch (IllegalAccessException e) {
						e.printStackTrace();
					} catch (NoSuchFieldException e) {
						e.printStackTrace();
					} catch (SecurityException e) {
						e.printStackTrace();
					}
				}
			}
		}
		
		return pw; 
	}
	
	private void setEnum(Packet packet, Field field, PacketValue value,
			Offset offset) throws IllegalArgumentException, IllegalAccessException,
			NoSuchFieldException, SecurityException {
		pw.setOffset(offset.value());
		pw.putUnsignedShort(EnumUtility.getEnumValue(field.get(packet)));
	}

	private void setStringWithLength(Packet packet, Field field,
			PacketValue value, Offset offset) throws IllegalArgumentException, IllegalAccessException {
		pw.setOffset(offset.value() + currentStringLength);
		pw.putUnsignedByte(((String) field.get(packet)).length());
        pw.putString(((String) field.get(packet)));
        currentStringLength += ((String) field.get(packet)).length();
	}
	


	public void setPrimitive(Packet packet, Field field, PacketValue value, Offset offset) 
			throws IllegalArgumentException, IllegalAccessException { 
		pw.setOffset(offset.value());
		switch(value.type()) {
			case BOOLEAN: 
				pw.putBoolean(field.getBoolean(packet));
				break;
			case UNSIGNED_BYTE:
				pw.putUnsignedByte(field.getInt(packet));
				break;
			case UNSIGNED_SHORT: 
				pw.putUnsignedShort(field.getInt(packet));
				break;
			case UNSIGNED_INT:
				pw.putUnsignedInteger(field.getLong(packet));
				break;
			case INT:
				pw.putSignedInteger(field.getInt(packet));
				break;
			default: 
				break;
		}
	}
}
