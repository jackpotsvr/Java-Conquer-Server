package net.co.java.server;

import java.io.IOException;
import java.nio.channels.AsynchronousSocketChannel;

import net.co.java.entity.Player;
import net.co.java.model.AccessException;
import net.co.java.model.AuthorizationPromise;
import net.co.java.model.Model;
import net.co.java.npc.dialogs.NPC_Dialog;
import net.co.java.packets.Character_Creation_Packet;
import net.co.java.packets.GeneralData;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.ItemUsage;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.packets.NPC_Initial_Packet;
import net.co.java.server.Server.GameServer;

public class GameServerClient extends AbstractClient {
	
	private final GameServer gameServer;
	private final Model model;

	public GameServerClient(GameServer gameServer, AsynchronousSocketChannel channel) {
		super(channel);
		this.gameServer = gameServer;
		this.model = gameServer.getModel();
	}

	@Override
	protected void handle(IncomingPacket incomingPacket) throws AccessException,
			IOException {
		Player player = this.getPlayer();
		switch(incomingPacket.getPacketType()) {
		case AUTH_LOGIN_RESPONSE:
			// Read the identity and token from the packet
			// and set these as keys for the cipher
			long identity = incomingPacket.readUnsignedInt(4);
			long token = incomingPacket.readUnsignedInt(8);
			this.setIdentity(identity);
			AuthorizationPromise promise = model.getAuthorizationPromise(identity);
			this.setKeys(token, identity);
			// Inform the client that the login was successful
			if (promise.hasCharacter())
			{	
				// Create the Entity object for the player, bound to
				// the current identity and client thread
				player = model.loadPlayer(promise);
				this.setPlayer(player);
				player.setClient(this);
				new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "ANSWER_OK")
						.setMessageType(MessageType.LoginInfo)
						.build().send(this);
				// Send the character information packet
				player.characterInformation().send(this);
			}
			else
			{
				new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "NEW_ROLE")
					.setMessageType(MessageType.LoginInfo)
					.build().send(this);
			}
			break;
		case ENTITY_MOVE_PACKET:
			player.walk(incomingPacket.readUnsignedByte(8), incomingPacket);
			break;
		case GENERAL_DATA_PACKET:
			new GeneralData(incomingPacket).handle(this);
			break;
		case ITEM_USAGE_PACKET:
			new ItemUsage(incomingPacket).handle(this);
			break;
		case MESSAGE_PACKET:
			MessagePacket mp = new MessagePacket(incomingPacket);
			if(mp.getMessage().startsWith("/")) {
				new Command(mp).handle(this);
			} else {
				System.out.println(mp.getFrom() + " said " + mp.getMessage() + ".");
			}
			break;
		case CHARACTER_CREATION_PACKET:
			if (model.createCharacter(new Character_Creation_Packet(incomingPacket)))
			{
				new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "ANSWER_OK")
				.setMessageType(MessageType.LoginInfo)
				.build().send(this);
				this.close();
			}
			else
			{
				new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "Failed to create character. Character name already in use.")
				.setMessageType(MessageType.Dialog)
				.build().send(this);
			}
			break;
		case INTERACT_PACKET:
			new InteractPacket(incomingPacket).handle(this);
			break;
		case NPC_INITIAL_PACKET:
			new NPC_Initial_Packet(incomingPacket).handle(this);
			break; 
		case NPC_DIALOG_PACKET:
			NPC_Dialog packet = player.getActiveDialog();
			if(packet != null)
			{
				int input = incomingPacket.readUnsignedByte(10);
				packet.setInput(input); 
				packet.handle(this);
			}
			break;
		default: 	
			System.out.println("Unimplemented " + incomingPacket.getPacketType().toString());
			break;
		}
	}

	@Override
	protected void connected() {
		gameServer.connect(this);
	}

	@Override
	protected void disconnected() {
		gameServer.disconnect(this);
	}

	/**
	 * @return the Model for this {@code Client}
	 */
	public Model getModel() {
		return model;
	}

	/**
	 * @return the GameServer for this {@code Client}
	 */
	public GameServer getGameServer() {
		return gameServer;
	}

}
