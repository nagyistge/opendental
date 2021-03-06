//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	public class MedicalOrderCrud {
		///<summary>Gets one MedicalOrder object from the database using the primary key.  Returns null if not found.</summary>
		public static MedicalOrder SelectOne(long medicalOrderNum){
			string command="SELECT * FROM medicalorder "
				+"WHERE MedicalOrderNum = "+POut.Long(medicalOrderNum);
			List<MedicalOrder> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one MedicalOrder object from the database using a query.</summary>
		public static MedicalOrder SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<MedicalOrder> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of MedicalOrder objects from the database using a query.</summary>
		public static List<MedicalOrder> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<MedicalOrder> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<MedicalOrder> TableToList(DataTable table){
			List<MedicalOrder> retVal=new List<MedicalOrder>();
			MedicalOrder medicalOrder;
			for(int i=0;i<table.Rows.Count;i++) {
				medicalOrder=new MedicalOrder();
				medicalOrder.MedicalOrderNum= PIn.Long  (table.Rows[i]["MedicalOrderNum"].ToString());
				medicalOrder.MedOrderType   = (MedicalOrderType)PIn.Int(table.Rows[i]["MedOrderType"].ToString());
				medicalOrder.PatNum         = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				medicalOrder.DateTimeOrder  = PIn.DateT (table.Rows[i]["DateTimeOrder"].ToString());
				medicalOrder.Description    = PIn.String(table.Rows[i]["Description"].ToString());
				medicalOrder.IsDiscontinued = PIn.Bool  (table.Rows[i]["IsDiscontinued"].ToString());
				medicalOrder.ProvNum        = PIn.Long  (table.Rows[i]["ProvNum"].ToString());
				retVal.Add(medicalOrder);
			}
			return retVal;
		}

		///<summary>Inserts one MedicalOrder into the database.  Returns the new priKey.</summary>
		public static long Insert(MedicalOrder medicalOrder){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				medicalOrder.MedicalOrderNum=DbHelper.GetNextOracleKey("medicalorder","MedicalOrderNum");
				int loopcount=0;
				while(loopcount<100){
					try {
						return Insert(medicalOrder,true);
					}
					catch(Oracle.DataAccess.Client.OracleException ex){
						if(ex.Number==1 && ex.Message.ToLower().Contains("unique constraint") && ex.Message.ToLower().Contains("violated")){
							medicalOrder.MedicalOrderNum++;
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
				return Insert(medicalOrder,false);
			}
		}

		///<summary>Inserts one MedicalOrder into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(MedicalOrder medicalOrder,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				medicalOrder.MedicalOrderNum=ReplicationServers.GetKey("medicalorder","MedicalOrderNum");
			}
			string command="INSERT INTO medicalorder (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="MedicalOrderNum,";
			}
			command+="MedOrderType,PatNum,DateTimeOrder,Description,IsDiscontinued,ProvNum) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(medicalOrder.MedicalOrderNum)+",";
			}
			command+=
				     POut.Int   ((int)medicalOrder.MedOrderType)+","
				+    POut.Long  (medicalOrder.PatNum)+","
				+    POut.DateT (medicalOrder.DateTimeOrder)+","
				+"'"+POut.String(medicalOrder.Description)+"',"
				+    POut.Bool  (medicalOrder.IsDiscontinued)+","
				+    POut.Long  (medicalOrder.ProvNum)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				medicalOrder.MedicalOrderNum=Db.NonQ(command,true);
			}
			return medicalOrder.MedicalOrderNum;
		}

		///<summary>Updates one MedicalOrder in the database.</summary>
		public static void Update(MedicalOrder medicalOrder){
			string command="UPDATE medicalorder SET "
				+"MedOrderType   =  "+POut.Int   ((int)medicalOrder.MedOrderType)+", "
				+"PatNum         =  "+POut.Long  (medicalOrder.PatNum)+", "
				+"DateTimeOrder  =  "+POut.DateT (medicalOrder.DateTimeOrder)+", "
				+"Description    = '"+POut.String(medicalOrder.Description)+"', "
				+"IsDiscontinued =  "+POut.Bool  (medicalOrder.IsDiscontinued)+", "
				+"ProvNum        =  "+POut.Long  (medicalOrder.ProvNum)+" "
				+"WHERE MedicalOrderNum = "+POut.Long(medicalOrder.MedicalOrderNum);
			Db.NonQ(command);
		}

		///<summary>Updates one MedicalOrder in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		public static void Update(MedicalOrder medicalOrder,MedicalOrder oldMedicalOrder){
			string command="";
			if(medicalOrder.MedOrderType != oldMedicalOrder.MedOrderType) {
				if(command!=""){ command+=",";}
				command+="MedOrderType = "+POut.Int   ((int)medicalOrder.MedOrderType)+"";
			}
			if(medicalOrder.PatNum != oldMedicalOrder.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(medicalOrder.PatNum)+"";
			}
			if(medicalOrder.DateTimeOrder != oldMedicalOrder.DateTimeOrder) {
				if(command!=""){ command+=",";}
				command+="DateTimeOrder = "+POut.DateT(medicalOrder.DateTimeOrder)+"";
			}
			if(medicalOrder.Description != oldMedicalOrder.Description) {
				if(command!=""){ command+=",";}
				command+="Description = '"+POut.String(medicalOrder.Description)+"'";
			}
			if(medicalOrder.IsDiscontinued != oldMedicalOrder.IsDiscontinued) {
				if(command!=""){ command+=",";}
				command+="IsDiscontinued = "+POut.Bool(medicalOrder.IsDiscontinued)+"";
			}
			if(medicalOrder.ProvNum != oldMedicalOrder.ProvNum) {
				if(command!=""){ command+=",";}
				command+="ProvNum = "+POut.Long(medicalOrder.ProvNum)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE medicalorder SET "+command
				+" WHERE MedicalOrderNum = "+POut.Long(medicalOrder.MedicalOrderNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one MedicalOrder from the database.</summary>
		public static void Delete(long medicalOrderNum){
			string command="DELETE FROM medicalorder "
				+"WHERE MedicalOrderNum = "+POut.Long(medicalOrderNum);
			Db.NonQ(command);
		}

	}
}