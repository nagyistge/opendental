﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using OpenDentBusiness;

namespace OpenDental{
	public class ApptViewItemL{
		///<summary>A list of the ApptViewItems for the current view.</summary>
		public static List<ApptViewItem> ForCurView;
		//these two are subsets of provs and ops. You can't include hidden prov or op in this list.
		///<summary>Visible provider bars in appt module.  List of indices to ProviderC.List(short).  Also see VisOps.  This is a subset of the available provs.  You can't include a hidden prov in this list.</summary>
		public static List<int> VisProvs;
		///<summary>Visible ops in appt module.  List of indices to Operatories.ListShort[ops].  Also see VisProvs.  This is a subset of the available ops.  You can't include a hidden op in this list.</summary>
		public static List<int> VisOps;
		///<summary>Subset of ForCurView. Just items for rowElements. If no view is selected, then the elements are filled with default info.</summary>
		public static List<ApptViewItem> ApptRows;

		public static void GetForCurView(int indexInList,bool isWeekly){
			if(indexInList<0){//might be -1 or -2
				GetForCurView(new ApptView(),isWeekly);
			}
			else{
				GetForCurView(ApptViewC.List[indexInList],isWeekly);
			}
		}

		///<summary>Gets (list)ForCurView, VisOps, VisProvs, and ApptRows.  Also sets TwoRows. Works even if supply -1 to indicate no apptview is selected.</summary>
		public static void GetForCurView(ApptView ApptViewCur,bool isWeekly){
			ForCurView=new List<ApptViewItem>();
			VisProvs=new List<int>();
			VisOps=new List<int>();
			ApptRows=new List<ApptViewItem>();
			//ArrayList ALprov=new ArrayList();
			//ArrayList ALops=new ArrayList();
			//ArrayList ALelements=new ArrayList();
			if(ApptViewCur.ApptViewNum==0){
				//MessageBox.Show("apptcategorynum:"+ApptCategories.Cur.ApptCategoryNum.ToString());
				//make visible ops exactly the same as the short ops list (all except hidden)
				for(int i=0;i<OperatoryC.ListShort.Count;i++){
					VisOps.Add(i);
				}
				//make visible provs exactly the same as the prov list (all except hidden)
				for(int i=0;i<ProviderC.List.Length;i++){
					VisProvs.Add(i);
				}
				//Hard coded elements showing
				ApptRows.Add(new ApptViewItem("PatientName",0,Color.Black));
				ApptRows.Add(new ApptViewItem("ASAP",1,Color.DarkRed));
				ApptRows.Add(new ApptViewItem("MedUrgNote",2,Color.DarkRed));
				ApptRows.Add(new ApptViewItem("PremedFlag",3,Color.DarkRed));
				ApptRows.Add(new ApptViewItem("Lab",4,Color.DarkRed));
				ApptRows.Add(new ApptViewItem("Procs",5,Color.Black));
				ApptRows.Add(new ApptViewItem("Note",6,Color.Black));
				ContrApptSheet.RowsPerIncr=1;
			}
			else{
				int index;
				for(int i=0;i<ApptViewItemC.List.Length;i++){
					if(ApptViewItemC.List[i].ApptViewNum==ApptViewCur.ApptViewNum){
						ForCurView.Add(ApptViewItemC.List[i]);
						if(ApptViewItemC.List[i].OpNum>0){//op
							index=Operatories.GetOrder(ApptViewItemC.List[i].OpNum);
							if(index!=-1){
								VisOps.Add(index);
							}
						}
						else if(ApptViewItemC.List[i].ProvNum>0){//prov
							index=Providers.GetIndex(ApptViewItemC.List[i].ProvNum);
							if(index!=-1){
								VisProvs.Add(index);
							}
						}
						else{//element
							ApptRows.Add(ApptViewItemC.List[i]);
						}
					}
				}
				ContrApptSheet.RowsPerIncr=ApptViewCur.RowsPerIncr;
			}
			VisOps.Sort();
			VisProvs.Sort();
			//ApptRows.Sort();
			/*
			ApptViewItemL.VisOps=new int[ALops.Count];
			for(int i=0;i<ALops.Count;i++){
				ApptViewItemL.VisOps[i]=(int)ALops[i];
			}
			Array.Sort(ApptViewItemL.VisOps);
			ApptViewItemL.VisProvs=new int[ALprov.Count];
			for(int i=0;i<ALprov.Count;i++){
				ApptViewItemL.VisProvs[i]=(int)ALprov[i];
			}
			Array.Sort(ApptViewItemL.VisProvs);
			ApptViewItemL.ApptRows=new ApptViewItem[ALelements.Count];
			for(int i=0;i<ALelements.Count;i++){
				ApptViewItemL.ApptRows[i]=(ApptViewItem)ALelements[i];
			}*/
		}

		///<summary>Returns the index of the provNum within VisProvs.</summary>
		public static int GetIndexProv(int provNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<VisProvs.Count;i++) {
				if(ProviderC.List[VisProvs[i]].ProvNum==provNum)
					return i;
			}
			return -1;
		}

		///<summary>Only used in FormApptViewEdit. Must have run GetForCurView first.</summary>
		public static bool OpIsInView(int opNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<ForCurView.Count;i++) {
				if(ForCurView[i].OpNum==opNum)
					return true;
			}
			return false;
		}

		///<summary>Only used in ApptViewItem setup. Must have run GetForCurView first.</summary>
		public static bool ProvIsInView(int provNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<ForCurView.Count;i++) {
				if(ForCurView[i].ProvNum==provNum)
					return true;
			}
			return false;
		}

		///<summary>Returns the index of the opNum within VisOps.  Returns -1 if not in visOps.</summary>
		public static int GetIndexOp(int opNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<VisOps.Count;i++) {
				if(OperatoryC.ListShort[VisOps[i]].OperatoryNum==opNum)
					return i;
			}
			return -1;
		}



	}
}
