package conquerServer;

public class Main {
	
	public static void main(String[] args){
		AuthServer authServer = new AuthServer();
		GameServer gameServer = new GameServer();
		Thread authServerThread = new Thread(authServer);
		Thread gameServerThread = new Thread(gameServer);
		authServerThread.start();
		gameServerThread.start();
	}

}
