package net.co.java.server;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.channels.AsynchronousSocketChannel;
import java.nio.channels.CompletionHandler;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.Queue;

import net.co.java.cipher.Cryptographer;
import net.co.java.entity.Player;
import net.co.java.model.AccessException;
import net.co.java.packets.IncomingPacket;

/**
 * {@code AbstractClient} represents a remote connection to the
 * {@code AbstractServer}. It contains methods for reading from and writing
 * packets to the channel. All reads and writers are asynchronous.
 * 
 * This implementation is based on the ChatServer example copyrighted by
 * Oracle.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public abstract class AbstractClient {

	private final AsynchronousSocketChannel channel;
	private final Cryptographer cipher;
	
	private volatile long identity;
	private volatile Player player;
	
	/**
	 * Construct a new {@code Client}
	 * @param channel that this client listens on
	 * @throws IOException If an I/O error occurs
	 */
	AbstractClient(AsynchronousSocketChannel channel) {
		super();
		this.channel = channel;
		this.cipher = new Cryptographer();
		/*
		 * TODO This option was part of the ChatServer example by Oracle, but
		 * should be used with caution: "The socket option should only be
		 * enabled in cases where it is known that the coalescing impacts
		 * performance". We need to figure out whether it is required to set
		 * this option to true.
		 * 
		 * try {
		 * 	this.channel.setOption(StandardSocketOptions.TCP_NODELAY, true);
		 * } catch (IOException e) {
		 *	e.printStackTrace();
		 * }
		 * 
		 */
	}
	
	private boolean started = false;
	
	/**
	 * Start this AbstractClient, call the connected method and start listening
	 * for incoming packets
	 */
	public void start() {
		if(!started) {
			connected();
			run();
		}
	}
	
	/**
	 * Handle an incoming packet
	 * @param ip
	 * @throws IOException 
	 * @throws AccessException 
	 */
	protected abstract void handle(IncomingPacket ip) throws AccessException, IOException;
	
	/**
	 * This method is called when a new client connects
	 */
	protected abstract void connected();
	
	/**
	 * this method is called when a client disconnects
	 */
	protected abstract void disconnected();
	
	/**
	 * Read incoming packets
	 */
	private void run() {
		if(channel.isOpen()) {
			ByteBuffer buffer = ByteBuffer.allocate(512);
			channel.read(buffer, buffer, new CompletionHandler<Integer, ByteBuffer>() {

				@Override
				public void completed(Integer result, ByteBuffer attachment) {
					if(result == -1) {
						AbstractClient.this.disconnected();
						return;
					};
					// Decrypt packet
					attachment.limit(result);
					cipher.decrypt(attachment);
					try {
						// Handle the packet
						AbstractClient.this.handle(new IncomingPacket(attachment));
						// Ask for the next packet
						AbstractClient.this.run();
					} catch (IOException exc) {
						this.failed(exc, attachment);
					}
				}

				@Override
				public void failed(Throwable exc, ByteBuffer attachment) {
					try {
						AbstractClient.this.close();
					} catch (IOException e) {
						e.printStackTrace();
					} finally {
						exc.printStackTrace();
					}
				}
				
			});
		}
	}
	
	private final Queue<ByteBuffer> queue = new LinkedList<ByteBuffer>();
	private boolean writing = false;
	
	/**
	 * Enqueues a write of the buffer to the {@code Client}. The call is
	 * asynchronous so the buffer is not safe to modify after passing the
	 * buffer here.
	 * @param buffer the buffer to send to the {@code Client}
	 */
	public void write(ByteBuffer buffer) {
		// Clone the ByteBuffer, because encryption should not affect other clients
		ByteBuffer cryptBuffer = ByteBuffer.wrap(Arrays.copyOf(buffer.array(), buffer.limit()));
		// A variable to check if this thread should write to the sockets output
		boolean threadShouldWrite = false;
		// Synchronise on the queue for thread safety
		synchronized(queue) {
			// Enqueue the buffer to be sent to the client
			queue.add(cryptBuffer);
			// If another thread is not already writing, start writing to this client
			if(!writing) {
				writing = true;
				threadShouldWrite = true;
			}
		}
		// No new data in buffer to write
		if(threadShouldWrite) {
			writeFromQueue();
		}
	}
	
	private void writeFromQueue() {
		ByteBuffer buffer;
		// Synchronise on the queue for thread safety
		synchronized(queue) {
			// Dequeue an ByteBuffer to be sent, if not null, continue
			buffer = queue.poll();
			if(buffer == null ) {
				writing = false;
			}
		}
		// If this thread should write
		if(writing) {
			// Encrypt the buffer
			cipher.encrypt(buffer);
			// Write the buffer to the socket
			channel.write(buffer, buffer, new CompletionHandler<Integer, ByteBuffer>() {

				@Override
				public void completed(Integer result, ByteBuffer attachment) {
					if(attachment.hasRemaining()) {
						channel.write(attachment, attachment, this);
					} else {
						writeFromQueue();
					}
				}

				@Override
				public void failed(Throwable exc, ByteBuffer attachment) {
					exc.printStackTrace();
				}
				
			});
		}
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
	 * @return the identity
	 */
	public long getIdentity() {
		return identity;
	}
	
	/**
	 * @param identity the identity to set
	 */
	public void setIdentity(long identity) {
		this.identity = identity;
	}
	
	/**
	 * @return the player
	 */
	public Player getPlayer() {
		return player;
	}
	
	/**
	 * @param player the player to set
	 */
	public void setPlayer(Player player) {
		this.player = player;
	}

	/**
	 * Close this {@code Client}
	 * @throws IOException
	 */
	public void close() throws IOException {
		AbstractClient.this.disconnected();
		channel.close();
	}

	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "Client [channel=" + channel + ", identity=" + identity
				+ ", player=" + player + "]";
	}

}
