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
import net.co.java.tasks.BlessTask;

/** aka lucky time */
public class Bless extends MagicSkill{
	
	private GameServerClient casterClient;
    private Player caster;
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
	public void handle(GameServerClient client, InteractPacket ip) {
		this.casterClient = client;
        this.caster = client.getPlayer();
		
		client.getPlayer().setFlag(Flag.LUCKYTIME);
		new UpdatePacket(client.getPlayer())
			.setAttribute(Mode.RaiseFlag, client.getPlayer().getFlags())
			.build().send(client);
		
		//GameServerTicks.calculateBless(this);
        BlessTask bt = new BlessTask(this, client.getPlayer());
        client.getModel().getGameServerTicks().addTickTask(bt);
	}
//
//	public void addBless(){
//
//		TargetBuilder tb = new TargetBuilder(casterClient.getPlayer()).inCircle(range(0));
//
//		casterClient.getPlayer().setBlessTime(casterClient.getPlayer().getBlessTime() + (BLESS_PERIOD * 3));
//
//		new UpdatePacket(casterClient.getPlayer())
//			.setAttribute(Mode.LuckyTime, casterClient.getPlayer().getBlessTime())
//			.build().send(casterClient);
//
//		for(Entity e : tb.getEntities())
//		{
//			if(e instanceof Player)
//			{
//				Player p = (Player)e;
//
//				if(p.isPraying())
//				{
//					if(p.getPrayerHost() == casterClient.getPlayer())
//					p.setBlessTime(p.getBlessTime() + BLESS_PERIOD);
//
//					new UpdatePacket(p)
//						.setAttribute(Mode.LuckyTime,p.getBlessTime())
//						.build().send(p.getClient());
//				} else {
//					if(!p.hasFlag(Flag.LUCKYTIME))
//					{
//						p.setFlag(Flag.PRAY);
//						p.setPrayerHost(casterClient.getPlayer());
//						new UpdatePacket(p)
//							.setAttribute(Mode.RaiseFlag, p.getFlags())
//							.build().send(p);
//					}
//				}
//			}
//		}
//	}

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

    public long getCasterId() {
        return caster.getIdentity();
    }

    public Player getCaster() {
        return caster;
    }

}
