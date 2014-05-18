package net.co.java.server;

import java.util.Timer;
import java.util.TimerTask;

import net.co.java.entity.Entity.Flag;
import net.co.java.entity.Player;
import net.co.java.model.Model;
import net.co.java.packets.GeneralData;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.UpdatePacket;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.packets.UpdatePacket.Mode;
import net.co.java.skill.Bless;

public class GameServerTicks 
{
	private Model model; 
	private Timer timer; 
	
	public GameServerTicks(Model model) {
		this.model = model;
		timer = new Timer();
		addWarnPlayerTask();
		staminaIncrease();
		xpIncrease();
	}
	
	public void addWarnPlayerTask(){
		long delay = 1000;
		long period = 30000;
		
		timer.scheduleAtFixedRate(new TimerTask() {
	        public void run() {
	           for(Player p : model.getPlayers().values())
	           {
	        		new MessagePacket(MessagePacket.SYSTEM, p.getName(), 
	        				"This message is being send very often, and is very annoying.")
					.setMessageType(MessageType.CENTER)
					.build().send(p.getClient());
	        	   //p.getClient()
	           }
	        }
	    }, delay, period);

	}
	
	public void staminaIncrease() {
		long delay = 1000;
		long period = 1000;
		
		timer.scheduleAtFixedRate(new TimerTask() {
	        public void run() {
				for(Player p : model.getPlayers().values())
				{
					int stamina = p.getStamina();
					if(stamina < 100)
					{
						stamina += 2;
						p.setStamina(stamina);
						new UpdatePacket(p)
						.setAttribute(UpdatePacket.Mode.Stamina, (long) stamina)
						.build().send(p.getClient());
					}
				}
	        }
		}, delay, period);
	}
	
	public void xpIncrease() { 
		long delay = 3000;
		long period = 3000; 
		
		timer.scheduleAtFixedRate(new TimerTask() {
	        public void run() {
				for(Player p : model.getPlayers().values())
				{
					if(!p.isXPON())
					{
						int xpRing = p.getXpRing();
						new UpdatePacket(p)
						.setAttribute(UpdatePacket.Mode.XPCircle, (long) xpRing)
						.build().send(p.getClient());
						
						if(xpRing < 100)
						{
							xpRing += 1;
							p.setXpRing(xpRing);
						}
						else {
							p.setFlag(Flag.EXP);
							new UpdatePacket(p)
							.setAttribute(Mode.RaiseFlag, p.getFlags())
							.build().send(p.getClient());
						}
					}
				}
	        }
		}, delay, period);
	}
	
	public static Timer calculateBless(final Bless bless){
		Timer tm = new Timer();
		tm.scheduleAtFixedRate(new TimerTask() {
	        public void run(){ bless.addBless(); } 
	        },Bless.BLESS_PERIOD, Bless.BLESS_PERIOD);
		return tm;
	}
	
}
