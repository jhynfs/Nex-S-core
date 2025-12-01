using MongoDB.Driver;
using MongoDB.Bson;
using NexScoreAdmin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NexScore.Models;
using NexScore.Helpers;

namespace NexScore
{
    public partial class LogInForm : Form
    {
        private readonly IMongoCollection<AdminModel> _adminCollection = Database.Admins;
        private AdminModel? _admin;
        private bool isPasswordVisible = false;
        private bool isNewPinVisible = false;
        private bool isConfirmPinVisible = false;

        public LogInForm()
        {
            InitializeComponent();

            txtPin.Text = "Enter new PIN";
            txtRePin.Text = "Confirm PIN";
            txtLoginPin.Text = "PIN";

            txtPin.ForeColor = Color.Gray;
            txtRePin.ForeColor = Color.Gray;
            txtLoginPin.ForeColor = Color.Gray;

            txtPin.Enter += TextBox_Enter;
            txtPin.Leave += TextBox_Leave;

            txtRePin.Enter += TextBox_Enter;
            txtRePin.Leave += TextBox_Leave;

            txtLoginPin.Enter += TextBox_Enter;
            txtLoginPin.Leave += TextBox_Leave;

            txtPin.KeyPress += DigitOnly_KeyPress;
            txtRePin.KeyPress += DigitOnly_KeyPress;
            txtLoginPin.KeyPress += DigitOnly_KeyPress;


            isPasswordVisible = false;

            btn_EnterShow.Visible = false; // blue eye hidden
            btn_EnterHide.Visible = true;  // black eye shown

            btn_NewShow1.Visible = false;  // blue hidden
            btn_NewHide1.Visible = true;   // black shown

            btn_NewShow2.Visible = false;  // blue hidden
            btn_NewHide2.Visible = true;   // black shown

            txtPin.UseSystemPasswordChar = false;
            txtRePin.UseSystemPasswordChar = false;
            txtLoginPin.UseSystemPasswordChar = false;

            this.AcceptButton = btnLogin;

            this.MinimumSize = new Size(820, 480);

            LoadAdmin();

        }

        private void LoadAdmin()
        {
            _admin = _adminCollection.Find(_ => true).FirstOrDefault();

            if (_admin == null)
            {
                // --- First time setup ---
                pnlSetup.Visible = true;
                pnlLogIn.Visible = false;
                this.ActiveControl = btnSave;
            }
            else
            {
                // --- Returning admin ---
                pnlSetup.Visible = false;
                pnlLogIn.Visible = true;
                this.ActiveControl = btnLogin;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (_admin == null)
            {
                MessageBox.Show("No admin record found. Please set up a PIN first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLoginPin.Text))
            {
                MessageBox.Show("PIN.");
                return;
            }

            if (SecurityHelper.HashString(txtLoginPin.Text) == _admin.PinHash)
            {
                var mainform = new MainForm();
                mainform.Show();

                this.Hide();
                mainform.FormClosed += (s, args) => this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect PIN!");
            }
        }

        private void btnForgotPin_Click(object sender, EventArgs e)
        {
            if (_admin == null)
            {
                MessageBox.Show("No admin record found. Please set up a PIN first.");
                return;
            }

            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter your recovery code:", "Forgot PIN", "");

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (SecurityHelper.HashString(input.Trim()) == _admin.RecoveryCodeHash)
            {
                MessageBox.Show("Recovery successful! You can now set a new PIN.");
                _adminCollection.DeleteMany(_ => true);
                _admin = null;
                pnlLogIn.Visible = false;
                pnlSetup.Visible = true;
            }
            else
            {
                MessageBox.Show("Invalid recovery code. Please try again.");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPin.Text) || string.IsNullOrWhiteSpace(txtRePin.Text))
            {
                MessageBox.Show("Please enter and re-enter your PIN.");
                return;
            }

            if (txtPin.Text != txtRePin.Text)
            {
                MessageBox.Show("PINs do not match. Try again.");
                return;
            }

            if (txtPin.Text.Length != 6)
            {
                MessageBox.Show("PIN must be exactly 6 digits long.");
                return;
            }

            if (!txtPin.Text.All(char.IsDigit))
            {
                MessageBox.Show("PIN must contain digits only (0–9).");
                return;
            }

            var recoveryCode = SecurityHelper.GenerateRecoveryCode();

            var newAdmin = new AdminModel
            {
                PinHash = SecurityHelper.HashString(txtPin.Text),
                RecoveryCodeHash = SecurityHelper.HashString(recoveryCode)
            };

            _adminCollection.InsertOne(newAdmin);

            MessageBox.Show($"✅ PIN setup successful!\n\nYour recovery code:\n{recoveryCode}\n\nPlease save this somewhere safe — it’s your only way to reset access.");

            _admin = newAdmin;
            pnlSetup.Visible = false;
            pnlLogIn.Visible = true;
        }


        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == txtPin && tb.Text == "Enter new PIN")
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }
            else if (tb == txtRePin && tb.Text == "Confirm PIN")
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }
            else if (tb == txtLoginPin && tb.Text == "PIN")
            {
                tb.Text = "";
                tb.ForeColor = Color.Black;
            }

