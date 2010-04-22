//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	internal class AccountCrud {
		///<summary>Gets one Account object from the database using the primary key.  Returns null if not found.</summary>
		internal static Account SelectOne(long accountNum){
			string command="SELECT * FROM account "
				+"WHERE AccountNum = "+POut.Long(accountNum);
			List<Account> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Account object from the database using a query.</summary>
		internal static Account SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Account> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Account object from the database using a query.</summary>
		internal static List<Account> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Account> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		internal static List<Account> TableToList(DataTable table){
			List<Account> retVal=new List<Account>();
			Account obj;
			for(int i=0;i<table.Rows.Count;i++) {
				obj=new Account();
				obj.AccountNum  = PIn.Long  (table.Rows[i]["AccountNum"].ToString());
				obj.Description = PIn.String(table.Rows[i]["Description"].ToString());
				obj.AcctType    = (AccountType)PIn.Int(table.Rows[i]["AcctType"].ToString());
				obj.BankNumber  = PIn.String(table.Rows[i]["BankNumber"].ToString());
				obj.Inactive    = PIn.Bool  (table.Rows[i]["Inactive"].ToString());
				obj.AccountColor= Color.FromArgb(PIn.Int(table.Rows[i]["AccountColor"].ToString()));
				retVal.Add(obj);
			}
			return retVal;
		}

		///<summary>Inserts one Account into the database.  Returns the new priKey.</summary>
		internal static long Insert(Account obj){
			if(PrefC.RandomKeys) {
				obj.AccountNum=ReplicationServers.GetKey("account","AccountNum");
			}
			string command="INSERT INTO account (";
			if(PrefC.RandomKeys) {
				command+="AccountNum,";
			}
			command+="Description,AcctType,BankNumber,Inactive,AccountColor) VALUES(";
			if(PrefC.RandomKeys) {
				command+=POut.Long(obj.AccountNum)+",";
			}
			command+=
				 "'"+POut.String(obj.Description)+"',"
				+    POut.Int   ((int)obj.AcctType)+","
				+"'"+POut.String(obj.BankNumber)+"',"
				+    POut.Bool  (obj.Inactive)+","
				+    POut.Int   (obj.AccountColor.ToArgb())+")";
			if(PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				obj.AccountNum=Db.NonQ(command,true);
			}
			return obj.AccountNum;
		}

		///<summary>Updates one Account in the database.</summary>
		internal static void Update(Account obj){
			string command="UPDATE account SET "
				+"Description = '"+POut.String(obj.Description)+"', "
				+"AcctType    =  "+POut.Int   ((int)obj.AcctType)+", "
				+"BankNumber  = '"+POut.String(obj.BankNumber)+"', "
				+"Inactive    =  "+POut.Bool  (obj.Inactive)+", "
				+"AccountColor=  "+POut.Int   (obj.AccountColor.ToArgb())+" "
				+"WHERE AccountNum = "+POut.Long(obj.AccountNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one Account from the database.</summary>
		internal static void Delete(long accountNum){
			string command="DELETE FROM account "
				+"WHERE AccountNum = "+POut.Long(accountNum);
			Db.NonQ(command);
		}
	}
}