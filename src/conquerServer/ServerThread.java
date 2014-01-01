package conquerServer;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.concurrent.ArrayBlockingQueue;

import packets.Cryptographer;
import packets.OutgoingPacket;
import packets.PacketType;

/**
 * An abstract class providing methods to encrypt, decrypt, send and receive
 * packets from a Socket
 */
public abstract class ServerThread implements Runnable {
	
	private final Socket client;
	private final InputStream in;
	private final OutputStream out;
	private final Cryptographer cipher;
	private final ArrayBlockingQueue<byte[]> packets;
	
	/**
	 * An abstract class providing methods to encrypt, decrypt, send and receive
	 * packets from a Socket
	 * @param client the {@code Socket} instance for the connecting client
	 * @throws IOException if an I/O error occurs
	 */
	public ServerThread(Socket client) throws IOException {
		this.client = client;
		this.in = client.getInputStream();
		this.out = client.getOutputStream();
		this.cipher =  new Cryptographer();
		this.packets = new ArrayBlockingQueue<byte[]>(10);
	}	
	
	@Override
	public void run() {
		connected();
		while (true) {
			try {
				retrievePacket();
				workQueue();
			} catch (IOException e) {
				disconnected();
				try { this.close(); } catch (IOException e1) { }
				break;
			}
		}
	}
	
	/**
	 * Try to retrieve a packet
	 * @throws IOException
	 */
	private void retrievePacket() throws IOException {
		/* TODO Zijn de regels met //[x] gemarkeerd echt nodig?
			Ik heb ze toegevoegd omdat er iets raars ging met de packet
			afhandeling, maar ik kan nu ineens niet meer begrijpen
			wat het voor nut zou moeten hebben...
			Eens een keertje debuggen...
		*/
		int available = in.available(); // [x]

		if (available > 0) {	// [x]
			byte[] data = new byte[available];
			in.read(data);
			cipher.decrypt(data);
			PacketType packetType = this.getPacketType(data);
			route(packetType, data);
		}	//	[x]
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
	 * Queues packet for transport to the client
	 * @param packet
	 * @return true if successfully added, false if the
	 * packet queue is full
	 */
	public boolean offer(OutgoingPacket packet) {
		return this.offer(packet.getData());
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
	 * This method is used to send the incoming data to the right handler
	 * @param data
	 */
	protected abstract void route(PacketType packetType, byte[] data);
	
	/**
	 * The subclass can override this method to listen on the client connected
	 * event
	 */
	protected void connected() {}
	
	/**
	 * The subclass can override this method to listen on the client
	 * disconnected event
	 */
	protected void disconnected() {}
	
	/**
	 * @param data
	 * @return the {@code PacketType} of the incomming packet
	 */
	protected PacketType getPacketType(byte[] data) {
		return PacketType.get((data[3] & 0xFF) << 8 | (data[2] & 0xFF));
	}

	/**
	 * Close the connection
	 * @throws IOException
	 */
	public void close() throws IOException {
		in.close();
		out.close();
		client.close();
	}
	
}
