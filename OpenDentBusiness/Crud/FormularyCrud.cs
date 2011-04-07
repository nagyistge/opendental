//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	internal class FormularyCrud {
		///<summary>Gets one Formulary object from the database using the primary key.  Returns null if not found.</summary>
		internal static Formulary SelectOne(long formularyNum){
			string command="SELECT * FROM formulary "
				+"WHERE FormularyNum = "+POut.Long(formularyNum);
			List<Formulary> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Formulary object from the database using a query.</summary>
		internal static Formulary SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Formulary> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Formulary objects from the database using a query.</summary>
		internal static List<Formulary> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Formulary> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		internal static List<Formulary> TableToList(DataTable table){
			List<Formulary> retVal=new List<Formulary>();
			Formulary formulary;
			for(int i=0;i<table.Rows.Count;i++) {
				formulary=new Formulary();
				formulary.FormularyNum= PIn.Long  (table.Rows[i]["FormularyNum"].ToString());
				formulary.Description = PIn.String(table.Rows[i]["Description"].ToString());
				retVal.Add(formulary);
			}
			return retVal;
		}

		///<summary>Inserts one Formulary into the database.  Returns the new priKey.</summary>
		internal static long Insert(Formulary formulary){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				formulary.FormularyNum=DbHelper.GetNextOracleKey("formulary","FormularyNum");
				int loopcount=0;
				while(loopcount<100){
					try {
						return Insert(formulary,true);
					}
					catch(Oracle.DataAccess.Client.OracleException ex){
						if(ex.Number==1 && ex.Message.ToLower().Contains("unique constraint") && ex.Message.ToLower().Contains("violated")){
							formulary.FormularyNum++;
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
				return Insert(formulary,false);
			}
		}

		///<summary>Inserts one Formulary into the database.  Provides option to use the existing priKey.</summary>
		internal static long Insert(Formulary formulary,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				formulary.FormularyNum=ReplicationServers.GetKey("formulary","FormularyNum");
			}
			string command="INSERT INTO formulary (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FormularyNum,";
			}
			command+="Description) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(formulary.FormularyNum)+",";
			}
			command+=
				 "'"+POut.String(formulary.Description)+"')";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				formulary.FormularyNum=Db.NonQ(command,true);
			}
			return formulary.FormularyNum;
		}

		///<summary>Updates one Formulary in the database.</summary>
		internal static void Update(Formulary formulary){
			string command="UPDATE formulary SET "
				+"Description = '"+POut.String(formulary.Description)+"' "
				+"WHERE FormularyNum = "+POut.Long(formulary.FormularyNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Formulary in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		internal static void Update(Formulary formulary,Formulary oldFormulary){
			string command="";
			if(formulary.Description != oldFormulary.Description) {
				if(command!=""){ command+=",";}
				command+="Description = '"+POut.String(formulary.Description)+"'";
			}
			if(command==""){
				return;
			}
			command="UPDATE formulary SET "+command
				+" WHERE FormularyNum = "+POut.Long(formulary.FormularyNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one Formulary from the database.</summary>
		internal static void Delete(long formularyNum){
			string command="DELETE FROM formulary "
				+"WHERE FormularyNum = "+POut.Long(formularyNum);
			Db.NonQ(command);
		}

	}
}