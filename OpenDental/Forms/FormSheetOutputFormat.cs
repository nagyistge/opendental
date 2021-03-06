using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetOutputFormat:Form {
		public int PaperCopies;
		public bool EmailPatOrLab;
		public string EmailPatOrLabAddress;
		public bool Email2;
		public string Email2Address;
		public bool Email2Visible;
		public bool IsForLab;

		public FormSheetOutputFormat() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormSheetOutputFormat_Load(object sender,EventArgs e) {
			textPaperCopies.Text=PaperCopies.ToString();
			checkEmailPat.Checked=EmailPatOrLab;
			if(IsForLab) {
				checkEmailPat.Text=Lan.g(this,"E-mail to Lab:");
			}
			textEmailPat.Text=EmailPatOrLabAddress;
			if(Email2Visible){
				checkEmail2.Checked=Email2;
				textEmail2.Text=Email2Address;
			}
			else{
				checkEmail2.Visible=false;
				textEmail2.Visible=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textPaperCopies.errorProvider1.GetError(textPaperCopies)!=""){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(checkEmailPat.Checked && textEmailPat.Text==""){
				MsgBox.Show(this,"Please enter an email address or uncheck the email box.");
				return;
			}
			if(Email2Visible){
				if(checkEmail2.Checked && textEmail2.Text==""){
					MsgBox.Show(this,"Please enter an email address or uncheck the email box.");
					return;
				}
			}
			if(PIn.Long(textPaperCopies.Text)==0
				&& !checkEmailPat.Checked
				&& !checkEmail2.Checked)
			{
				MsgBox.Show(this,"There are no output methods selected.");
				return;
			}
			PaperCopies=PIn.Int(textPaperCopies.Text);
			EmailPatOrLab=checkEmailPat.Checked;
			EmailPatOrLabAddress=textEmailPat.Text;
			if(Email2Visible){
				Email2=checkEmail2.Checked;
				Email2Address=textEmail2.Text;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}