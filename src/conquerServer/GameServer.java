package conquerServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;

import data.Location;
import data.Map;
import data.Monster;
import data.Player;

public class GameServer implements Runnable {
	
	private static final int PORT = 5816;
	
	private final ServerSocket server;
	
	public int PLAYER_COUNT = 0;
	
	Map map = new Map(1002);
	
	private List<GameServerThread> connections = new CopyOnWriteArrayList<GameServerThread>();

	/**
	 * Constructor creates a new Socket
	 * @throws IOException 
	 */
	public GameServer() throws IOException {
		server =  new ServerSocket(PORT);
		System.out.println("GameServer running on port " + PORT);	
				
		Monster mob = new Monster(new Location(map, 378, 343), 564564, "BullMessenger",  112, 117, 55000);
		map.addEntity(mob);
	}
	
	public void broadcast(byte[] data) {
		for ( GameServerThread client : connections ) {
			client.offer(data);
		}
	}
	
	public synchronized void disconnect(GameServerThread GST) {
		connections.remove(GST);
		System.out.println("Client disconnected");
		System.out.println("Connected clients: " + connections.size());
	}

	@Override
	/**
	 * Run function starts a new thread for each incomming connection
	 */
	public void run() {
		System.out.println("Waiting for incomming connection...");
		while(true){
			try {
				Thread.sleep(1);
				Socket client = server.accept();
				GameServerThread GST = new GameServerThread(client, this);
				connections.add(GST);
				Thread thread = new Thread(GST);
				thread.start();
				System.out.println("Connected clients: " + connections.size());
			} catch (IOException e) {
				e.printStackTrace();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

	public static void main(String[] args) throws IOException {
		GameServer GS = new GameServer();
		Thread thread = new Thread(GS);
		thread.start();
	}
	
	public Map getMap()
	{
		return map; 
	}

}
