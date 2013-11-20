package conquerServer;

import java.io.IOException;

public interface ServerThread {
	void send(byte[] data) throws IOException;
	// void receive(byte[] data) throws IOException;
}
