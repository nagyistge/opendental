//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	internal class PatPlanCrud {
		///<summary>Gets one PatPlan object from the database using the primary key.  Returns null if not found.</summary>
		internal static PatPlan SelectOne(long patPlanNum){
			string command="SELECT * FROM patplan "
				+"WHERE PatPlanNum = "+POut.Long(patPlanNum)+" LIMIT 1";
			List<PatPlan> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one PatPlan object from the database using a query.</summary>
		internal static PatPlan SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PatPlan> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of PatPlan objects from the database using a query.</summary>
		internal static List<PatPlan> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PatPlan> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		internal static List<PatPlan> TableToList(DataTable table){
			List<PatPlan> retVal=new List<PatPlan>();
			PatPlan patPlan;
			for(int i=0;i<table.Rows.Count;i++) {
				patPlan=new PatPlan();
				patPlan.PatPlanNum  = PIn.Long  (table.Rows[i]["PatPlanNum"].ToString());
				patPlan.PatNum      = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				patPlan.PlanNum     = PIn.Long  (table.Rows[i]["PlanNum"].ToString());
				patPlan.Ordinal     = PIn.Byte  (table.Rows[i]["Ordinal"].ToString());
				patPlan.IsPending   = PIn.Bool  (table.Rows[i]["IsPending"].ToString());
				patPlan.Relationship= (Relat)PIn.Int(table.Rows[i]["Relationship"].ToString());
				patPlan.PatID       = PIn.String(table.Rows[i]["PatID"].ToString());
				patPlan.InsSubNum   = PIn.Long  (table.Rows[i]["InsSubNum"].ToString());
				retVal.Add(patPlan);
			}
			return retVal;
		}

		///<summary>Inserts one PatPlan into the database.  Returns the new priKey.</summary>
		internal static long Insert(PatPlan patPlan){
			return Insert(patPlan,false);
		}

		///<summary>Inserts one PatPlan into the database.  Provides option to use the existing priKey.</summary>
		internal static long Insert(PatPlan patPlan,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				patPlan.PatPlanNum=ReplicationServers.GetKey("patplan","PatPlanNum");
			}
			string command="INSERT INTO patplan (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="PatPlanNum,";
			}
			command+="PatNum,PlanNum,Ordinal,IsPending,Relationship,PatID,InsSubNum) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(patPlan.PatPlanNum)+",";
			}
			command+=
				     POut.Long  (patPlan.PatNum)+","
				+    POut.Long  (patPlan.PlanNum)+","
				+    POut.Byte  (patPlan.Ordinal)+","
				+    POut.Bool  (patPlan.IsPending)+","
				+    POut.Int   ((int)patPlan.Relationship)+","
				+"'"+POut.String(patPlan.PatID)+"',"
				+    POut.Long  (patPlan.InsSubNum)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				patPlan.PatPlanNum=Db.NonQ(command,true);
			}
			return patPlan.PatPlanNum;
		}

		///<summary>Updates one PatPlan in the database.</summary>
		internal static void Update(PatPlan patPlan){
			string command="UPDATE patplan SET "
				+"PatNum      =  "+POut.Long  (patPlan.PatNum)+", "
				+"PlanNum     =  "+POut.Long  (patPlan.PlanNum)+", "
				+"Ordinal     =  "+POut.Byte  (patPlan.Ordinal)+", "
				+"IsPending   =  "+POut.Bool  (patPlan.IsPending)+", "
				+"Relationship=  "+POut.Int   ((int)patPlan.Relationship)+", "
				+"PatID       = '"+POut.String(patPlan.PatID)+"', "
				+"InsSubNum   =  "+POut.Long  (patPlan.InsSubNum)+" "
				+"WHERE PatPlanNum = "+POut.Long(patPlan.PatPlanNum)+" LIMIT 1";
			Db.NonQ(command);
		}

		///<summary>Updates one PatPlan in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		internal static void Update(PatPlan patPlan,PatPlan oldPatPlan){
			string command="";
			if(patPlan.PatNum != oldPatPlan.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(patPlan.PatNum)+"";
			}
			if(patPlan.PlanNum != oldPatPlan.PlanNum) {
				if(command!=""){ command+=",";}
				command+="PlanNum = "+POut.Long(patPlan.PlanNum)+"";
			}
			if(patPlan.Ordinal != oldPatPlan.Ordinal) {
				if(command!=""){ command+=",";}
				command+="Ordinal = "+POut.Byte(patPlan.Ordinal)+"";
			}
			if(patPlan.IsPending != oldPatPlan.IsPending) {
				if(command!=""){ command+=",";}
				command+="IsPending = "+POut.Bool(patPlan.IsPending)+"";
			}
			if(patPlan.Relationship != oldPatPlan.Relationship) {
				if(command!=""){ command+=",";}
				command+="Relationship = "+POut.Int   ((int)patPlan.Relationship)+"";
			}
			if(patPlan.PatID != oldPatPlan.PatID) {
				if(command!=""){ command+=",";}
				command+="PatID = '"+POut.String(patPlan.PatID)+"'";
			}
			if(patPlan.InsSubNum != oldPatPlan.InsSubNum) {
				if(command!=""){ command+=",";}
				command+="InsSubNum = "+POut.Long(patPlan.InsSubNum)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE patplan SET "+command
				+" WHERE PatPlanNum = "+POut.Long(patPlan.PatPlanNum)+" LIMIT 1";
			Db.NonQ(command);
		}

		///<summary>Deletes one PatPlan from the database.</summary>
		internal static void Delete(long patPlanNum){
			string command="DELETE FROM patplan "
				+"WHERE PatPlanNum = "+POut.Long(patPlanNum)+" LIMIT 1";
			Db.NonQ(command);
		}

	}
}