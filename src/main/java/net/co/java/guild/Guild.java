package net.co.java.guild;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;
import net.co.java.entity.Player;


public class Guild {
	
	private String name; 
	private int fund; 
	private int uid; 
	
	/** All the allies and enemies guilds. (CAPPED at 5). */ 
	private Guild[] allies = {null, null, null, null, null};
	private Guild[] enemies = {null, null, null, null, null};
	
	
	/** For each member of the guild that checks in, it will be added to this list. The name of the player will be removed
	 * from the offline members <String> list. 
	 */
	private final List<GuildMember> members = new CopyOnWriteArrayList<GuildMember>();
	
	public Guild(String guildName, int fund, int uid) {
		this.name = guildName; 
		this.fund = fund; 
		this.uid = uid; 
	}
	
	/**
	 * @param the ally guild. 
	 */
	public void addAlly(Guild g){
		for(int i = 0; i < allies.length; i++){
			if(allies[i] == null)
			{
				allies[i] = g;
				return;
			}
		}
	}
	
	/**
	 * @param the enemy guild.
	 */
	public void addEnemy(Guild g){
		for(int i = 0; i < enemies.length; i++){
			if(enemies[i] == null)
			{
				enemies[i] = g;
				return; 
			}
		}
	}
	
	/**
	 * @param oldAlly the ally guild to be removed. 
	 */
	public void removeAlly(Guild oldAlly){
		for(Guild g : allies)
			if(g == oldAlly) g = null; 
	}
	
	/**
	 * @param oldEnemy the enemy guild to be removed.
	 */
	public void removeEnemy(Guild oldEnemy){
		for(Guild g : enemies)
			if(g == oldEnemy) g = null; 
	}

	/**
	 * @return the name
	 */
	public String getGuildName() {
		return name;
	}
	
	/**
	 * @return the fund
	 */
	public int getFund() {
		return fund;
	}

	/**
	 * @return the allies
	 */
	public Guild[] getAllies() {
		return allies;
	}

	/**
	 * @return the enemies
	 */
	public Guild[] getEnemies() {
		return enemies;
	}
	
	/**
	 * @return A list with all the player objects of the online guild members.
	 */
	public List<Player> getOnlineMembers(){
		List<Player> online = new ArrayList<Player>(); 
		
		for(GuildMember gm : members)
			if(gm.isOnline())
				online.add(gm.getPlayer());
		
		return online;
	}
	
	public String getGuildLeaderName(){
		for(GuildMember gm : members)
			if(gm.getRank() == GuildRank.GUILDLEADER)
				return gm.getName(); 
		return null; // O_o I hope it never comes here.. 
	}
	
	public void addGuildMember(GuildMember gm){
		members.add(gm);
	}
	
	public GuildMember addOnlineMember(Player p) { 
		for(GuildMember gm : members)
			if(!gm.isOnline() && gm.getName().equals(p.getName()))
			{
				gm.setPlayer(p);
				return gm;
			}
		return null; // shouldn't get here.
	}
	
	public void removeOnlineMember(Player p){
		for(GuildMember gm : members)
			if(gm.getPlayer() == p)
				gm.goOffline();
	}
	
	public int getMemberCount(){
		return members.size();
	}
	
	public List<GuildMember> getMembers() {
		return members;
	}
	
	public int getUID(){
		return uid; 
	}
}
