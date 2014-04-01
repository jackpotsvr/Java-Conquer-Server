package net.co.java.server;

import java.io.Closeable;
import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.net.StandardSocketOptions;
import java.nio.channels.AsynchronousChannelGroup;
import java.nio.channels.AsynchronousServerSocketChannel;
import java.nio.channels.AsynchronousSocketChannel;
import java.nio.channels.CompletionHandler;
import java.util.concurrent.Executor;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;

import org.simpleframework.xml.Attribute;
import org.simpleframework.xml.Root;

/**
 * Implements an abstract server which can be extended by the Authentication and
 * Game servers. This class sets up a server socket using an
 * {@code AsynchronousServerSocketChannel} listening to a specified host and port.
 * 
 * This implementation is based on the ChatServer example copyrighted by Oracle.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
@Root
public abstract class AbstractServer implements Closeable, Executor {
	
	protected final ExecutorService executor;
	private final AsynchronousChannelGroup channelGroup;
	private final AsynchronousServerSocketChannel listener;
	
	/**
	 * Create an AbstractServer with the {@code AsynchronousServerSocketChannel}
	 * bound to a wildcard address but a specific port.
	 * @param port to listen to
	 * @throws java.io.IOException when failing to start the server
	 */
	public AbstractServer(@Attribute(name="port") int port) throws IOException {
		this(new InetSocketAddress(port));
	}
	
	/**
	 * Create an AbstractServer with the {@code AsynchronousServerSocketChannel}
	 * bound to a specific host address and port.
	 * @param host to listen to
	 * @param port to listen to
	 * @throws java.io.IOException when failing to start the server
	 */
	public AbstractServer(@Attribute(name="host") String host,
						  @Attribute(name="port") int port) throws IOException {
		this(new InetSocketAddress(host, port));
	}
	
	/**
	 * @param local The socket address that the server is bound to
	 * @throws IOException when failing to start the server
	 */
	protected AbstractServer(SocketAddress local) throws IOException {
		this.executor = Executors.newCachedThreadPool();
		this.channelGroup = AsynchronousChannelGroup.withThreadPool(executor);
		this.listener = AsynchronousServerSocketChannel
				.open(channelGroup)
				.setOption(StandardSocketOptions.SO_REUSEADDR, true)
				.bind(local);
		this.run();
		this.listening();
	}
	
	/**
	 * Called when the server is listening
	 */
	protected abstract void listening();

	/**
	 * A hook to override the default client implementation
	 * @param channel
	 * @return a new Client instance
	 */
	protected abstract AbstractClient createClient(AsynchronousSocketChannel channel);

	/**
	 * Start accepting incoming connections
	 */
	private void run() {
		/*
		 * Call accept to wait for connections, tell it to call our
		 * CompletionHandler when there is a new incoming connection
		 */
		listener.accept(null, new CompletionHandler<AsynchronousSocketChannel, Void>() {

			@Override
			public void completed(AsynchronousSocketChannel result,
					Void attachment) {
				// Request a new accept
				listener.accept(null, this);
				// Handle the incoming connection
				AbstractServer.this.createClient(result);
			}

			@Override
			public void failed(Throwable exc, Void attachment) {
				exc.printStackTrace();
			}
			
		});
	}
	
	/**
	 * @return The {@code InetSocketAddress} that the server is bound to
	 * @throws java.io.IOException if an I/O exception occurs
	 */
	public final InetSocketAddress getSocketAddress() throws IOException {
		return (InetSocketAddress) listener.getLocalAddress();
	}
	
	/**
	 * @return The host name that the server is bound to
	 * @throws java.io.IOException if an I/O exception occurs
	 */
	@Attribute(name="host")
	public String getHost() throws IOException {
		return this.getSocketAddress().getHostString();
	}
	
	/**
	 * @return The port number that the server is bound to
	 * @throws java.io.IOException if an I/O exception occurs
	 */
	@Attribute(name="port")
	public int getPort() throws IOException {
		return this.getSocketAddress().getPort();
	}
	
	@Override
	public final void close() throws IOException {
		channelGroup.shutdownNow();
		try {
			channelGroup.awaitTermination(1, TimeUnit.MINUTES);
		} catch (InterruptedException e) {
			throw new IOException(e);
		}
	}

	@Override
	public void execute(Runnable command) {
		executor.execute(command);
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "AbstractServer [listener=" + listener + "]";
	}

}
