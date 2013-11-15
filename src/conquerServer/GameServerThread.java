package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

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
							Auth_Response packet = new Auth_Response(ip);
							long key1 = packet.getKey2();
							long key2 = packet.getKey1();
							cipher.GenerateKeys((byte) (key1 & 0xff), (byte) (key2 & 0xff));
							Message_Packet reply = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "NEW_ROLE");  //(long aRGB, long type, long chatID, String from, String to, String  message) 2101 = Login Info, no enum yet 
							reply.encrypt(cipher);
							reply.send(out);
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
