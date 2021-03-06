//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class CustReferenceCrud {
		///<summary>Gets one CustReference object from the database using the primary key.  Returns null if not found.</summary>
		public static CustReference SelectOne(long custReferenceNum){
			string command="SELECT * FROM custreference "
				+"WHERE CustReferenceNum = "+POut.Long(custReferenceNum);
			List<CustReference> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one CustReference object from the database using a query.</summary>
		public static CustReference SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<CustReference> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of CustReference objects from the database using a query.</summary>
		public static List<CustReference> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<CustReference> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<CustReference> TableToList(DataTable table){
			List<CustReference> retVal=new List<CustReference>();
			CustReference custReference;
			for(int i=0;i<table.Rows.Count;i++) {
				custReference=new CustReference();
				custReference.CustReferenceNum= PIn.Long  (table.Rows[i]["CustReferenceNum"].ToString());
				custReference.PatNum          = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				custReference.DateMostRecent  = PIn.Date  (table.Rows[i]["DateMostRecent"].ToString());
				custReference.Note            = PIn.String(table.Rows[i]["Note"].ToString());
				custReference.IsBadRef        = PIn.Bool  (table.Rows[i]["IsBadRef"].ToString());
				retVal.Add(custReference);
			}
			return retVal;
		}

		///<summary>Inserts one CustReference into the database.  Returns the new priKey.</summary>
		public static long Insert(CustReference custReference){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				custReference.CustReferenceNum=DbHelper.GetNextOracleKey("custreference","CustReferenceNum");
				int loopcount=0;
				while(loopcount<100){
					try {
						return Insert(custReference,true);
					}
					catch(Oracle.DataAccess.Client.OracleException ex){
						if(ex.Number==1 && ex.Message.ToLower().Contains("unique constraint") && ex.Message.ToLower().Contains("violated")){
							custReference.CustReferenceNum++;
							loopcount++;
						}
						else{
							throw ex;
						}
					}
				}
				throw new ApplicationException("Insert failed.  Could not generate primary key.");
			}
			else {
				return Insert(custReference,false);
			}
		}

		///<summary>Inserts one CustReference into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(CustReference custReference,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				custReference.CustReferenceNum=ReplicationServers.GetKey("custreference","CustReferenceNum");
			}
			string command="INSERT INTO custreference (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="CustReferenceNum,";
			}
			command+="PatNum,DateMostRecent,Note,IsBadRef) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(custReference.CustReferenceNum)+",";
			}
			command+=
				     POut.Long  (custReference.PatNum)+","
				+    POut.Date  (custReference.DateMostRecent)+","
				+"'"+POut.String(custReference.Note)+"',"
				+    POut.Bool  (custReference.IsBadRef)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				custReference.CustReferenceNum=Db.NonQ(command,true);
			}
			return custReference.CustReferenceNum;
		}

		///<summary>Updates one CustReference in the database.</summary>
		public static void Update(CustReference custReference){
			string command="UPDATE custreference SET "
				+"PatNum          =  "+POut.Long  (custReference.PatNum)+", "
				+"DateMostRecent  =  "+POut.Date  (custReference.DateMostRecent)+", "
				+"Note            = '"+POut.String(custReference.Note)+"', "
				+"IsBadRef        =  "+POut.Bool  (custReference.IsBadRef)+" "
				+"WHERE CustReferenceNum = "+POut.Long(custReference.CustReferenceNum);
			Db.NonQ(command);
		}

		///<summary>Updates one CustReference in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		public static void Update(CustReference custReference,CustReference oldCustReference){
			string command="";
			if(custReference.PatNum != oldCustReference.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(custReference.PatNum)+"";
			}
			if(custReference.DateMostRecent != oldCustReference.DateMostRecent) {
				if(command!=""){ command+=",";}
				command+="DateMostRecent = "+POut.Date(custReference.DateMostRecent)+"";
			}
			if(custReference.Note != oldCustReference.Note) {
				if(command!=""){ command+=",";}
				command+="Note = '"+POut.String(custReference.Note)+"'";
			}
			if(custReference.IsBadRef != oldCustReference.IsBadRef) {
				if(command!=""){ command+=",";}
				command+="IsBadRef = "+POut.Bool(custReference.IsBadRef)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE custreference SET "+command
				+" WHERE CustReferenceNum = "+POut.Long(custReference.CustReferenceNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one CustReference from the database.</summary>
		public static void Delete(long custReferenceNum){
			string command="DELETE FROM custreference "
				+"WHERE CustReferenceNum = "+POut.Long(custReferenceNum);
			Db.NonQ(command);
		}

	}
}