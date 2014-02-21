package net.co.java.server;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.item.EquipmentSlot;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemInstance.Mode;
import net.co.java.model.AccessException;
import net.co.java.model.Mock;
import net.co.java.model.Model;
import net.co.java.packets.GeneralData;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.ItemUsage;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.MessagePacket.MessageType;

/**
 * The server is the main class for the Conquer Online server. 
 * It initialzes one GameServer and one AuthServer.
 * 
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 *
 */
public class Server {
	
	private final Model model;
	
	/**
	 * The default Game Server port
	 */
	private static final int GAME_PORT = 5816;

	/**
	 * The default Auth Server port
	 */
	private static final int AUTH_PORT = 9958;
	
	/**
	 * Construct a new Server
	 * @throws IOException
	 */
	public Server() throws IOException {
		//this.model = new PostgreSQL(Config.HOST, Config.USERNAME, Config.PASSWORD);
		this.model = new Mock();
		this.new AuthServer();
		this.new GameServer();
	}
	
	/**
	 * The Authentication Server handles user login requests
	 * and allows the client to connect to the Game Server.
	 * The Authentication Server listens at port 9958 by
	 * default.  
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public class AuthServer implements Runnable {
		
		private final ServerSocket socket;
		
		/**
		 * Construct a new AuthServer listening at the default
		 * port (9958). Starts listening automatically from a
		 * new thread.
		 * @throws IOException
		 */
		AuthServer() throws IOException {
			socket = new ServerSocket(AUTH_PORT);
			new Thread(this).start();
			System.out.println("Authentication server listening on port " + AUTH_PORT);
		}

		@Override
		public void run() {
			for(;;) {
				// Wait for an incoming connection
				// While no incoming connection is available,
				// this method blocks.
				try {
					Socket client = socket.accept();
					new Client(client);
				} catch (IOException e) {
					// Catch it here, so the server won't stop
					// when a client disconnects
					e.printStackTrace();
				}
			}
		}
		
		/**
		 * The {@code Client} is the worker thread for every client listening to
		 * the {@code AuthServer}. It listens for incoming packets and works
		 * the queue of outgoing packets. All packets are delegated to their
		 * corresponding handler here.
		 * @author Jan-Willem Gmelig Meyling
		 * @author Thomas Gmelig Meyling
		 */
		public class Client extends ServerThread {
			
			Client(Socket client) throws IOException {
				super(client);
			}

			@Override
			public void handle(IncomingPacket packet) {
				switch(packet.getPacketType()) {
				case AUTH_LOGIN_PACKET:
					AuthLogin(packet);
					break;
				case AUTH_LOGIN_RESPONSE:
					AuthLoginResponse(packet);
					break;
				default:
					break;
				}
			}			

			/**
			 * When the user logs in, an Auth Login packet is sent from the client.
			 * In this function we prepare the response and check if the credentials
			 * are correct.
			 * @param packet
			 */
			private void AuthLogin(IncomingPacket packet) {
				String accountName	= packet.readString(4,16);
				String password	= packet.readPassword();
				String serverName	= packet.readString(36, 16);
				
				try {
					if (model.isAuthorised(serverName, accountName, password)) {
						PacketWriter pw = new PacketWriter(PacketType.AUTH_LOGIN_FORWARD, 0x20);
						Long identity = model.getIdentity("Jackpotsvr");
						long token = 5; // SUCCESS
						pw.putUnsignedInteger(identity);
						pw.putUnsignedInteger(token);
						pw.putString("127.000.000.001", 16);
						pw.putUnsignedInteger(GAME_PORT);
						pw.send(this);
					}
				} catch (AccessException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			
			/**
			 * The relatively simple AuthLoginResponse packet is handled here
			 * @param packet
			 */
			private void AuthLoginResponse(IncomingPacket packet) {
				long identity = packet.readUnsignedInt(4);
				long resNumber = packet.readUnsignedInt(8);
				String resLocation = packet.readString(12,16);
				System.out.println("ALR: " + resLocation + " " + identity + ", "  + resNumber);
			}
			
		}
	}
	
	/**
	 * The game server contains all required data for the entities and ensures
	 * that updates of these data - for example equipment or location - are
	 * efficiently broadcasted to all the connected clients. 
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public class GameServer implements Runnable {
		
		private volatile int AMOUNT_OF_PLAYERS = 0;

		private final ServerSocket socket;
		
		/**
		 * Construct a new AuthServer listening at the default
		 * port (9958). Starts listening automatically from a
		 * new thread.
		 * @throws IOException
		 */
		GameServer() throws IOException {
			socket = new ServerSocket(GAME_PORT);
			new Thread(this).start();
			System.out.println("Game server listening on port " + GAME_PORT);
		}

