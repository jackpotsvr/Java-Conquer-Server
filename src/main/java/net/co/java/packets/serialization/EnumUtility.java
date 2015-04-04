package net.co.java.packets.serialization;

import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;

public class EnumUtility {
	@SuppressWarnings("unchecked") // the hard cast won't result in problems here. 
	public static <G extends Enum<G>> G valueOf(Class<G> enumType, int value) 
			throws IllegalAccessException, IllegalArgumentException, InvocationTargetException,
			NoSuchMethodException, SecurityException {
		return (G) enumType.getMethod("valueOf", Integer.TYPE).invoke(null, value);
	}
	
	public static int getEnumValue(Object theEnum)
			throws IllegalArgumentException, IllegalAccessException { 
		int value = 0; 
		for(Field field : theEnum.getClass().getDeclaredFields()) {
			if(!field.isEnumConstant()) { 
				if(field.getType() == Integer.TYPE) { 
					field.setAccessible(true);
					value = field.getInt(theEnum);
				}
			}
		}
		return value; 
	}
}
