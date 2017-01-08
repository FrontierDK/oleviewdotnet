﻿//    This file is part of OleViewDotNet.
//    Copyright (C) James Forshaw 2014
//
//    OleViewDotNet is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    OleViewDotNet is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with OleViewDotNet.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace OleViewDotNet
{
    public partial class ConfigureSymbolsForm : Form
    {
        public ConfigureSymbolsForm()
        {
            InitializeComponent();
            textBoxDbgHelp.Text = Environment.Is64BitProcess ?
                Properties.Settings.Default.DbgHelpPath64 :
                Properties.Settings.Default.DbgHelpPath32;
            textBoxSymbolPath.Text = Properties.Settings.Default.SymbolPath;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "DBGHELP DLL|dbghelp.dll";
                dlg.Multiselect = false;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    textBoxDbgHelp.Text = dlg.FileName;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool valid_dll = false;
            try
            {
                using (SafeLibraryHandle lib = COMUtilities.SafeLoadLibrary(textBoxDbgHelp.Text))
                {
                    valid_dll = true;
                }
            }
            catch(Win32Exception)
            {
            }

            if (!valid_dll)
            {
                MessageBox.Show(this, "Invalid DBGHELP.DLL file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Environment.Is64BitProcess)
            {
                Properties.Settings.Default.DbgHelpPath64 = textBoxDbgHelp.Text;
            }
            else
            {
                Properties.Settings.Default.DbgHelpPath32 = textBoxDbgHelp.Text;
            }
            Properties.Settings.Default.SymbolPath = textBoxSymbolPath.Text;
            Properties.Settings.Default.SymbolsConfigured = true;
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Program.ShowError(this, ex);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
