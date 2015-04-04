package net.co.java.tools;
import java.io.File;
import java.io.FileNotFoundException;
import java.sql.DriverManager;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.Scanner;

/**
 * Reads all the items from the ini file, and inserts them into the postgresql database.
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 *
 */

public class InsertItems
{
	static Connection connection = null;
	
	public static void main(String[] args) throws FileNotFoundException
	{
		
		File items = new File("ini/COItems.txt");
		
		try {
			Class.forName("org.postgresql.Driver");
		} catch (ClassNotFoundException e) {
 
			System.out.println("Where is your PostgreSQL JDBC Driver? "
					+ "Include in your library path!");
			e.printStackTrace();
			return;
 
		}
		
		try {
			connection = DriverManager.getConnection(
					args[0], args[1],
					args[2]);
			read(items);
		} catch (SQLException e) {

			System.out.println("Connection Failed! Check output console");

			e.printStackTrace();
			return;

		}	
	}
	
	static void read(File file) throws FileNotFoundException {
		Scanner sc = new Scanner(file);		
		if(sc.hasNext()&&sc.next().equalsIgnoreCase("[Items]")) {
			while(sc.hasNext()) {
				try {
					read(sc.next());
				} catch (Exception e) {	}
			}
		}
		sc.close();
	}
	
	static void read(String string) throws SQLException {
		PreparedStatement pst = null;
		
		String stm = "INSERT INTO items(item_SID, item_name, item_maxDura, item_worth, item_CPSworth, item_classReq, "
				+ "item_profReq, item_lvlReq, item_sexReq, item_strReq, item_agiReq, item_minxAtk, item_maxAtk, "
				+ "item_defence, item_mDef, item_mAttack, item_dodge, item_agility) "
				+ "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";  //18
		
		pst = connection.prepareStatement(stm);
		
		String[] split = string.split("=");
		Long id = Long.valueOf(split[0]);
		Scanner sc = new Scanner(split[1]);
		sc.useDelimiter("-[^\\w]*");
		
		String name = sc.next();
		int classReq = sc.nextInt();
		int profReq = sc.nextInt();
		int lvlReq = sc.nextInt();
		int sexReq = sc.nextInt();
		int strReq = sc.nextInt();
		int agiReq = sc.nextInt();
		int worth = sc.nextInt();
		int minAtk = sc.nextInt();
		int maxAtk = sc.nextInt();
		int defence = sc.nextInt();
		int mDef = sc.nextInt();
		int mAttack = sc.nextInt();
		int dodge = sc.nextInt();
		int agility = sc.nextInt();
		int CPWorth = sc.nextInt();
		
		int maxDura = 0;
		if(sc.hasNextInt()) {
			maxDura = sc.nextInt() * 100;
		}
	
		pst.setLong(1, id.longValue());
		pst.setString(2, name);
		pst.setInt(3, maxDura);
		pst.setInt(4, worth);
		pst.setInt(5, CPWorth);
		pst.setInt(6, classReq);
		pst.setInt(7, profReq);
		pst.setInt(8, lvlReq);
		pst.setInt(9, sexReq);
		pst.setInt(10, strReq);
		pst.setInt(11, agiReq);
		pst.setInt(12, minAtk);
		pst.setInt(13, maxAtk);
		pst.setInt(14, defence);
		pst.setInt(15, mDef);
		pst.setInt(16, mAttack);
		pst.setInt(17, dodge);
		pst.setInt(18, agility);
		
		pst.executeUpdate();
	
		sc.close();
	}

}
