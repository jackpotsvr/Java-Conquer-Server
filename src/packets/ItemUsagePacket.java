package packets;

import conquerServer.GameServerThread;

public class ItemUsagePacket {
	
	public static enum Mode{
		BuyItem(1), SellItem(2), RemoveInventory(3), EquipItem(4), SetEquipPosition(
		5), UnEquipItem(6), ShowWarehouseMoney(9), DepositWarehouseMoney(
		10), WithdrawWarehouseMoney(11), DropGold(12), RepairItem(14), UpdateDurability(
		17), RemoveEquipment(18), UpgradeDragonball(19), UpgradeMeteor(
		20), ShowVendingList(21), AddVendingItem(22), RemoveVendingItem(
		23), BuyVendingItem(24), UpdateArrowCount(25), ParticleEffect(
		26), Ping(27);
		
		private final int index; 
		
		private Mode(int index){
			this.index = index; 
		}
		
		public static Mode getMode(long index){
			for(Mode m : Mode.values())
			{
				if (m.index == index)
				{
					return m;
				}
			}
			return null;
		}
	}   
	
	public static IncommingPacket in(byte[] data, final GameServerThread client) {
		return new IncommingPacket(PacketType.ITEM_USAGE_PACKET, data) {{
			long identity = this.readUnsignedInt(4);
			long parameter = this.readUnsignedInt(8);
			Mode m = Mode.getMode(this.readUnsignedInt(12));
			long timestamp = this.readUnsignedInt(16);
			
			switch(m)
			{
				case Ping:
					out(identity, 0, m, timestamp).send(client);
					break;
			}
		}};
	}
	
	public static OutgoingPacket out(final long identity, final long parameter, final Mode mode, final long timestamp) {
		return new OutgoingPacket(PacketType.ITEM_USAGE_PACKET, new byte[28]) {{
			this.putUnsignedInteger(identity);
			this.putUnsignedInteger(parameter);
			this.putUnsignedInteger(mode.index);
			this.putUnsignedInteger(timestamp);
		}};
	}
}
