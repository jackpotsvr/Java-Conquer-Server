package conquerServer;

import java.nio.ByteBuffer;

public class Cryptography {
	
	private CryptCounter encryptCounter; 
	private CryptCounter decryptCounter; 
	private boolean usingAlternate;
	
	
	/* constant keys, ALWAYS the same!  */ 
	/* TIP: Klik op het minnetje, om het overzicht te behouden! */
	private static final byte[] constKey1 = {
		  (byte)0x9D, (byte)0x90, (byte)0x83, (byte)0x8A, (byte)0xD1, (byte)0x8C, (byte)0xE7, (byte)0xF6, (byte)0x25, (byte)0x28, (byte)0xEB, (byte)0x82, (byte)0x99, (byte)0x64, (byte)0x8F, (byte)0x2E,
		  (byte)0x2D, (byte)0x40, (byte)0xD3, (byte)0xFA, (byte)0xE1, (byte)0xBC, (byte)0xB7, (byte)0xE6, (byte)0xB5, (byte)0xD8, (byte)0x3B, (byte)0xF2, (byte)0xA9, (byte)0x94, (byte)0x5F, (byte)0x1E,
		  (byte)0xBD, (byte)0xF0, (byte)0x23, (byte)0x6A, (byte)0xF1, (byte)0xEC, (byte)0x87, (byte)0xD6, (byte)0x45, (byte)0x88, (byte)0x8B, (byte)0x62, (byte)0xB9, (byte)0xC4, (byte)0x2F, (byte)0x0E,
		  (byte)0x4D, (byte)0xA0, (byte)0x73, (byte)0xDA, (byte)0x01, (byte)0x1C, (byte)0x57, (byte)0xC6, (byte)0xD5, (byte)0x38, (byte)0xDB, (byte)0xD2, (byte)0xC9, (byte)0xF4, (byte)0xFF, (byte)0xFE,
		  (byte)0xDD, (byte)0x50, (byte)0xC3, (byte)0x4A, (byte)0x11, (byte)0x4C, (byte)0x27, (byte)0xB6, (byte)0x65, (byte)0xE8, (byte)0x2B, (byte)0x42, (byte)0xD9, (byte)0x24, (byte)0xCF, (byte)0xEE,
		  (byte)0x6D, (byte)0x00, (byte)0x13, (byte)0xBA, (byte)0x21, (byte)0x7C, (byte)0xF7, (byte)0xA6, (byte)0xF5, (byte)0x98, (byte)0x7B, (byte)0xB2, (byte)0xE9, (byte)0x54, (byte)0x9F, (byte)0xDE,
		  (byte)0xFD, (byte)0xB0, (byte)0x63, (byte)0x2A, (byte)0x31, (byte)0xAC, (byte)0xC7, (byte)0x96, (byte)0x85, (byte)0x48, (byte)0xCB, (byte)0x22, (byte)0xF9, (byte)0x84, (byte)0x6F, (byte)0xCE,
		  (byte)0x8D, (byte)0x60, (byte)0xB3, (byte)0x9A, (byte)0x41, (byte)0xDC, (byte)0x97, (byte)0x86, (byte)0x15, (byte)0xF8, (byte)0x1B, (byte)0x92, (byte)0x09, (byte)0xB4, (byte)0x3F, (byte)0xBE,
		  (byte)0x1D, (byte)0x10, (byte)0x03, (byte)0x0A, (byte)0x51, (byte)0x0C, (byte)0x67, (byte)0x76, (byte)0xA5, (byte)0xA8, (byte)0x6B, (byte)0x02, (byte)0x19, (byte)0xE4, (byte)0x0F, (byte)0xAE,
		  (byte)0xAD, (byte)0xC0, (byte)0x53, (byte)0x7A, (byte)0x61, (byte)0x3C, (byte)0x37, (byte)0x66, (byte)0x35, (byte)0x58, (byte)0xBB, (byte)0x72, (byte)0x29, (byte)0x14, (byte)0xDF, (byte)0x9E,
		  (byte)0x3D, (byte)0x70, (byte)0xA3, (byte)0xEA, (byte)0x71, (byte)0x6C, (byte)0x07, (byte)0x56, (byte)0xC5, (byte)0x08, (byte)0x0B, (byte)0xE2, (byte)0x39, (byte)0x44, (byte)0xAF, (byte)0x8E,
		  (byte)0xCD, (byte)0x20, (byte)0xF3, (byte)0x5A, (byte)0x81, (byte)0x9C, (byte)0xD7, (byte)0x46, (byte)0x55, (byte)0xB8, (byte)0x5B, (byte)0x52, (byte)0x49, (byte)0x74, (byte)0x7F, (byte)0x7E,
		  (byte)0x5D, (byte)0xD0, (byte)0x43, (byte)0xCA, (byte)0x91, (byte)0xCC, (byte)0xA7, (byte)0x36, (byte)0xE5, (byte)0x68, (byte)0xAB, (byte)0xC2, (byte)0x59, (byte)0xA4, (byte)0x4F, (byte)0x6E,
		  (byte)0xED, (byte)0x80, (byte)0x93, (byte)0x3A, (byte)0xA1, (byte)0xFC, (byte)0x77, (byte)0x26, (byte)0x75, (byte)0x18, (byte)0xFB, (byte)0x32, (byte)0x69, (byte)0xD4, (byte)0x1F, (byte)0x5E,
		  (byte)0x7D, (byte)0x30, (byte)0xE3, (byte)0xAA, (byte)0xB1, (byte)0x2C, (byte)0x47, (byte)0x16, (byte)0x05, (byte)0xC8, (byte)0x4B, (byte)0xA2, (byte)0x79, (byte)0x04, (byte)0xEF, (byte)0x4E,
		  (byte)0x0D, (byte)0xE0, (byte)0x33, (byte)0x1A, (byte)0xC1, (byte)0x5C, (byte)0x17, (byte)0x06, (byte)0x95, (byte)0x78, (byte)0x9B, (byte)0x12, (byte)0x89, (byte)0x34, (byte)0xBF, (byte)0x3E }; 
	  		
