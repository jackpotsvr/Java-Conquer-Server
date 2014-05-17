package net.co.java.packets;

import java.util.ArrayList;
import java.util.List;

import net.co.java.guild.Guild;
import net.co.java.guild.GuildMember;
import net.co.java.packets.Guild_Request_Packet.GuildRequestType;
import net.co.java.server.GameServerClient;

public class String_Packet implements PacketHandler{

	private StringPacketType type; 
	private long guild_id; 
	
	
	public String_Packet(IncomingPacket ip) {
		type = StringPacketType.valueOf(ip.readUnsignedByte(8));
	}
	
	public String_Packet(GuildRequestType type, long guild_id){
		if(type == GuildRequestType.RequestName)
			this.type = StringPacketType.GuildName;
		this.guild_id = guild_id; 
	}

	@Override
	public PacketWriter build() {
		return null;
	}

	@Override
	public void handle(GameServerClient client) {
		switch(type)
		{
			case GuildMemberList:
				List <GuildMember> members = client.getPlayer().getGuildMember().getGuild().getMembers();
				int totalstrlength = 0;
				ArrayList<String> memberNames = new ArrayList<String>();
				
				for(int i = 0; i < members.size() && i < 10; i++) {
					memberNames.add(members.get(i).getName());
					totalstrlength += memberNames.get(i).length();
				}
				
				
				PacketWriter packet = new PacketWriter(PacketType.STRING_PACKET, (11 + (totalstrlength + memberNames.size())))
					.putUnsignedInteger(client.getIdentity())
					.putUnsignedByte(type.type)
					.putUnsignedByte(memberNames.size());
				
				for(String s : memberNames)	{
					packet.putUnsignedByte(s.length());
					packet.putString(s);
				}
				
				for(GuildMember gm : members){
					new Guild_Member_Information_Packet(gm).handle(client);
				}
				
				packet.send(client);

				break;
			case GuildName:
				//String guildName = client.getPlayer().getGuildMember().getGuild().getGuildName();
				String guildName; 
				for(Guild g : client.getModel().getGuilds())
						if(g.getUID() == guild_id)	
						{
							guildName = g.getGuildName();
							new PacketWriter(PacketType.STRING_PACKET, 11 + 1 + guildName.length())
								.putUnsignedInteger(g.getUID())
								.putUnsignedByte(type.type)
								.putUnsignedByte(1)
								.putUnsignedByte(guildName.length())
								.putString(guildName)
								.send(client);
						}
				 break;
			default:
				System.out.println("String packet with type: " + type + " not implemented.");
		}
		
	}
	
	public enum StringPacketType
	{
		Fireworks			(1),
		GuildName 			(3),
		Spouse 				(6),
		Wanted 				(8),
		MapEffect 			(9),
		Effect 				(10),
		GuildMemberList 	(11),
		QueryWanted 		(13),
		QueryPoliceWanted 	(14),
		PoliceWanted 		(15),
		ViewEquipment 		(16),
		AddDicePlayer 		(17),
		DeleteDicePlayer 	(18),
		DiceBonus 			(19),
		Sound 				(20),
		AllyGuild 			(21),
		EnemyGuild 			(22);
		                
		public final int type; 
		
		private StringPacketType(int type){
			this.type = type;
		}
		
		public static StringPacketType valueOf(int type)  {
			for ( StringPacketType spt : StringPacketType.values() ) {
				if ( spt.type == type )
					return spt;
			}
			return null; 
		}
	}

}
