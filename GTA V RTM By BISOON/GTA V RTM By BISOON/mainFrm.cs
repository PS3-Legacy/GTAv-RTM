using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System.Threading;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using PS3Lib;

namespace GTA_V_RTM_By_BISOON
{
    /*
       Codded By " BISOON "
       this project has  created long time ago
       but i had no time to complete it ..
       i hope you enjoy and learn using this source
      
      Thanks 7aMaD for releasing the tool at his chennel ..
  
      "http://www.youtube.com/c/BISOON"
     "http://www.instagram.com/iBisoon"
     "http://www.youtube.com/c/By7aMaDx"
     
     */

    public partial class mainFrm : XtraForm
    {
        public Scripts scripts = null;
        Dictionary<string, int> clientBlip = new Dictionary<string, int>();
        Dictionary<int, SpawnedVehicle> spawnedveh = new Dictionary<int, SpawnedVehicle>();
        Dictionary<int, SpawnedPed> spawnedped = new Dictionary<int, SpawnedPed>();
        Dictionary<int, AddedBlips> addedBlip = new Dictionary<int, AddedBlips>();
        static PS3API PS3 = API.PS3;
        static SaveLoc myLoc = null;
        public int aimedBlip = 0; 
        float boostLvl = 1f;
        static bool[] boostBolean = new bool[2];
        static bool isOnDE;
        static bool tGun;
        static Handling handle;
        static Icon myicon = LoadIcon("Icons.blueIcon.ico");
     
