using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSupplyInventory:Form {
		private List<SupplyNeeded> listNeeded;
		private List<Supply> listSupply;
		private List<Supplier> listSupplier;

		public FormSupplyInventory() {
			InitializeComponent();
			Lan.F(this);
		}

		private void FormInventory_Load(object sender,EventArgs e) {
			FillGridNeeded();
			FillSuppliers();
			if(comboSupplier.Items.Count>0){
				comboSupplier.SelectedIndex=0;
			}
			FillGridSupply();
		}

		private void FillSuppliers(){
			listSupplier=Suppliers.CreateObjects();
			comboSupplier.Items.Clear();
			for(int i=0;i<listSupplier.Count;i++){
				comboSupplier.Items.Add(listSupplier[i].Name);
			}
		}

		private void FillGridNeeded(){
			listNeeded=SupplyNeededs.CreateObjects();
			gridNeeded.BeginUpdate();
			gridNeeded.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Date Added"),90);
			gridNeeded.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Description"),300);
			gridNeeded.Columns.Add(col);
			gridNeeded.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<listNeeded.Count;i++){
				row=new ODGridRow();
				row.Cells.Add(listNeeded[i].DateAdded.ToShortDateString());
				row.Cells.Add(listNeeded[i].Description);
				gridNeeded.Rows.Add(row);
			}
			gridNeeded.EndUpdate();
		}

		private void gridNeeded_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormSupplyNeededEdit FormS=new FormSupplyNeededEdit();
			FormS.Supp=listNeeded[e.Row];
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				FillGridNeeded();
			}
		}

		private void butAddNeeded_Click(object sender,EventArgs e) {
			SupplyNeeded supp=new SupplyNeeded();
			supp.IsNew=true;
			supp.DateAdded=DateTime.Today;
			FormSupplyNeededEdit FormS=new FormSupplyNeededEdit();
			FormS.Supp=supp;
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK){
				FillGridNeeded();
			}
		}

		private void FillGridSupply(){
			int supplier=0;
			if(comboSupplier.SelectedIndex!=-1){
				supplier=listSupplier[comboSupplier.SelectedIndex].SupplierNum;
			}
			listSupply=Supplies.CreateObjects(checkShowHidden.Checked,supplier);
			gridSupply.BeginUpdate();
			gridSupply.Columns.Clear();
			ODGridColumn col=new ODGridColumn(Lan.g(this,"Category"),100);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Catalog #"),60);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Catalog Description"),190);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Common Name"),140);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Unit"),50);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Level"),40,HorizontalAlignment.Right);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Price"),50,HorizontalAlignment.Right);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Note"),170);
			gridSupply.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Hidden"),40,HorizontalAlignment.Center);
			gridSupply.Columns.Add(col);
			gridSupply.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<listSupply.Count;i++){
				row=new ODGridRow();
				if(i==0 || listSupply[i].Category!=listSupply[i-1].Category){
					row.Cells.Add(DefB.GetName(DefCat.SupplyCats,listSupply[i].Category));
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(listSupply[i].CatalogNumber);
				row.Cells.Add(listSupply[i].CatalogDescript);
				row.Cells.Add(listSupply[i].CommonName);
				row.Cells.Add(listSupply[i].UnitType);
				if(listSupply[i].LevelDesired==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(listSupply[i].LevelDesired.ToString());
				}
				if(listSupply[i].Price==0){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(listSupply[i].Price.ToString("n"));
				}
				row.Cells.Add(listSupply[i].Note);
				if(listSupply[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				gridSupply.Rows.Add(row);
			}
			gridSupply.EndUpdate();
		}

		private void gridSupply_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			FormSupplyEdit FormS=new FormSupplyEdit();
			FormS.Supp=listSupply[e.Row];
			FormS.ListSupplier=listSupplier;
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				FillGridSupply();
			}
		}

		private void butAddSupply_Click(object sender,EventArgs e) {
			if(listSupplier.Count==0){
				MsgBox.Show(this,"Please add suppliers first.  Use the menu at the top of this window.");
				return;
			}
			if(DefB.Short[(int)DefCat.SupplyCats].Length==0) {
				MsgBox.Show(this,"Please add supply categories first.  Use the menu at the top of this window.");
				return;
			}
			if(comboSupplier.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a supplier first.");
				return;
			}
			Supply supp=new Supply();
			supp.IsNew=true;
			supp.SupplierNum=listSupplier[comboSupplier.SelectedIndex].SupplierNum;
			FormSupplyEdit FormS=new FormSupplyEdit();
			FormS.Supp=supp;
			FormS.ListSupplier=listSupplier;
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				FillGridSupply();
			}
		}

		private void checkShowHidden_Click(object sender,EventArgs e) {
			FillGridSupply();
		}

		private void comboSupplier_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridSupply();
		}

		private void menuItemSuppliers_Click(object sender,EventArgs e) {
			FormSuppliers FormS=new FormSuppliers();
			FormS.ShowDialog();
			FillSuppliers();
			FillGridSupply();//clears main supply grid.
		}

		private void menuItemCategories_Click(object sender,EventArgs e) {
			FormDefinitions FormD=new FormDefinitions(DefCat.SupplyCats);
			FormD.ShowDialog();
			FillGridSupply();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	

		

		

		

		

		

		

		

		
	}
}