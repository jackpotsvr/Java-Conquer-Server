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
		else if(command.equalsIgnoreCase("stop")) {
			// client.getGameServer().
		} else if(command.equalsIgnoreCase("stamina")) {
			int st = Integer.parseInt(args[1]);
			client.getPlayer().setStamina(st);
			client.getPlayer().sendStamina();
		} else if (command.equalsIgnoreCase("uskill")) {
			new PacketWriter(PacketType.SKILL_UPDATE_PACKET, 12)
			.putUnsignedInteger(Long.parseLong(args[1])) // 468743 Exp
			.putUnsignedShort(Integer.parseInt(args[2])) // 1045 FAST BLADE
			.putUnsignedShort(Integer.parseInt(args[3])) // 0 prof, 1 magic, [2 skill]
			.send(client);
		} else if (command.equalsIgnoreCase("skill")) {
			new PacketWriter(PacketType.SKILL_PACKET, 12)
			.putUnsignedInteger(Long.parseLong(args[1])) // 468743 Exp
			.putUnsignedShort(Integer.parseInt(args[2])) // 1045 FAST BLADE
			.putUnsignedShort(Integer.parseInt(args[3])) // 1 lvl
			.send(client);
		}
	}

}
