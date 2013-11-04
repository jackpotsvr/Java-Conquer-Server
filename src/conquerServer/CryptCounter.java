package conquerServer;

//Used for Cryptography 

public class CryptCounter {
		
		private short counter; 
		public CryptCounter(){/*default CTOR */}
		
		public CryptCounter(short input) { /* alternative CTOR */ 
			counter = input; 
		}
		
		public short Key1(){
			return (short)(counter & 0xFF);
		}
		
		public short Key2(){
			return (short)(counter >> 8);
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
