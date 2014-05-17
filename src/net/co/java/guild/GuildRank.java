package net.co.java.guild;

public enum GuildRank {
	NONE 				(0),
	MEMBER				(50),
	INTERNMANAGER 		(60),
	DEPUTYMANAGER 		(70),
	BRANCHMANAGER 		(80),
	DEPUTYLEADER 		(90),
	GUILDLEADER 		(100);
	
	public final int rank;
	
	private GuildRank(int rank) {
		this.rank = rank;
	}
	
	public static GuildRank valueOf(int rank)  {
		for ( GuildRank gr : GuildRank.values() ) {
			if ( gr.rank == rank )
				return gr;
		}
		return null; 
	}		
	
	public int getRank() {
		return rank;
	}
}