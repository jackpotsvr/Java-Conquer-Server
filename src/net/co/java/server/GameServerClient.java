package net.co.java.server;

import java.io.IOException;
import java.nio.channels.AsynchronousSocketChannel;

import net.co.java.entity.Player;
import net.co.java.model.AccessException;
import net.co.java.model.AuthorizationPromise;
import net.co.java.model.Model;
import net.co.java.packets.Character_Creation_Packet;
import net.co.java.packets.Guild_Request_Packet;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.ItemUsage;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.packets.serialization.DeserializationException;
import net.co.java.packets.serialization.PacketDeserializer;
import net.co.java.packets.String_Packet;
import net.co.java.packets.serialization.PacketSerializer;
import net.co.java.packets.serialization.Packets;
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
                model.getGameServerTicks().addEntity(player);
				player.setClient(this);

                new PacketSerializer(new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "ANSWER_OK")
                        .setMessageType(MessageType.LOGININFO)).serialize().send(this);
				// Send the character information packet
				player.characterInformation().send(this);
			}
			else
			{
                new PacketSerializer(new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "NEW_ROLE")
                        .setMessageType(MessageType.LOGININFO)).serialize().send(this);
			}
			break;
		case ENTITY_MOVE_PACKET:
			player.walk(incomingPacket.readUnsignedByte(8), incomingPacket);
            model.getGameServerTicks().didInteract(player);
			break;
		case ITEM_USAGE_PACKET:
			new ItemUsage(incomingPacket).handle(this);
			break;
		/*case MESSAGE_PACKET:
			MessagePacket mp = new MessagePacket(incomingPacket);
			
			if(mp.getMessage().startsWith("/")) {
				new Command(mp).handle(this);
			} else {
				mp.handle(this);
			}
			break; */
		case CHARACTER_CREATION_PACKET:
			if (model.createCharacter(new Character_Creation_Packet(incomingPacket)))
			{
                new PacketSerializer(
                    new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "ANSWER_OK")
                    .setMessageType(MessageType.LOGININFO)).serialize().send(this);
				this.close();
			}
			else
			{
                new PacketSerializer(
                    new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "Failed to create character. Character name already in use.")
                    .setMessageType(MessageType.DIALOG))
                    .serialize().send(this);
			}
			break;
		case INTERACT_PACKET:
			new InteractPacket(incomingPacket).handle(this);
			break;
		case NPC_DIALOG_PACKET:
//			NPC_Dialog packet = player.getActiveDialog();
//			if(packet != null)
//			{
//				int input = incomingPacket.readUnsignedByte(10);
//				packet.setInput(input); 
//				packet.handle(this);
//			}
			break;
		case GUILD_MEMBER_INFORMATION:
			//new Guild_Member_Information_Packet(incomingPacket).handle(this, null);
			break;
		case GUILD_REQUEST:
			new Guild_Request_Packet(incomingPacket).handle(this);
			break;	
		case STRING_PACKET:
			new String_Packet(incomingPacket).handle(this);
			break;
		default:
            try {
//				PacketDeserializer pd = PacketDeserializerFactory.valueOf(incomingPacket.getPacketType())
//					.getInstance(incomingPacket);
//				pd.getHandlerStrategy(pd.deserialize()).handle(this);
                System.out.println("No old fashioned way of handling the packet, will use the "
                + "PacketDeserializer and PacketHandler");
                PacketDeserializer pd = new PacketDeserializer(incomingPacket);
                Packets.getHandlerStrategy(pd.deserialize()).handle(this);
            } catch (DeserializationException e) {
                e.printStackTrace();
            }
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
