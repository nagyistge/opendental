using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAnestheticMedsInventory:Form {

        private List<AnesthMed> listAnestheticMeds;

		public FormAnestheticMedsInventory() {
			InitializeComponent();
			Lan.F(this);
            FillGrid();
		}


        private void FormAnestheticMedsInventory_Load(object sender, System.EventArgs e)
        {

            FillGrid();
        }

        private void FillGrid()
        {

            listAnestheticMeds = AnestheticMeds.CreateObjects();
            gridAnesthMedsInventory.BeginUpdate();
            gridAnesthMedsInventory.Columns.Clear();
            ODGridColumn col = new ODGridColumn(Lan.g(this, "Anesthetic Medication"), 200);
            gridAnesthMedsInventory.Columns.Add(col);
            col = new ODGridColumn(Lan.g(this, "How Supplied"), 200);
            gridAnesthMedsInventory.Columns.Add(col);
            col = new ODGridColumn(Lan.g(this, "Quantity on Hand"), 180);
            gridAnesthMedsInventory.Columns.Add(col);
            gridAnesthMedsInventory.Rows.Clear();
            ODGridRow row;
            for (int i = 0; i < listAnestheticMeds.Count; i++)
            {
                row = new ODGridRow();
                row.Cells.Add(listAnestheticMeds[i].AnesthMedName);
                row.Cells.Add(listAnestheticMeds[i].AnesthHowSupplied);
                row.Cells.Add(listAnestheticMeds[i].QtyOnHand);
                gridAnesthMedsInventory.Rows.Add(row);
            }
            gridAnesthMedsInventory.EndUpdate();

        }

        private void butAddAnesthMeds_Click(object sender, EventArgs e)
        {
            AnesthMed med = new AnesthMed();
            med.IsNew = true;
            FormAnestheticMedsEdit FormME = new FormAnestheticMedsEdit();
            FormME.Med = med;
            FormME.ShowDialog();
            if (FormME.DialogResult == DialogResult.OK)
            {
                FillGrid();
            }
        }
		private void butOK_Click(object sender,EventArgs e) {
			
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

        private void gridAnesthMedsInventory_CellDoubleClick(object sender, ODGridClickEventArgs e)
        {
            FormAnestheticMedsEdit FormME = new FormAnestheticMedsEdit();
            FormME.Med = listAnestheticMeds[e.Row];
            FormME.ShowDialog();
            if (FormME.DialogResult == DialogResult.OK)
            {
                FillGrid();
            }
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }


	}
}