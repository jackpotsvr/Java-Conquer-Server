package conquerServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;

public class AuthServer implements Runnable {
	private int port = 9958;
	private ServerSocket server = null;
	private ArrayList<AuthServerThread> connections = new ArrayList<AuthServerThread>();

	/**
	 * Constructor creates a new Socket
	 */
	public AuthServer() {
		try {
			server =  new ServerSocket(port);
			System.out.println("AuthServer running on port " + port);
		} catch (IOException e) {
			System.out.println("Port " + port + " is already being used by another process.");
		}	
	}
	
	public synchronized void disconnect(AuthServerThread AST) {
		connections.remove(AST);
		System.out.println("Current amount of connections: " + connections.size());
	}

	@Override
	/**
	 * Run function starts a new thread for each incomming connection
	 */
	public void run() {
		while(true){
			System.out.println("Waiting for incomming connection...");
			try {
				Socket client = server.accept();
				AuthServerThread AST = new AuthServerThread(client, this);
				connections.add(AST);
				Thread thread = new Thread(AST);
				thread.start();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

	public static void main(String[] args) {
		AuthServer AS = new AuthServer();
		Thread thread = new Thread(AS);
		thread.start();
	}

}
