package net.co.java.packets.packethandlers;

import com.sun.org.apache.xml.internal.security.algorithms.MessageDigestAlgorithm;
import net.co.java.entity.Player;
import net.co.java.guild.GuildMember;
import net.co.java.packets.*;
import net.co.java.server.GameServerClient;

public class MessagePacketHandler extends AbstractPacketHandler {
	public MessagePacketHandler(Packet packet) {
		super(packet);
	}

	@Override
	public void handle(GameServerClient client) {
        if(!(packet instanceof MessagePacket))
            return;
        MessagePacket msgPckt = (MessagePacket) packet;


        switch(msgPckt.getMessageType())
        {
            case TALK:
            {
                for(Player p : client.getPlayer().view.getPlayers())
                {
                    if(client.getPlayer() == p)
                        continue;
                    this.build().send(p.getClient());
                }
                break;
            }
            case WHISPER:
            {
                for(Player p : client.getModel().getPlayers().values())
                    if(p.getName().equals(msgPckt.getTo()))
                        this.build().send(p.getClient());
                break;
            }
            case GUILD:
            {
                for(GuildMember gm : client.getPlayer().getGuild().getMembers())
                    if(gm.getPlayer() != null && gm.getPlayer().getClient() != client)
                        this.build().send(gm.getPlayer().getClient());
                break;
            }
            default:
            {
                break; // TODO
            }
        }
	}
}
