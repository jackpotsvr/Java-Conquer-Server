/**
 * 
 */
package testProjects;

/**
 * **********************************************************************
 * Copyright 2012 Charles Benger
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ***************************************************************************
 */
public class Cryptographer {
	public static byte[] Key1 = {
		    (byte) 157,
		    (byte) 144, (byte) 131, (byte) 138, (byte) 209, (byte) 140, (byte) 231, (byte) 246, (byte) 37, (byte) 40, (byte) 235,
		    (byte) 130, (byte) 153, (byte) 100, (byte) 143, (byte) 46, (byte) 45, (byte) 64, (byte) 211, (byte) 250, (byte) 225,
		    (byte) 188, (byte) 183, (byte) 230, (byte) 181, (byte) 216, (byte) 59, (byte) 242, (byte) 169, (byte) 148, (byte) 95,
		    (byte) 30, (byte) 189, (byte) 240, (byte) 35, (byte) 106, (byte) 241, (byte) 236, (byte) 135, (byte) 214, (byte) 69,
		    (byte) 136, (byte) 139, (byte) 98, (byte) 185, (byte) 196, (byte) 47, (byte) 14, (byte) 77, (byte) 160, (byte) 115,
		    (byte) 218, (byte) 1, (byte) 28, (byte) 87, (byte) 198, (byte) 213, (byte) 56, (byte) 219, (byte) 210, (byte) 201,
		    (byte) 244, (byte) 255, (byte) 254, (byte) 221, (byte) 80, (byte) 195, (byte) 74, (byte) 17, (byte) 76, (byte) 39,
		    (byte) 182, (byte) 101, (byte) 232, (byte) 43, (byte) 66, (byte) 217, (byte) 36, (byte) 207, (byte) 238, (byte) 109,
		    (byte) 0, (byte) 19, (byte) 186, (byte) 33, (byte) 124, (byte) 247, (byte) 166, (byte) 245, (byte) 152, (byte) 123,
		    (byte) 178, (byte) 233, (byte) 84, (byte) 159, (byte) 222, (byte) 253, (byte) 176, (byte) 99, (byte) 42, (byte) 49,
		    (byte) 172, (byte) 199, (byte) 150, (byte) 133, (byte) 72, (byte) 203, (byte) 34, (byte) 249, (byte) 132, (byte) 111,
		    (byte) 206, (byte) 141, (byte) 96, (byte) 179, (byte) 154, (byte) 65, (byte) 220, (byte) 151, (byte) 134, (byte) 21,
		    (byte) 248, (byte) 27, (byte) 146, (byte) 9, (byte) 180, (byte) 63, (byte) 190, (byte) 29, (byte) 16, (byte) 3,
		    (byte) 10, (byte) 81, (byte) 12, (byte) 103, (byte) 118, (byte) 165, (byte) 168, (byte) 107, (byte) 2, (byte) 25,
		    (byte) 228, (byte) 15, (byte) 174, (byte) 173, (byte) 192, (byte) 83, (byte) 122, (byte) 97, (byte) 60, (byte) 55,
		    (byte) 102, (byte) 53, (byte) 88, (byte) 187, (byte) 114, (byte) 41, (byte) 20, (byte) 223, (byte) 158, (byte) 61,
		    (byte) 112, (byte) 163, (byte) 234, (byte) 113, (byte) 108, (byte) 7, (byte) 86, (byte) 197, (byte) 8, (byte) 11,
		    (byte) 226, (byte) 57, (byte) 68, (byte) 175, (byte) 142, (byte) 205, (byte) 32, (byte) 243, (byte) 90, (byte) 129,
		    (byte) 156, (byte) 215, (byte) 70, (byte) 85, (byte) 184, (byte) 91, (byte) 82, (byte) 73, (byte) 116, (byte) 127,
		    (byte) 126, (byte) 93, (byte) 208, (byte) 67, (byte) 202, (byte) 145, (byte) 204, (byte) 167, (byte) 54, (byte) 229,
		    (byte) 104, (byte) 171, (byte) 194, (byte) 89, (byte) 164, (byte) 79, (byte) 110, (byte) 237, (byte) 128, (byte) 147,
		    (byte) 58, (byte) 161, (byte) 252, (byte) 119, (byte) 38, (byte) 117, (byte) 24, (byte) 251, (byte) 50, (byte) 105,
		    (byte) 212, (byte) 31, (byte) 94, (byte) 125, (byte) 48, (byte) 227, (byte) 170, (byte) 177, (byte) 44, (byte) 71,
		    (byte) 22, (byte) 5, (byte) 200, (byte) 75, (byte) 162, (byte) 121, (byte) 4, (byte) 239, (byte) 78, (byte) 13,
		    (byte) 224, (byte) 51, (byte) 26, (byte) 193, (byte) 92, (byte) 23, (byte) 6, (byte) 149, (byte) 120, (byte) 155,
		    (byte) 18, (byte) 137, (byte) 52, (byte) 191, (byte) 62
	};