            UpdatePasswordVisibility();
            UpdateSignupVisibility();

        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == txtPin && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.UseSystemPasswordChar = false;
                tb.Text = "Enter new PIN";
                tb.ForeColor = Color.Gray;
            }
            else if (tb == txtRePin && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.UseSystemPasswordChar = false;
                tb.Text = "Confirm PIN";
                tb.ForeColor = Color.Gray;
            }
            else if (tb == txtLoginPin && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.UseSystemPasswordChar = false;
                tb.Text = "PIN";
                tb.ForeColor = Color.Gray;
            }

            UpdatePasswordVisibility();
            UpdateSignupVisibility();

        }

        private void btn_NewHide1_Click(object sender, EventArgs e)
        {
            isNewPinVisible = true;
            UpdateSignupVisibility(updateSecond: false);
        }

        private void btn_NewShow1_Click(object sender, EventArgs e)
        {
            isNewPinVisible = false;
            UpdateSignupVisibility(updateSecond: false);
        }

        private void btn_NewHide2_Click(object sender, EventArgs e)
        {
            isConfirmPinVisible = true;
            UpdateSignupVisibility(updateFirst: false);
        }

        private void btn_NewShow2_Click(object sender, EventArgs e)
        {
            isConfirmPinVisible = false;
            UpdateSignupVisibility(updateFirst: false);
        }

        private void btn_EnterHide_Click(object sender, EventArgs e)
        {
            // black eye > show password
            isPasswordVisible = true;
            UpdatePasswordVisibility();
        }

        private void btn_EnterShow_Click(object sender, EventArgs e)
        {
            // blue eye > hide password
            isPasswordVisible = false;
            UpdatePasswordVisibility();
        }

        private void UpdateSignupVisibility(bool updateFirst = true, bool updateSecond = true)
        {
            if (updateFirst)
            {
                bool hasPlaceholder = txtPin.ForeColor == Color.Gray || txtPin.Text == "Enter new PIN";
                txtPin.UseSystemPasswordChar = !isNewPinVisible && !hasPlaceholder;

                btn_NewShow1.Enabled = !hasPlaceholder;
                btn_NewHide1.Enabled = !hasPlaceholder;

                btn_NewShow1.Visible = isNewPinVisible;
                btn_NewHide1.Visible = !isNewPinVisible;
            }

            if (updateSecond)
            {
                bool hasPlaceholder = txtRePin.ForeColor == Color.Gray || txtRePin.Text == "Confirm PIN";
                txtRePin.UseSystemPasswordChar = !isConfirmPinVisible && !hasPlaceholder;

                btn_NewShow2.Enabled = !hasPlaceholder;
                btn_NewHide2.Enabled = !hasPlaceholder;

                btn_NewShow2.Visible = isConfirmPinVisible;
                btn_NewHide2.Visible = !isConfirmPinVisible;
            }
        }

        private void UpdatePasswordVisibility()
        {
            if (txtLoginPin.ForeColor != Color.Gray)
                txtLoginPin.UseSystemPasswordChar = !isPasswordVisible;

            btn_EnterShow.Visible = isPasswordVisible;
            btn_EnterHide.Visible = !isPasswordVisible;
        }

        private void DigitOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

    }
}