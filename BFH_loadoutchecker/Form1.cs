using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BFH_loadoutchecker
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();
        Font myFont;

        Hashtable activeLoadout;
        ArrayList cops_active_preset;
        ArrayList criminals_active_preset;

        public Form1()
        {
            InitializeComponent();
            byte[] fontData = Properties.Resources.FrutigerLTStd_Bold;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.FrutigerLTStd_Bold.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.FrutigerLTStd_Bold.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            myFont = new Font(fonts.Families[0], 8.0F);

            // Retrieve en_US
            if (Language.en_US == null)
                Language.retrieveNames();
            if (Language.en_US_ID == null || Language.en_US_STRING == null)
                Language.parseLanguage(Language.en_US);
        }

        public bool fetchLoadout(string PlayerName)
        {
            Loadout loadout = new Loadout();
            Hashtable active_loadout = loadout.getActiveKit(PlayerName);
            if (active_loadout == null)
                return false;
            activeLoadout = active_loadout;
            tBox_personaID.Text = loadout.personaID;
            Button btn = (Button)this.Controls.Find("btn_" + loadout.active_Kit.ToString(), true)[0];
            btn.FlatAppearance.BorderSize = 3;
            return true;
        }

        private void btn_fetch_Click(object sender, EventArgs e)
        {
            resetAll();

            if (String.IsNullOrEmpty(tBox_username.Text))
                return;
            resetKitButtons();
            if (fetchLoadout(tBox_username.Text))
            {
                parseCOPS(activeLoadout);
                parseCRIMINALS(activeLoadout);
            }
        }

        private void parseCOPS(Hashtable active_loadout)
        {
            // Check which preset is active
            Hashtable Faction = (Hashtable)((ArrayList)active_loadout["factions"])[0];
            ArrayList presets = (ArrayList)Faction["presets"];
            int active_preset = 0;
            foreach (Hashtable htable in presets)
            {
                if ((Boolean)htable["isActive"])
                    break;
                active_preset++;
            }

            // Parse slots
            ArrayList slots = (ArrayList)((Hashtable)presets[active_preset])["slots"];
            cops_active_preset = slots;

            int slot_index = 0;
            foreach (Hashtable htable in slots)
            {
                // Get current slot
                Hashtable slot = (Hashtable)slots[slot_index];

                // Set labels
                Label lbl = (Label)this.Controls.Find("lbl_cops_" + slot_index, true)[0];
                lbl.Text = Language.getString(slot["name"].ToString());
                lbl.Visible = true;

                // Set textboxes
                Hashtable item = (Hashtable)slot["item"];
                String id = item["id"].ToString();
                String name = item["name"].ToString();
                TextBox tBox = (TextBox)this.Controls.Find("tBox_cops_" + slot_index, true)[0];
                tBox.Text = String.Format("{0} | {1}", Language.getString(name), id);
                tBox.Tag = slot_index;
                tBox.Visible = true;

                // Raise the index
                slot_index++;
            }
            gBox_COPS.Visible = true;
        }

        private void parseCRIMINALS(Hashtable active_loadout)
        {
            // Check which preset is active
            Hashtable Faction = (Hashtable)((ArrayList)active_loadout["factions"])[1];
            ArrayList presets = (ArrayList)Faction["presets"];
            int active_preset = 0;
            foreach (Hashtable htable in presets)
            {
                if ((Boolean)htable["isActive"])
                    break;
                active_preset++;
            }

            // Parse slots
            ArrayList slots = (ArrayList)((Hashtable)presets[active_preset])["slots"];
            criminals_active_preset = slots;

            int slot_index = 0;
            foreach (Hashtable htable in slots)
            {
                // Get current slot
                Hashtable slot = (Hashtable)slots[slot_index];

                // Set labels
                Label lbl = (Label)this.Controls.Find("lbl_criminals_" + slot_index, true)[0];
                lbl.Text = Language.getString(slot["name"].ToString());
                lbl.Visible = true;

                // Set textboxes
                Hashtable item = (Hashtable)slot["item"];
                String id = item["id"].ToString();
                String name = item["name"].ToString();
                TextBox tBox = (TextBox)this.Controls.Find("tBox_criminals_" + slot_index, true)[0];
                tBox.Text = String.Format("{0} | {1}", Language.getString(name), id);
                tBox.Tag = slot_index;
                tBox.Visible = true;

                // Raise the index
                slot_index++;
            }
            gBox_CRIMINALS.Visible = true;
        }

        private void cops_item_Click(object sender, EventArgs e)
        {
            if (cops_active_preset == null)
                return;

            resetCOPS_A();

            TextBox TB = (TextBox)sender;

            // Get accessories
            Hashtable item = (Hashtable)((Hashtable)cops_active_preset[(int)TB.Tag])["item"];
            ArrayList accessories = (ArrayList)item["accessories"];
            if (accessories == null || accessories.Count < 1)
                return;

            int slot_index = 0;
            foreach (Hashtable htable in accessories)
            {
                // Get current acceesory
                Hashtable index = (Hashtable)accessories[slot_index];

                // Set labels
                Label lbl = (Label)this.Controls.Find("lbl_cops_a_" + slot_index, true)[0];
                lbl.Text = Language.getString(index["name"].ToString());
                lbl.Visible = true;

                // Set textboxes
                Hashtable _item = (Hashtable)index["item"];
                String id = _item["id"].ToString();
                String name = _item["name"].ToString();
                if(index["name"].ToString() == "BFH_ID_P_CAT_OPTIC")
                {
                    if (name == "BFH_ID_P_ANAME_ATNTHOR") { lbl_cops_flir.Text = "FLIR"; lbl_cops_flir.ForeColor = System.Drawing.Color.Crimson; } else { lbl_cops_flir.Text = "No FLIR"; lbl_cops_flir.ForeColor = System.Drawing.Color.SeaGreen; }
                    if (name == "BFH_ID_P_ANAME_IRNV") { lbl_cops_irnv.Text = "IRNV"; lbl_cops_irnv.ForeColor = System.Drawing.Color.Crimson; } else { lbl_cops_irnv.Text = "No IRNV"; lbl_cops_irnv.ForeColor = System.Drawing.Color.SeaGreen; }
                }
                TextBox tBox = (TextBox)this.Controls.Find("tBox_cops_a_" + slot_index, true)[0];
                tBox.Text = String.Format("{0} | {1}", Language.getString(name), id);
                tBox.Tag = slot_index;
                tBox.Visible = true;

                // Raise the index
                slot_index++;
            }
            gBox_cops_accessories.Visible = true;
        }

        private void criminals_item_Click(object sender, EventArgs e)
        {
            if (criminals_active_preset == null)
                return;

            resetCRIMINALS_A();

            TextBox TB = (TextBox)sender;

            // Get accessories
            Hashtable item = (Hashtable)((Hashtable)criminals_active_preset[(int)TB.Tag])["item"];
            ArrayList accessories = (ArrayList)item["accessories"];
            if (accessories == null || accessories.Count < 1)
                return;

            int slot_index = 0;
            foreach (Hashtable htable in accessories)
            {
                // Get current acceesory
                Hashtable index = (Hashtable)accessories[slot_index];

                // Set labels
                Label lbl = (Label)this.Controls.Find("lbl_criminals_a_" + slot_index, true)[0];
                lbl.Text = Language.getString(index["name"].ToString());
                lbl.Visible = true;

                // Set textboxes
                Hashtable _item = (Hashtable)index["item"];
                String id = _item["id"].ToString();
                String name = _item["name"].ToString();
                if (index["name"].ToString() == "BFH_ID_P_CAT_OPTIC")
                {
                    if (name == "BFH_ID_P_ANAME_ATNTHOR") { lbl_criminals_flir.Text = "FLIR"; lbl_criminals_flir.ForeColor = System.Drawing.Color.Crimson; } else { lbl_criminals_flir.Text = "No FLIR"; lbl_criminals_flir.ForeColor = System.Drawing.Color.SeaGreen; }
                    if (name == "BFH_ID_P_ANAME_IRNV") { lbl_criminals_irnv.Text = "IRNV"; lbl_criminals_irnv.ForeColor = System.Drawing.Color.Crimson; } else { lbl_criminals_irnv.Text = "No IRNV"; lbl_criminals_irnv.ForeColor = System.Drawing.Color.SeaGreen; }
                }
                TextBox tBox = (TextBox)this.Controls.Find("tBox_criminals_a_" + slot_index, true)[0];
                tBox.Text = String.Format("{0} | {1}", Language.getString(name), id);
                tBox.Tag = slot_index;
                tBox.Visible = true;

                // Raise the index
                slot_index++;
            }
            gBox_criminals_accessories.Visible = true;
        }

        private void resetAll()
        {
            resetCOPS();
            resetCRIMINALS();
            resetCOPS_A();
            resetCRIMINALS_A();
        }

        private void resetCOPS()
        {
            for (int i = 0; i < 7; i++) // Reset labels and textboxes
            {
                Label lbl = (Label)this.Controls.Find("lbl_cops_" + i, true)[0];
                lbl.Visible = false;
                lbl.Text = "empty";

                TextBox tBox = (TextBox)this.Controls.Find("tBox_cops_" + i, true)[0];
                tBox.Visible = false;
                tBox.Text = String.Empty;
            }
            gBox_COPS.Visible = false;
        }

        private void resetCRIMINALS()
        {
            for (int i = 0; i < 7; i++) // Reset labels and textboxes
            {
                Label lbl = (Label)this.Controls.Find("lbl_criminals_" + i, true)[0];
                lbl.Visible = false;
                lbl.Text = "empty";

                TextBox tBox = (TextBox)this.Controls.Find("tBox_criminals_" + i, true)[0];
                tBox.Visible = false;
                tBox.Text = String.Empty;
            }
            gBox_CRIMINALS.Visible = false;
        }

        private void resetCOPS_A()
        {
            for (int i = 0; i < 5; i++) // Reset labels and textboxes
            {
                Label lbl = (Label)this.Controls.Find("lbl_cops_a_" + i, true)[0];
                lbl.Visible = false;
                lbl.Text = "empty";

                TextBox tBox = (TextBox)this.Controls.Find("tBox_cops_a_" + i, true)[0];
                tBox.Visible = false;
                tBox.Text = String.Empty;
            }
            gBox_cops_accessories.Visible = false;
        }

        private void resetCRIMINALS_A()
        {
            for (int i = 0; i < 5; i++) // Reset labels and textboxes
            {
                Label lbl = (Label)this.Controls.Find("lbl_criminals_a_" + i, true)[0];
                lbl.Visible = false;
                lbl.Text = "empty";

                TextBox tBox = (TextBox)this.Controls.Find("tBox_criminals_a_" + i, true)[0];
                tBox.Visible = false;
                tBox.Text = String.Empty;
            }
            gBox_criminals_accessories.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add font to every control
            //foreach (Control c in this.Controls)
            //{
            //    c.Font = myFont;
            //}
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tool created by xfileFIN!");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Step 1: Write your Battlelog username into 'Battlelog Username:' -TextBox" + Environment.NewLine +
                "Step 2: Click 'Fetch Information' -Button" + Environment.NewLine +
                "Step 3 (Optional): Click the TextBoxes to get their accessories listed (If any)", 
                "Help!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool fetchWantedLoadout(string PlayerName, int kit_index)
        {
            Loadout loadout = new Loadout();
            Hashtable active_loadout = loadout.getWantedKit(PlayerName, kit_index);
            if (active_loadout == null)
                return false;
            activeLoadout = active_loadout;
            tBox_personaID.Text = loadout.personaID;
            return true;
        }

        private void btn_kit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if(btn == btn_OPERATOR)
            {
                resetAll();
                if (String.IsNullOrEmpty(tBox_username.Text))
                    return;

                if (fetchWantedLoadout(tBox_username.Text, 0))
                {
                    resetKitButtons();
                    parseCOPS(activeLoadout);
                    parseCRIMINALS(activeLoadout);
                    btn_OPERATOR.FlatAppearance.BorderSize = 3;
                }
            }
            else if(btn == btn_MECHANIC)
            {
                resetAll();
                if (String.IsNullOrEmpty(tBox_username.Text))
                    return;

                if (fetchWantedLoadout(tBox_username.Text, 1))
                {
                    resetKitButtons();
                    parseCOPS(activeLoadout);
                    parseCRIMINALS(activeLoadout);
                    btn_MECHANIC.FlatAppearance.BorderSize = 3;
                }
            }
            else if (btn == btn_ENFORCER)
            {
                resetAll();
                if (String.IsNullOrEmpty(tBox_username.Text))
                    return;

                if (fetchWantedLoadout(tBox_username.Text, 2))
                {
                    resetKitButtons();
                    parseCOPS(activeLoadout);
                    parseCRIMINALS(activeLoadout);
                    btn_ENFORCER.FlatAppearance.BorderSize = 3;
                }
            }
            else if (btn == btn_PROFESSIONAL)
            {
                resetAll();
                if (String.IsNullOrEmpty(tBox_username.Text))
                    return;

                if (fetchWantedLoadout(tBox_username.Text, 3))
                {
                    resetKitButtons();
                    parseCOPS(activeLoadout);
                    parseCRIMINALS(activeLoadout);
                    btn_PROFESSIONAL.FlatAppearance.BorderSize = 3;
                }
            }
        }

        private void resetKitButtons()
        {
            btn_OPERATOR.FlatAppearance.BorderSize = 1;
            btn_MECHANIC.FlatAppearance.BorderSize = 1;
            btn_ENFORCER.FlatAppearance.BorderSize = 1;
            btn_PROFESSIONAL.FlatAppearance.BorderSize = 1;
        }
    }
}
