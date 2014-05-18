package net.co.java.skill;

import net.co.java.entity.Entity;
import net.co.java.entity.Entity.Flag;
import net.co.java.entity.Player;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.UpdatePacket;
import net.co.java.packets.UpdatePacket.Mode;
import net.co.java.server.GameServerClient;
import net.co.java.server.GameServerTicks;
import net.co.java.skill.Skill.MagicSkill;

/** aka lucky time */
public class Bless extends MagicSkill{
	
	private GameServerClient caster; 
	public final static int BLESS_PERIOD = 2000;

	@Override
	public double damageMutiplier(int level) {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int range(int level) {
		return 5;
	}

	@Override
	public int distance(int level) {
		return 0;
	}

	@Override
	public void handle(GameServerClient player, InteractPacket ip) {
		this.caster = player;
		
		player.getPlayer().setFlag(Flag.LUCKYTIME);
		new UpdatePacket(player.getPlayer())
			.setAttribute(Mode.RaiseFlag, player.getPlayer().getFlags())
			.build().send(player);
		
		GameServerTicks.calculateBless(this);
	}
	
	public void addBless(){
		
		TargetBuilder tb = new TargetBuilder(caster.getPlayer()).inCircle(range(0));
		
		caster.getPlayer().setBlessTime(caster.getPlayer().getBlessTime() + (BLESS_PERIOD * 3));
		
		new UpdatePacket(caster.getPlayer())
			.setAttribute(Mode.LuckyTime, caster.getPlayer().getBlessTime())
			.build().send(caster);
		
		for(Entity e : tb.getEntities())
		{
			if(e instanceof Player)
			{
				Player p = (Player)e;
				
				if(p.isPraying())
				{
					if(p.getPrayerHost() == caster.getPlayer())
					p.setBlessTime(p.getBlessTime() + BLESS_PERIOD);
					
					new UpdatePacket(p)
						.setAttribute(Mode.LuckyTime,p.getBlessTime())
						.build().send(p.getClient());
				} else {
					if(!p.hasFlag(Flag.LUCKYTIME)) 
					{
						p.setFlag(Flag.PRAY);
						p.setPrayerHost(caster.getPlayer());
						new UpdatePacket(p)
							.setAttribute(Mode.RaiseFlag, p.getFlags())
							.build().send(p);
					}
				}
			}
		}
	}

	@Override
	public int getSkillID() {
		return 9876 ;
	}

	@Override
	public int getTargetExp(int level) {
		return 0;
	}

	@Override
	public int levelUpLevel(int level) {
		return 0;
	}

	@Override
	public int maxLevel() {
		return 0;
	}

}
