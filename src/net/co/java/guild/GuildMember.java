package net.co.java.guild;

import net.co.java.entity.Player;

public class GuildMember {
	
	private Player player = null; // The player object could be usefull for guild chat purposes etc.
	private String name = null;  // if player is not online, there is no player object, else there is. 
	private GuildRank rank; // e.g. Guild Leader 
	private int donation; 
	private Guild guild;
	private String bulletin; 
	
	/** Not entirely sure if this one is actually ever needed. */ 
	public GuildMember(Guild guild, Player player, GuildRank rank, int donation)
	{
		this.guild = guild;
		this.player = player;
		this.rank = rank;
		this.donation = donation; 
	}
	
	public GuildMember(Guild guild, String name, GuildRank rank, int donation)
	{
		this.guild = guild; 
		this.name = name;
		this.rank = rank;
		this.donation = donation; 
	}
	
	/**
	 *  Can be used as a check to see whether you should use getPlayer() or getName(). 
	 *  @return
	 */
	public boolean isOnline()
	{
		return (player != null); // if there is a player object, the player ought to be online. 
	}
	
	public void setPlayer(Player p)
	{
		// no check for p.getName() == name, cause we will do it at a higher level.
		this.player = p; 
		this.name = null; 
	}
	
	public Player getPlayer(){
		return player;
	}
	
	// returns player.getName() if there is a player instance, else it returns the name. 
	public String getName(){
		return (player == null) ? name : player.getName(); 
	}
	
	public void donate(int amount){
		donation += amount;
	}
	
	public void goOffline(){
		name = player.getName();
		player = null;
	}

	/**
	 * @return the rank
	 */
	public GuildRank getRank() {
		return rank;
	}

	/**
	 * @return the donation
	 */
	public int getDonation() {
		return donation;
	}
	
	public Guild getGuild(){
		return guild; 
	}
}