	private static final byte[] constKey2 = {
		  (byte) 0x62, (byte) 0x4F, (byte) 0xE8, (byte) 0x15, (byte) 0xDE, (byte) 0xEB, (byte) 0x04, (byte) 0x91, (byte) 0x1A, (byte) 0xC7, (byte) 0xE0, (byte) 0x4D, (byte) 0x16, (byte) 0xE3, (byte) 0x7C, (byte) 0x49, 
		  (byte) 0xD2, (byte) 0x3F, (byte) 0xD8, (byte) 0x85, (byte) 0x4E, (byte) 0xDB, (byte) 0xF4, (byte) 0x01, (byte) 0x8A, (byte) 0xB7, (byte) 0xD0, (byte) 0xBD, (byte) 0x86, (byte) 0xD3, (byte) 0x6C, (byte) 0xB9, 
		  (byte) 0x42, (byte) 0x2F, (byte) 0xC8, (byte) 0xF5, (byte) 0xBE, (byte) 0xCB, (byte) 0xE4, (byte) 0x71, (byte) 0xFA, (byte) 0xA7, (byte) 0xC0, (byte) 0x2D, (byte) 0xF6, (byte) 0xC3, (byte) 0x5C, (byte) 0x29, 
		  (byte) 0xB2, (byte) 0x1F, (byte) 0xB8, (byte) 0x65, (byte) 0x2E, (byte) 0xBB, (byte) 0xD4, (byte) 0xE1, (byte) 0x6A, (byte) 0x97, (byte) 0xB0, (byte) 0x9D, (byte) 0x66, (byte) 0xB3, (byte) 0x4C, (byte) 0x99, 
		  (byte) 0x22, (byte) 0x0F, (byte) 0xA8, (byte) 0xD5, (byte) 0x9E, (byte) 0xAB, (byte) 0xC4, (byte) 0x51, (byte) 0xDA, (byte) 0x87, (byte) 0xA0, (byte) 0x0D, (byte) 0xD6, (byte) 0xA3, (byte) 0x3C, (byte) 0x09, 
		  (byte) 0x92, (byte) 0xFF, (byte) 0x98, (byte) 0x45, (byte) 0x0E, (byte) 0x9B, (byte) 0xB4, (byte) 0xC1, (byte) 0x4A, (byte) 0x77, (byte) 0x90, (byte) 0x7D, (byte) 0x46, (byte) 0x93, (byte) 0x2C, (byte) 0x79, 
		  (byte) 0x02, (byte) 0xEF, (byte) 0x88, (byte) 0xB5, (byte) 0x7E, (byte) 0x8B, (byte) 0xA4, (byte) 0x31, (byte) 0xBA, (byte) 0x67, (byte) 0x80, (byte) 0xED, (byte) 0xB6, (byte) 0x83, (byte) 0x1C, (byte) 0xE9, 
		  (byte) 0x72, (byte) 0xDF, (byte) 0x78, (byte) 0x25, (byte) 0xEE, (byte) 0x7B, (byte) 0x94, (byte) 0xA1, (byte) 0x2A, (byte) 0x57, (byte) 0x70, (byte) 0x5D, (byte) 0x26, (byte) 0x73, (byte) 0x0C, (byte) 0x59, 
		  (byte) 0xE2, (byte) 0xCF, (byte) 0x68, (byte) 0x95, (byte) 0x5E, (byte) 0x6B, (byte) 0x84, (byte) 0x11, (byte) 0x9A, (byte) 0x47, (byte) 0x60, (byte) 0xCD, (byte) 0x96, (byte) 0x63, (byte) 0xFC, (byte) 0xC9, 
		  (byte) 0x52, (byte) 0xBF, (byte) 0x58, (byte) 0x05, (byte) 0xCE, (byte) 0x5B, (byte) 0x74, (byte) 0x81, (byte) 0x0A, (byte) 0x37, (byte) 0x50, (byte) 0x3D, (byte) 0x06, (byte) 0x53, (byte) 0xEC, (byte) 0x39, 
		  (byte) 0xC2, (byte) 0xAF, (byte) 0x48, (byte) 0x75, (byte) 0x3E, (byte) 0x4B, (byte) 0x64, (byte) 0xF1, (byte) 0x7A, (byte) 0x27, (byte) 0x40, (byte) 0xAD, (byte) 0x76, (byte) 0x43, (byte) 0xDC, (byte) 0xA9, 
		  (byte) 0x32, (byte) 0x9F, (byte) 0x38, (byte) 0xE5, (byte) 0xAE, (byte) 0x3B, (byte) 0x54, (byte) 0x61, (byte) 0xEA, (byte) 0x17, (byte) 0x30, (byte) 0x1D, (byte) 0xE6, (byte) 0x33, (byte) 0xCC, (byte) 0x19, 
		  (byte) 0xA2, (byte) 0x8F, (byte) 0x28, (byte) 0x55, (byte) 0x1E, (byte) 0x2B, (byte) 0x44, (byte) 0xD1, (byte) 0x5A, (byte) 0x07, (byte) 0x20, (byte) 0x8D, (byte) 0x56, (byte) 0x23, (byte) 0xBC, (byte) 0x89, 
		  (byte) 0x12, (byte) 0x7F, (byte) 0x18, (byte) 0xC5, (byte) 0x8E, (byte) 0x1B, (byte) 0x34, (byte) 0x41, (byte) 0xCA, (byte) 0xF7, (byte) 0x10, (byte) 0xFD, (byte) 0xC6, (byte) 0x13, (byte) 0xAC, (byte) 0xF9, 
		  (byte) 0x82, (byte) 0x6F, (byte) 0x08, (byte) 0x35, (byte) 0xFE, (byte) 0x0B, (byte) 0x24, (byte) 0xB1, (byte) 0x3A, (byte) 0xE7, (byte) 0x00, (byte) 0x6D, (byte) 0x36, (byte) 0x03, (byte) 0x9C, (byte) 0x69, 
		  (byte) 0xF2, (byte) 0x5F, (byte) 0xF8, (byte) 0xA5, (byte) 0x6E, (byte) 0xFB, (byte) 0x14, (byte) 0x21, (byte) 0xAA, (byte) 0xD7, (byte) 0xF0, (byte) 0xDD, (byte) 0xA6, (byte) 0xF3, (byte) 0x8C, (byte) 0xD9};
	
