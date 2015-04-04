package net.co.java.packets;

import net.co.java.guild.GuildMember;
import net.co.java.server.GameServerClient;

public class Guild_Member_Information_Packet implements PacketHandler {

	private String name; 
	
	public Guild_Member_Information_Packet(IncomingPacket ip, Packet packet) {
		this.name = ip.readString(9, 16);
	}
	
	public Guild_Member_Information_Packet(GuildMember gm) {
		name = gm.getName();
	}
	
	@Override
	public PacketWriter build() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void handle(GameServerClient client) {
		for(GuildMember gm : client.getPlayer().getGuildMember().getGuild().getMembers())
			if(gm.getName().equals(name))
				new PacketWriter(PacketType.GUILD_MEMBER_INFORMATION, 25)
					.putSignedInteger(gm.getDonation())
					.putUnsignedByte(gm.getRank().rank)
					.putString(gm.getName())
					.send(client);
	}

}
