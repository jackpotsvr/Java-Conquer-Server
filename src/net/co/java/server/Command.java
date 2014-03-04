package net.co.java.server;

import net.co.java.packets.MessagePacket;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.server.Server.GameServer.Client;

public class Command implements PacketHandler {

	private final String[] args;
	
	public Command(MessagePacket mp) {
		args = mp.getMessage().substring(1).split(" ");
	}

	@Override
	public PacketWriter build() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void handle(Client client) {
		String command = args[0];
		if(command.equalsIgnoreCase("exit"))
			client.close();
		else if(command.equalsIgnoreCase("stamina")) {
			int st = Integer.parseInt(args[1]);
			client.getPlayer().setStamina(st);
			client.getPlayer().sendStamina();
		} else if (command.equalsIgnoreCase("skill")) {
			new PacketWriter(PacketType.SKILL_UPDATE_PACKET, 12)
			.putUnsignedInteger(Long.parseLong(args[1]))
			.putUnsignedShort(Integer.parseInt(args[2]))
			.putUnsignedShort(Integer.parseInt(args[3]))
			.send(client);
		}
	}

}