	private byte[] uniqueKey3; 
	private byte[] uniqueKey4; 
	private byte[] inKey1; 
	private byte[] inKey2; 
	
	public Cryptography(){ /* CONSTRUCTOR */ 
	    uniqueKey3 = new byte[0x100]; 
	    uniqueKey4 = new byte[0x100];
	    usingAlternate = false; 
	    
	    encryptCounter = new CryptCounter();
	    decryptCounter = new CryptCounter();
	    
	    encryptCounter.setCounter((short) 0);
	    decryptCounter.setCounter((short) 0);
	}
	
	/* method to encrypt outgoing packets, should not work. */	
	public void encrypt(byte[] data){
		for(int i = 0; i < data.length; i++){
			data[i] ^= (byte) 0xAB; 
			data[i] = (byte)((data[i] >> 4) & 0xf | (data[i] << 4));
			data[i] ^= (byte)(constKey1[encryptCounter.Key1()] ^ constKey2[encryptCounter.Key2()]);
			
			encryptCounter.counterIncrement();
		}
	}
	
	
	/* method to decrypt incomming packets, should work. */	
	public void decrypt(byte[] data)
	{
		for(int i = 0; i < data.length; i++){
			data[i] ^= (byte) 0xAB;
			data[i]  = (byte) ((data[i] >> 4) & 0xF | (data[i] << 4));
			
			if(usingAlternate){
				data[i] ^= (byte)(uniqueKey4[decryptCounter.Key2()] ^ uniqueKey3[decryptCounter.Key1()]);
			} else {
				data[i] ^= (byte)(constKey2[decryptCounter.Key2()] ^ constKey1[decryptCounter.Key1()]);
			}
			
			decryptCounter.counterIncrement();
		}
	}
	
