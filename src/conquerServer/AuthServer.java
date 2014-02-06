package conquerServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;

public class AuthServer implements Runnable {
	
	private static final int PORT = 9958;
	
	private final ServerSocket server;

	/**
	 * Constructor creates a new Socket
	 * @throws IOException 
	 */
	public AuthServer() throws IOException {
		server =  new ServerSocket(PORT);
		System.out.println("AuthServer running on port " + PORT);
	}
	
	@Override
	public void run() {
		while(true){
			System.out.println("Waiting for incomming connection...");
			try {
				Thread.sleep(1);
				Socket client = server.accept();
				AuthServerThread AST = new AuthServerThread(client, this);
				Thread thread = new Thread(AST);
				thread.start();
			} catch (IOException e) {
				e.printStackTrace();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	public static void main(String[] args) throws IOException {
		AuthServer AS = new AuthServer();
		Thread thread = new Thread(AS);
		thread.start();
	}

}
