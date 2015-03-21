package net.co.java.packets.serialization;

import java.lang.reflect.Field;
import java.lang.reflect.Type;

import net.co.java.packets.Packet;
import net.co.java.packets.PacketWriter;

public abstract class PacketSerializer<T extends Packet> {
	protected int totalStringLength = 0; 
	protected final Packet packet; 
	protected final Type type; 
	protected Class<?> clasz;
	protected PacketWriter pw = null; 
	
	public PacketSerializer(Packet packet) { 
		this.packet = packet; 
		type = GenericUtility.getGenericType(this);
	
		try {
			clasz = GenericUtility.getClass(type);
			setTotalStringLength(packet);
		} catch (IllegalArgumentException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
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
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (IllegalAccessException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (NoSuchFieldException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (SecurityException e) {
						// TODO Auto-generated catch block
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
		pw.setOffset(offset.value());
		pw.putUnsignedByte(((String) field.get(packet)).length());
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
