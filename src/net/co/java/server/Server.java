package net.co.java.server;

import java.io.Closeable;
import java.io.File;
import java.io.IOException;
import java.nio.channels.AsynchronousSocketChannel;
import java.util.concurrent.atomic.AtomicInteger;

import org.simpleframework.xml.Element;
import org.simpleframework.xml.Root;
import org.simpleframework.xml.Serializer;
import org.simpleframework.xml.core.Persister;

import net.co.java.entity.Player;
import net.co.java.model.AccessException;
import net.co.java.model.AuthorizationPromise;
import net.co.java.model.Model;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;

/**
 * The server is the main class for the Conquer Online server. 
 * It initialises one GameServer and one AuthServer.
 * 
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 *
 */
@Root
public class Server implements Closeable {
	
	@Element(name="model") private final Model model;
	private final AuthServer authServer;
	private final GameServer gameServer;
	
	/**
	 * Construct a new Server
	 * @throws IOException
	 */
	public Server(@Element(name="model") Model model) throws IOException {
		this.model = model;
		this.authServer = new AuthServer();
		this.gameServer = new GameServer();
	}

	/**
	 * The Authentication Server handles user login requests
	 * and allows the client to connect to the Game Server.
	 * The Authentication Server listens at port 9958 by
	 * default. 
	 * 
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public class AuthServer extends AbstractServer {
		
		public final static int DEFAULT_PORT = 9958;
		
		/**
		 * Construct a new AuthServer listening at the default
		 * port (9958). Starts listening automatically from a
		 * new thread.
		 * @throws IOException when failing to start the server
		 */
		public AuthServer() throws IOException {
			super(DEFAULT_PORT);
		}

		@Override
		protected void listening() {
			try {
				System.out.println("AuthServer listening at " + this.getSocketAddress().toString());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}

		@Override
		protected AbstractClient createClient(AsynchronousSocketChannel channel) {
			return new AbstractClient(channel) {

				@Override
				protected void handle(IncomingPacket ip) throws AccessException, IOException {
					switch(ip.getPacketType()) {
					case AUTH_LOGIN_PACKET:
						this.AuthLogin(ip);
						break;
					case AUTH_LOGIN_RESPONSE:
						this.AuthLoginResponse(ip);
						break;
					default:
						break;
					}
				}

				@Override
				protected void connected() {
					System.out.println("AUTH : Client connected: " + this.toString());
				}

				@Override
				protected void disconnected() {
					System.out.println("AUTH : Client disconnected: " + this.toString());
				}
				
				/**
				 * The relatively simple AuthLoginResponse packet is handled here
				 * @param packet
				 */
				private void AuthLoginResponse(IncomingPacket incomingPacket) {
					long identity = incomingPacket.readUnsignedInt(4);
					long resNumber = incomingPacket.readUnsignedInt(8);
					String resLocation = incomingPacket.readString(12,16);
					System.out.println("ALR: " + resLocation + " " + identity + ", "  + resNumber);
				}

				/**
				 * When the user logs in, an Auth Login packet is sent from the client.
				 * In this function we prepare the response and check if the credentials
				 * are correct.
				 * @param packet
				 * @throws AccessException 
				 * @throws IOException 
				 */
				private void AuthLogin(IncomingPacket incomingPacket) throws AccessException, IOException {
					String accountName	= incomingPacket.readString(4,16);
					String password	= incomingPacket.readPassword();
					String serverName	= incomingPacket.readString(36, 16);
					
					if (model.isAuthorised(serverName, accountName, password)) {
						PacketWriter pw = new PacketWriter(PacketType.AUTH_LOGIN_FORWARD, 0x20);
						AuthorizationPromise promise = model.createAuthorizationPromise(accountName);
						Long identity = promise.getIdentity();
						long token = promise.getToken(); // SUCCESS
						pw.putUnsignedInteger(identity);
						pw.putUnsignedInteger(token);
						pw.putString("127.000.000.001", 16);
						pw.putUnsignedInteger(5816);
						pw.send(this);
					} else {
						// TODO Send a response to the client
						this.close();
					}
				}
				
			};
		}
		
	}
	
	/**
	 * The game server contains all required data for the entities and ensures
	 * that updates of these data - for example equipment or location - are
	 * efficiently broadcasted to all the connected clients.
	 * 
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public class GameServer extends AbstractServer {
		
		public final static int DEFAULT_PORT = 5816;
		
		private AtomicInteger AMOUNT_OF_PLAYERS = new AtomicInteger(0);
		
		/**
		 * Construct a new AuthServer listening at the default
		 * port (9958). Starts listening automatically from a
		 * new thread.
		 * @throws IOException when failing to start the server
		 */
		public GameServer() throws IOException {
			super(DEFAULT_PORT);
		}

		@Override
		protected void listening() {
			try {
				System.out.println("GameServer listening at " + this.getSocketAddress().toString());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}

		@Override
		protected AbstractClient createClient(AsynchronousSocketChannel channel) {
			return new GameServerClient(GameServer.this, channel);
		}

		protected void connect(GameServerClient client) {
			System.out.println("GAME : Client connected: " + client.toString() + " (" + AMOUNT_OF_PLAYERS.incrementAndGet() + ")");
		}

		protected void disconnect(GameServerClient client) {
			System.out.println("GAME : Client disconnected: " + client.toString() + " (" + AMOUNT_OF_PLAYERS.decrementAndGet() + ")");
			Player p = client.getPlayer();
			if ( p != null ) p.remove();
		}
		
		/**
		 * @return the amount of online players
		 */
		public int getAmountOfPlayers() {
			return AMOUNT_OF_PLAYERS.intValue();
		}

		/**
		 * @return the Model for this GameServer instance
		 */
		public Model getModel() {
			return model;
		}
		
	}
	
	public static void main(String... args) throws Exception {
		Serializer serializer = new Persister();
		File source = new File("config.xml");
		serializer.read(Server.class, source);
		// For some reason the main thread needs to be maintained
		while(true) Thread.sleep(10000000); 
	}

	@Override
	public void close() throws IOException {
		authServer.close();
		gameServer.close();
	}

}
