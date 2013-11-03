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
public class ClientWorker implements Runnable {
	private Socket client;
	
	/**
	 * 
	 */
	public ClientWorker(Socket client) {
		// TODO Auto-generated constructor stub
		this.client = client;
	}

	/* (non-Javadoc)
	 * @see java.lang.Runnable#run()
	 */
	@Override
	public void run() {
		try {
			InputStream in = client.getInputStream();
			OutputStream out = client.getOutputStream();
			
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
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

}
