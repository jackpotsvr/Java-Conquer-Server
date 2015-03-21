package net.co.java.server;

import java.io.IOException;

import net.co.java.packets.MessagePacket;
import net.co.java.packets.Packet;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;

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
	public void handle(GameServerClient client, Packet packet) {
		String command = args[0];
		if(command.equalsIgnoreCase("exit"))
			try {
				client.close();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		else if(command.equalsIgnoreCase("stop")) {
			// client.getGameServer().
		} else if(command.equalsIgnoreCase("stamina")) {
			int st = ( args.length > 1 ) ? Integer.parseInt(args[1]) : 100;
			client.getPlayer().setStamina(st);
			client.getPlayer().sendStamina();
		} else if ( command.equalsIgnoreCase("raise")) {
			
		}	else if (command.equals("stigg")) {
			String animation = "attackup35";
			new PacketWriter(PacketType.STRING_PACKET, 11 + animation.length())
				.putUnsignedInteger(client.getPlayer().getIdentity())
				.putUnsignedByte(10) // Type
				.putUnsignedByte(1) // Str count
				.putUnsignedByte(animation.length()) // Str length
				.putString(animation)
				.send(client);
		} else if (command.equals("item")){
			/* to do make function to add items */
		}
	}

}
