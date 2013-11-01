﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing.Printing;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEhrQualityMeasureEdit2014:Form {
		public QualityMeasure Qcur;
		private DataTable table;
		public DateTime DateStart;
		public DateTime DateEnd;
		public long ProvNum;

		public FormEhrQualityMeasureEdit2014() {
			InitializeComponent();
		}

		private void FormQualityEdit2014_Load(object sender,EventArgs e) {
			string exceptNALabel="Exceptions.";
			string exceptLabel=exceptNALabel+"  Subtracted from the denominator only if not in the numerator.";
			string exclusNALabel="Exclusions.";
			string exclusLabel=exclusNALabel+"  Subtracted from the denominator before applying numerator criteria.";
			textId.Text=Qcur.Id;
			textDescription.Text=Qcur.Descript;
			FillGrid();
			textDenominator.Text=Qcur.Denominator.ToString();
			textNumerator.Text=Qcur.Numerator.ToString();
			//Our 9 measures only have an exclusion or an exception, not both.  Use the same explain text box for either explanation and fill the appropriate text box and label.
			if(Qcur.ExclusionsExplain.ToString()=="N/A") {//No exclusions, only exceptions are possible, may be N/A as well
				//top box is exception, bottom box is exclusion, 
				textExclusExceptExplain.Text=Qcur.ExceptionsExplain.ToString();
				labelExclusExceptNA.Text=exclusNALabel;
				if(Qcur.ExceptionsExplain.ToString()=="N/A") {//also no exceptions
					textExclusExcept.Text="None";
					labelExclusExcept.Text=exceptNALabel;
					textExclusExcept.BackColor=System.Drawing.SystemColors.Control;
				}
				else {
					textExclusExcept.Text=Qcur.Exceptions.ToString();
					labelExclusExcept.Text=exceptLabel;
					textExclusExcept.BackColor=System.Drawing.SystemColors.ControlLightLight;
				}
			}
			else {
				//there is a valid exclusion explanation for this measure, top box is exclusion, bottom box is exception
				textExclusExceptExplain.Text=Qcur.ExclusionsExplain.ToString();
				textExclusExcept.Text=Qcur.Exclusions.ToString();
				textExclusExcept.BackColor=System.Drawing.SystemColors.ControlLightLight;
				labelExclusExcept.Text=exclusLabel;
				labelExclusExceptNA.Text=exceptNALabel;
			}
			textNotMet.Text=Qcur.NotMet.ToString();
			//Reporting rate represents the percentage of patients in the denominator who fall into one of the other sub-populations. Rate=(Numerator + Exclusions + Exceptions)/(Denominator)
			textReportingRate.Text=Qcur.ReportingRate.ToString()+"%";
			textPerformanceRate.Text=Qcur.Numerator.ToString()+"/"+(Qcur.Numerator+Qcur.NotMet).ToString()
					+"  = "+Qcur.PerformanceRate.ToString()+"%";
			textDenominatorExplain.Text=Qcur.DenominatorExplain;
			textNumeratorExplain.Text=Qcur.NumeratorExplain;
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			ODGridColumn col=new ODGridColumn("PatNum",50);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Patient Name",140);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Numerator",65,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Exclusion",60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Exception",60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new ODGridColumn("Explanation",140);
			gridMain.Columns.Add(col);
			table=QualityMeasures.GetTable2014(Qcur.Type2014,DateStart,DateEnd,ProvNum);
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<table.Rows.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(table.Rows[i]["PatNum"].ToString());
				row.Cells.Add(table.Rows[i]["patientName"].ToString());
				row.Cells.Add(table.Rows[i]["numerator"].ToString());
				row.Cells.Add(table.Rows[i]["exclusion"].ToString());
				row.Cells.Add(table.Rows[i]["exception"].ToString());
				row.Cells.Add(table.Rows[i]["explanation"].ToString());
				//if(table.Rows[i]["met"].ToString()=="X") {
				//	row.ColorBackG=Color.LightGreen;
				//}
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}

	

		

		

	}
}