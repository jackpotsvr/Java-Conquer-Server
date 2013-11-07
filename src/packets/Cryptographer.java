package packets;

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
public class Cryptographer
{

    class CryptCounter
    {
        short m_Counter = 0x00;

        public short Key2()
        {
            return (short) ((m_Counter >> 8) & 0xF);
        }

        public short Key1()
        {
            return (short) (m_Counter & 0xFF);
        }

        public void Increment()
        {
            m_Counter++;
        }
    }

    private CryptCounter _decryptCounter;
    private CryptCounter _encryptCounter;
    private byte[] _cryptKey1;
    private byte[] _cryptKey2;

    public Cryptographer()
    {
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

    public void Decrypt(byte[] buffer)
    {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) 0xAB;
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xF | (buffer[i] << 4));
            buffer[i] ^= (byte) (_cryptKey1[_decryptCounter.Key1()] ^ _cryptKey2[_decryptCounter.Key2()]);
            _decryptCounter.Increment();
        }
    }

    public void Encrypt(byte[] buffer)
    {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) 0xAB;
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xf | buffer[i] << 4);
            buffer[i] ^= (_cryptKey1[_encryptCounter.Key1()] ^ _cryptKey2[_encryptCounter.Key2()]);
            _encryptCounter.Increment();
        }
    }

    public void EncryptBackwards(byte[] buffer)
    {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) (_cryptKey2[_encryptCounter.Key2()] ^ _cryptKey1[_encryptCounter.Key1()]);
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xf | (buffer[i] << 4));
            buffer[i] ^= (byte) 0xAB;
            _encryptCounter.Increment();
        }
    }

    public void DecryptBackwards(byte[] buffer)
    {
        for (int i = 0; i < buffer.length; i++)
        {
            buffer[i] ^= (byte) (_cryptKey2[_decryptCounter.Key2()] ^ _cryptKey1[_decryptCounter.Key1()]);
            buffer[i] = (byte) ((buffer[i] >> 4) & 0xF | (buffer[i] << 4));
            buffer[i] ^= (byte) (0xAB);
            _decryptCounter.Increment();
        }
    }

    public void GenerateKeys(byte CryptoKey, byte AccountID)
    {
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
}
