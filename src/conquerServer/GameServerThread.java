package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

import packets.Cryptographer;
import packets.IncommingPacket;
import packets.*;

public class GameServerThread implements Runnable {
	private Socket client = null;
	private GameServer gameServer = null;
	private InputStream in = null;
	private OutputStream out = null;
	private Cryptographer cipher = new Cryptographer(); 
	
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
		System.out.println("Incomming connection on GameServer");
		
		while(true) {
			try {
				int available = in.available();
				
				if(available > 0){ 
					byte[] dataIn = new byte[available];
					in.read(dataIn);
					cipher.Decrypt(dataIn);
					IncommingPacket ip = new IncommingPacket(dataIn);
					
					switch(ip.getPacketType()){
						case auth_login_response: 
							Auth_Response 
							
							
					}
					
					
					
					System.out.println();
				}
				
			} catch (IOException e) {
				gameServer.disconnect(this);
				break;
			}
		}
	}


}
