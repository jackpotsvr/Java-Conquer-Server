package conquerServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;

public class GameServer implements Runnable {
	
	private static final int PORT = 5816;
	
	private final ServerSocket server;
	
	private List<GameServerThread> connections = new CopyOnWriteArrayList<GameServerThread>();

	/**
	 * Constructor creates a new Socket
	 * @throws IOException 
	 */
	public GameServer() throws IOException {
		server =  new ServerSocket(PORT);
		System.out.println("GameServer running on port " + PORT);	
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
				Socket client = server.accept();
				GameServerThread GST = new GameServerThread(client, this);
				connections.add(GST);
				Thread thread = new Thread(GST);
				thread.start();
				System.out.println("Connected clients: " + connections.size());
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}

	public static void main(String[] args) throws IOException {
		GameServer GS = new GameServer();
		Thread thread = new Thread(GS);
		thread.start();
	}

}
