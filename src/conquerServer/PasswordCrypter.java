package conquerServer;

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
public class PasswordCrypter
{
	
    private static int rollLeft(long Value, byte Roll, byte Size)
    {
        Roll = (byte) ((Roll & 0xFF) & 0x1f);
        return (int) ((((Value << (((Roll & 0xFF)) & 0x1F)) & 0xFFFFFFFFL)) | ((Value & 0xFFFFFFFFL) >> ((Size & 0xFF) - (Roll & 0xFF))));
    }

    private static int rollRight(long Value, byte Roll, byte Size)
    {
        Roll = (byte) ((Roll & 0xFF) & 0x1f);
        return (int) ((((Value << ((((Size & 0xFF) - (Roll & 0xFF))) & 0x1F)) & 0xFFFFFFFFL)) | ((Value & 0xFFFFFFFFL) >> (Roll & 0xFF)));
    }

    private static long[] PasswordKey = new long[]{
            0xebe854bcL, 0xb04998f7L, 0xfffaa88cL, 0x96e854bbL, 0xa9915556L, 0x48e44110, 0x9f32308fL, 0x27f41d3e, 0xcf4f3523L, 0xeac3c6b4L, 0xe9ea5e03L, 0xe5974bbaL, 0x334d7692, 0x2c6bcf2e, 0xdc53b74, 0x995c92a6L,
            0x7e4f6d77, 0x1eb2b79f, 0x1d348d89, 0xed641354L, 0x15e04a9d, 0x488da159, 0x647817d3, 0x8ca0bc20L, 0x9264f7feL, 0x91e78c6cL, 0x5c9a07fb, 0xabd4dcceL, 0x6416f98d, 0x6642ab5b
    };

    public static void decrypt(long[] Password)
    {
        for (byte i = 1; i >= 0; i = (byte) (i - 1))
        {
            long num = Password[(i * 2) + 1];

            long num2 = Password[i * 2];
            for (byte j = 11; j >= 0; j = (byte) (j - 1))
            {
                num = ((long) rollRight((num & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[(j * 2) + 7] & 0xFFFFFFFFL), (byte) (num2 & 0xFFFFFFFFL), (byte) 0x20)) ^ (num2 & 0xFFFFFFFFL);
                num2 = ((long) rollRight((num2 & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[(j * 2) + 6] & 0xFFFFFFFFL), (byte) (num & 0xFFFFFFFFL), (byte) 0x20)) ^ (num & 0xFFFFFFFFL);
            }
            Password[(i * 2) + 1] = (num & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[5] & 0xFFFFFFFFL);
            Password[i * 2] = (num2 & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[4] & 0xFFFFFFFFL);
        }
    }
    
    public static void decrypt(byte[] Password)
    {
        for (byte i = 1; i >= 0; i = (byte) (i - 1))
        {
            long num = Password[(i * 2) + 1];

            long num2 = Password[i * 2];
            for (byte j = 11; j >= 0; j = (byte) (j - 1))
            {
                num = ((long) rollRight((num & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[(j * 2) + 7] & 0xFFFFFFFFL), (byte) (num2 & 0xFFFFFFFFL), (byte) 0x20)) ^ (num2 & 0xFFFFFFFFL);
                num2 = ((long) rollRight((num2 & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[(j * 2) + 6] & 0xFFFFFFFFL), (byte) (num & 0xFFFFFFFFL), (byte) 0x20)) ^ (num & 0xFFFFFFFFL);
            }
            Password[(i * 2) + 1] = (byte) ((num & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[5] & 0xFFFFFFFFL));
            Password[i * 2] = (byte) ((num2 & 0xFFFFFFFFL) - (PasswordCrypter.PasswordKey[4] & 0xFFFFFFFFL));
        }
    }


}