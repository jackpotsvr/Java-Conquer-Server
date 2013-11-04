package conquerServer;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

public class Main {

	private static final int AuthPort = 9958;
	private static final int GameServerPort = 5816;

	public static Map<String, byte[]> sharedData = Collections.synchronizedMap(new HashMap<String, byte[]>());
	
	private static ServerSocket AuthServer = null;
	private static ServerSocket GameServer = null;
	
	public static void main(String[] args){
		try {
			AuthServer = new ServerSocket(AuthPort);
			System.out.println("Server running on port " + AuthPort);
		} catch (IOException e) {
			System.out.println("Port " + AuthPort + " is being used by another process");
		}
		
		while(true){
			try {
				Socket client = AuthServer.accept();
				ClientWorker w = new ClientWorker(client);
				Thread t = new Thread(w);
				t.start();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

}
