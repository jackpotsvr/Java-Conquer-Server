package testProjects;

public class KeyGeneration {
		public static void main(String[]Args)
		{
			/* We need to be able to generate keys. We used to make unsigned longs... */ 
			long TheKeys = ((long)(Math.random() * 0x98968) << 32); 
			TheKeys = (long)(TheKeys | (long) (Math.random() * 10000000));
				
			byte[] Key1 = new byte[4];
			byte[] Key2 = new byte[4];
			int[] Keys = new int[2];
			
			Key1[0] = (byte) ((long)(TheKeys & 0xff00000000000000L) >> 56);
	        Key1[1] = (byte) ((long)(TheKeys & 0xff000000000000L) >> 48);
	        Key1[2] = (byte) ((long)(TheKeys & 0xff0000000000L) >> 40);
	        Key1[3] = (byte) ((long)(TheKeys & 0xff00000000L) >> 32);
	        Key2[0] = (byte) ((TheKeys & 0xff000000) >> 24);
	        Key2[1] = (byte) ((TheKeys & 0xff0000) >> 16);
	        Key2[2] = (byte) ((TheKeys & 0xff00) >> 8);
	        Key2[3] = (byte) (TheKeys & 0xff);
	
			Keys[0] = fourBytesToInt(Key1);
			Keys[1] = fourBytesToInt(Key2);
			
			
			for(int i = 0; i<4; i++){
				System.out.printf("Key1[%s]=%s and Key2[%s]=%s\n", i, Key1[i], i, Key2[i]);
			}
			
			System.out.print("\n Key1:" + Keys[0] + "  and Key2:" + Keys[1] + "\n");

			
		}
		
		public static int fourBytesToInt(byte[] bytes){
			return ((bytes[0] & 0xFF) << 24) | ((bytes[1] & 0xFF) << 16) | ((bytes[2] & 0xFF) << 8) | (bytes[3] & 0xFF);
		}
		

}
