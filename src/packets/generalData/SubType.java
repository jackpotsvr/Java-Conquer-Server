package packets.generalData;

public enum SubType
{
	LOCATION 			(0x4A), // 74 
	GET_SURROUNDINGS	(0x72); //114

	
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
		return null;
	}
	
}