﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using OpenDentBusiness;

namespace OpenDental{
	public class CacheL {
		public static void Refresh(InvalidTypes itypes){
			DataSet ds=Gen.GetDS(MethodNameDS.Cache_Refresh,itypes);
			if((itypes & InvalidTypes.AutoCodesProcButtons)==InvalidTypes.AutoCodesProcButtons){
				//AutoCodeL.Refresh();
				//AutoCodeItemL.Refresh();
				//AutoCodeCondL.Refresh();
				//ProcButtons.Refresh();
				//ProcButtonItems.Refresh();
			}
			if((itypes & InvalidTypes.Carriers)==InvalidTypes.Carriers){
				//Carriers.Refresh();//run on startup, after telephone reformat, after list edit.
			}
			if((itypes & InvalidTypes.ClaimForms)==InvalidTypes.ClaimForms){
				//ClaimFormItemL.Refresh();
				//ClaimForms.Refresh();
			}
			if((itypes & InvalidTypes.ClearHouses)==InvalidTypes.ClearHouses){
				//kh until we add an EasyHideClearHouses						Clearinghouses.Refresh();
				//SigElementDefs.Refresh();
				//SigButDefs.Refresh();//includes SigButDefElements.Refresh()
			}
			if((itypes & InvalidTypes.Computers)==InvalidTypes.Computers){
				//Computers.Refresh();
				//Printers.Refresh();
			}
			if((itypes & InvalidTypes.Defs)==InvalidTypes.Defs){
				Defs.FillCache(ds.Tables["Def"]);
			}
			if((itypes & InvalidTypes.DentalSchools)==InvalidTypes.DentalSchools){
				//SchoolClasses.Refresh();
				//SchoolCourses.Refresh();
			}
			if((itypes & InvalidTypes.Email)==InvalidTypes.Email){
				//EmailTemplates.Refresh();
				//DiseaseDefs.Refresh();
			}
			if((itypes & InvalidTypes.Employees)==InvalidTypes.Employees){
				//Employees.Refresh();
				//PayPeriods.Refresh();
			}
			if((itypes & InvalidTypes.Fees)==InvalidTypes.Fees){
				//Fees.Refresh();
			}
			if((itypes & InvalidTypes.InsCats)==InvalidTypes.InsCats){
				//CovCatL.Refresh();
				//CovSpanL.Refresh();
				DisplayFields.FillCache(ds.Tables["DisplayField"]);;
			}
			if((itypes & InvalidTypes.Letters)==InvalidTypes.Letters){
				//Letters.Refresh();
			}
			if((itypes & InvalidTypes.LetterMerge)==InvalidTypes.LetterMerge){
				//LetterMergeFields.Refresh();
				//LetterMerges.Refresh();
			}
			if((itypes & InvalidTypes.Operatories)==InvalidTypes.Operatories){
				Operatories.FillCache(ds.Tables["Operatory"]);
				//AccountingAutoPayL.Refresh();
			}
			if((itypes & InvalidTypes.Prefs)==InvalidTypes.Prefs){
				Prefs.FillCache(ds.Tables["Pref"]);
			}
			if((itypes & InvalidTypes.ProcCodes)==InvalidTypes.ProcCodes){
				//ProcedureCodes.Refresh();
				//ProcCodeNotes.Refresh();
			}
			if((itypes & InvalidTypes.Programs)==InvalidTypes.Programs){
				//Programs.Refresh();
				//ProgramProperties.Refresh();
			}
			if((itypes & InvalidTypes.Providers)==InvalidTypes.Providers){
				Providers.FillCache(ds.Tables["Provider"]);
			}
			if((itypes & InvalidTypes.QuickPaste)==InvalidTypes.QuickPaste){
				//QuickPasteNotes.Refresh();
				//QuickPasteCats.Refresh();
			}
			if((itypes & InvalidTypes.Security)==InvalidTypes.Security){
				Userods.FillCache(ds.Tables["Userod"]);
			}
			if((itypes & InvalidTypes.Startup)==InvalidTypes.Startup){
				//Employers.Refresh();//only needed when opening the prog. After that, automated.
				//ElectIDs.Refresh();//only run on startup
				//Referrals.Refresh();//Referrals are also refreshed dynamically.
			}
			//InvalidTypes.Tasks not handled here.
			if((itypes & InvalidTypes.ToolBut)==InvalidTypes.ToolBut){
				//ToolButItems.Refresh();
			}
			if((itypes & InvalidTypes.Views)==InvalidTypes.Views){
				ApptViews.FillCache(ds.Tables["ApptView"]);
				ApptViewItems.FillCache(ds.Tables["ApptViewItem"]);
			}
			if((itypes & InvalidTypes.ZipCodes)==InvalidTypes.ZipCodes){
				//ZipCodes.Refresh();
				//PatFieldDefs.Refresh();
			}




		}

	}
}