		@Override
		public void run() {
			for(;;) {
				// Wait for an incoming connection
				// While no incoming connection is available,
				// this method blocks.
				try {
					Socket client = socket.accept();
					new Client(client);
				} catch (IOException e) {
					// Catch it here, so the server won't stop
					// when a client disconnects
					e.printStackTrace();
				}
			}
		}
		
		/**
		 * @return the {@code Server} instance for this {@code GameServer}
		 */
		public Server getServer() {
			return Server.this;
		}
		
		/**
		 * @return the amount of online players
		 */
		public int getAmountOfPlayers() {
			return AMOUNT_OF_PLAYERS;
		}

		/**
		 * The {@code Client} is the worker thread for every client listening to
		 * the {@code GameServer}. It listens for incoming packets and works
		 * the queue of outgoing packets. All packets are delegated to their
		 * corresponding handler here.
		 * @author Jan-Willem Gmelig Meyling
		 * @author Thomas Gmelig Meyling
		 */
		public class Client extends ServerThread {
			
			private long identity;
			private Player player;

			Client(Socket client) throws IOException {
				super(client);
			}
			
			@Override
			protected void connected() {
				AMOUNT_OF_PLAYERS++;
				System.out.println("Amount of players: " + AMOUNT_OF_PLAYERS);
			}
			
			@Override
			protected void disconnected() {
				AMOUNT_OF_PLAYERS--;
				player.getLocation().getMap().removeEntity(player);
				player.removeEntity().sendToSurroundings(player);
				System.out.println("Amount of players: " + AMOUNT_OF_PLAYERS);
			}
			
			/**
			 * @return the current GameServer instance
			 */
			public GameServer getGameServer() {
				return GameServer.this;
			}
			
			/**
			 * @return the identity for this Client
			 */
			public long getIdentity() {
				return identity;
			}
			
			/**
			 * @return the player for this Client
			 */
			public Player getPlayer() {
				return player;
			}

			@Override
			public void handle(IncomingPacket packet) {
				switch(packet.getPacketType()) {
				case AUTH_LOGIN_RESPONSE:
					// Read the identity and token from the packet
					// and set these as keys for the cipher
					identity = packet.readUnsignedInt(4);
					player = model.getPlayer(identity);
					player.setClient(this);
					long token = packet.readUnsignedInt(8);
					this.setKeys(token, identity);
					// Inform the client that the login was successful
					new MessagePacket(MessagePacket.SYSTEM, MessagePacket.ALL_USERS, "ANSWER_OK")
							.setMessageType(MessageType.LoginInfo)
							.build().send(this);
					// Create the Entity object for the player, bound to
					// the current identity and client thread
					player.setLocation(Map.CentralPlain.new Location(382, 341), null);
					// Send the character information packet
					player.characterInformation().send(this);
					// Send an item
					EquipmentInstance.get(2342239l).new ItemInformationPacket(Mode.DEFAULT, EquipmentSlot.Inventory).send(this);
					break;
				case ENTITY_MOVE_PACKET:
					player.walk(packet.readUnsignedByte(8), packet);
					break;
				case GENERAL_DATA_PACKET:
					handleGeneralData(packet);
					break;
				case ITEM_USAGE_PACKET:
					new ItemUsage(packet).handle(this);
					break;
				case MESSAGE_PACKET:
					MessagePacket mp = new MessagePacket(packet);
					System.out.println(mp.getFrom() + " said " + mp.getMessage() + ".");
					break;
				default:
					System.out.println("Unimplemented " + packet.getPacketType().toString());
					break;
				}
			}
			
			private void handleGeneralData(IncomingPacket packet) {
				GeneralData gd = new GeneralData(packet);
				switch(gd.getSubType()){
				case GET_SURROUNDINGS:
					for (Entity e : player.getSurroundings())
						e.spawn().send(this);
					break;
				case JUMP:
					int[] coordinates = gd.getShorts();
					player.jump(coordinates[0], coordinates[1], packet);
					break;
				case LOCATION:
					player.retrieveLocation().build().send(this);
					break;
				default:
					System.out.println("Unimplemented " + gd.getSubType().toString());
					break;
				}
			}
		}
	}

	/**
	 * The Map enum contains all Maps for the current Server
	 * Map can be accessed by name (Map.CentralPlain) or id (1002).
	 * Maps should be instantiated only once and therefore the Enum
	 * singleton pattern is used. Each Map contains a Collection of
	 * Entities, and has a constructor for Locations that are in this
	 * Map.
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public enum Map {
		CentralPlain(1002);
		
		private final Integer id;
		
		private final List<Entity> entities = new CopyOnWriteArrayList<Entity>();
	
		/**
		 * Construct a new Map with a given id
		 * @param id
		 */
		private Map(int id) {
			this.id = Integer.valueOf(id);
			System.out.println("Initializing map: " + this.toString() + " (" + this.id + ")");
		}
		
