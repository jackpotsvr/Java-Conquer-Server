/**
 * 
 */
package conquerServer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
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
		BufferedReader in = null;
		PrintWriter out = null;
		String line;
		
		try {
			in = new BufferedReader(new InputStreamReader(client.getInputStream()));
			out = new PrintWriter(client.getOutputStream(), true);
		} catch (IOException e) { }
		try {
			line = in.readLine();
			//Send data back to client
			out.println(line);
		} catch (IOException e) { }
	}

}
