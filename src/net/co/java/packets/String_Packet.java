package net.co.java.packets;

import java.util.ArrayList;
import java.util.List;

import net.co.java.guild.Guild;
import net.co.java.guild.GuildMember;
import net.co.java.packets.Guild_Request_Packet.GuildRequestType;
import net.co.java.server.GameServerClient;

public class String_Packet implements PacketHandler{

	private StringPacketType type; 
	private long identity; 
	private String string = null; 
	
	
	public String_Packet(IncomingPacket ip) {
		type = StringPacketType.valueOf(ip.readUnsignedByte(8));
	}
	
	public String_Packet(StringPacketType type) {
		this.type = type;
	}
	
	public String_Packet(StringPacketType type, String string, long identity) {
		this(type);
		this.string = string;
		this.identity = identity;
	}
	
	public String_Packet(GuildRequestType type, long guild_id){
		if(type == GuildRequestType.RequestName)
			this.type = StringPacketType.GuildName;
		this.identity = guild_id; 
	}

	@Override
	public PacketWriter build() {
		switch(type) {
			case Effect:
			{
				if(string != null)
				{
					return new PacketWriter(PacketType.STRING_PACKET, 11 + 1 + string.length())
						.putUnsignedInteger(identity)
						.putUnsignedByte(type.type)
						.putUnsignedByte(1)
						.putUnsignedByte(string.length())
						.putString(string);
				}
			}
		default:
			return null;
		}
	}

	@Override
	public void handle(GameServerClient client) {
		switch(type)
		{
			case GuildMemberList:
			{
				List <GuildMember> members = client.getPlayer().getGuildMember().getGuild().getMembers();
				int totalstrlength = 0;
				ArrayList<String> memberNames = new ArrayList<String>();
				
				for(int i = 0; i < members.size() && i < 10; i++) {
					memberNames.add(members.get(i).getName());
					totalstrlength += memberNames.get(i).length();
				}
				
				
				PacketWriter outgoingPacket = new PacketWriter(PacketType.STRING_PACKET, (11 + (totalstrlength + memberNames.size())))
					.putUnsignedInteger(client.getIdentity())
					.putUnsignedByte(type.type)
					.putUnsignedByte(memberNames.size());
				
				for(String s : memberNames)	{
					outgoingPacket.putUnsignedByte(s.length());
					outgoingPacket.putString(s);
				}
				
				for(GuildMember gm : members){
					new Guild_Member_Information_Packet(gm).handle(client);
				}
				
				outgoingPacket.send(client);

				break;
			}
			case GuildName:
				//String guildName = client.getPlayer().getGuildMember().getGuild().getGuildName();
				String guildName; 
				for(Guild g : client.getModel().getGuilds())
						if(g.getUID() == identity)	
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
			case EnemyGuild: 
			{
				Guild g = client.getPlayer().getGuildMember().getGuild();
				int totalstrlength = 0; 
				int strcount = 0;
				
				for(int i = 0; i < g.getEnemies().length; i++)
					if(g.getEnemies()[i] != null) {
						totalstrlength += g.getEnemies()[i].getGuildName().length();
						strcount++; 
					}
				
				PacketWriter outgoingPacket = new PacketWriter(PacketType.STRING_PACKET, 11 + strcount + totalstrlength)
					.putUnsignedInteger(client.getIdentity())
					.putUnsignedByte(type.type)
					.putUnsignedByte(strcount);
				
				for(Guild enemy : g.getEnemies())
					if(enemy != null)
					{
						outgoingPacket.putUnsignedByte(enemy.getGuildName().length());
						outgoingPacket.putString(enemy.getGuildName());
					}
				
				outgoingPacket.send(client);
				
				break;
			}
			case AllyGuild:
			{
				Guild g = client.getPlayer().getGuildMember().getGuild();
				int totalstrlength = 0; 
				int strcount = 0;
				
				for(int i = 0; i < g.getAllies().length; i++)
					if(g.getAllies()[i] != null) {
						totalstrlength += g.getAllies()[i].getGuildName().length();
						strcount++; 
					}
				
				PacketWriter outgoingPacket = new PacketWriter(PacketType.STRING_PACKET, 11 + strcount + totalstrlength)
					.putUnsignedInteger(client.getIdentity())
					.putUnsignedByte(type.type)
					.putUnsignedByte(strcount);
				
				for(Guild enemy : g.getAllies())
					if(enemy != null)
					{
						outgoingPacket.putUnsignedByte(enemy.getGuildName().length());
						outgoingPacket.putString(enemy.getGuildName());
					}
				
				outgoingPacket.send(client);
				
				break;
			}
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