		/**
		 * @return the id for a Map
		 */
		public int getMapID() {
			return id;
		}
		
		/**
		 * Add an {@code Entity} to this Map - teleport or spawn.
		 * @param entity
		 */
		public void addEntity(Entity entity) {
			entities.add(entity);
		}
		
		/**
		 * Remove an {@code Entity} from this Map - death or teleport.
		 * @param entity
		 */
		public void removeEntity(Entity entity) {
			entities.remove(entity);
		}
		
		/**
		 * @return all entities in this map
		 */
		public List<Entity> getEntities() {
			return entities;
		}
		
		/**
		 * @param me
		 * @return all entities within the default range excluding me
		 */
		public List<Entity> getEntitiesInRange(Entity me) {
			List<Entity> result = new ArrayList<Entity>();
			
			for ( Entity e : entities ) {
				if ( e.equals(me) )
					continue;
				else if (e.getLocation().inView(me.getLocation()))
					result.add(e);
			}
			
			return result;
		}
		
		/**
		 * @param me
		 * @return all players within the default range excluding me
		 */
		public List<Player> getPlayersInRange(Entity me) {
			List<Player> result = new ArrayList<Player>();
			
			for ( Entity e : entities ) {
				if ( e.equals(me) || !(e instanceof Player) )
					continue;
				else if (e.getLocation().inView(me.getLocation()))
					result.add((Player) e);
			}
			
			return result;
		}
		
		/**
		 * The Location class contains the information for a location
		 * in a Map
		 * @author Jan-Willem Gmelig Meyling
		 * @author Thomas Gmelig Meyling
		 */
		public class Location  {

			private final static int VIEW_RANGE = 18;
			
			private final int xCord, yCord;
			private int direction;

			/**
			 * Construct a new Location in this Map
			 * @param xCord
			 * @param yCord
			 */
			public Location(int xCord, int yCord) {
				this.xCord = xCord;
				this.yCord = yCord;
				this.direction = 0;
			}

			/**
			 * @return the Map for this Location
			 */
			public Map getMap() {
				return Map.this;
			}
			
			/**
			 * @return the x-coordinate for this Location
			 */
			public int getxCord() {
				return xCord;
			}
			
			public int getDirection() {
				return direction;
			}
			
			public void setDirection(int direction) {
				this.direction = direction % 8;
			}
			
			/**
			 * @return the y-coordinate for this Location
			 */
			public int getyCord() {
				return yCord;
			}
			
			/**
			 * @param x offset
			 * @param y offset
			 * @return a new Location in this name with a given offset
			 * from this Location
			 */
			public Location atOffset(int x, int y) {
				return new Location(xCord + x, yCord + y);
			}
			
			public Location inDirection(int direction) {
				switch (direction%8) {
		        case 0:
		            return new Location(xCord, yCord+1);
		        case 1:
		            return new Location((xCord-1), yCord+1);
		        case 2:
		            return new Location((xCord-1), yCord);
		        case 3:
		            return new Location((xCord-1), yCord-1);
		        case 4:
		            return new Location(xCord, yCord-1);
		        case 5:
		            return new Location((xCord+1), yCord-1);
		        case 6:
		            return new Location((xCord+1), yCord);
		        default:
		            return new Location((xCord+1), yCord+1);
				}
			}
			
			/**
			 * @param location
			 * @return true if an entity is within the default sight of
			 * the entity at this location
			 */
			public boolean inView(Location location) {
				return location.getMap().equals(getMap())
						&& Math.sqrt(Math.pow(location.xCord - xCord, 2) + Math.pow(location.yCord - yCord, 2)) < VIEW_RANGE;
				//		&& Math.abs(location.xCord - xCord) < VIEW_RANGE
				//		&& Math.abs(location.yCord - yCord) < VIEW_RANGE;
			}
			
			
			@Override
			public String toString() {
				return Map.this.toString() + ": (" + xCord + "; " + yCord + ")";
			}

			@Override
			public boolean equals(Object obj) {
				if (obj instanceof Location) {
					Location other = (Location) obj;
					return other.getMap().equals(Map.this) && other.xCord == xCord
							&& other.yCord == yCord;
				}
				return super.equals(obj);
			}
			
		}
	}
	
	public static void initSQL()
	{
		try {
			Class.forName("org.postgresql.Driver");
		} catch (ClassNotFoundException e) {
 
			System.out.println("Where is your PostgreSQL JDBC Driver? "
					+ "Include in your library path!");
			e.printStackTrace();
			return;
		}	
	}

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		new Server();
	}
	
	

}
