/**
 * 
 */
package conquerServer;

import static org.junit.Assert.*;

import org.junit.Test;

/**
 * @author jan-willem
 *
 */
public class TestCryptography {

	/**
	 * Test method for {@link conquerServer.Cryptography#Cryptography()}.
	 */
	@Test
	public void testCryptography() {
		fail("Not yet implemented");
	}

	/**
	 * Test method for {@link conquerServer.Cryptography#encrypt(byte[])}.
	 */
	@Test
	public void testEncrypt() {
		Cryptography crypt = new Cryptography();
		byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
		crypt.encrypt(data);
		byte[] expected = new byte[] { -6, -9, -28, -19 };
		assertEquals(expected, data);
	}

	/**
	 * Test method for {@link conquerServer.Cryptography#decrypt(byte[])}.
	 */
	@Test
	public void testDecrypt() {
		Cryptography crypt = new Cryptography();
		byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
		crypt.decrypt(data);
		byte[] expected = new byte[] { -6, -9, -28, -19  };
		assertEquals(expected, data);
	}

	/**
	 * Test method for {@link conquerServer.Cryptography#setKeys(int, int)}.
	 */
	@Test
	public void testSetKeys() {
		fail("Not yet implemented");
	}

}
