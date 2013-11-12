/**
 * 
 */
package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

import packets.*;

/**
 * @author jan-willem
 *
 */
public class AuthServerThread implements Runnable {
	private Socket client = null;
	private AuthServer authServer = null;
	private InputStream in = null;
	private OutputStream out = null;
	private Cryptographer cipher = new Cryptographer(); 
	
	/**
	 * 
	 * @param client
	 * @param authServer
	 * @throws IOException 
	 */
	public AuthServerThread(Socket client, AuthServer authServer) throws IOException {
		this.client = client;
		this.authServer = authServer;
		in = client.getInputStream();
		out = client.getOutputStream();
	}

	/* (non-Javadoc)
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		System.out.println("Incomming connection on AuthServer");
		while(true) {
			try {
				int available = in.available();
				if ( available > 0 ) {
					byte[] data = new byte[available];
					in.read(data);
					cipher.Decrypt(data);
					IncommingPacket ip = new IncommingPacket(data);
				
					switch(ip.getPacketType()) {
						case auth_login_packet:
							Auth_Login_Packet ALP = new Auth_Login_Packet(ip);
							Auth_Login_Forward ALF = new Auth_Login_Forward(23, 5, "127.000.000.001", 5816);
							ALF.encrypt(cipher);
							ALF.send(out);
							break;
						case auth_login_response:
							long Identity = ip.readUnsignedInt(4);
							long ResNumber = ip.readUnsignedInt(8);
							String ResLocation = ip.readString(12,16);
							break;
						default:
							break;
					}
				}				
			} catch (IOException e) {
				authServer.disconnect(this);
				break;
			}
		}		
	}
	
}
