package packets;

import conquerServer.AuthServerThread;
import conquerServer.GameServerThread;
import data.Entity;
import data.Location;
import data.Map;
import data.Monster;
import data.Player;

public class Auth_Login_Response extends IncommingPacket
{
	
	public Auth_Login_Response(PacketType packetType, byte[] data, GameServerThread thread)
	{
		super(packetType, data);
		
		long inKey2 = this.readUnsignedInt(4);
		long inKey1 = this.readUnsignedInt(8);
		thread.setKeys(inKey1, inKey2);
		
		 /*
		  * long aRGB, long type, long chatID, String from, String to, String  message) 2101 = Login Info, no enum yet 
		  */
        //Message_Packet reply = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "NEW_ROLE");
		Message_Packet reply1 = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "ANSWER_OK");
		
		
		Map map = thread.getMap();
		Player player = new Player(thread, "Jackpotsvr", new Location(map, 382, 341), 500);
		player.setLevel(130);
		
		map.addEntity(player);
		

		
		thread.setPlayer(player);
		spawnEntity(thread);
		
		thread.offer(reply1.data);
		thread.offer(new CharacterInfoPacket(player));
		
	}
	
	/**
	 *  Make others see you :) 
	 */
	public void spawnEntity(GameServerThread thread)
	{
		Map map = thread.getMap(); 
		Player me = thread.getPlayer();
		OutgoingPacket esp = me.spawn();
		
		for( Entity e : map.getEntitiesInRange(thread.getPlayer()) ) {
			if (e == me)
				continue;
			
			if (e instanceof Player)
			{
				Player p = (Player) e;
				p.getThread().offer(esp);
			}
		}
	}
	
	public Auth_Login_Response(PacketType packetType, byte[] data, AuthServerThread thread) {
		super(packetType, data);
		
		long identity = this.readUnsignedInt(4);
		long resNumber = this.readUnsignedInt(8);
		String resLocation = this.readString(12,16);
		System.out.println("ALR: " + resLocation + " " + identity + ", "  + resNumber);
	}

}