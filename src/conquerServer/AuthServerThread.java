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
public class AuthServerThread implements Runnable, ServerThread
{
	
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
	public AuthServerThread(Socket client, AuthServer authServer) throws IOException
	{
		this.client = client;
		this.authServer = authServer;
		in = client.getInputStream();
		out = client.getOutputStream();
	}

	@Override
	public void send(byte[] data) throws IOException
	{
		cipher.encrypt(data);
		out.write(data);
	}

	@Override
	public void run()
	{
		System.out.println("Incomming connection on AuthServer");
		while(true)
		{
			try
			{
				int available = in.available();
				
				if ( available > 0 )
				{
					byte[] data = new byte[available];
					in.read(data);
					cipher.decrypt(data);
					Packet.route(data, this);
				}
				
			} catch (IOException e) {
				authServer.disconnect(this);
				break;
			}
		}		
	}
	
}
