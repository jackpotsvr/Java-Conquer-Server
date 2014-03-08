package net.co.java.server;

import net.co.java.packets.MessagePacket;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.UpdatePacket;
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
		else if(command.equalsIgnoreCase("stop")) {
			// client.getGameServer().
		} else if(command.equalsIgnoreCase("stamina")) {
			int st = Integer.parseInt(args[1]);
			client.getPlayer().setStamina(st);
			new UpdatePacket(client.getPlayer())
					.setAttribute(UpdatePacket.Mode.Money, 100000l)
					.setAttribute(UpdatePacket.Mode.Stamina, 200l).build()
					.send(client);
		}
	}

}
