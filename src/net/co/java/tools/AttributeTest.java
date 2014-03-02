package net.co.java.tools;

public class AttributeTest {


	public static void main(String[] args) {
		int[] target = {5, 2, 3, 0};
		int[] current = {5, 2, 3, 0};
		
		for (int l = 1; l <= 120; l++ ) {
		
			if ( l > 1 ) {
				for ( int k = 0; k < 3; k++ ) {
					
					int best = 4;
					double lowestAfw = Double.MAX_VALUE;
					
					for ( int i = 0; i < 4; i++ ) {
						if ( target[i] == 0 )
							continue;
						int temp = current[i] + 1;
						double growth = ((double) temp) / ((double) target[i]);
						double afw = 0;
						
						for ( int j = 0; j < 4; j++ ) {
							if ( j == i ) continue;
							afw += target[j] * growth - current[j]; 
						}
						if ( afw < lowestAfw ) {
							lowestAfw = afw;
							best = i;
						}
					}
					
					current[best] = current[best] + 1;
				}
			}
			
			System.out.println(l + "; " + current[0] + "; " + current[1] + "; " + current[2] + "; " + current[3]);
		
		}

	}

}