	public static byte[] Key2 = {(byte) 98,
	    (byte) 79, (byte) 232, (byte) 21, (byte) 222, (byte) 235, (byte) 4, (byte) 145, (byte) 26, (byte) 199, (byte) 224,
	    (byte) 77, (byte) 22, (byte) 227, (byte) 124, (byte) 73, (byte) 210, (byte) 63, (byte) 216, (byte) 133, (byte) 78,
	    (byte) 219, (byte) 244, (byte) 1, (byte) 138, (byte) 183, (byte) 208, (byte) 189, (byte) 134, (byte) 211, (byte) 108,
	    (byte) 185, (byte) 66, (byte) 47, (byte) 200, (byte) 245, (byte) 190, (byte) 203, (byte) 228, (byte) 113, (byte) 250,
	    (byte) 167, (byte) 192, (byte) 45, (byte) 246, (byte) 195, (byte) 92, (byte) 41, (byte) 178, (byte) 31, (byte) 184,
	    (byte) 101, (byte) 46, (byte) 187, (byte) 212, (byte) 225, (byte) 106, (byte) 151, (byte) 176, (byte) 157, (byte) 102,
	    (byte) 179, (byte) 76, (byte) 153, (byte) 34, (byte) 15, (byte) 168, (byte) 213, (byte) 158, (byte) 171, (byte) 196,
	    (byte) 81, (byte) 218, (byte) 135, (byte) 160, (byte) 13, (byte) 214, (byte) 163, (byte) 60, (byte) 9, (byte) 146,
	    (byte) 255, (byte) 152, (byte) 69, (byte) 14, (byte) 155, (byte) 180, (byte) 193, (byte) 74, (byte) 119, (byte) 144,
	    (byte) 125, (byte) 70, (byte) 147, (byte) 44, (byte) 121, (byte) 2, (byte) 239, (byte) 136, (byte) 181, (byte) 126,
	    (byte) 139, (byte) 164, (byte) 49, (byte) 186, (byte) 103, (byte) 128, (byte) 237, (byte) 182, (byte) 131, (byte) 28,
	    (byte) 233, (byte) 114, (byte) 223, (byte) 120, (byte) 37, (byte) 238, (byte) 123, (byte) 148, (byte) 161, (byte) 42,
	    (byte) 87, (byte) 112, (byte) 93, (byte) 38, (byte) 115, (byte) 12, (byte) 89, (byte) 226, (byte) 207, (byte) 104,
	    (byte) 149, (byte) 94, (byte) 107, (byte) 132, (byte) 17, (byte) 154, (byte) 71, (byte) 96, (byte) 205, (byte) 150,
	    (byte) 99, (byte) 252, (byte) 201, (byte) 82, (byte) 191, (byte) 88, (byte) 5, (byte) 206, (byte) 91, (byte) 116,
	    (byte) 129, (byte) 10, (byte) 55, (byte) 80, (byte) 61, (byte) 6, (byte) 83, (byte) 236, (byte) 57, (byte) 194,
	    (byte) 175, (byte) 72, (byte) 117, (byte) 62, (byte) 75, (byte) 100, (byte) 241, (byte) 122, (byte) 39, (byte) 64,
	    (byte) 173, (byte) 118, (byte) 67, (byte) 220, (byte) 169, (byte) 50, (byte) 159, (byte) 56, (byte) 229, (byte) 174,
	    (byte) 59, (byte) 84, (byte) 97, (byte) 234, (byte) 23, (byte) 48, (byte) 29, (byte) 230, (byte) 51, (byte) 204,
	    (byte) 25, (byte) 162, (byte) 143, (byte) 40, (byte) 85, (byte) 30, (byte) 43, (byte) 68, (byte) 209, (byte) 90,
	    (byte) 7, (byte) 32, (byte) 141, (byte) 86, (byte) 35, (byte) 188, (byte) 137, (byte) 18, (byte) 127, (byte) 24,
	    (byte) 197, (byte) 142, (byte) 27, (byte) 52, (byte) 65, (byte) 202, (byte) 247, (byte) 16, (byte) 253, (byte) 198,
	    (byte) 19, (byte) 172, (byte) 249, (byte) 130, (byte) 111, (byte) 8, (byte) 53, (byte) 254, (byte) 11, (byte) 36,
	    (byte) 177, (byte) 58, (byte) 231, (byte) 0, (byte) 109, (byte) 54, (byte) 3, (byte) 156, (byte) 105, (byte) 242,
	    (byte) 95, (byte) 248, (byte) 165, (byte) 110, (byte) 251, (byte) 20, (byte) 33, (byte) 170, (byte) 215, (byte) 240,
	    (byte) 221, (byte) 166, (byte) 243, (byte) 140, (byte) 217
	};

