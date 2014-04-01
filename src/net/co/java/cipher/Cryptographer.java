package net.co.java.cipher;

import java.nio.ByteBuffer;

public final class Cryptographer
{

	private int inCounter = 0;
	private int outCounter = 0;
	private boolean usingAlternate = false;

	private static byte[] key1 = new byte[256];
	private static byte[] key2 = new byte[256];
	private byte[] key3 = new byte[256];
	private byte[] key4 = new byte[256];
	
	private static long[] passwordKey = {
		0xebe854bc, 0xb04998f7, 0xfffaa88c, 0x96e854bb, 0xa9915556, 0x48e44110, 0x9f32308f,
		0x27f41d3e, 0xcf4f3523, 0xeac3c6b4, 0xe9ea5e03, 0xe5974bba, 0x334d7692, 0x2c6bcf2e,
		0xdc53b74, 0x995c92a6, 0x7e4f6d77, 0x1eb2b79f, 0x1d348d89, 0xed641354L, 0x15e04a9d,
		0x488da159, 0x647817d3, 0x8ca0bc20, 0x9264f7fe, 0x91e78c6c, 0x5c9a07fb, 0xabd4dcce,
		0x6416f98d, 0x6642ab5b
	};

	static
	{
		byte i_key1 = (byte) 0x9D, i_key2 = 0x62;
		
		for (int i = 0; i < 256; i++) {
			key1[i] = i_key1;
			key2[i] = i_key2;
			i_key1 = (byte) ((0x0F + (byte) (i_key1 * 0xFA)) * i_key1 + 0x13);
			i_key2 = (byte) ((0x79 - (byte) (i_key2 * 0x5C)) * i_key2 + 0x6D);
		}
	}
	
    private static int rollRight(long Value, long num)
    {
        num &= 0x1F;
        return (int) (Value << (32 - num) | ((Value & 0xFFFFFFFFL) >> num));
    }
    
    public synchronized void encrypt(ByteBuffer b)
    {
    	byte[] buffer = b.array();
    	for (int i = 0, length = b.limit(); i < length; i++)
		{
			buffer[i] ^= 0xAB;
			buffer[i] = (byte) (((buffer[i] & 0xFF) >> 4) | ((buffer[i] & 0xFF) << 4));
			buffer[i] ^= key2[(outCounter%65536) >> 8] ^ key1[(outCounter%65536) & 0xFF];
			outCounter++;
		}
    }
    
	public synchronized void encrypt(byte[] buffer)
	{
		for (int i = 0; i < buffer.length; i++)
		{
			buffer[i] ^= 0xAB;
			buffer[i] = (byte) (((buffer[i] & 0xFF) >> 4) | ((buffer[i] & 0xFF) << 4));
			buffer[i] ^= key2[(outCounter%65536) >> 8] ^ key1[(outCounter%65536) & 0xFF];
			outCounter++;
		}
	}
	
	public synchronized void decrypt(byte[] buffer)
	{
		for (int i = 0; i < buffer.length; i++)
		{
			buffer[i] ^= 0xAB;
			buffer[i] = (byte) (((buffer[i] & 0xFF) >> 4) | ((buffer[i] & 0xFF) << 4));
			
			if(usingAlternate)
			{
				buffer[i] ^= key4[inCounter >> 8] ^ key3[inCounter & 0xFF];
			}
			else
			{
				buffer[i] ^= key2[inCounter >> 8] ^ key1[inCounter & 0xFF];
			}
			
			inCounter++;
		}
	}

	public synchronized void decrypt(ByteBuffer attachment) {
		byte[] buffer = attachment.array();
		for (int i = 0, length = attachment.limit(); i < length; i++)
		{
			buffer[i] ^= 0xAB;
			buffer[i] = (byte) (((buffer[i] & 0xFF) >> 4) | ((buffer[i] & 0xFF) << 4));
			
			if(usingAlternate)
			{
				buffer[i] ^= key4[inCounter >> 8] ^ key3[inCounter & 0xFF];
			}
			else
			{
				buffer[i] ^= key2[inCounter >> 8] ^ key1[inCounter & 0xFF];
			}
			
			inCounter++;
		}
	}

	public synchronized void setKeys(long inKey1, long inKey2)
	{
		long DWordKey = ((inKey1 + inKey2) ^ 0x4321) ^ inKey1;
		long IMul = DWordKey * DWordKey;
		
		byte[]  XorKey =  {
			(byte) (DWordKey & 0xFF),
			(byte) ((DWordKey >> 8) & 0xFF),
			(byte) ((DWordKey >> 16) & 0xFF),
			(byte) ((DWordKey >> 24) & 0xFF),
			
			(byte)  (IMul & 0xFF),
			(byte) ((IMul >> 8) & 0xFF),
			(byte) ((IMul >> 16) & 0xFF),
			(byte) ((IMul >> 24) & 0xFF)
		};
		
		for ( int i = 0; i < 256; i++ )
		{
			key3[i] = (byte) (XorKey[i % 4] ^ key1[i]);
			key4[i] = (byte) (XorKey[i%4+4] ^ key2[i]);
		}

		usingAlternate = true;
		outCounter = 0;
	}
	
	public static String decryptPassword(byte[] password, int offset)
	{
		long[] pSeeds = new long[4];
		byte[] destination = new byte[16];
		
		for(int i = 0; i < 16; i++)
		{
			pSeeds[i/4] |= (password[i + offset] & 0xFF) << (i % 4 << 3);
		}
		
		for(int i = 0; i < 2; i++)
		{
            long num1 = pSeeds[(i * 2) + 1], num2 = pSeeds[i * 2];
            
            for (int j = 11; j >= 0; j--)
            {
                num1 = rollRight(num1 - (passwordKey[(j * 2) + 7]), num2 ) ^ num2;
                num2 = rollRight(num2 - (passwordKey[(j * 2) + 6]), num1 ) ^ num1;
 
            }
            pSeeds[i * 2 + 1] = num1 - passwordKey[5];
            pSeeds[i * 2] = num2 - passwordKey[4];
		}

		for(int i = 0; i < 16; i++)
		{
			destination[i] = (byte) (( pSeeds[i/4] >> ( i % 4 << 3 )));
		}
		
		return new String(destination);
	}

}