        static Image LoadImage(string imagePath)
        {
            string Path = Assembly.GetExecutingAssembly().GetName().Name.Replace(" ", "_") + "." + imagePath;
            Image myImage = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(Path));
            return myImage;
        }
        static Icon LoadIcon(string icon )
        {
            try
            {
                    Icon myIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetName().Name.Replace(" ", "_") + "." + icon));
                    return myIcon;
            }
            catch
            {
                return default(Icon);
            }
        }
       
        public static Image waitImage = LoadImage("Icons.wait.gif");
        
        void ClearImage(PictureBox picBox)
        {
            if (picBox != null)
            {
                picBox.Dispose();
                picBox.Image = null;
            }
        }
        class AddedBlip
        {
            public string PlayerName { get; set; }
            public int BlipId { get; set; }
        }
        public void Thread(Action act)
        {
            CheckForIllegalCrossThreadCalls = false;
            new Thread(() => act()).Start();
        }
        Random a = new Random();
        string RandomName(int length)
        {
            string latter = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                SB.Append(latter[a.Next(0, latter.Length)]);
            }
            string res = SB[0].ToString().ToUpper() + SB.ToString().Substring(1).ToLower();
            return res;
        }
        int Selected_Client_Ped_Id
        {
            get
            {
                if (clientBox.SelectedIndex == -1)
                {
                    XtraMessageBox.Show(@"Error !!
Select Player First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return scripts.Ped.GET_PLAYER_PED(clientBox.SelectedIndex);
            }
        }
        int Selected_Client_Index
        {
            get
            {
                if (clientBox.SelectedIndex == -1)
                {
                    XtraMessageBox.Show(@"Error !!
Select Player First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                return clientBox.SelectedIndex;
            }
        }
        float[] Selected_Client_Position
        {
            get
            {
                if (clientBox.SelectedIndex == -1)
                {
                    XtraMessageBox.Show(@"Error !!
Select Player First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return default(float[]);
                }
                return scripts.Entity.GET_ENTITY_COORDS(Selected_Client_Ped_Id);
            }
        }
        string Selected_Client_Name
        {
            get
            {
                return clientBox.SelectedItem.ToString().Substring(4);
            }
        }
        int Selected_Spawned_Vehicle
        {
            get
            {
                try
                {
                    return ReturnSpawnedVehKey(spawnedVehicleBox.SelectedItem.ToString());
                }
                catch
                {
                    XtraMessageBox.Show(@"Error !!
Select Vehicle First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }
        int Selected_Spawned_Ped
        {
            get
            {
                try
                {
                    return ReturnSpawnedPedKey(ped_spawnedPedBox.SelectedItem.ToString());
                }
                catch
                {
                    XtraMessageBox.Show(@"Error !!
Select Ped First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }
        int Selected_Object
        {
            get
            {
                try
                {
                    string objName = spawnedObjBox.SelectedItem.ToString();
                    return int.Parse(objName.Substring(objName.IndexOf("[") + 1).Replace("]", "").Replace(" ", ""));
                }
                catch
                {
                    XtraMessageBox.Show(@"Error !!
Select Object First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }
        int Selected_Blip
        {
            get
            {
                try
                {
                    string blip = blipBox.SelectedItem.ToString();
                    return int.Parse(Regex.Match(blip, @"\[ \d+\ ]").Value.Replace("[ ", "").Replace(" ]",""));
                }
                catch
                {
                    XtraMessageBox.Show(@"Error !!
Select Blip First..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
        }
        public mainFrm()
        {
            InitializeComponent();
            try
            {
                scripts = new Scripts();
                string userName = "";
                foreach (var item in Enum.GetNames(typeof(Character)))
                {
                    charBox.Properties.Items.Add(item);
                    Application.DoEvents();
                }
                charBox.SelectedIndex = (int)scripts.SetChar;
                charBox.SelectedIndexChanged += delegate { scripts.SetChar = (Character)Enum.Parse(typeof(Character), charBox.Text); };
                conBtn.Click += delegate
                {
                    if (apiBox.SelectedIndex == 0)
                        PS3.ChangeAPI(SelectAPI.ControlConsole);
                    else
                        PS3.ChangeAPI(SelectAPI.TargetManager);
                    if (PS3.ConnectTarget() && PS3.AttachProcess())
                    {
                        userName = scripts.PLAYER_NAME;
                        scripts.TypeText("BISOON Loves You " + userName + " " + Scripts.Colors.White + " : ) ", Scripts.Show.Perm);
                        scripts.Enable();
                        Thread(() =>
                        {
                            PS3.InitTarget();
                            handle = new Handling(scripts.Finder.FindOffset(0x411edca4, 0x889680, new byte[] { 0x5d, 10, 0xac, 0x8f }, 0));
                            if (handle.HandlingOffset != 0)
                            {
                                drifitBtn.Enabled = true;
                                currentCarBtn.Enabled = true;
                            }
                        });
                    }
                };
                nameSetBtn.Click += delegate { scripts.PLAYER_NAME = nameBox.Text; };
                nameGetBtn.Click += delegate { scripts.PLAYER_NAME = userName; };
                client_blipColorBox.SelectedIndexChanged += delegate
                {
                    string clientName = clientBox.SelectedItem.ToString();
                    if (clientBlip.ContainsKey(clientName))
                        scripts.Blip.REMOVE_BLIP(clientBlip[clientName]);
                    int blip = scripts.Blip.ADD_BLIP_FOR_ENTITY(Selected_Client_Ped_Id);
                    scripts.Blip.SET_BLIP_SCALE(blip, 0.7f);
                    scripts.Blip.SET_BLIP_COLOUR(blip, (Blip.BlipColor)Enum.Parse(typeof(Blip.BlipColor), client_blipColorBox.SelectedText));
                    if (!clientBlip.ContainsKey(clientName))
                        clientBlip.Add(clientName, blip);
                    else clientBlip[clientName] = blip;
                };
                client_addBlipBtn.Click += delegate
                {
                    client_blipColorBox.ShowPopup();
                };
                refreshClientBtn.Click += delegate
                {
                    clientBox.Items.Clear();
                    client_victimBox.Properties.Items.Clear();
                    for (int i = 0; i < 17; i++)
                    {
                        string playerName = i + " - " + scripts.Player.GET_PLAYER_NAME(i);
                        clientBox.Items.Add(playerName);
                        client_victimBox.Properties.Items.Add(playerName);
                        Application.DoEvents();
                    }
                    ClientOptions(true, xtraTabControl1);
                };
                foreach (var item in Enum.GetNames(typeof(Weapon.Weapons)))
                {
                    giveWeaponBox.Properties.Items.Add(item);
                    ped_weaponsBox.Properties.Items.Add(item);
                }
                giveWeaponBox.CloseUp += (b, c) =>
                {
                    if (c.CloseMode == PopupCloseMode.Normal)
                    {
                        foreach (var selectedPlayer in giveWeaponBox.Properties.Items.GetCheckedValues())
                        {
                            scripts.Weapon.GIVE_WEAPON_TO_PED(Selected_Client_Ped_Id, (Weapon.Weapons)Enum.Parse(typeof(Weapon.Weapons), selectedPlayer.ToString()));
                            Application.DoEvents();
                        }
                    }
                };
                client_removeWeaponBtn.Click += delegate
                {
                    scripts.Weapon.REMOVE_ALL_PED_WEAPONS(Selected_Client_Ped_Id);
                };
                client_clonePlayerBtn.Click += delegate
                {
                    int clonedPed = scripts.Ped.CLONE_PED(Selected_Client_Ped_Id);
                    int playePed = Selected_Client_Ped_Id;
                    int selectedWeapon = scripts.Weapon.GET_SELECTED_PED_WEAPON(playePed);
                    if (selectedWeapon != 0)
                        scripts.Weapon.GIVE_WEAPON_TO_PED(clonedPed, (uint)selectedWeapon);
                    scripts.Ped.PED_AS_BODY_GUARD(clonedPed, playePed, scripts.Player.GET_PLAYER_GROUP(Selected_Client_Index));
                };
                client_kickPlayerBtn.Click += delegate
                {
                    if (scripts.Network.NETWORK_IS_HOST)
                        scripts.Network.NETWORK_SESSION_KICK_PLAYER(Selected_Client_Index);
                    else XtraMessageBox.Show(@"You Have To Be The Host To Kick Player ..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                client_kickVehicleBtn.Click += delegate
                {
                    int selectedPlayer = Selected_Client_Ped_Id;
                    if (scripts.Ped.IS_PED_IN_ANY_VEHICLE(selectedPlayer))
                        scripts.Task.CLEAR_PED_TASKS_IMMEDIATELY(selectedPlayer);
                };
                client_TeleportBtn.Click += delegate
                {
                    int ped = scripts.Ped.PLAYER_PED_ID;
                    if (!scripts.Ped.IS_PED_IN_ANY_VEHICLE(ped))
                        scripts.Entity.SET_ENTITY_COORDS(ped, Selected_Client_Position);
                    else scripts.Entity.SET_ENTITY_COORDS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Selected_Client_Position);
                };
                client_ShothimBtn.Click += delegate
                {
                    scripts.Ped.SET_PED_SHOOTS_AT_COORD(scripts.Ped.PLAYER_PED_ID, Selected_Client_Position);
                };
                client_attachCamBtn.Click += delegate
                {
                    if (client_attachCamBtn.Text == "Attach Cam")
                    {
                        scripts.Cam.ATTACH_CAM_TO_ENTITY(Selected_Client_Ped_Id, true);
                        client_attachCamBtn.Text = "Detach";
                        return;
                    }
                    scripts.Cam.ATTACH_CAM_TO_ENTITY(Selected_Client_Ped_Id, false);
                    client_attachCamBtn.Text = "Attach Cam";
                };
                client_gameHostBtn.Click += delegate
                {
                    string hostName = scripts.Network.GET_HOST;
                    XtraMessageBox.Show(hostName != "Null" ? "The Host is : " + hostName : "Cannot Get The Host ..?", "Host", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                client_playerInfoBtn.Click += delegate
                {
                    try
                    {
                        Scripts.PlayerInfo player = new Scripts.PlayerInfo(Selected_Client_Index);
                        XtraMessageBox.Show(string.Format("{0}  {1} {2} {3} {4}\nhe is {5} {6}  {7}", Selected_Client_Name, player.HasMic, player.MutedMe, player.IsMale, player.GetCoords, player.IsInCar, player.VehicleName, player.InWater), "Player Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch { XtraMessageBox.Show("Error Can't read the player info", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                };
                client_victimBox.CloseUp += (v, b) =>
                {
                    if (b.CloseMode == PopupCloseMode.Normal)
                    {
                        foreach (var selectedPlayer in client_victimBox.Properties.Items.GetCheckedValues())
                        {
                            int victim = clientBox.FindString(selectedPlayer.ToString());
                            scripts.Fire.ADD_OWNED_EXPLOSION(Selected_Client_Ped_Id, scripts.Entity.GET_ENTITY_COORDS(scripts.Ped.GET_PLAYER_PED(victim)));
                            Application.DoEvents();
                        }
                    }
                };
                client_dropMoneyBtn.Click += delegate
                {
                    if (client_dropMoneyBox.Text == "")
                        client_dropMoneyBox.Text = "1";
                    int input = int.Parse(client_dropMoneyBox.Text);
                    int times = input / 40000;
                    if (client_dropMoneyBtn.Text == "Drop")
                    {
                        client_dropMoneyBtn.Text = "Stop";
                        scripts.Network.MoneyDrop(Selected_Client_Index, input <= 40000 ? 1 : times, input <= 40000 ? input : 40000, true);
                    }
                    else
                    {
                        scripts.Network.MoneyDrop(Selected_Client_Index, input <= 40000 ? 1 : times, input <= 40000 ? input : 40000, false);
                        client_dropMoneyBtn.Text = "Drop";
                    }
                    client_dropMoneyBtn.Text = "Drop";
                };
                int spawnedVeh = 0;
                sv_spawnBtn.Click += delegate
                {
                    if (!sv_vehiclesBox.Properties.Items.Contains(sv_vehiclesBox.Text))
                    { XtraMessageBox.Show("Choose your vehicle first ..!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                    spawnedVeh = scripts.Vehicle.SPAWN_VEHICLE(sv_vehiclesBox.Text, Selected_Client_Position);
                    if (spawnedVeh != 0)
                    {
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(spawnedVeh, Vehicle.INDICATOR.LEFT, true);
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(spawnedVeh, Vehicle.INDICATOR.RIGHT, true);
                        AddSpawnedVeh(spawnedVeh, sv_vehiclesBox.Text, scripts.Vehicle.GET_VEHICLE_NUMBER_PLATE_TEXT(spawnedVeh));
                    }
                    spawnedVehicleBox.SelectedIndex = -1;

                };
                sv_gmVehicleBtn.Click += delegate { scripts.Entity.SET_ENTITY_INVINCIBLE(Selected_Spawned_Vehicle, true); };
                sv_fixVehicleBtn.Click += delegate { scripts.Vehicle.SET_VEHICLE_FIXED(Selected_Spawned_Vehicle); };
                sv_deleteVehBtn.Click += delegate { scripts.Vehicle.DELETE_VEHICLE(Selected_Spawned_Vehicle); try { spawnedVehicleBox.Items.RemoveAt(spawnedVehicleBox.SelectedIndex); } catch { } spawnedVehicleBox.SelectedIndex = -1; };
                sv_destroyVehBtn.Click += delegate { scripts.Vehicle.NETWORK_EXPLODE_VEHICLE(Selected_Spawned_Vehicle); };
                sv_boostVehBtn.Click += delegate { scripts.Vehicle.SET_VEHICLE_FORWARD_SPEED(Selected_Spawned_Vehicle, 50f); };
                sv_blipVehBtn.Click += delegate { scripts.Blip.ADD_CUSTOM_BLIP_TO_ENTITY(Selected_Spawned_Vehicle, Blip.BlipColor.Yellow, Blip.Blips.PersonalVehicleCar, 200, 1f); };
                sv_ColorBtn.Click += delegate { sv_colorDialog.ShowPopup(); };
                sv_colorDialog.ColorChanged += delegate { scripts.Vehicle.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR(Selected_Spawned_Vehicle, sv_colorDialog.Color.R, sv_colorDialog.Color.G, sv_colorDialog.Color.B); };
                sv_openDoorsBtn.Click += delegate { scripts.Vehicle.OPEN_DOORS(Selected_Spawned_Vehicle, true); };
                sv_attachVehBtn.Click += delegate
                {
                    if (clientBox.SelectedIndex != -1)
                    {
                        scripts.Entity.Attach_Entity_To_Entity(Selected_Spawned_Vehicle, Selected_Client_Ped_Id);
                    }
                };
                clientBox.SelectedIndexChanged += delegate
                {
                    if (spawnedVehicleBox.SelectedIndex != -1 && clientBox.SelectedIndex != -1)
                    {
                        sv_attachVehBtn.Text = "Attach To " + Selected_Client_Name;
                        sv_teleportVehToPlayerBtn.Text = "Teleport To " + Selected_Client_Name;
                    }
                    else
                    {
                        sv_attachVehBtn.Text = "Attach To";
                        sv_teleportVehToPlayerBtn.Text = "Teleport To";
                    }
                };
                ClientOptions(false, xtraTabControl1);
                sv_teleportToMeBtn.Click += delegate
                {
                    scripts.Entity.SET_ENTITY_COORDS(Selected_Spawned_Vehicle, scripts.Blip.GetMyLocByBlip);
                };
                sv_teleportMeToVehBtn.Click += delegate
                {
                    scripts.Entity.SET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID, scripts.Entity.GET_ENTITY_COORDS(Selected_Spawned_Vehicle));
                };
                sv_teleportVehToPlayerBtn.Click += delegate
                {
                    scripts.Entity.SET_ENTITY_COORDS(Selected_Spawned_Vehicle, Selected_Client_Position);
                };
                healthCh.CheckedChanged += delegate { scripts.Entity.SET_ENTITY_INVINCIBLE(scripts.Ped.PLAYER_PED_ID, healthCh.Checked); };
                ammoCh.CheckedChanged += delegate { scripts.Weapon.SET_PED_INFINITE_AMMO_CLIP(scripts.Ped.PLAYER_PED_ID, ammoCh.Checked); };
                noCapCh.CheckedChanged += delegate { scripts.Player.SET_MAX_WANTED_LEVEL(noCapCh.Checked == true ? 0 : 5); };
                invisibleCh.CheckedChanged += delegate { scripts.Entity.SET_ENTITY_VISIBLE(scripts.Ped.PLAYER_PED_ID, !invisibleCh.Checked); };
                nightVisionCh.CheckedChanged += delegate { scripts.Graphics.SET_SEETHROUGH(nightVisionCh.Checked); };
                fastRunCh.CheckedChanged += delegate { scripts.Player.SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER(scripts.Player.PLAYER_ID, fastRunCh.Checked == true ? 1.49f : 1f); };
                policeRadarCh.CheckedChanged += delegate { scripts.Player.SET_POLICE_RADAR_BLIPS(policeRadarCh.Checked); };
                dunceCapCh.CheckedChanged += delegate { scripts.Ped.DUNCE_CAP(dunceCapCh.Checked); };
                strongHandsCh.CheckedChanged += delegate { scripts.Weapon.SET_PLAYER_MELEE_WEAPON_DAMAGE_MODIFIER(scripts.Player.PLAYER_ID, strongHandsCh.Checked == true ? 50000f : 1f); };
                blackoutCh.CheckedChanged += delegate { scripts.GamePlay._SET_BLACKOUT(blackoutCh.Checked); };
                debugModelCh.CheckedChanged += delegate { scripts.Ped.DEBUG_MODEL(debugModelCh.Checked); };
                antiTargettedCh.CheckedChanged += delegate { scripts.Ped.SET_PED_CAN_BE_TARGETTED(scripts.Ped.PLAYER_PED_ID, !antiTargettedCh.Checked); };
                miniMapCh.CheckedChanged += delegate { scripts.Graphics.DISPLAY_RADAR(!miniMapCh.Checked); };
                destroyMobileBtn.Click += delegate { scripts.Mobile.DESTROY_MOBILE_PHONE(); };
                highJumpCh.CheckedChanged += delegate
                {
                    PS3.SetMemory(0x5EE9B0, highJumpCh.Checked == true ? new byte[] { 0x7B, 0xFF, 0xFF, 0xF3 } : new byte[] { 0x41, 0x82, 0x00, 0x10 });
                };
                deleteAimedEntityCh.CheckedChanged += delegate
                {
                    isOnDE = deleteAimedEntityCh.Checked;
                    while (isOnDE)
                    {
                        if (scripts.Buttonz.ButtonPrassed(Buttonz.Buttons.L2))
                        {
                            scripts.Entity.DELETE_ENTITY(scripts.Entity._GET_AIMED_ENTITY(scripts.Player.PLAYER_ID));
                        }
                        Application.DoEvents();
                    }
                };
                randomPropsBtn.Click += delegate { scripts.Ped.SET_PED_RANDOM_PROPS(scripts.Ped.PLAYER_PED_ID); };
                timeBtn.Click += delegate { scripts.GamePlay.NETWORK_OVERRIDE_CLOCK_TIME(timeBox.Time.Hour, timeBox.Time.Minute); };
                gravityBar.EditValueChanged += delegate
                {
                    gravityLbl.Text = string.Format("[ {0} ]", gravityBar.Value);
                    scripts.GamePlay.SET_GRAVITY_LEVEL = gravityBar.Value;
                };
                wantedLvlBar.EditValueChanged += delegate
                {
                    wantedLvlLbl.Text = string.Format("[ {0} ]", wantedLvlBar.Value);
                    scripts.Player.SET_PLAYER_WANTED_LEVEL_NOW(scripts.Player.PLAYER_ID, wantedLvlBar.Value);
                };
                maxWantedLvlBar.EditValueChanged += delegate
                {
                    maxWantedLvlLbl.Text = string.Format("[ {0} ]", maxWantedLvlBar.Value);
                    scripts.Player.SET_MAX_WANTED_LEVEL(maxWantedLvlBar.Value);
                };
                healthBar.Properties.Maximum = 330;
                healthBar.Properties.Minimum = 100;
                healthBar.EditValueChanged += delegate
                {
                    healthLbl.Text = string.Format("[ {0} ]", (healthBar.Value - 100));
                    scripts.Entity.SET_ENTITY_HEALTH(scripts.Ped.PLAYER_PED_ID, healthBar.Value);
                };
                timeScaleBar.Properties.Maximum = 5;
                timeScaleBar.Properties.Minimum = 0;
                timeScaleBar.EditValueChanged += delegate
                {
                    timeScaleLbl.Text = string.Format("[ {0} ]", (timeScaleBar.Value * -1));
                    float val = (float)(timeScaleBar.Value * -1);
                    scripts.GamePlay.SET_TIME_SCALE(val == 0f ? 1f : val);
                };
                radarZoomBar.Properties.Maximum = 20;
                radarZoomBar.Properties.Minimum = 0;
                radarZoomBar.EditValueChanged += delegate
                {
                    radarZoomLbl.Text = string.Format("[ {0} ]", (radarZoomBar.Value));
                    scripts.GamePlay.SET_RADAR_ZOOM(radarZoomBar.Value);
                };
                setModelBtn.Click += delegate
                {
                    if (modelsBox.SelectedIndex == 0)
                    {
                        int rand = new Random().Next(modelsBox.Properties.Items.Count);
                        scripts.Player.SET_MODEL(modelsBox.Properties.Items[rand].ToString());
                        return;
                    }
                    scripts.Player.SET_MODEL(modelsBox.Text);
                };
                setWeatherBtn.Click += delegate
                {
                    if (weathersBox.SelectedIndex == 0)
                    {
                        int rand = new Random().Next(weathersBox.Properties.Items.Count);
                        scripts.GamePlay.SET_OVERRIDE_WEATHER(weathersBox.Properties.Items[rand].ToString());
                        return;
                    }
                    scripts.GamePlay.SET_OVERRIDE_WEATHER(weathersBox.Text);
                };
                ped_spawnBtn.Click += delegate
                {
                    int spawnedPed = 0;
                    if (ped_modelsBox.SelectedIndex == 0)
                    {
                        int rand = new Random().Next(ped_modelsBox.Properties.Items.Count);
                        spawnedPed = scripts.Ped.CREATE_PED(ped_modelsBox.Properties.Items[rand].ToString(), Selected_Client_Position);
                        if (spawnedPed != 0)
                        {
                            AddSpawnedPed(spawnedPed, RandomName(5), ped_modelsBox.Properties.Items[rand].ToString());
                            ped_spawnedPedBox.SelectedIndex = -1;
                        }
                        return;
                    }
                    spawnedPed = scripts.Ped.CREATE_PED(ped_modelsBox.Text, Selected_Client_Position);
                    if (spawnedPed != 0)
                    {
                        AddSpawnedPed(spawnedPed, RandomName(5), ped_modelsBox.Text);
                        ped_spawnedPedBox.SelectedIndex = -1;
                    }
                };
                ped_godmodBtn.Click += delegate
                {
                    scripts.Entity.SET_ENTITY_INVINCIBLE(Selected_Spawned_Ped, true);
                };
                ped_ammoBtn.Click += delegate { scripts.Weapon.SET_PED_INFINITE_AMMO_CLIP(Selected_Spawned_Ped, true); };
                ped_deleteBtn.Click += delegate
                {
                    scripts.Ped.DELETE_PED(Selected_Spawned_Ped);
                    try
                    {

                        ped_spawnedPedBox.Items.RemoveAt(ped_spawnedPedBox.SelectedIndex);
                    }
                    catch { } ped_spawnedPedBox.SelectedIndex = -1;
                };
                ped_killBtn.Click += delegate { scripts.Ped.APPLY_DAMAGE_TO_PED(Selected_Spawned_Ped, 500); };
                ped_freezeBtn.Click += delegate { scripts.Entity.FREEZE_ENTITY_POSITION(Selected_Spawned_Ped, true); };
                ped_addblipBtn.Click += delegate { scripts.Blip.ADD_CUSTOM_BLIP_TO_ENTITY(Selected_Spawned_Ped, Blip.BlipColor.White, Blip.Blips.Defult, 100, 1f); };
                ped_guardBtn.Click += delegate { int groupIndex = scripts.Player.GET_PLAYER_GROUP(Selected_Client_Index); scripts.Ped.PED_AS_BODY_GUARD(Selected_Spawned_Ped, Selected_Client_Ped_Id, groupIndex); };
                ped_invisibleBtn.CheckedChanged += delegate
                {
                    scripts.Entity.SET_ENTITY_VISIBLE(Selected_Spawned_Ped, !ped_invisibleBtn.Checked);
                };
                ped_controlPedBtn.Click += delegate { scripts.Player.CHANGE_PLAYER_PED(scripts.Player.PLAYER_ID, Selected_Spawned_Ped); };
                ped_compatBtn.Click += delegate { scripts.Task.TASK_COMBAT_PED(Selected_Spawned_Ped, Selected_Client_Ped_Id); };
                ped_weaponsBox.SelectedIndexChanged += delegate { scripts.Weapon.GIVE_WEAPON_TO_PED(Selected_Spawned_Ped, (Weapon.Weapons)Enum.Parse(typeof(Weapon.Weapons), ped_weaponsBox.Text)); };
                ped_teleportToMeBtn.Click += delegate { scripts.Entity.SET_ENTITY_COORDS(Selected_Spawned_Ped, scripts.Entity.GET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID)); };
                ped_teleportToPedBtn.Click += delegate { scripts.Entity.SET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID, scripts.Entity.GET_ENTITY_COORDS(Selected_Spawned_Ped)); };
                ped_componentBtn.Click += delegate { scripts.Ped.SET_PED_RANDOM_COMPONENT_VARIATION(Selected_Spawned_Ped); };
                obj_spawnBtn.Click += delegate
                {
                    int sObj = scripts.Object.CREATE_OBJECT(Selected_Client_Position, obj_objBox.Text);
                    if (sObj != 0)
                        spawnedObjBox.Items.Add(obj_objBox.Text + " [ " + sObj + " ] ");
                };
                obj_deleteBtn.Click += delegate
                {
                    scripts.Entity.DELETE_ENTITY(Selected_Object); try
                    {

                        spawnedObjBox.Items.RemoveAt(spawnedObjBox.SelectedIndex);
                    }
                    catch { } spawnedObjBox.SelectedIndex = -1;
                };
                obj_attachToBtn.Click += delegate { scripts.Entity.Attach_Entity_To_Entity(Selected_Object, Selected_Client_Ped_Id); };
                obj_alphaBox.ButtonClick += (b, c) =>
                {
                    if (c.Button.Index == 1)
                    {
                        scripts.Entity.SET_ENTITY_ALPHA(Selected_Object, Convert.ToInt32(obj_alphaBox.EditValue));
                        return;
                    }
                    obj_alphaBox.EditValue = scripts.Entity.GET_ENTITY_ALPHA(Selected_Object);
                };
                obj_resetAlphaBtn.Click += delegate { scripts.Entity.RESET_ENTITY_ALPHA(Selected_Object); };
                wpTeleportBtn.Click += delegate
                {
                    float[] wp = scripts.Vector.Get_Way_point;
                    if (wp == null)
                    {
                        XtraMessageBox.Show(@"Error !!
Set your Waypoint first..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                    } scripts.Entity.SET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID, wp);
                };
                getCurrentLocBtn.Click += delegate { float[] currentLoc = scripts.Blip.GetMyLocByBlip; xBox.Text = currentLoc[0].ToString(); yBox.Text = currentLoc[1].ToString(); zBox.Text = currentLoc[2].ToString(); };
                teleToCustomLocBtn.Click += delegate { scripts.Entity.SET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID, new float[] { float.Parse(xBox.Text), float.Parse(yBox.Text), float.Parse(zBox.Text) }); };
                teleportToClosestVehBtn.Click += delegate { scripts.Vehicle.TELEPORT_TO_CLOSEST_VEHICLE(); };
                teleToLastVehBtn.Click += delegate { scripts.Vehicle.SET_PED_INTO_VEHICLE(scripts.Ped.PLAYER_PED_ID, scripts.Player.GET_PLAYERS_LAST_VEHICLE, -1); };
                teleLastVehBtn.Click += delegate { scripts.Entity.SET_ENTITY_COORDS(scripts.Player.GET_PLAYERS_LAST_VEHICLE, scripts.Blip.GetMyLocByBlip); };
                teleportGunCheck.CheckedChanged += delegate
                {
                    tGun = teleportGunCheck.Checked;
                    int ped = scripts.Ped.PLAYER_PED_ID;
                    while (tGun)
                    {
                        float[] fireLoc = scripts.Weapon.GET_PED_LAST_WEAPON_IMPACT_COORD(ped);
                        if (fireLoc[0] != 0)
                        { scripts.Entity.SET_ENTITY_COORDS(ped, fireLoc); } Application.DoEvents();
                    }
                };
                myLoc = new SaveLoc();
                RefreshSavedLoc();
                saveLocBtn.Click += delegate
                {
                    try
                    {
                        myLoc.SaveLocation(locNameBox.Text, scripts.Blip.GetMyLocByBlip);
                        RefreshSavedLoc();
                    }
                    catch (Exception e)
                    { XtraMessageBox.Show(e.Message); }
                };
                deleteLocBtn.Click += delegate
                {
                    try
                    {
                        myLoc.DeleteLocation(savedLocBox.Text);
                        RefreshSavedLoc();
                        savedLocBox.Text = "";
                    }
                    catch (Exception e)
                    { XtraMessageBox.Show(e.Message); }
                };
                teleportToSavedLocBtn.Click += delegate
                {
                    if (savedLocBox.Text == "")
                    {
                        XtraMessageBox.Show(@"Error !!
Select your saved location first..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    } scripts.Entity.SET_ENTITY_COORDS(scripts.Ped.PLAYER_PED_ID, myLoc.ReturnSavedLocation(savedLocBox.Text));
                };
                fixVehicleBtn.Click += delegate { scripts.Vehicle.SET_VEHICLE_FIXED(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IS_USING); };
                boostforwardBtn.Click += delegate { scripts.Vehicle.SET_VEHICLE_FORWARD_SPEED(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, float.Parse(boostLvlBox.Text)); };
                boostBackBtn.Click += delegate { scripts.Vehicle.SET_VEHICLE_FORWARD_SPEED(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, float.Parse(boostLvlBox.Text) * -1); };
                boostforwardCh.CheckedChanged += delegate
                {
                    Thread(() =>
                    {
                        PS3.InitTarget();
                        boostBolean[0] = boostforwardCh.Checked;
                        while (boostBolean[0])
                        {
                            if (scripts.Buttonz.ButtonPrassed(Buttonz.Buttons.L3))
                                scripts.Vehicle.SET_VEHICLE_FORWARD_SPEED(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, boostLvl);
                            scripts.wait(0.5);
                        }
                    });
                };
                boostbackCh.CheckedChanged += delegate
                {
                    Thread(() =>
                    {
                        PS3.InitTarget();
                        boostBolean[1] = boostbackCh.Checked;
                        while (boostBolean[1])
                        {
                            if (scripts.Buttonz.ButtonPrassed(Buttonz.Buttons.R3))
                                scripts.Vehicle.SET_VEHICLE_FORWARD_SPEED(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, boostLvl * -1);
                            scripts.wait(0.5);
                        }
                    });
                };
                boostLvlBox.TextChanged += delegate
                {
                    try
                    {
                        boostLvl = float.Parse(boostLvlBox.Text);
                    }
                    catch
                    {
                    }
                };
                bool forPrimary = false;
                primaryColorBtn.Click += delegate { carColorBox.ShowPopup(); forPrimary = true; };
                secondaryColorBtn.Click += delegate { carColorBox.ShowPopup(); forPrimary = false; };
                carColorBox.ColorChanged += delegate { if (forPrimary) scripts.Vehicle.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, carColorBox.Color.R, carColorBox.Color.G, carColorBox.Color.B); else scripts.Vehicle.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, carColorBox.Color.R, carColorBox.Color.G, carColorBox.Color.B); };
                upGradeBtn.Click += delegate { scripts.Vehicle.Upgrade_Downgrade(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, true); }; downGradeBtn.Click += delegate { scripts.Vehicle.Upgrade_Downgrade(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, false); };
                flasherChecks.SelectedIndex = 3;
                flasherChecks.SelectedIndexChanged += delegate
                {
                    if (flasherChecks.SelectedIndex == 0)
                    {
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.RIGHT, !true);
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.LEFT, true);
                    }
                    else if (flasherChecks.SelectedIndex == 2)
                    {
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.LEFT, !true);
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.RIGHT, true);
                    }
                    else if (flasherChecks.SelectedIndex == 1)
                    {
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.LEFT, true);
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.RIGHT, true);
                    }
                    else
                    {
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.LEFT, !true);
                        scripts.Vehicle.SET_VEHICLE_INDICATOR_LIGHTS(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, Vehicle.INDICATOR.RIGHT, !true);
                    }
                };
                foreach (Control cntr in vehOptionsGroup.Controls)
                {
                    cntr.Click += delegate
                    {
                        if (cntr.Name.Contains("Damage"))
                        {
                            scripts.Entity.SET_ENTITY_INVINCIBLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, true);
                        }
                        else if (cntr.Name.Contains("offVeh"))
                        {
                            if (cntr.Text.Contains("Off"))
                            {
                                scripts.Vehicle.SET_VEHICLE_UNDRIVEABLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, !false);
                                cntr.Text = "On Vehicle";
                                return;
                            }
                            scripts.Vehicle.SET_VEHICLE_UNDRIVEABLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, !true);
                            cntr.Text = "Off Vehicle";
                        }
                        else if (cntr.Name.Contains("driveW"))
                        {
                            scripts.Task.TASK_VEHICLE_DRIVE_WANDER(scripts.Ped.PLAYER_PED_ID, scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, true);
                        }
                        else if (cntr.Name.Contains("ghostVe"))
                        {
                            scripts.Entity.SET_ENTITY_ALPHA(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, 55);
                        }
                        else if (cntr.Name.Contains("invisibleVe"))
                        {
                            if (cntr.Text.Contains("Invi"))
                            {
                                scripts.Entity.SET_ENTITY_VISIBLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, false);
                                cntr.Text = "Visible";
                                return;
                            }
                            scripts.Entity.SET_ENTITY_VISIBLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, true);
                            cntr.Text = "Invisible";
                        }
                        else if (cntr.Name.Contains("explode"))
                        {
                            scripts.Vehicle.NETWORK_EXPLODE_VEHICLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN);
                        }
                        else if (cntr.Name.Contains("delete"))
                        {
                            scripts.Vehicle.DELETE_VEHICLE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN);
                        }
                        else if (cntr.Name.Contains("Plate"))
                        {
                            scripts.Vehicle.SET_PLATE(scripts.Vehicle.GET_MY_PLAYER_VEHICLE_IN, plateBox.Text);
                        }
                    };

                }
                weaponDamageBtn.Click += delegate { scripts.Weapon.SET_PLAYER_WEAPON_DAMAGE_MODIFIER(scripts.Player.PLAYER_ID, float.Parse(weaponDamageBox.Text)); };
                weaponDefenseBtn.Click += delegate { scripts.Weapon.SET_PLAYER_WEAPON_DEFENSE_MODIFIER(scripts.Player.PLAYER_ID, float.Parse(weaponDefenseBox.Text)); };
                meleeDamageBtn.Click += delegate { scripts.Weapon.SET_PLAYER_MELEE_WEAPON_DAMAGE_MODIFIER(scripts.Player.PLAYER_ID, float.Parse(meleeDamageBox.Text)); };
                meleeDefenseBtn.Click += delegate { scripts.Weapon.SET_PLAYER_MELEE_WEAPON_DEFENSE_MODIFIER(scripts.Player.PLAYER_ID, float.Parse(meleeDefenseBox.Text)); };
                currentCarBtn.Click += delegate
                {
                    handle.InitVeh();
                    currentCarBtn.Text = string.Format("Current Car\n[ {0} ]", handle.CarName);
                    speedLvlBox.Text = handle.Speed.ToString("0.0000");
                    heightLvlBox.Text = handle.Height.ToString("0.0000");

                };
                drifitBtn.Click += delegate
                {
                    handle.VehDrift();
                };
                speedLvlBox.ButtonClick += (speed, setLvl) =>
                {
                    if (setLvl.Button.Index == 0)
                        handle.SetValue(Modz.SuperFast1, float.Parse(speedLvlBox.Text));
                    else
                        handle.returnValue(Modz.SuperFast1);
                };
                heightLvlBox.ButtonClick += (speed, setLvl) =>
                {
                    if (setLvl.Button.Index == 0)
                        handle.SetValue(Modz.CarHeight, float.Parse(heightLvlBox.Text));
                    else
                        handle.returnValue(Modz.CarHeight);
                };
                foreach (var camo in Enum.GetNames(typeof(Weapon.WeaponTint)))
                {
                    weapCamoBox.Properties.Items.Add(camo);
                }
                weapCamoBox.SelectedIndexChanged += delegate
                {
                    int ped = scripts.Ped.PLAYER_PED_ID; uint currentWeapon = (uint)scripts.Weapon.GET_CURRENT_PED_WEAPON(ped); if (!(currentWeapon == 2725352035 || currentWeapon == 0)) scripts.Weapon.SET_PED_WEAPON_TINT_INDEX(ped, currentWeapon, (Weapon.WeaponTint)Enum.Parse(typeof(Weapon.WeaponTint), weapCamoBox.Text));
                    else XtraMessageBox.Show(@"Error !!
choose your weapon first from your weapons wheel..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                Blipfrm blipFrm = null;
                bliptoAimedEntityBtn.Click += delegate
                {

                    aimedBlip = scripts.Entity._GET_AIMED_ENTITY(scripts.Player.PLAYER_ID); if (aimedBlip != 0)
                    {
                        blipFrm = new Blipfrm();
                        blipFrm.Icon = myicon;
                        blipFrm.ShowDialog();
                    }
                    else XtraMessageBox.Show(@"Error !!
Couldn't find your target..!!
Aim at your target first..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                removeBlipBtn.Click += delegate
                {
                    try
                    {
                        scripts.Blip.REMOVE_BLIP(Selected_Blip);
                        blipBox.Items.RemoveAt(blipBox.SelectedIndex);
                    }
                    catch { }
                };
                setCashBtn.Click += delegate { scripts.Stats.SetCash(int.Parse(cashBox.Text)); };
                setBankBtn.Click += delegate { scripts.Network.NETWORK_EARN_FROM_ROCKSTAR(int.Parse(bankBox.Text)); };
                removeMoneyBtn.Click += delegate { scripts.Stats.NETWORK_SPENT_CASH_DROP = int.Parse(removeMoneyBox.Text); };
                for (int i = 0; i <= 8000; i++)
                {
                    rankBox.Properties.Items.Add(i);
                    Application.DoEvents();
                }
                rankBtn.Click += delegate { scripts.Stats.SET_RANK = rankBox.SelectedIndex; };
                foreach (Control butt in groupControl2.Controls)
                {
                    if (butt is SimpleButton)
                    {
                        butt.Click += delegate
                        {
                            if (butt.Text.Contains("Kill"))
                                scripts.Stats.KILLS_PLAYERS = int.Parse(killsBox.Text);
                            else if (butt.Text.Contains("Death"))
                                scripts.Stats.DEATHS_PLAYER = int.Parse(deathsBox.Text);
                            else scripts.Stats.Kill_Death_Ratio = float.Parse(ratioBox.Text);
                        };
                    }
                }
                shootabilityBtn.Click += delegate { scripts.Stats.SHOOTING_ABILITY = 450; };
                unlockAllTrophyBtn.Click += delegate
                {
                    foreach (var trophie in Enum.GetValues(typeof(Player.Trophies)))
                    {
                        scripts.Player.GIVE_ACHIEVEMENT_TO_PLAYER((Player.Trophies)trophie);
                    }
                };
                unlockAllBtn.Click += delegate
                {
                    Thread(delegate
                    {
                        PS3.InitTarget();
                        double num = 0;
                        foreach (var item in scripts.allStatsString)
                        {
                            if (item.Contains("(int)") || item.Contains("(float)") || item.Contains("(bool)"))
                            {
                                string[] stat = item.Replace(" ", "").Replace("MP#_", "").Split(',');
                                if (item.Contains("(int)"))
                                {
                                    scripts.STAT_SET_INT(stat[0].Replace("(int)", ""), int.Parse(stat[1]), int.Parse(stat[2]));
                                }
                                else if (item.Contains("(bool)"))
                                {
                                    scripts.STAT_SET_BOOL(stat[0].Replace("(bool)", ""), int.Parse(stat[1]), int.Parse(stat[2]));
                                }
                                else if (item.Contains("(float)"))
                                {
                                    scripts.STAT_SET_FLOAT(stat[0].Replace("(float)", ""), int.Parse(stat[1]), int.Parse(stat[2]));
                                }
                            }
                            num += 0.23 + 0.0048;
                            unlockAllBtn.Text = "Unlock All " + num.ToString("0") + " %";
                        }
                    });
                };
                inventoryBox.ButtonClick += (inv, ory) =>
                {
                    int val = 0;
                    if (int.TryParse(inventoryBox.Text, out val))
                    {
                        scripts.Stats.SMOKES = val;
                        scripts.Stats.SNACKS = val;
                        scripts.Stats.FIREWORK = val;
                        scripts.Stats.ARMOUR = val;
                    }
                    else
                        XtraMessageBox.Show(@"Error !!
It's long number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                skillsBar.ValueChanged += delegate { scripts.Stats.SKILLS = skillsBar.Value * 10; };
                mentalBar.ValueChanged += delegate { scripts.Stats.PLAYER_MENTAL_STATE = Convert.ToSingle(skillsBar.Value * 10); };
                this.Icon = myicon;
                cloneKillBtn.Click += delegate
                {
                    int targetPed = Selected_Client_Ped_Id;
                    int createdPed = scripts.Ped.CLONE_PED(targetPed);
                    if (createdPed != 0)
                        if (XtraMessageBox.Show(@"Give him a unlimated health ? ", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            scripts.Entity.SET_ENTITY_INVINCIBLE(createdPed, true);
                    scripts.Weapon.GIVE_WEAPON_TO_PED(createdPed, Weapon.Weapons.HOMINGLAUNCHER);
                    scripts.Weapon.SET_PED_INFINITE_AMMO_CLIP(createdPed, true);
                    scripts.Task.TASK_COMBAT_PED(createdPed, targetPed);
                };
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        void ClientOptions(bool value, XtraTabControl cont)
        {
            foreach (XtraTabPage page in cont.TabPages)
            {
                foreach (Control tp in page.Controls)
                {
                    if (tp is GroupControl)
                    {
                        foreach (Control item in ((GroupControl)tp).Controls)
                        {
                            item.Enabled = value;
                        }
                    }

                    tp.Enabled = value;
                }
            }
            clientGroubBox.Enabled = value;
        }
        void RefreshSavedLoc()
        {
            savedLocBox.Properties.Items.Clear();
            savedLocBox.Properties.Items.AddRange(myLoc.ReturnAllLocations);
        }
        class SpawnedVehicle
        {
            public string VehicleName { get; set; }
            public string VehiclePlate { get; set; }
            public int VehicleId { get; set; }
        }
        class SpawnedPed
        {
            public string PedName { get; set; }
            public string PedModel { get; set; }
            public int PedId { get; set; }
        }

        void AddSpawnedVeh(int Id, string vehicleName, string vehiclePlate)
        {
            SpawnedVehicle spV = new SpawnedVehicle() { VehicleId = Id, VehicleName = vehicleName, VehiclePlate = vehiclePlate };
            if (!spawnedveh.ContainsKey(spV.VehicleId))
            {
                spawnedveh.Add(spV.VehicleId, spV);
                spawnedVehicleBox.Items.Add(string.Format("{0} [ {1} ]", spV.VehicleName, spV.VehiclePlate));
            }
        }
        int ReturnSpawnedVehKey(string value)
        {
            return spawnedveh.First(x => x.Value.VehiclePlate == Regex.Match(value.Replace(" ", ""), @"(?<=\[)[0-9A-Z]*(?=\])").Value).Key;
        }
        void AddSpawnedPed(int iD, string pedName, string pedModel)
        {
            SpawnedPed sPed = new SpawnedPed() { PedId = iD, PedName = pedName, PedModel = pedModel };
            if (!spawnedveh.ContainsKey(sPed.PedId))
            {
                spawnedped.Add(sPed.PedId, sPed);
                ped_spawnedPedBox.Items.Add(string.Format("{0} [ {1} ]", sPed.PedName, sPed.PedModel));
            }
        }
        int ReturnSpawnedPedKey(string value)
        {
            string val = value.Substring(0, value.IndexOf(" ["));
            return spawnedped.First(x => x.Value.PedName == val).Key;
        }
        class AddedBlips
        {
            public int BlipId { get; set; }
            public string BlipColor { get; set; }
            public string BlipName { get; set; }
        }
        public void AddCreatedBlip(int blipId, string blipName, string blipColor)
        {
            AddedBlips ablip = new AddedBlips() { BlipId = blipId, BlipName = blipName, BlipColor = blipColor };
            if (!addedBlip.ContainsKey(blipId))
            {
                addedBlip.Add(blipId, ablip);
                blipBox.Items.Add(string.Format("{0} [ {1} ] {2}", ablip.BlipName, ablip.BlipId, ablip.BlipColor));
            }
        }
        class SaveLoc
        {
            static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"/BISOON RTM/";
            static readonly string Locations = path + "Locations";
            public SaveLoc()
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!File.Exists(Locations))
                    File.WriteAllText(Locations, "");
            }
            public void SaveLocation(string locName, float[] loc)
            {
                File.AppendAllText(Locations, string.Format("{0}={1},{2},{3}", locName, loc[0], loc[1], loc[2]) + "\n");
            }
            public void DeleteLocation(string locName)
            {
                List<string> allLocs = new List<string>();
                allLocs.AddRange(File.ReadAllLines(Locations));
                allLocs.Remove(allLocs.Find(x => x.Contains(locName)));
                File.WriteAllLines(Locations, allLocs);
            }
            public float[] ReturnSavedLocation(string locName)
            {
                string[] allLocs = File.ReadAllLines(Locations);
                foreach (var loc in allLocs)
                {
                    string[] locationData = loc.Split('=');
                    if (locationData[0] == locName)
                    {
                        string[] readLoc = locationData[1].Split(',');
                        return new float[] { float.Parse(readLoc[0]), float.Parse(readLoc[1]), float.Parse(readLoc[2]) };
                    }
                }
                return default(float[]);
            }
            public string[] ReturnAllLocations
            {
                get
                {
                    try
                    {

                        List<string> locs = new List<string>();
                        string[] allLocs = File.ReadAllLines(Locations);
                        foreach (var item in allLocs)
                        {
                            locs.Add(item.Split('=')[0]);
                        }
                        return locs.ToArray();
                    }
                    catch { return null; }
                }
            }
        }
    }
}