    class CryptCounter {
        byte m_Counter = 0x00;

        public byte Key2() {
            return (byte) ((m_Counter >> 8) & 0xF);
        }

        public byte Key1() {
            return (byte) (m_Counter & 0xFF);
        }

        public void Increment() {
            m_Counter++;
        }
    }

    private CryptCounter _decryptCounter;
    private CryptCounter _encryptCounter;
    private byte[] _cryptKey1;
    private byte[] _cryptKey2;

    public Cryptographer() {
        _decryptCounter = new CryptCounter();
        _encryptCounter = new CryptCounter();
        _cryptKey1 = new byte[0x100];
        _cryptKey2 = new byte[0x100];
        byte i_key1 = (byte) 0x9D;
        byte i_key2 = 0x62;
        for (int i = 0; i < 0x100; i++)
        {
            _cryptKey1[i] = i_key1;
            _cryptKey2[i] = i_key2;
            i_key1 = (byte) ((0x0f + (byte) (i_key1 * 0xfa)) * i_key1 + 0x13);
            i_key2 = (byte) ((0x79 - (byte) (i_key2 * 0x5c)) * i_key2 + 0x6d);
        }
    }

    public void Decrypt(byte[] buffer) {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) 0xAB;
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xF | (buffer[i] << 4));
            buffer[i] ^= (byte) (_cryptKey1[_decryptCounter.Key1()] ^ _cryptKey2[_decryptCounter.Key2()]);
            _decryptCounter.Increment();
        }
    }

    public void Encrypt(byte[] buffer) {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) 0xAB;
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xf | buffer[i] << 4);
            buffer[i] ^= (_cryptKey1[_encryptCounter.Key1()] ^ _cryptKey2[_encryptCounter.Key2()]);
            _encryptCounter.Increment();
        }
    }

    public void EncryptBackwards(byte[] buffer) {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) (_cryptKey2[_encryptCounter.Key2()] ^ _cryptKey1[_encryptCounter.Key1()]);
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xf | (buffer[i] << 4));
            buffer[i] ^= (byte) 0xAB;
            _encryptCounter.Increment();
        }
    }

    public void DecryptBackwards(byte[] buffer) {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) (_cryptKey2[_decryptCounter.Key2()] ^ _cryptKey1[_decryptCounter.Key1()]);
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xF | (buffer[i] << 4));
            buffer[i] ^= (byte) (0xAB);
            _decryptCounter.Increment();
        }
    }

    public void GenerateKeys(byte CryptoKey, byte AccountID) {
        int tmpkey1 = 0, tmpkey2 = 0;
        tmpkey1 = ((CryptoKey + AccountID) ^ (0x4321)) ^ CryptoKey;
        tmpkey2 = tmpkey1 * tmpkey1;

        for (int i = 0; i < 256; i++)
        {
            byte right = (byte) ((3 - (i % 4)) * 8);
            byte left = (byte) (((i % 4)) * 8 + right);
            _cryptKey1[i] ^= (tmpkey1 & 0xFF << right >>> left);
            _cryptKey2[i] ^= (tmpkey2 & 0xFF << right >>> left);
        }
    }

	/**
	 * @param args
	 */
	
	public static void main(String[] args) {
		Cryptographer crypt = new Cryptographer();
		byte[] input = new byte[] { 0x00, 0x01, 0x02, 0x04};
			printByteArray(input);
		crypt.Encrypt(input);
			printByteArray(input);
		crypt.Decrypt(input);
			printByteArray(input);
	}
	
	private static void printByteArray(byte[] data) {
		for ( byte b : data )
			System.out.print(b + " ");
		System.out.println();
	}

}
