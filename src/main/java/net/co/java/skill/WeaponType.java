package net.co.java.skill;

public enum WeaponType {
	BLADE(410),
    SWORD(420),
    BACKSWORD(421),
    HOOK(430),
    WHIP(440),
    AXE(450),
    HAMMER(460),
    CLUB(480),
    SCEPTER(481),
    DAGGER(490),
    BOW(500),
    GLAIVE(510),
    POLEAXE(530),
    LONGHAMMER(540),
    SPEAR(560),
    WAND(561),
    HALBERT(580);
	
	public final int ProfID;
	
	private WeaponType(int ProfID) {
		this.ProfID = ProfID;
	}
	
	public static WeaponType valueOf(int id)  {
		for ( WeaponType wt : WeaponType.values() ) {
			if ( wt.ProfID == id )
				return wt;
		}
		return null; 
	}
	
	public static boolean equals(long item1_id, long item2_id)
	{
		return ((item1_id / 1000) == (item2_id / 1000));
	}
	
}
