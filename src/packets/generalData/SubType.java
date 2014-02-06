package packets.generalData;

public enum SubType
{
	UNIMPLEMENTED		(0x00), // 0
	LOCATION 			(0x4A), // 74 
	CHANGE_DIRECTION 	(0x4F), // 79
	GET_SURROUNDINGS	(0x72), //114
	JUMP				(0x85); //133

	
	private final int type;
	
	private SubType(int type)
	{
		this.type = type;
	}
	
	/**
	 * @return Packet type
	 */
	public int getType() {
		return type;
	}
	
	public static SubType get(int type)
	{
		for ( SubType st : SubType.values() ) 
		{
			if ( st.type == type )
			{
				return st;
			}
		}
		
		return UNIMPLEMENTED; 
	}
	
}