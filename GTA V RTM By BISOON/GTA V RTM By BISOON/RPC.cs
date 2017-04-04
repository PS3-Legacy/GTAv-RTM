using GTA_V_RTM_By_BISOON;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


public enum Character
    {
       Character_one,
       Character_Two,
       Character_Three
    }
   public  class RPC : API
    {
        public void wait(double seconds)
        {
            DateTime dt = DateTime.Now.AddSeconds(seconds);
            while (dt > DateTime.Now)
                Application.DoEvents();
        }
         uint FindAddress(string NativeString)
        {
            object NativeHash = 0;
            if (NativeString.ToString().Any(c => char.IsDigit(c)))
            {
                NativeHash = Convert.ToUInt32(NativeString.ToString(), 0x10);
            }
            else
            {
                NativeHash = Hash((string)(object)NativeString);
            }
            short[] bl_Dictionary = { 0x4AE5, 0x4AE4, 0x4AEB, 0x4AE9, 0x4AE6, 0x4AA8, 0x4AEA, 0x4AE8, 0x4AE3, 0x4AE1, 0x4AE2, 0x4AE7, 0x4AB9, 0x4AB8, 0x4AB7, 0x4AB6, 0x4AB5, 0x4AB4, 0x4AB3, 0x4AB2, 0x4AB1 };
            uint hash = (uint)NativeHash & 0xff;
            uint Struct = PS3.Extension.ReadUInt32(0x1E6FF38 + (hash * 4));
            while (Struct != 0)
            {
                int NativeCount = PS3.Extension.ReadInt32(Struct + 0x20);
                for (uint i = 0; i < NativeCount; i++)
                {
                    if (PS3.Extension.ReadUInt32((Struct + 0x24) + (i * 4)) == (uint)NativeHash)
                    {
                        uint Native = PS3.Extension.ReadUInt32(PS3.Extension.ReadUInt32((Struct + 4) + (i * 4)));
                        for (uint e = 0; e < 100; ++e)
                        {
                            short NativeLocation = PS3.Extension.ReadInt16(Native + (e * 0x4));
                            for (int u = 0; u < bl_Dictionary.Length; ++u)
                            {
                                if (bl_Dictionary[u] == NativeLocation)
                                {
                                    byte[] call = PS3.GetBytes(Native + (e * 0x4), 4);
                                    Array.Reverse(call);
                                    return ((uint)(BitConverter.ToUInt32(call, 0) - 0x48000001)) + Native + (e * 0x4) - 0x4000000;
                                }
                            }
                        }
                    }
                }
                Struct = PS3.Extension.ReadUInt32(Struct);
            }
            return 0xFF;
        }
        public int Call<T>(T address, params object[] parameters)
        {
            object func_address = 0;
            if (typeof(T) == typeof(string))
                func_address = FindAddress((string)(object)address);
            else
                func_address = Convert.ChangeType(address, TypeCode.UInt32);
            int length = parameters.Length;
            int index = 0;
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            uint num6 = 0;
            while (index < length)
            {
                if (parameters[index] is int)
                {
                    PS3.Extension.WriteInt32(0x10020000 + (num3 * 4), (int)parameters[index]);
                    num3++;
                }
                else if (parameters[index] is bool)
                {
                    PS3.Extension.WriteUInt32(0x10020000 + (num3 * 4), Convert.ToUInt32(parameters[index]));
                    num3++;
                }
                else if (parameters[index] is uint)
                {
                    PS3.Extension.WriteUInt32(0x10020000 + (num3 * 4), (uint)parameters[index]);
                    num3++;
                }
                else
                {
                    uint num7;
                    if (parameters[index] is string)
                    {
                        num7 = 0x10022000 + (num4 * 0x400);
                        PS3.Extension.WriteString(num7, Convert.ToString(parameters[index]));
                        PS3.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);
                        num3++;
                        num4++;
                    }
                    else if (parameters[index] is float)
                    {
                        WriteSingle(0x10020024 + (num5 * 4), (float)parameters[index]);
                        num5++;
                    }
                    else if (parameters[index] is float[])
                    {
                        float[] input = (float[])parameters[index];
                        num7 = 0x10021000 + (num6 * 4);
                        WriteSingle(num7, input);
                        PS3.Extension.WriteUInt32(0x10020000 + (num3 * 4), num7);
                        num3++;
                        num6 += (uint)input.Length;
                    }
                }
                index++;
            }
            PS3.Extension.WriteUInt32(0x1002004C, (uint)func_address);
            int timeOut = 0;
            while (PS3.Extension.ReadUInt32(0x1002004C) != 00)
            {
                Thread.Sleep(10);
                timeOut++;
                if (timeOut == 60)
                    break;
                Application.DoEvents();
            }
            return PS3.Extension.ReadInt32(0x10020050);
        }
         void WriteSingle<T>(uint address, T value)
        {
            if (value is float)
            {
                byte[] buffer = BitConverter.GetBytes((float)(object)value);
                PS3.SetMemory(address, buffer.Reverse<byte>().ToArray());
            }
            else
            {
                float[] input = (float[])(object)value;
                int length = input.Length;
                byte[] array = new byte[length * 4];
                for (int i = 0; i < length; i++)
                {
                    BitConverter.GetBytes(input[i]).Reverse<byte>().ToArray().CopyTo(array, (int)(i * 4));
                }
                PS3.SetMemory(address, array);
            }
        }

        bool IsEnable()
        {
            bool reslut = false;
            if (PS3.GetBytes(Addresses.RPC_Enable, 4).SequenceEqual(new byte[] { 0x3D, 0x60, 0x10, 0x05 }))
                reslut = true;
            return reslut;
        }

        byte[] patchjmp(uint is_player_online)
        {
            is_player_online += 0xC;
            uint a = Addresses.RPC_Enable;
            uint bytes = a - is_player_online;
            byte[] f = new byte[4];
            byte[] result = new byte[4];
            f[3] = (byte)(bytes >> 24);
            f[2] = (byte)(bytes >> 16);
            f[1] = (byte)(bytes >> 8);
            f[0] = (byte)(bytes);
            result[3] = (byte)(f[0] + 1);
            result[2] = f[1];
            result[1] = f[2];
            result[0] = 0x49;

            return result;
        }


        public void Enable()
        {
            if (!IsEnable())
            {
            PS3.Extension.WriteString(0x10030000, "freeroam");
            byte[] buffer2 = new byte[] { 0xf8, 0x21, 0xfd, 0xA1, 0x7c, 8, 2, 0xa6, 0xf8, 1, 0x02, 0x70, 60, 0x60, 0x10, 2, 0x81, 0x83, 0, 0x4c, 0x2c, 12, 0, 0, 0x41, 130, 0, 100, 0x80, 0x83, 0, 4, 0x80, 0xa3, 0, 8, 0x80, 0xc3, 0, 12, 0x80, 0xe3, 0, 0x10, 0x81, 3, 0, 20, 0x81, 0x23, 0, 0x18, 0x81, 0x43, 0, 0x1c, 0x81, 0x63, 0, 0x20, 0xc0, 0x23, 0, 0x24, 0xc0, 0x43, 0, 40, 0xc0, 0x63, 0, 0x2c, 0xc0, 0x83, 0, 0x30, 0xc0, 0xa3, 0, 0x34, 0xc0, 0xc3, 0, 0x38, 0xc0, 0xe3, 0, 60, 0xc1, 3, 0, 0x40, 0xc1, 0x23, 0, 0x48, 0x80, 0x63, 0, 0, 0x7d, 0x89, 3, 0xa6, 0x4e, 0x80, 4, 0x21, 60, 0x80, 0x10, 2, 0x38, 160, 0, 0, 0x90, 0xa4, 0, 0x4c, 0x90, 100, 0, 80, 0xe8, 1, 0x02, 0x70, 0x7c, 8, 3, 0xa6, 0x38, 0x21, 0x02, 0x60, 0x38, 0x60, 0x00, 0x03, 0x4E, 0x80, 0x00, 0x20 };
                PS3.SetMemory(Addresses.RPC_Enable, new byte[] { 0x3D, 0x60, 0x10, 0x05, 0x81, 0x6B, 0x00, 0x00, 0x7D, 0x69, 0x03, 0xA6, 0x4E, 0x80, 0x04, 0x20 });
                PS3.SetMemory(Addresses.SFA4, buffer2);
                PS3.Extension.WriteUInt32(0x10050000, Addresses.SFA4);
                byte[] on = patchjmp(Addresses.IS_PLAYER_ONLINE);
                PS3.SetMemory(Addresses.IS_PLAYER_ONLINE, new byte[] { 0xF8, 0x21, 0xFF, 0x91, 0x7C, 0x08, 0x02, 0xA6, 0xF8, 0x01, 0x00, 0x80, on[0], on[1], on[2], on[3] });
                PS3.SetMemory(Addresses.IS_PLAYER_ONLINE + 0x18, new byte[] { 0x7C, 0x08, 0x03, 0xA6, 0x38, 0x21, 0x00, 0x70, 0x4E, 0x80, 0x00, 0x20 });
            }
        }
        public uint Hash(string input)
        {
            byte[] stingbytes = Encoding.UTF8.GetBytes(input.ToLower());
            uint num1 = 0U;
            for (int i = 0; i < stingbytes.Length; i++)
            {
                uint num2 = num1 + (uint)stingbytes[i];
                uint num3 = num2 + (num2 << 10);
                num1 = num3 ^ num3 >> 6;
            }
            uint num4 = num1 + (num1 << 3);
            uint num5 = num4 ^ num4 >> 11;
            return num5 + (num5 << 15);
        }




       public  string[] allStatsString = { "Chrome Rims:",
"(int) MP#_AWD_WIN_CAPTURES, 25, 1",
"(int) MP#_AWD_DROPOFF_CAP_PACKAGES, 100, 1",
"(int) MP#_AWD_KILL_CARRIER_CAPTURE, 100, 1",
"(int) MP#_AWD_FINISH_HEISTS, 50, 1",
"(int) MP#_AWD_FINISH_HEIST_SETUP_JOB, 50, 1",
"(int) MP#_AWD_NIGHTVISION_KILLS, 100, 1",
"(int) MP#_AWD_WIN_LAST_TEAM_STANDINGS, 50, 1",
"(int) MP#_AWD_ONLY_PLAYER_ALIVE_LTS, 50, 1",
"Exclusive T-Shirts:",
"(int) MP#_AWD_FMHORDWAVESSURVIVE, 10, 1",
"(int) MP#_AWD_FMPICKUPDLCCRATE1ST, 1, 1",
"(int) MP#_AWD_WIN_CAPTURE_DONT_DYING, 25, 1",
"(int) MP#_AWD_DO_HEIST_AS_MEMBER, 25, 1",
"(int) MP#_AWD_PICKUP_CAP_PACKAGES, 100, 1",
"(bool) MP#_AWD_FINISH_HEIST_NO_DAMAGE, 1, 1",
"(int) MP#_AWD_WIN_GOLD_MEDAL_HEISTS, 25, 1",
"(int) MP#_AWD_KILL_TEAM_YOURSELF_LTS, 25, 1",
"(int) MP#_AWD_KILL_PSYCHOPATHS, 100, 1",
"(int) MP#_AWD_DO_HEIST_AS_THE_LEADER, 25, 1",
"(bool) MP#_AWD_STORE_20_CAR_IN_GARAGES, 1, 1",
"Max Skills:",
"(int) MP#_SCRIPT_INCREASE_STAM, 100, 1",
"(int) MP#_SCRIPT_INCREASE_STRN, 100, 1",
"(int) MP#_SCRIPT_INCREASE_LUNG, 100, 1",
"(int) MP#_SCRIPT_INCREASE_DRIV, 100, 1",
"(int) MP#_SCRIPT_INCREASE_FLY, 100, 1",
"(int) MP#_SCRIPT_INCREASE_SHO, 100, 1",
"(int) MP#_SCRIPT_INCREASE_STL, 100, 1",
"(int) MP#_SCRIPT_INCREASE_MECH, 100, 1",
"Unlock All LSC:",
"(int) MP#_RACES_WON, 50, 1",
"(int) MP#_CHAR_FM_CARMOD_1_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_2_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_3_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_4_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_5_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_6_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_CARMOD_7_UNLCK, -1, 1",
"(int) MP#_AWD_FMRALLYWONDRIVE, 1, 1",
"(int) MP#_AWD_FMRALLYWONNAV, 1, 1",
"(int) MP#_AWD_FMWINSEARACE, 1, 1",
"(int) MP#_AWD_FMWINAIRRACE, 1, 1",
"(int) MP#_NUMBER_TURBO_STARTS_IN_RACE, 50, 1",
"(int) MP#_USJS_COMPLETED, 50, 1",
"(int) MP#_AWD_FM_RACES_FASTEST_LAP, 50, 1",
"(int) MP#_NUMBER_SLIPSTREAMS_IN_RACE, 100, 1",
"Unlock Camos & Parachutes:",
"(int) MP#_CHAR_KIT_1_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_2_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_3_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_4_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_5_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_6_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_7_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_8_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_9_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_10_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_11_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_12_FM_UNLCK, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE2, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE3, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE4, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE5, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE6, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE7, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE8, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE9, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE10, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE11, -1, 1",
"(int) MP#_CHAR_KIT_FM_PURCHASE12, -1, 1",
"Unlock Heist Vehicles:",
"(int) MP#_CHAR_FM_VEHICLE_1_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_VEHICLE_2_UNLCK, -1, 1",
"Unlock Tattoos:",
"(int) MP#_AWD_FM_DM_WINS, 50, 1",
"(int) MP#_AWD_FM_TDM_MVP, 50, 1",
"(int) MP#_AWD_FM_DM_TOTALKILLS, 500, 1",
"(bool) MP#_AWD_FMATTGANGHQ, 1, 1",
"(int) MP#_AWD_FMBBETWIN, 50000, 1",
"(bool) MP#_AWD_FMWINEVERYGAMEMODE, 1, 1",
"(bool) MP#_AWD_FMRACEWORLDRECHOLDER, 1, 1",
"(bool) MP#_AWD_FMFULLYMODDEDCAR, 1, 1",
"(bool) MP#_AWD_FMMOSTKILLSSURVIVE, 1, 1",
"(bool) MP#_AWD_FMKILL3ANDWINGTARACE, 1, 1",
"(int) MP#_AWD_FMKILLBOUNTY, 25, 1",
"(int) MP#_AWD_FMREVENGEKILLSDM, 50, 1",
"(bool) MP#_AWD_FMKILLSTREAKSDM, 1, 1",
"(int) MP#_AWD_HOLD_UP_SHOPS, 20, 1",
"(int) MP#_AWD_LAPDANCES, 25, 1",
"(int) MP#_AWD_SECURITY_CARS_ROBBED, 25, 1",
"(int) MP#_AWD_RACES_WON, 50, 1",
"(int) MP#_AWD_CAR_BOMBS_ENEMY_KILLS, 25, 1",
"(int) MP#_PLAYER_HEADSHOTS, 500, 1",
"(int) MP#_DB_PLAYER_KILLS, 1000, 1",
"Redesign Character Prompt:",
"(bool) MP#_FM_CHANGECHAR_ASKED, 0, 1",
"Snacks:",
"(int) MP#_NO_BOUGHT_YUM_SNACKS, 500000, 1",
"(int) MP#_NO_BOUGHT_HEALTH_SNACKS, 500000, 1",
"(int) MP#_NO_BOUGHT_EPIC_SNACKS, 500000, 1",
"(int) MP#_NUMBER_OF_ORANGE_BOUGHT, 500000, 1",
"(int) MP#_CIGARETTES_BOUGHT, 500000, 1",
"Armor:",
"(int) MP#_MP_CHAR_ARMOUR_1_COUNT, 500000, 1",
"(int) MP#_MP_CHAR_ARMOUR_2_COUNT, 500000, 1",
"(int) MP#_MP_CHAR_ARMOUR_3_COUNT, 500000, 1",
"(int) MP#_MP_CHAR_ARMOUR_4_COUNT, 500000, 1",
"(int) MP#_MP_CHAR_ARMOUR_5_COUNT, 500000, 1",
"Heist Trophies:",
"(int) MPPLY_HEIST_ACH_TRACKER, -1, 1",
"Unlock All Hair:",
"(int) MP#_CLTHS_AVAILABLE_HAIR, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_7, -1, 1",
"Achievements:",
"(int) MP#_PLAYER_HEADSHOTS, 500, 1",
"(int) MP#_PISTOL_ENEMY_KILLS, 500, 1",
"(int) MP#_SAWNOFF_ENEMY_KILLS, 500, 1",
"(int) MP#_MICROSMG_ENEMY_KILLS, 500, 1",
"(int) MP#_SNIPERRFL_ENEMY_KILLS, 100, 1",
"(int) MP#_UNARMED_ENEMY_KILLS, 50, 1",
"(int) MP#_STKYBMB_ENEMY_KILLS, 50, 1",
"(int) MP#_GRENADE_ENEMY_KILLS, 50, 1",
"(int) MP#_RPG_ENEMY_KILLS, 50, 1",
"(int) MP#_CARS_EXPLODED, 500, 1",
"(int) MP#_AWD_5STAR_WANTED_AVOIDANCE, 50, 1",
"(int) MP#_AWD_CAR_BOMBS_ENEMY_KILLS, 25, 1",
"(int) MP#_AWD_CARS_EXPORTED, 50, 1",
"(int) MP#_PASS_DB_PLAYER_KILLS, 100, 1",
"(int) MP#_AWD_FM_DM_WINS, 50, 1",
"(int) MP#_AWD_FM_GOLF_WON, 25, 1",
"(int) MP#_AWD_FM_GTA_RACES_WON, 50, 1",
"(int) MP#_AWD_FM_SHOOTRANG_CT_WON, 25, 1",
"(int) MP#_AWD_FM_SHOOTRANG_RT_WON, 25, 1",
"(int) MP#_AWD_FM_SHOOTRANG_TG_WON, 25, 1",
"(int) MP#_AWD_FM_TDM_WINS, 50, 1",
"(int) MP#_AWD_FM_TENNIS_WON, 25, 1",
"(int) MP#_MOST_SPINS_IN_ONE_JUMP, 5, 1",
"(int) MPPLY_AWD_FM_CR_DM_MADE, 25, 1",
"(int) MP#_AWD_FMHORDWAVESSURVIVE, 10, 1",
"(int) MP#_AWD_HOLD_UP_SHOPS, 20, 1",
"(int) MP#_ASLTRIFLE_ENEMY_KILLS, 500, 1",
"(int) MP#_MG_ENEMY_KILLS, 500, 1",
"(int) MP#_AWD_LAPDANCES, 25, 1",
"(int) MP#_MOST_ARM_WRESTLING_WINS, 25, 1",
"(int) MP#_AWD_NO_HAIRCUTS, 25, 1",
"(int) MP#_AWD_RACES_WON, 50, 1",
"(int) MP#_AWD_SECURITY_CARS_ROBBED, 25, 1",
"(int) MP#_AWD_VEHICLES_JACKEDR, 500, 1",
"(int) MP#_MOST_FLIPS_IN_ONE_JUMP, 5, 1",
"(int) MP#_AWD_WIN_AT_DARTS, 25, 1",
"(int) MP#_AWD_PASSENGERTIME, 4, 1",
"(int) MP#_AWD_TIME_IN_HELICOPTER, 4, 1",
"(int) MP#_AWD_FM_DM_3KILLSAMEGUY, 50, 1",
"(int) MP#_AWD_FM_DM_KILLSTREAK, 100, 1",
"(int) MP#_AWD_FM_DM_STOLENKILL, 50, 1",
"(int) MP#_AWD_FM_DM_TOTALKILLS, 500, 1",
"(int) MP#_AWD_FM_GOLF_BIRDIES, 25, 1",
"(bool) MP#_AWD_FM_GOLF_HOLE_IN_1, 1, 1",
"(int) MP#_AWD_FM_RACE_LAST_FIRST, 25, 1",
"(int) MP#_AWD_FM_RACES_FASTEST_LAP, 25, 1",
"(bool) MP#_AWD_FM_SHOOTRANG_GRAN_WON, 1, 1",
"(int) MP#_AWD_FM_TDM_MVP, 50, 1",
"(int) MP#_AWD_FM_TENNIS_ACE, 25, 1",
"(bool) MP#_AWD_FM_TENNIS_STASETWIN, 1, 1",
"(bool) MP#_AWD_FM6DARTCHKOUT, 1, 1",
"(bool) MP#_AWD_FMATTGANGHQ, 1, 1",
"(int) MP#_AWD_PARACHUTE_JUMPS_20M, 25, 1",
"(int) MP#_AWD_PARACHUTE_JUMPS_50M, 25, 1",
"(int) MP#_AIR_LAUNCHES_OVER_40M, 25, 1",
"(bool) MP#_AWD_BUY_EVERY_GUN, 1, 1",
"(bool) MP#_AWD_FMWINEVERYGAMEMODE, 1, 1",
"(int) MP#_AWD_FMDRIVEWITHOUTCRASH, 255, 1",
"(int) MP#_AWD_FMCRATEDROPS, 25, 1",
"(bool) MP#_AWD_FM25DIFFERENTDM, 1, 1",
"(bool) MP#_AWD_FM_TENNIS_5_SET_WINS, 1, 1",
"(int) MPPLY_AWD_FM_CR_PLAYED_BY_PEEP, 100, 1",
"(int) MPPLY_AWD_FM_CR_RACES_MADE, 25, 1",
"(bool) MP#_AWD_FM25DIFFERENTRACES, 1, 1",
"(bool) MP#_AWD_FM25DIFITEMSCLOTHES, 1, 1",
"(bool) MP#_AWD_FMFULLYMODDEDCAR, 1, 1",
"(int) MP#_AWD_FMKILLBOUNTY, 25, 1",
"(int) MP#_KILLS_PLAYERS, 1000, 1",
"(bool) MP#_AWD_FMPICKUPDLCCRATE1ST, 1, 1",
"(int) MP#_AWD_FMSHOOTDOWNCOPHELI, 25, 1",
"(bool) MP#_AWD_FMKILL3ANDWINGTARACE, 1, 1",
"(bool) MP#_AWD_FMKILLSTREAKSDM, 1, 1",
"(bool) MP#_AWD_FMMOSTKILLSGANGHIDE, 1, 1",
"(bool) MP#_AWD_FMMOSTKILLSSURVIVE, 1, 1",
"(bool) MP#_AWD_FMRACEWORLDRECHOLDER, 1, 1",
"(int) MP#_AWD_FMRALLYWONDRIVE, 25, 1",
"(int) MP#_AWD_FMRALLYWONNAV, 25, 1",
"(int) MP#_AWD_FMREVENGEKILLSDM, 50, 1",
"(int) MP#_AWD_FMWINAIRRACE, 25, 1",
"(bool) MP#_AWD_FMWINCUSTOMRACE, 1, 1",
"(int) MP#_AWD_FMWINRACETOPOINTS, 25, 1",
"(int) MP#_AWD_FMWINSEARACE, 25, 1",
"(int) MP#_AWD_FMBASEJMP, 25, 1",
"(bool) MP#_MP0_AWD_FMWINALLRACEMODES, 1, 1",
"(bool) MP#_AWD_FMTATTOOALLBODYPARTS, 1, 1",
"(int) MP#_CHAR_WANTED_LEVEL_TIME5STAR, 2147483647, 1",
"(float) MP#_LONGEST_WHEELIE_DIST, 1000, 1",
"Fireworks:",
"(int) MP#_FIREWORK_TYPE_1_WHITE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_1_RED, 500000, 1",
"(int) MP#_FIREWORK_TYPE_1_BLUE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_2_WHITE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_2_RED, 500000, 1",
"(int) MP#_FIREWORK_TYPE_2_BLUE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_3_WHITE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_3_RED, 500000, 1",
"(int) MP#_FIREWORK_TYPE_3_BLUE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_4_WHITE, 500000, 1",
"(int) MP#_FIREWORK_TYPE_4_RED, 500000, 1",
"(int) MP#_FIREWORK_TYPE_4_BLUE, 500000, 1",
"Unlock All Clothing:",
"(int) MP#_CLTHS_AVAILABLE_HAIR, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_HAIR_7, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_OUTFIT, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_OUTFIT, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_JBIB_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_JBIB_7, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_LEGS_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_LEGS_7, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_FEET_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_FEET_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_8, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_9, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_PROPS_10, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_TEETH, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_TEETH_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_TEETH_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_TEETH, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_TEETH_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_TEETH_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_BERD_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_BERD_7, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_TORSO, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_TORSO, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_3, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_4, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_5, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_6, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL_7, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL2, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_SPECIAL2_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_1, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_3, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_4, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_5, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_6, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL_7, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL2, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_SPECIAL2_1, -1, 1",
"(int) MP#_CLTHS_AVAILABLE_DECL, -1, 1",
"(int) MP#_CLTHS_ACQUIRED_DECL, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_0, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_1, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_2, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_3, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_4, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_5, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_6, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_7, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_8, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_9, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_10, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_11, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_12, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_13, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_14, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_15, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_16, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_17, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_18, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_19, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_20, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_21, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_22, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_23, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_24, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_25, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_26, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_27, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_28, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_29, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_30, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_31, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_32, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_33, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_34, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_35, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_36, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_37, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_38, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_39, -1, 1",
"(int) MP#_DLC_APPAREL_ACQUIRED_40, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_1, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_2, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_3, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_4, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_5, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_6, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_7, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_8, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_9, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_10, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_11, -1, 1",
"(int) MP#_ADMIN_CLOTHES_GV_BS_12, -1, 1",
"Unlock All Weapons:",
"(int) MP#_ADMIN_WEAPON_GV_BS_1, -1, 1",
"(int) MP#_ADMIN_WEAPON_GV_BS_2, -1, 1",
"(int) MP#_ADMIN_WEAPON_GV_BS_3, -1, 1",
"(int) MP#_BOTTLE_IN_POSSESSION, -1, 1",
"(int) MP#_CHAR_FM_WEAP_UNLOCKED, -1, 1",
"(int) MP#_CHAR_FM_WEAP_UNLOCKED2, -1, 1",
"(int) MP#_CHAR_WEAP_FM_PURCHASE, -1, 1",
"(int) MP#_CHAR_WEAP_FM_PURCHASE2, -1, 1",
"(int) MP#_CHAR_FM_WEAP_ADDON_1_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_WEAP_ADDON_2_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_WEAP_ADDON_3_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_WEAP_ADDON_4_UNLCK, -1, 1",
"(int) MP#_CHAR_FM_WEAP_ADDON_5_UNLCK, -1, 1",
"(int) MP#_WEAP_FM_ADDON_PURCH, -1, 1",
"(int) MP#_WEAP_FM_ADDON_PURCH2, -1, 1",
"(int) MP#_WEAP_FM_ADDON_PURCH3, -1, 1",
"(int) MP#_WEAP_FM_ADDON_PURCH4, -1, 1",
"(int) MP#_WEAP_FM_ADDON_PURCH5, -1, 1",
"Infinite Ammo:",
"(int) MP#_FLAREGUN_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_GRNLAUNCH_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_RPG_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_MINIGUNS_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_GRENADE_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_SMKGRENADE_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_STKYBMB_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_MOLOTOV_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_PETROLCAN_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_PRXMINE_FM_AMMO_CURRENT, -1, 1",
"(int) MP#_HOMLNCH_FM_AMMO_CURRENT, -1, 1",
"Bypass Vehicle Sell Time:",
"(int) MPPLY_VEHICLE_SELL_TIME, 0, 1",
"Enable Roosevelt in Phone:",
"(int) MPPLY_VEHICLE_ID_ADMIN_WEB, 117401876, 1",
"(int) SHOOTING_ABILITY, 350, 1"
};
    }

