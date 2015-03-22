package net.co.java.packets;

import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.packets.String_Packet.StringPacketType;
import net.co.java.server.GameServerClient;

public class GemEffect implements PacketHandler
{
	/** in percents */
	private double GEMCHANCE = 3; // just a value for now. It seemed a nice idea for me that
	// if you have more gems, the chance is higher, but linear might suck for people with just 1 gem. 
		
	@Override
	public PacketWriter build() {
		// TODO Auto-generated method stub
		return null;
	}
	
	/** should be called every time a player hits, chance division will be done from here */ 
	@Override
	public void handle(GameServerClient client) {

		for(EquipmentInstance eq : client.getPlayer().inventory.getEquipments())
		{
			if(eq != null)
			{
				if(eq.firstSocket != null && eq.firstSocket != EquipmentInstance.Socket.None)
					attempt(client, eq.firstSocket);
				if(eq.secondSocket != null && eq.secondSocket != EquipmentInstance.Socket.None)
					attempt(client, eq.secondSocket);
			}
		}
	}
	
	public void attempt(GameServerClient client, EquipmentInstance.Socket sock){	
		switch(sock)
		{
			case SuperPhoenix:
				if(computeChance()) superPhoenixGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperDragon:
				if(computeChance()) superDragonGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperFury:
				if(computeChance()) superFuryGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperRainbowGem:
				if(computeChance()) superRainbowGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperKylinGem:
				if(computeChance()) superKylinGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperVioletGem:
				if(computeChance()) superVioletGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperMoonGem:
				if(computeChance()) superMoonGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			case SuperTortoiseGem: 
				if(computeChance()) superTortoiseGem(client).sendTo(client.getPlayer().view.getPlayers());
				break;
			default:
				break;
		}
	}
	
	public boolean computeChance(){
		GEMCHANCE-= 0.25; 
		return (Math.random() < (GEMCHANCE / 100));
	}
	
	public static PacketWriter superPhoenixGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "phoenix", client.getIdentity()).build();
	}

	public static PacketWriter superDragonGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "goldendragon", client.getIdentity()).build();
	}
	
	public static PacketWriter superFuryGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "fastflash", client.getIdentity()).build();
	}

	public static PacketWriter superRainbowGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "rainbow", client.getIdentity()).build();
	}
	
	public static PacketWriter superKylinGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "goldenkylin", client.getIdentity()).build();
	}

	public static PacketWriter superVioletGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "purpleray", client.getIdentity()).build();
	}

	public static PacketWriter superMoonGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "moon", client.getIdentity()).build();
	}
	
	public static PacketWriter superTortoiseGem(GameServerClient client){
		return 	new String_Packet(StringPacketType.Effect, "recovery", client.getIdentity()).build();
	}	
}
