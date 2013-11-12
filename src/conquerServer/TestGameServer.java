package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;

public class TestGameServer {
	private static final String hostname = "127.0.0.1";
	private static final int AuthPort = 5816;
	
	public static void main(String[] args) throws IOException {
		
		Socket socket = new Socket(hostname, AuthPort);
		OutputStream out = socket.getOutputStream();
		InputStream in = socket.getInputStream();
		
		
		while(true) {
			byte[] dataIn = new byte[47];
			in.read(dataIn);
			for ( byte b : dataIn )
				System.out.print(b + " ");
			System.out.println();
		}
	}

}
