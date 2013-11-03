package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Map;

public class TestAuthServer {
	
	private static final String hostname = "127.0.0.1";
	private static final int AuthPort = 9958;

	public static void main(String[] args) throws UnknownHostException, IOException {
		
		Socket socket = new Socket(hostname, AuthPort);
		OutputStream out = socket.getOutputStream();
		InputStream in = socket.getInputStream();
		
		Map<String, byte[]> privateData = Main.sharedData;
		
		byte[] dataOut = new byte[47];
		dataOut[0] = 127;
		dataOut[46] = 127;
		out.write(dataOut);
		out.flush();
		
		byte[] dataIn = new byte[47];
		in.read(dataIn);
		for ( byte b : dataIn )
			System.out.print(b + " ");
		System.out.println();
		
	}

}
