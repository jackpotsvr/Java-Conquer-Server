package net.co.java.model;

public class AccessException extends Exception {
	
	private static final long serialVersionUID = -2377985130567103182L;

	public AccessException(String message) {
		super(message);
	}
	
	public AccessException(Exception e) {
		super(e.getMessage(), e.getCause());
	}
}
