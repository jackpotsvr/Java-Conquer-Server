package conquerServer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class Main {

	private static final int AuthPort = 9978;
	private static final int GameServerPort = 5816;
	
	
	private static ServerSocket server = null;
	
	public static void main(String[] args){
		try {
			server = new ServerSocket(AuthPort);
			while(true){
				Socket client = server.accept();
				ClientWorker w = new ClientWorker(client);
				Thread t = new Thread(w);
				t.start();
			}
		} catch (IOException e) {
			System.out.println("Port " + AuthPort + " is bezet door een ander proces.");
		}
	}

}
