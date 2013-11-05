/**
 * 
 */
package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Arrays;

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
	private Cryptography cipher = new Cryptography(); 
	
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
		while(true) {
			try {
				Packet.delegate(in);
			} catch (IOException e) {
				authServer.disconnect(this);
				break;
			}
		}		
	}
	
}
