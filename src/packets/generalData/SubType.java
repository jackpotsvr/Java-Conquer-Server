package packets.generalData;

public enum SubType
{
	JUST_SPACE_HOLDER	(0),
	NO_VALUES_ATM		(1);

	
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