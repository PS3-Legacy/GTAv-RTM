using DevExpress.XtraEditors;
using PS3Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTA_V_RTM_By_BISOON
{
    public partial class Blipfrm : XtraForm
    {
        PS3API PS3 = API.PS3;
        public Blipfrm()
        {
            InitializeComponent();
            bool[] blipopt = new bool[2] { false, false };
            string blipName = "", blipColor = "";
            foreach (var blip in Enum.GetNames(typeof(Blip.Blips)))
            {
                blip_blipsComboBox.Properties.Items.Add(blip);
            }
            blip_blipsComboBox.SelectedIndexChanged += delegate
            {
                blipName = blip_blipsComboBox.Text;
                blipopt[0] = true; if (blipopt[0] && blipopt[1])
                    this.Close();
            };
            blip_colorComboBox.SelectedIndexChanged += delegate
            {
                blipColor = blip_colorComboBox.Text;
                blipopt[1] = true;
                if (blipopt[0] && blipopt[1])
                    this.Close();
            };

            FormClosing += (b, cl) =>
            {
                Program.main.Thread(delegate
                {
                    Program.main.bliptoAimedEntityBtn.Enabled = false;
                    PS3.InitTarget();
                    int blip = Program.main.scripts.Blip.ADD_CUSTOM_BLIP_TO_ENTITY(Program.main.aimedBlip, (Blip.BlipColor)Enum.Parse(typeof(Blip.BlipColor), blipColor), (Blip.Blips)Enum.Parse(typeof(Blip.Blips), blipName), 200, 1f);
                    if (blip != 0)
                        Program.main.AddCreatedBlip(blip, blipName, blipColor);
                    Program.main.bliptoAimedEntityBtn.Enabled = true;
                    blip_colorComboBox.SelectedIndex = -1;
                    blip_blipsComboBox.SelectedIndex = -1;
                    Program.main.aimedBlip = 0;
                });
                blipopt[0] = false;
                blipopt[1] = false;
            };
        }
    }
}
