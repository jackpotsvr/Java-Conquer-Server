package net.co.java.npc.dialogs;

import net.co.java.entity.NPC;
import net.co.java.entity.Player;
import net.co.java.entity.Player.Profession;
import net.co.java.packets.GeneralData;
import net.co.java.packets.UpdatePacket;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.server.GameServerClient;

public class TrojanStar extends NPC_Dialog{
	private Player player; 
	
	public TrojanStar(NPC npc) {
		super(npc);
	}

	@Override
	protected void npc_handle(GameServerClient client) {
		player = client.getPlayer();
		
		switch(input)
		{	
			case INITIAL_DIALOG:
			{
				if(player.getProfession().isTrojan())
				{
					NPC_Say("Wielding dual weapons, the Trojans always charge fearlessly into combat and they believe "
							+ "courage is the secret of victory. So, what can I do for you?" ).send(client); 
					
					NPC_Link1(1, "I want to get promoted.").send(client);
					NPC_Link1(2, "Learn class and weapon skills.").send(client);
					NPC_Link1(DIALOG_QUIT, "Okay, I see.").send(client);
					
					NPC_SetFace().send(client);
					
					NPC_Finish().send(client);
				}
				else {
					NPC_Say("Trojans do not share their secrets of battle with others. I shall not teach you." ).send(client); 
					NPC_Link1(DIALOG_QUIT, "I see.").send(client);		
					NPC_SetFace().send(client);
					NPC_Finish().send(client);
				}
				break;
			}
			case 1:
			{
				if(player.isPromotable())
				{
					NPC_Say("I can promote you to " 
							+ Profession.valueOf(client.getPlayer().getProfession().value + 1)
							+ " if you wish!" ).send(client);
					
					NPC_Link1(3, "Sure").send(client);
					NPC_Link1(DIALOG_QUIT, "I do not want to be promoted.").send(client);
					new GeneralData(SubType.CONFIRM_PROFS, client.getPlayer()).handle(client);						
				} else { 
					NPC_Say("Sorry, I cannot promote you yet." ).send(client);
					NPC_Link1(DIALOG_QUIT, "Sigh.").send(client);
				}
				NPC_SetFace().send(client);
				NPC_Finish().send(client);
		
				break;
			}
			case 2: 
			{
				NPC_Say("Skills are yet to be implemented.").send(client);
				NPC_Link1(DIALOG_QUIT, "Alright.").send(client);
				NPC_SetFace().send(client);
				NPC_Finish().send(client);
				
				break;
			}
			case 3:
			{
				player.promote();

				new UpdatePacket(client.getPlayer())
					.setAttribute(UpdatePacket.Mode.Job, (long)client.getPlayer().getProfession().value)
					.build().send(client);
				
				NPC_Say("Congratulations. You have been promoted to "
						+ client.getPlayer().getProfession() 
						+ "!").send(client); 
				NPC_Link1(DIALOG_QUIT, "Thanks").send(client);
				NPC_SetFace().send(client);
				NPC_Finish().send(client);
		
				break;
			}
			default:
				break;
		}	
	}
}
