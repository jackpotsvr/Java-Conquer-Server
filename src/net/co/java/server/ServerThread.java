package net.co.java.server;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.concurrent.ArrayBlockingQueue;

import net.co.java.cipher.Cryptographer;
import net.co.java.model.AccessException;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.PacketType.UnimplementedPacketTypeException;

/**
 * The ServerThread is an abstract client worker. It listens for
 * incoming packets, decrypts the data and calls the abstract
 * Packet handler.
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public abstract class ServerThread implements Runnable {
	
	private boolean cont = true;
	private final Socket client;
	private final InputStream in;
	private final OutputStream out;
	private final Cryptographer cipher;
	private final ArrayBlockingQueue<byte[]> packets;
	
	/**
	 * Construct a new client worker in a new thread
	 * @param client {@code Socket} from the {@code SocketServer}
	 * @throws IOException 
	 */
	ServerThread(Socket client) throws IOException {
		this.client = client;
		this.in = client.getInputStream();
		this.out = client.getOutputStream();
		this.cipher =  new Cryptographer();
		this.packets = new ArrayBlockingQueue<byte[]>(40);
		new Thread(this).start();
	}

	@Override
	public void run() {
		connected();
		while(cont) {
			try {
				retrievePacket();
				workQueue();
				// Since we are checking if there are bytes
				// available before we call the blocking inputstream.read()
				// method, this loop is not blocking when no data
				// is available. Thus, we need to sleep a while to prevent
				// excessive CPU use.
				Thread.sleep(1);
			} catch (IOException e) {
				// Client disconnected
				e.printStackTrace();
				break;
			} catch (InterruptedException e) {
				// Thread was interrupted, ignore
			} catch (UnimplementedPacketTypeException e) {
				// We have to implement a new PacketType
				e.printStackTrace();
			}
		}
		try {
			client.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} finally {
			disconnected();			
		}
	}
	
	/**
	 * Check if bytes are available and process the incoming data
	 * @throws IOException
	 * @throws UnimplementedPacketTypeException 
	 */
	private void retrievePacket() throws IOException, UnimplementedPacketTypeException {
		int available = in.available();
		// If bytes available, read and decrypt the bytes
		// and handle the packet in the subclass
		if (available > 0) {
			byte[] data = new byte[available];
			in.read(data);
			cipher.decrypt(data);
			try {
				this.handle(new IncomingPacket(data));
			} catch (AccessException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	/**
	 * Polls the last data from the packet queue, encrypts it with the cipher,
	 * and sends it to the {@code OutputSteam}
	 * @throws IOException
	 */
	private void workQueue() throws IOException {
		byte[] data = packets.poll();
		if ( data != null ) {
			cipher.encrypt(data);
			out.write(data);
		}
	}
	
	/**
	 * Queues packet data for transport to the client
	 * @param data
	 * @return true if successfully added, false if the
	 * packet queue is full
	 */
	public boolean offer(byte[] data) {
		return packets.offer(data);
	}
	
	/**
	 * Set the keys for the {@code Cryptographer}
	 * @param inKey1
	 * @param inKey2
	 */
	public void setKeys(long inKey1, long inKey2) {
		cipher.setKeys(inKey1, inKey2);
	}
	
	/**
	 * Subclasses should override this method to
	 * handle the incoming packets
	 * @param packet
	 */
	public abstract void handle(IncomingPacket packet) throws AccessException;
	
	/**
	 * Subclasses can override this method to listen for
	 * some event handling for a newly connected client
	 */
	protected void connected() {};
	
	/**
	 * Subclasses can override this method to listen for
	 * some event handling for a disconnected client
	 */
	protected void disconnected() {};
	
	/**
	 * Close the connection for this {@code Socket}.
	 * Also closes the connections' {@code InputStream}
	 * and {@code OutputStream}.
	 */
	public void close() {
		cont = false;
	}
	
}