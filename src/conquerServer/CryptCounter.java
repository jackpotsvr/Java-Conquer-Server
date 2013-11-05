package conquerServer;

//Used for Cryptography 

public class CryptCounter {
		
		private short counter; 
		public CryptCounter(){
			counter = 0x00;
		}
		
		public CryptCounter(short input) { /* alternative CTOR */ 
			counter = input; 
		}
		
		public byte Key1(){
			return (byte)(counter & 0xFF);
		}
		
		public byte Key2(){
			return (byte)((counter >> 8) & 0xF);
		}
		
		
		// ACCESSORS
		public short getCounter(){
			return counter; 
		}
		
		public void setCounter(short counter){
			this.counter = counter; 
		}
		
		public void counterIncrement(){
			counter++; 
		}
		
}
