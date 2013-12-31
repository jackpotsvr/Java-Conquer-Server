package packets.generalData;

public enum SubType
{
	LOCATION 			(0x4A), // 74 
	GET_SURROUNDINGS	(0x72); //114

	
	private final long type;
	
	private SubType(long type)
	{
		this.type = type;
	}
	
	/**
	 * @return Packet type
	 */
	public long getType() {
		return type;
	}
	
	public static SubType get(long type)
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