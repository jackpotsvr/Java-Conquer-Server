package net.co.java.entity;

public enum Action {
	None( 0x00),
	Cool ( 0xE6),
	Kneel ( 0xD2),
	Sad ( 0xAA),
	Happy ( 0x96),
	Angry (0xA0),
	Lie ( 0x0E),
	Dance ( 0x01),
	Wave ( 0xBE),
	Bow ( 0xC8),
	Sit ( 0xFA),
	Jump ( 0x64);
	
	private final int index;
	
	private Action(int index) {
		this.index = index;
	}
	
	public int getIndex() {
		return index;
	}
}
