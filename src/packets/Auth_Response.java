package packets;

public class Auth_Response extends IncommingPacket{
	private long key2;
	private long key1;
	
	public Auth_Response(IncommingPacket ip){
		super(ip);
		key2 = this.readUnsignedInt(4);
		key1 = this.readUnsignedInt(8);
	}
	
	public long getKey2(){
		return key2;
	}
	
	public long getKey1(){
		return key1;
	}
}

/**
public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName;
	private String password;
	private String serverName;
	
	public Auth_Login_Packet(IncommingPacket ip) {
		super(ip);
		accoutName	= this.readString(4,16);
		password	= this.readPassword(20);
		serverName	= this.readString(36, 16);
	}
	
	*/