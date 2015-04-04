package net.co.java.model;

/**
 * The authorization promise is an object that contains the information that the
 * Auth server binds to a new player identity.
 * @author Jan-Willem Gmelig Meyling
 *
 */
public class AuthorizationPromise {
	
	private final Long identity;
	private final long token = 5;
	private final String accountName;
	private String characterName;

	public AuthorizationPromise(Long identity, String accountName) {
		this.identity = identity;
		this.accountName = accountName;
	}
	
	public AuthorizationPromise(Long identity, String accountName, String characterName) {
		this(identity, accountName);
		setCharacterName(characterName);
	}
	
	public Long getIdentity() {
		return identity;
	}
	
	public long getToken() {
		return token;
	}
	
	public void setCharacterName(String name) {
		this.characterName = name;
	}
	
	public String getCharacterName() {
		return characterName;
	}
	
	public boolean hasCharacter() {
		return characterName != null;
	}
	
	public String getAccountName() {
		return accountName;
	}

}
