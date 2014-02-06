package net.co.java.guild;

public enum GuildRank {
	None(0),
	Member(50),
	InternManager(60),
	DeputyManager(70),
	BranchManager(80),
	DeputyLeader(90),
	Leader(100);
	
	final int rank;
	
	private GuildRank(int rank) {
		this.rank = rank;
	}
	
	public int getRank() {
		return rank;
	}
}
