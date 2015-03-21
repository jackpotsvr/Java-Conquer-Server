//package net.co.java.npc.dialogs;
//
//import net.co.java.entity.Location;
//import net.co.java.entity.NPC;
//import net.co.java.packets.GeneralData;
//import net.co.java.packets.GeneralData.SubType;
//import net.co.java.server.GameServerClient;
//import net.co.java.server.Map;
//
//public class TC_Conductress extends NPC_Dialog{
//
//	public TC_Conductress(NPC npc) {
//		super(npc);
//	}
//	
//	/**
//	 *  Can't do a real lot with conductress yet, cause we still making improvements on setting the location of entities.
//	 */
//
//	@Override
//	protected void npc_handle(GameServerClient client) {
//		switch(input)
//		{	
//			case INITIAL_DIALOG:
//			{
//				NPC_Say("Where are you heading for? I can teleport you for a price of 100 silver.").send(client); 
//				
//				NPC_Link1(1, "Phoenix Castle").send(client);
//				NPC_Link1(2, "Desert City").send(client);
//				NPC_Link1(3, "Ape Mountain").send(client);
//				NPC_Link1(4, "Bird City").send(client);
//				NPC_Link1(5, "Mine Cave").send(client);
//				NPC_Link1(6, "Market").send(client);
//				NPC_Link1(DIALOG_QUIT, "Just passing by.").send(client);
//				
//				NPC_SetFace().send(client);
//				
//				NPC_Finish().send(client);
//				break;
//			}
//			case 1:
//			{
//				// TODO use the teleport method here we will make ;) 
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 957, 556)); 
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client, packet);
//				break;
//			}
//			case 2:
//			{
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 066, 465)); 
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
//				break;
//			}
//			case 3: 
//			{
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 554, 957)); 
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
//				break;
//			}
//			case 4:
//			{
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 227, 194)); 
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
//				break;
//			}
//			case 5:
//			{
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 056, 403)); 
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
//				break;
//			}
//			case 6:
//			{
//				client.getPlayer().setLocation(new Location(Map.CentralPlain, 439, 383));
//				new GeneralData(SubType.LOCATION, client.getPlayer()).handle(client);
//				break;
//			}
//			default:
//				break;
//		}	
//	}
//
//}
