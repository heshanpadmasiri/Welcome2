using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Welcome
{
    public partial class Dialog : Form
    {
        frmBg baseForm;
        public Dialog(frmBg form)
        {
            this.baseForm = form;
            InitializeComponent(form);
            form.AcceptButton = this.btn_submit;
            this.baseForm.AcceptButton = this.btn_submit;
            this.btn_submit.Click += new EventHandler(btnSubmit_Click);
        }

        private void btnSubmit_Click(object sender,System.EventArgs args)
        {
            //baseForm.playAnimation(txtIndex.Text);
            baseForm.AcceptButton = this.btn_submit;
            this.btn_submit.DialogResult = DialogResult.OK;
            baseForm.updateFrom(txtIndex.Text);
        }



        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_submit.PerformClick();
            }
        }


    }
}
