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
	private byte[] dataIn = new byte[47];
	
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
				dataIn = new byte[50];
				in.read(dataIn);
				
				packetHandler();
				
				out.write(dataIn);
				
				for ( byte b : dataIn )
					System.out.print((int)b + " ");
				System.out.println();
				
			} catch (IOException e) {
				authServer.disconnect(this);
				break;
			}
		}		
	}
	
	private void packetHandler(){
		cipher.decrypt(dataIn);
		
		byte[] temp = new byte[2];
		Header packetHeader = new Header();
		
		System.arraycopy(dataIn, 0, temp, 0, 2);
		packetHeader.setPacketSize(ByteConversion.bytesToShort(temp));
		
		temp = new byte[2];

		System.arraycopy(dataIn, 2, temp, 0, 2);
		
		packetHeader.setType(ByteConversion.bytesToShort(temp));

		PacketType packetType = packetHeader.getType();
		
		switch(packetType) {
			case auth_login_packet:
				System.out.print("Succesfully received first packet.");
				break;
			default:
				System.out.println("Received a yet unimplemented packet.");
				break;
		}
	}

}
