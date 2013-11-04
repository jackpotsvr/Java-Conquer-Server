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
	 */
	public GameServerThread(Socket client, GameServer gameServer) {
		this.client = client;
		this.gameServer = gameServer;
	}

	/* (non-Javadoc)
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		try {
			in = client.getInputStream();
			out = client.getOutputStream();
						
			byte[] dataIn = new byte[47];
			in.read(dataIn);
			
			for ( byte b : dataIn )
				System.out.print(b + " ");	
			
			byte[] dataOut = new byte[47];
			dataOut[1] = -127;
			dataOut[45] = 127;
			out.write(dataOut);
			
			System.out.println();
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			gameServer.disconnect(this);
		}
	}

}
