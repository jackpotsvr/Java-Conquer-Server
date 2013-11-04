/**
 * 
 */
package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

/**
 * @author jan-willem
 *
 */
public class AuthServerThread implements Runnable {
	private Socket client = null;
	private AuthServer authServer = null;
	private InputStream in = null;
	private OutputStream out = null;
	
	/**
	 * 
	 * @param client
	 * @param authServer
	 */
	public AuthServerThread(Socket client, AuthServer authServer) {
		this.client = client;
		this.authServer = authServer;
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
			authServer.disconnect(this);
		}
	}

}
