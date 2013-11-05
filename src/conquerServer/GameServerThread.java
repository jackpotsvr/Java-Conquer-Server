package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

public class GameServerThread implements Runnable {
	private Socket client = null;
	private GameServer gameServer = null;
	private InputStream in = null;
	private OutputStream out = null;
	
	/**
	 * 
	 * @param client
	 * @param authServer
	 * @throws IOException 
	 */
	public GameServerThread(Socket client, GameServer gameServer) throws IOException {
		this.client = client;
		this.gameServer = gameServer;
		this.in = client.getInputStream();
		this.out = client.getOutputStream();
	}

	public synchronized void send(byte[] data) throws IOException {
		out.write(data);
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		while(true) {
			try {
				
				byte[] dataIn = new byte[47];
				in.read(dataIn);
				
				gameServer.broadcast(dataIn);
				
				for ( byte b : dataIn )
					System.out.print((int)b + " ");
				System.out.println();
				
			} catch (IOException e) {
				gameServer.disconnect(this);
				break;
			}
		}
	}


}
