package conquerServer;

import java.io.IOException;

public class Main {
	
	public static void main(String[] args) throws IOException{
		AuthServer authServer = new AuthServer();
		GameServer gameServer = new GameServer();
		Thread authServerThread = new Thread(authServer);
		Thread gameServerThread = new Thread(gameServer);
		authServerThread.start();
		gameServerThread.start();
		String SQLUrl = args[0];
		System.out.println(SQLUrl);
	}

}
