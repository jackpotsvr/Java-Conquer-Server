package net.co.java.packets;

import net.co.java.guild.GuildMember;
import net.co.java.packets.String_Packet.StringPacketType;
import net.co.java.server.GameServerClient;

public class Guild_Request_Packet implements PacketHandler{
	
	private GuildRequestType type; 
	private IncomingPacket ip;
	private long parameter_a;
	
	public Guild_Request_Packet(IncomingPacket ip){
		type = GuildRequestType.valueOf(ip.readUnsignedByte(4));
		parameter_a = ip.readUnsignedInt(8);
		this.ip = ip;
	}

	@Override
	public PacketWriter build() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void handle(GameServerClient client){
		switch(type){
			case RequestName:
				new String_Packet(type, parameter_a).handle(client);
				break;
			case RequestInfo:
				GuildMember gm = client.getPlayer().getGuildMember();
				new PacketWriter(PacketType.GUILD_INFORMATION, 40)
					.putUnsignedInteger(gm.getGuild().getUID()) // Guild ID
					.putUnsignedInteger(gm.getDonation()) // Donation
					.putUnsignedInteger(gm.getGuild().getFund()) // Fund
					.putUnsignedInteger(gm.getGuild().getMemberCount()) // Members count
					.setOffset(20).putUnsignedByte(gm.getRank().rank) // position
					.putString(gm.getGuild().getGuildLeaderName(), 16) // leader
					.send(client);
				break;
				default: 
					System.out.println(type);
		}
	}
	
	
	
	
	/**
	 * @return the type
	 */
	public GuildRequestType getType() {
		return type;
	}

	/**
	 * @return the ip
	 */
	public IncomingPacket getIncomingPacket() {
		return ip;
	}




	public enum GuildRequestType
	{
		RequestJoin		(1),
		AcceptJoin  	(2),
		Quit 			(3),
		RequestName 	(6),
		Ally 			(7),
		UnAlly 			(8),
		Enemy 			(9),
		UnEnemy 		(10),
		Donate 			(11),
		RequestInfo 	(12),
		UpdateGuild 	(13),
		UpdateBranch 	(14),
		UniteSubSyn 	(15),
		UniteSyn	 	(16),
		SetWhiteSyn 	(17),
		SetBlackSyn 	(18),
		Leave 			(19);
		
		public final int type; 
		
		private GuildRequestType(int type){
			this.type = type;
		}
		
		public static GuildRequestType valueOf(int type)  {
			for ( GuildRequestType grt : GuildRequestType.values() ) {
				if ( grt.type == type )
					return grt;
			}
			return null; 
		}
	}

}
