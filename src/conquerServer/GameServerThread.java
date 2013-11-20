package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

import packets.*;

public class GameServerThread implements Runnable, ServerThread
{
	
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
	public GameServerThread(Socket client, GameServer gameServer) throws IOException
	{
		this.client = client;
		this.gameServer = gameServer;
		this.in = client.getInputStream();
		this.out = client.getOutputStream();
	}

	public synchronized void send(byte[] data) throws IOException
	{
		cipher.encrypt(data);
		out.write(data);
	}
	
	@Override
	public void run() {
		System.out.println("Incomming connection on GameServer");
		
		while(true)
		{
			try
			{
				int available = in.available();
				
				if(available > 0)
				{ 
					byte[] data = new byte[available];
					in.read(data);
					cipher.decrypt(data);
					Packet.route(data, this);
				}
			}
			catch (IOException e)
			{
				gameServer.disconnect(this);
				break;
			}
		}
	}


}