	public void setKeys(int inKey1, int inKey2){
		/* highly doubtable if this work as intented, but we'll figure it out after sending some packets */
		int temp1 = ((inKey1 + inKey2)^0x4321) ^ inKey1;
		int temp2 = (int)(inKey1 * inKey1);
		
		this.inKey1 = ByteConversion.intToFourBytes(inKey1);
		this.inKey2 = ByteConversion.intToFourBytes(inKey2);
		
		//byte[] addKey1 = intToFourBytes(temp1);
		//byte[] addKey2 = intToFourBytes(temp2);
		
		//byte[] addResult = new byte[4];
		byte[] tempKey = new byte[4]; 
		
		long lmuler; 
		
		int adder3 = temp1 + temp2; 
		tempKey = ByteConversion.intToFourBytes(adder3);
		
		tempKey[2] = (byte)(tempKey[2] ^ (byte)0x43);
		tempKey[3] = (byte)(tempKey[3] ^ (byte)0x21);
		
		for(int i = 0; i<4; i++){
			tempKey[i] = (byte)(tempKey[i] ^ this.inKey1[i]);
		}
		
		//  To build the 3rd key. 
		for(int i = 0; i<256; i++){
			uniqueKey3[i] = (byte)(tempKey[3 - (i % 4)] ^ constKey1[i]);
		}
		
		byte[] addResult = new byte[4];
		
		for(int i = 0; i <4; i++){
			addResult[i] = tempKey[3 - i];
		}
		
		adder3 = ByteConversion.bytesToInt(addResult);
		lmuler = adder3*adder3;
		lmuler = lmuler << 32;
		lmuler = lmuler >> 32;
		
		adder3 = (int) (lmuler & 0xffffffff);
		
		addResult = ByteConversion.intToFourBytes(adder3);
		
		for(int i = 3; i >= 0; i--){
			tempKey[3 - i] = addResult[i];
		}
		
		
		//  To build the 4rd key. 
		for(int i = 0; i < 256; i++){
			uniqueKey4[i] = (byte)(tempKey[3 - (i % 4)] ^ constKey2[i]);
		}		
		
		usingAlternate = true; 
	}
	
    private static int[] generateKeys()
	{
		/* We need to be able to generate keys.  */ 
		int[] Keys = new int[2]; // return value
		
		long TheKeys = ((long)(Math.random() * 0x98968) << 32); 
		TheKeys = (long)(TheKeys | (long) (Math.random() * 10000000));		

		byte[] Key1 = new byte[4];
		byte[] Key2 = new byte[4];
		/* generate keys */ 
		Key1[0] = (byte) ((long)(TheKeys & 0xff00000000000000L) >> 56);
        Key1[1] = (byte) ((long)(TheKeys & 0xff000000000000L) >> 48);
        Key1[2] = (byte) ((long)(TheKeys & 0xff0000000000L) >> 40);
        Key1[3] = (byte) ((long)(TheKeys & 0xff00000000L) >> 32);
        Key2[0] = (byte) ((TheKeys & 0xff000000) >> 24);
        Key2[1] = (byte) ((TheKeys & 0xff0000) >> 16);
        Key2[2] = (byte) ((TheKeys & 0xff00) >> 8);
        Key2[3] = (byte) (TheKeys & 0xff);

		Keys[0] = ByteConversion.bytesToInt(Key1);
		Keys[1] = ByteConversion.bytesToInt(Key2);
		
		
		return Keys; // return the generated Keys as an array. 
	}

	
	public static void main(String[] args) {
		Cryptography crypt = new Cryptography();
		byte[] input = new byte[] { 0x00, 0x01, 0x02, 0x04};
			printByteArray(input);
		crypt.encrypt(input);
			printByteArray(input);
		crypt.decrypt(input);
			printByteArray(input);
	}
	
	private static void printByteArray(byte[] data) {
		for ( byte b : data )
			System.out.print(b + " ");
		System.out.println();
	}
	
}
