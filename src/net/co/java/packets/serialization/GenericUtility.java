package net.co.java.packets.serialization;

import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;


public class GenericUtility {
	private static final String TYPE_NAME_PREFIX = "class ";
    private static Type type;

    private static String getClassName(Type type) {
        GenericUtility.type = type;
        if (type==null) return "";
	    String className = type.toString();
	    if (className.startsWith(TYPE_NAME_PREFIX)) {
	        className = className.substring(TYPE_NAME_PREFIX.length());
	    }
	    return className;
	}
	 
	public static Class<?> getClass(Type type) 
	            throws ClassNotFoundException {
	    String className = getClassName(type);
	    if (className==null || className.isEmpty()) {
	        return null;
	    }
	    return Class.forName(className);
	}
	
	public static Type getGenericType(Object instance) {
		Type mySuperclass = instance.getClass().getGenericSuperclass();
		Type tType = ((ParameterizedType)mySuperclass).getActualTypeArguments()[0];
		return tType; 
	}
	

}
