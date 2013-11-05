package conquerServer;

import static org.junit.Assert.*;

import org.junit.Test;

public class TestByteConversion {

	@Test
	public void testFourBytesToInt255() {
		byte[] bytes = new byte[] { (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0xFF };
		int result = ByteConversion.fourBytesToInt(bytes);
		assertEquals(255, result); // Expected 0 but was...
	}

	@Test
	public void testFourBytesToInt0() {
		byte[] bytes = new byte[] { (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00 };
		int result = ByteConversion.fourBytesToInt(bytes);
		assertEquals(0, result); // Expected 0 but was...
	}
	
	@Test
	public void testBytesToInt() {
		fail("Not yet implemented");
	}

	@Test
	public void testIntToFourBytes() {
		fail("Not yet implemented");
	}

	@Test
	public void testShortToTwoBytes() {
		fail("Not yet implemented");
	}

	@Test
	public void testBytesToShort() {
		fail("Not yet implemented");
	}

}
