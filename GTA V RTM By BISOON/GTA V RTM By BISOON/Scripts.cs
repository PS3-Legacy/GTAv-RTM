using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GTA_V_RTM_By_BISOON;



public  class Scripts : RPC
{
    public  class PlayerInfo
    {
        public string IsInCar { get; set; }
        public string HasMic { get; set; }
        public string MutedMe { get; set; }
        public string IsMale { get; set; }
        public string InWater { get; set; }
        public string IsAlive { get; set; }
        public string VehicleName { get; set; }
        public string GetCoords { set; get; }
        public PlayerInfo(int player)
        {
            IsInCar = isInCar(player);
            HasMic = NETWORK_PLAYER_HAS_HEADSET(player);
            MutedMe = NETWORK_AM_I_MUTED_BY_PLAYER(player);
            IsMale = IS_PED_MALE(player);
            InWater = IS_ENTITY_IN_WATER(player);
            IsAlive = isAlive(player);
            GetCoords = getCoord(player);
            VehicleName = VehName(player);
        }

        string isInCar(int player)
        { return new Ped().IS_PED_IN_ANY_VEHICLE(new Ped().GET_PLAYER_PED(player)) ? "In Vehicle" : "Not in vehicle"; }
        string NETWORK_PLAYER_HAS_HEADSET(int player)
        {
            return new Network().NETWORK_PLAYER_HAS_HEADSET(player) ? " has mic" : " doesn't have mic";
        }
        string NETWORK_AM_I_MUTED_BY_PLAYER(int player)
        {
            return new Network().NETWORK_AM_I_MUTED_BY_PLAYER(player) ? "and he muted you" : "and he didn't mute you";
        }

        string IS_PED_MALE(int player)
        {
            return new Ped().IS_PED_MALE(new Ped().GET_PLAYER_PED(player)) ? "and his gender is Man" : "and his gender is Famle";
        }
        string IS_ENTITY_IN_WATER(int player)
        {
            return new Entity().IS_ENTITY_IN_WATER(new Ped().GET_PLAYER_PED(player)) ? "and he is in water" : "";
        }
        string isAlive(int player)
        {
            return new Entity().IS_ENTITY_DEAD(player) ? "and he dead" : "and yes he is alive";
        }
        string getCoord(int player)
        {
            float[] flt = new Entity().GET_ENTITY_COORDS(new Ped().GET_PLAYER_PED(player));
            return string.Format("His Current Location | X : {0}f, Y : {1}f, Z : {2}f", flt[0], flt[1], flt[2]);
        }
        string VehName(int player)
        {
            return isInCar(player) == "In Vehicle" ? " and his Vehicle is : " + new Vehicle().GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(new Vehicle().GET_VEHICLE_PED_IS_IN(new Ped().GET_PLAYER_PED(player))) : "";
        }
    }
    public enum Show
    {
        End = 0x02,
        Temp = 0x01,
        Perm = 0x03
    }
    public static class Colors
    {
        public  static string Red = "~r~",
            Blue = "~b~",
            Green = "~g~",
            Yellow = "~y~",
            Purple = "~p~",
            Orange = "~o~",
            Grey = "~c~",
            DarkGrey = "~m~",
            Black = "~u~",
            SkipLine = "~n~",
            White = "~s~",
            RockstarVerifiedIcon = "¦",
            RockstarIcon = "÷",
            RockstarIcon2 = "∑";
    }
    public  void TypeText(string text, Show type)
    {
        string txt = text;
        byte buff = (byte)type;
        PS3.Extension.WriteByte(0x20672EF/*0x2057217*/, buff);
        PS3.Extension.WriteString(0x2066BDC/*0x2056B04*/, txt);
    }

    public string PLAYER_NAME
    {
        set { PS3.Extension.WriteString(0x200255C, value); PS3.Extension.WriteString(0x41143344, value); }
        get { return PS3.Extension.ReadString(0x41143344); }
    }
    public static bool TimeOut(double secounds)
    {
        DateTime dt = DateTime.Now.AddSeconds(secounds);
        while (dt > DateTime.Now)
            Application.DoEvents();
        return true;
    }
    public static void Delay(double seconds)
    {
        DateTime dt = DateTime.Now.AddSeconds(seconds);
        while (dt > DateTime.Now)
            Application.DoEvents();
    }
    public void SetMultiOffsets(byte[] value, params uint[] addresses)
    {
        foreach (var item in addresses)
        {
            PS3.SetMemory(item, value);
        }
    }
    Character PlayerChar = Character.Character_one;
    public Character SetChar
    {
        set
        {
            PlayerChar = value;
        }
        get
        {
            return PlayerChar;
        }
    }

    public void STAT_SET_INT(string stat, int val, int savec)
    {
        try
        {

            if (stat.Substring(0, 6) == "MPPLY_")
            {
                Call("STAT_SET_INT", Hash(stat), val, savec);
            }
            else
            {
                if (PlayerChar == Character.Character_one)
                {
                    Call("STAT_SET_INT", Hash("MP0_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Two)
                {
                    Call("STAT_SET_INT", Hash("MP1_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Three)
                {
                    Call("STAT_SET_INT", Hash("MP2_" + stat), val, savec);
                }
            }
        }
        catch
        {
        }
    }
    public int STAT_GET_INT(string stat)
    {
        if (stat.Substring(0, 6) == "MPPLY_")
        {
            Call("STAT_GET_INT", Hash(stat), 0x10030040);
            return PS3.Extension.ReadInt32(0x10030040);
        }
        else
        {
            if (PlayerChar == Character.Character_one)
            {
                Call("STAT_GET_INT", Hash("MP0_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else if (PlayerChar == Character.Character_Two)
            {
                Call("STAT_GET_INT", Hash("MP1_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else if (PlayerChar == Character.Character_Three)
            {
                Call("STAT_GET_INT", Hash("MP2_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else
            {
                return 0;
            }
        }
    }

    public void STAT_SET_BOOL(string stat, int val, int savec)
    {
        try
        {
            if (stat.Substring(0, 6) == "MPPLY_")
            {
                Call("STAT_SET_BOOL", Hash(stat), val, savec);
            }
            else
            {
                if (PlayerChar == Character.Character_one)
                {
                    Call("STAT_SET_BOOL", Hash("MP0_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Two)
                {
                    Call("STAT_SET_BOOL", Hash("MP1_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Three)
                {
                    Call("STAT_SET_BOOL", Hash("MP2_" + stat), val, savec);
                }
            }
        }
        catch
        {
        }
    }

    public int STAT_GET_BOOL(string stat)
    {
        if (stat.Substring(0, 6) == "MPPLY_")
        {
            Call("STAT_GET_BOOL", Hash(stat), 0x10030040);
            return PS3.Extension.ReadInt32(0x10030040);
        }
        else
        {
            if (PlayerChar == Character.Character_one)
            {
                Call("STAT_GET_BOOL", Hash("MP0_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else if (PlayerChar == Character.Character_Two)
            {
                Call("STAT_GET_BOOL", Hash("MP1_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else if (PlayerChar == Character.Character_Three)
            {
                Call("STAT_GET_BOOL", Hash("MP2_" + stat), 0x10030040);
                return PS3.Extension.ReadInt32(0x10030040);
            }
            else
            {
                return 0;
            }
        }
    }
    public void STAT_SET_FLOAT(string stat, float val, int savec)
    {
        try
        {
            if (stat.Substring(0, 6) == "MPPLY_")
            {
                Call("STAT_SET_FLOAT", Hash(stat), val, savec);
            }
            else
            {
                if (PlayerChar == Character.Character_one)
                {
                    Call("STAT_SET_FLOAT", Hash("MP0_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Two)
                {
                    Call("STAT_SET_FLOAT", Hash("MP1_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Three)
                {
                    Call("STAT_SET_FLOAT", Hash("MP2_" + stat), val, savec);
                }
            }
        }
        catch
        {
        }
    }
    public float STAT_GET_FLOAT(string stat)
    {
        if (stat.Substring(0, 6) == "MPPLY_")
        {
            Call("STAT_GET_FLOAT", Hash(stat), 0x10030040);
            return PS3.Extension.ReadFloat(0x10030040);
        }
        else
        {
            if (PlayerChar == Character.Character_one)
            {
                Call("STAT_GET_FLOAT", Hash("MP0_" + stat), 0x10030040);
                return PS3.Extension.ReadFloat(0x10030040);
            }
            else if (PlayerChar == Character.Character_Two)
            {
                Call("STAT_GET_FLOAT", Hash("MP1_" + stat), 0x10030040);
                return PS3.Extension.ReadFloat(0x10030040);
            }
            else if (PlayerChar == Character.Character_Three)
            {
                Call("STAT_GET_FLOAT", Hash("MP2_" + stat), 0x10030040);
                return PS3.Extension.ReadFloat(0x10030040);
            }
            else
            {
                return 0;
            }
        }
    }
    private void STAT_SET_STRING(string stat, string val, int savec)
    {

        try
        {
            if (stat.Substring(0, 6) == "MPPLY_")
            {
                Call("STAT_SET_STRING", Hash(stat), val, savec);
            }
            else
            {
                if (PlayerChar == Character.Character_one)
                {
                    Call("STAT_SET_STRING", Hash("MP0_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Two)
                {
                    Call("STAT_SET_STRING", Hash("MP1_" + stat), val, savec);
                }
                else if (PlayerChar == Character.Character_Three)
                {
                    Call("STAT_SET_STRING", Hash("MP2_" + stat), val, savec);
                }
            }
        }
        catch
        {
        }
    }
    
    public enum ReturnType
    {
        ByteArray,
        Int32,
        Uint32,
    }
    public object STRING_TO(string stringValue, ReturnType T)
    {
        Call("STRING_TO_INT", stringValue, 0x10050730);
        object result = 0;
        if (T == ReturnType.Int32)
        {
            return PS3.Extension.ReadInt32(0x10050730);
        }
        else if (T == ReturnType.Uint32)
        {
            return PS3.Extension.ReadUInt32(0x10050730);
        }
        else if (T == ReturnType.ByteArray)
        {
            return BitConverter.ToString(PS3.Extension.ReadBytes(0x10050730, 4)).Replace("-", " ");
        }
        else return default(object);
    }


    public Vehicle Vehicle
    {
        get { return new Vehicle(); }
    }
    public Blip Blip
    {
        get { return new Blip(); }
    }
    public Ped Ped
    {
        get { return new Ped(); }
    }
    public Entity Entity
    {
        get { return new Entity(); }
    }
    public Cam Cam
    {
        get { return new Cam(); }
    }
    public PathFind PathFind
    {
        get { return new PathFind(); }
    }
    public Weapon Weapon
    {
        get { return new Weapon(); }
    }
    public Player Player
    {
        get { return new Player(); }
    }
    public Object Object
    {
        get { return new Object(); }
    }
    public Task Task
    {
        get { return new Task(); }
    }
    public Vector Vector
    {
        get { return new Vector(); }
    }
    public Controls Control
    {
        get { return new Controls(); }
    }
    public Fire Fire
    {
        get { return new Fire(); }
    }
    public Stats Stats
    {
        get { return new Stats(); }
    }
    public Buttonz Buttonz
    {
        get { return new Buttonz(); }
    }
    public GamePlay GamePlay
    {
        get { return new GamePlay(); }
    }
    public Mobile Mobile
    {
        get { return new Mobile(); }
    }
    public Zone Zone
    {
        get { return new Zone(); }
    }
    public PickUp Pickup
    {
        get { return new PickUp(); }
    }
    public Graphics Graphics
    {
        get { return new Graphics(); }
    }
    public Network Network
    {
        get { return new Network(); }
    }
    public Finder Finder
    {
        get { return new Finder(); }
    }
}

public class Vehicle : RPC
{
    public int GET_CLOSEST_VEHICLE
    {
        get
        {
            return Call("GET_CLOSEST_VEHICLE", new Blip().GetMyLocByBlip, 5000f, 0, 0);
        }
    }
    public string GET_DISPLAY_NAME_FROM_VEHICLE_MODEL(int vehId)
    {
        return PS3.Extension.ReadString((uint)Call("GET_DISPLAY_NAME_FROM_VEHICLE_MODEL", new Entity().GET_ENTITY_MODEL(vehId)));
    }
    public string GET_VEHICLE_NUMBER_PLATE_TEXT(int veh)
    {
        return PS3.Extension.ReadString((uint)Call("GET_VEHICLE_NUMBER_PLATE_TEXT", veh));
    }
    public int CREATE_PED_INSIDE_VEHICLE(int veh, string modelName, int seat)
    {
        uint ModelHash = Hash(modelName);
        new Streaming().REQUEST_MODEL(ModelHash);
        while (!new Streaming().HAS_MODEL_LOADED(ModelHash))
        {
            Application.DoEvents();
        }
        return Call("CREATE_PED_INSIDE_VEHICLE", veh, 1, ModelHash, seat, 1, 1);
    }
    public int GET_VEHICLE_PED_IS_IN(int ped)
    {
        return Call("GET_VEHICLE_PED_IS_IN", ped);
    }
    public int GET_MY_PLAYER_VEHICLE_IN
    {
        get
        {
            return Call("GET_VEHICLE_PED_IS_IN", new Ped().PLAYER_PED_ID);
        }
    }
    public int GET_MY_PLAYER_VEHICLE_IS_USING
    {
        get
        {
            return Call("GET_VEHICLE_PED_IS_USING", new Ped().PLAYER_PED_ID);
        }
    }
    public int GET_VEHICLE_PED_IS_USING(int ped)
    {
        return Call("GET_VEHICLE_PED_IS_USING", ped);
    }
    public int SPAWN_VEHICLE(object vehName, float[] Pos)
    {
        uint vehHash = 0;
        if (vehName is string)
            vehHash = Hash(vehName.ToString());
        else vehHash = (uint)vehName;
        new Streaming().REQUEST_MODEL(vehHash);
        int VehID = 0;
        for (int i = 0; i < 50; i++)
        {
            if (new Streaming().HAS_MODEL_LOADED(vehHash))
            {
                VehID = Create_Vehicle(vehHash, Pos);
                new Streaming().SET_MODEL_AS_NO_LONGER_NEEDED(vehHash);
                break;
            }
            Application.DoEvents();
        }
        return VehID;
    }
    int Create_Vehicle(uint hash, float[] Pos)
    {
        float[] flt = Pos;
        return Call("CREATE_VEHICLE", hash, flt, 0f, 1, 1);
    }

    public void DELETE_VEHICLE(int vehId)
    {
        Call("SET_ENTITY_AS_MISSION_ENTITY", vehId, 1, 1);
        PS3.Extension.WriteInt32(0x10030220, vehId);
        Call("DELETE_VEHICLE", 0x10030220);
    }
    public void SET_VEHICLE_LIGHT_MULTIPLIER(int vehicleID, float strength)
    {
        Call("SET_VEHICLE_LIGHT_MULTIPLIER", vehicleID, strength);
    }
    public enum INDICATOR
    {
        RIGHT,
        LEFT
    }
    public void SET_VEHICLE_INDICATOR_LIGHTS(int vehId, INDICATOR value, bool Toggle)
    {
        Call("SET_VEHICLE_INDICATOR_LIGHTS", vehId, (int)value, Toggle == true ? 1 : 0);
    }
    public void SET_VEHICLE_FIXED(int vehId)
    {
        Call("SET_VEHICLE_FIXED", vehId);
    }
    public void SET_PED_INTO_VEHICLE(int ped, int vehId, int seat)
    {
        Call("SET_PED_INTO_VEHICLE", ped, vehId, seat);
    }
    public void SET_VEHICLE_UNDRIVEABLE(int vehID, bool toggle)
    {
        Call("SET_VEHICLE_UNDRIVEABLE", vehID, toggle);
    }
    public void NETWORK_EXPLODE_VEHICLE(int vehId)
    {
        Call("NETWORK_EXPLODE_VEHICLE", vehId);
    }
    public void SET_VEHICLE_GRAVITY(int vehID, bool toggle)
    {
        Call("SET_VEHICLE_GRAVITY", vehID, toggle);
    }
    public void SET_VEHICLE_OUT_OF_CONTROL(int vehId)
    {
        Call("SET_VEHICLE_OUT_OF_CONTROL", vehId, 1, 1);
    }
    public  void Upgrade_Downgrade(int vehID, bool true_upgrade)
    {

        int VehID = vehID;
        if (true_upgrade)
        {
            Call("SET_VEHICLE_MOD_KIT", VehID, (int)0);
            Call("SET_VEHICLE_COLOURS", VehID, 120, 120);
            Call("SET_VEHICLE_NUMBER_PLATE_TEXT", VehID, Convert.ToString(" BISOON "));
            Call("SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX", VehID, (int)1);
            Call("TOGGLE_VEHICLE_MOD", VehID, (int)18, (int)1);
            Call("TOGGLE_VEHICLE_MOD", VehID, (int)22, (int)1);
            Call("SET_VEHICLE_MOD", VehID, (int)16, (int)4);
            Call("SET_VEHICLE_MOD", VehID, (int)12, (int)2);
            Call("SET_VEHICLE_MOD", VehID, (int)11, (int)3);
            Call("SET_VEHICLE_MOD", VehID, (int)14, (int)14);
            Call("SET_VEHICLE_MOD", VehID, (int)15, (int)3);
            Call("SET_VEHICLE_MOD", VehID, (int)13, (int)3);
            Call("SET_VEHICLE_WHEEL_TYPE", VehID, (int)7);
            Call("SET_VEHICLE_WINDOW_TINT", VehID, (int)2);
            Call("SET_VEHICLE_MOD", VehID, (int)23, (int)19, (int)1);
            Call("SET_VEHICLE_MOD", VehID, (int)0, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)1, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)2, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)3, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)4, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)5, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)6, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)7, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)8, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)9, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)10, 0);
            Call("SET_VEHICLE_MOD", VehID, (int)24, (int)12, (int)1);
        }
        else
        {
            Call("SET_VEHICLE_MOD_KIT", VehID, (int)0);
            Call("SET_VEHICLE_COLOURS", VehID, 0x84, 0x84);
            Call("SET_VEHICLE_NUMBER_PLATE_TEXT", VehID, Convert.ToString(" BISOON "));
            Call("SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX", VehID, (int)1);
            Call("TOGGLE_VEHICLE_MOD", VehID, (int)18, (int)0);
            Call("TOGGLE_VEHICLE_MOD", VehID, (int)22, (int)0);
            Call("SET_VEHICLE_MOD", VehID, (int)16, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)12, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)11, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)14, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)15, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)13, (int)-1);
            Call("SET_VEHICLE_WHEEL_TYPE", VehID, (int)-1);
            Call("SET_VEHICLE_WINDOW_TINT", VehID, (int)-1);
            Call("SET_VEHICLE_MOD", VehID, (int)23, (int)-1, (int)1);
            Call("SET_VEHICLE_MOD", VehID, (int)0, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)1, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)2, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)3, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)4, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)5, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)6, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)7, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)8, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)9, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)10, -1);
            Call("SET_VEHICLE_MOD", VehID, (int)24, (int)-1, (int)1);
        }
    }
    public void SET_VEHICLE_FORWARD_SPEED(int vehId, float speed)
    {
        Call("SET_VEHICLE_FORWARD_SPEED", vehId, speed);
    }
    public void SET_PLATE(int vehID, string newPlate)
    {
        Call("SET_VEHICLE_MOD_KIT", vehID, 0);
        Call("SET_VEHICLE_NUMBER_PLATE_TEXT", vehID, newPlate);
    }
    public void SET_VEHICLE_CUSTOM_PRIMARY_COLOUR(int vehId, int r, int g, int b)
    {
        Call("SET_VEHICLE_CUSTOM_PRIMARY_COLOUR", vehId, r, g, b);
    }
    public void SET_VEHICLE_CUSTOM_SECONDARY_COLOUR(int vehId, int r, int g, int b)
    {
        Call("SET_VEHICLE_CUSTOM_SECONDARY_COLOUR", vehId, r, g, b);
    }
    public void CLEAR_VEHICLE_CUSTOM_PRIMARY_COLOUR(int vehId)
    {
        Call("CLEAR_VEHICLE_CUSTOM_PRIMARY_COLOUR", vehId);
    }
    public enum ColorIds
    {
        Random = -1,
        Metallic_Black = 0,
        Metallic_Graphite_Black = 1,
        Metallic_Black_Steal = 2,
        Metallic_Dark_Silver = 3,
        Metallic_Silver = 4,
        Metallic_Blue_Silver = 5,
        Metallic_Steel_Gray = 6,
        Metallic_Shadow_Silver = 7,
        Metallic_Stone_Silver = 8,
        Metallic_Midnight_Silver = 9,
        Metallic_Gun_Metal = 10,
        Metallic_Anthracite_Grey = 11,
        Matte_Black = 12,
        Matte_Gray = 13,
        Matte_Light_Grey = 14,
        Util_Black = 15,
        Util_Black_Poly = 16,
        Util_Dark_silver = 17,
        Util_Silver = 18,
        Util_Gun_Metal = 19,
        Util_Shadow_Silver = 20,
        Worn_Black = 21,
        Worn_Graphite = 22,
        Worn_Silver_Grey = 23,
        Worn_Silver = 24,
        Worn_Blue_Silver = 25,
        Worn_Shadow_Silver = 26,
        Metallic_Red = 27,
        Metallic_Torino_Red = 28,
        Metallic_Formula_Red = 29,
        Metallic_Blaze_Red = 30,
        Metallic_Graceful_Red = 31,
        Metallic_Garnet_Red = 32,
        Metallic_Desert_Red = 33,
        Metallic_Cabernet_Red = 34,
        Metallic_Candy_Red = 35,
        Metallic_Sunrise_Orange = 36,
        Metallic_classic_Gold = 37,
        Metallic_Orange = 38,
        Matte_Red = 39,
        Matte_Dark_Red = 40,
        Matte_Orange = 41,
        Matte_Yellow = 42,
        Util_Red = 43,
        Util_Bright_Red = 44,
        Util_Garnet_Red = 45,
        Worn_Red = 46,
        Worn_Golden_Red = 47,
        Worn_Dark_Red = 48,
        Metallic_Dark_Green = 49,
        Metallic_Racing_Green = 50,
        Metallic_Sea_Green = 51,
        Metallic_Olive_Green = 52,
        Metallic_Green = 53,
        Metallic_Gasoline_Blue_Green = 54,
        Matte_Lime_Green = 55,
        Util_Dark_Green = 56,
        Util_Green = 57,
        Worn_Dark_Green = 58,
        Worn_Green = 59,
        Worn_Sea_Wash = 60,
        Metallic_Midnight_Blue = 61,
        Metallic_Dark_Blue = 62,
        Metallic_Saxony_Blue = 63,
        Metallic_Blue = 64,
        Metallic_Mariner_Blue = 65,
        Metallic_Harbor_Blue = 66,
        Metallic_Diamond_Blue = 67,
        Metallic_Surf_Blue = 68,
        Metallic_Nautical_Blue = 69,
        Metallic_Bright_Blue = 70,
        Metallic_Purple_Blue = 71,
        Metallic_Spinnaker_Blue = 72,
        Metallic_Ultra_Blue = 73,
        Metallic_Bright_Blue1 = 74,
        Util_Dark_Blue = 75,
        Util_Midnight_Blue = 76,
        Util_Blue = 77,
        Util_Sea_Foam_Blue = 78,
        Uil_Lightning_blue = 79,
        Util_Maui_Blue_Poly = 80,
        Util_Bright_Blue = 81,
        Matte_Dark_Blue = 82,
        Matte_Blue = 83,
        Matte_Midnight_Blue = 84,
        Worn_Dark_blue = 85,
        Worn_Blue = 86,
        Worn_Light_blue = 87,
        Metallic_Taxi_Yellow = 88,
        Metallic_Race_Yellow = 89,
        Metallic_Bronze = 90,
        Metallic_Yellow_Bird = 91,
        Metallic_Lime = 92,
        Metallic_Champagne = 93,
        Metallic_Pueblo_Beige = 94,
        Metallic_Dark_Ivory = 95,
        Metallic_Choco_Brown = 96,
        Metallic_Golden_Brown = 97,
        Metallic_Light_Brown = 98,
        Metallic_Straw_Beige = 99,
        Metallic_Moss_Brown = 100,
        Metallic_Biston_Brown = 101,
        Metallic_Beechwood = 102,
        Metallic_Dark_Beechwood = 103,
        Metallic_Choco_Orange = 104,
        Metallic_Beach_Sand = 105,
        Metallic_Sun_Bleeched_Sand = 106,
        Metallic_Cream = 107,
        Util_Brown = 108,
        Util_Medium_Brown = 109,
        Util_Light_Brown = 110,
        Metallic_White = 111,
        Metallic_Frost_White = 112,
        Worn_Honey_Beige = 113,
        Worn_Brown = 114,
        Worn_Dark_Brown = 115,
        Worn_straw_beige = 116,
        Brushed_Steel = 117,
        Brushed_Black_steel = 118,
        Brushed_Aluminium = 119,
        Chrome = 120,
        Worn_Off_White = 121,
        Util_Off_White = 122,
        Worn_Orange = 123,
        Worn_Light_Orange = 124,
        Metallic_Securicor_Green = 125,
        Worn_Taxi_Yellow = 126,
        police_car_blue = 127,
        Matte_Green = 128,
        Matte_Brown = 129,
        Worn_Orange1 = 130,
        Matte_White = 131,
        Worn_White = 132,
        Worn_Olive_Army_Green = 133,
        Pure_White = 134,
        Hot_Pink = 135,
        Salmon_pink = 136,
        Metallic_Vermillion_Pink = 137,
        Orange = 138,
        Green = 139,
        Blue = 140,
        Mettalic_Black_Blue = 141,
        Metallic_Black_Purple = 142,
        Metallic_Black_Red = 143,
        hunter_green = 144,
        Metallic_Purple = 145,
        Metaillic_V_Dark_Blue = 146,
        MODSHOP_BLACK1 = 147,
        Matte_Purple = 148,
        Matte_Dark_Purple = 149,
        Metallic_Lava_Red = 150,
        Matte_Forest_Green = 151,
        Matte_Olive_Drab = 152,
        Matte_Desert_Brown = 153,
        Matte_Desert_Tan = 154,
        Matte_Foilage_Green = 155,
        DEFAULT_ALLOY_COLOR = 156,
        Epsilon_Blue = 157,
    }
    public void SET_VEHICLE_COLOURS(int vehId, ColorIds primaryColor, ColorIds secondaryColor)
    {
        Call("SET_VEHICLE_COLOURS", vehId, (int)primaryColor, (int)secondaryColor);
    }
    public void SET_VEHICLE_SIREN(int vehId, bool toggle)
    {
        Call("SET_VEHICLE_SIREN", vehId, toggle);
    }
    public void START_VEHICLE_ALARM(int vehId)
    {
        Call("SET_VEHICLE_ALARM", vehId, 1);
        Call("START_VEHICLE_ALARM", vehId);
    }
    public void OPEN_DOORS(int vehId, bool toggle)
    {
        string address = toggle == true ? "SET_VEHICLE_DOOR_OPEN" : "SET_VEHICLE_DOOR_SHUT";
        for (int i = 0; i < 8; i++)
        {
            Call(address, vehId, i, 1, 1);
            Application.DoEvents();
        }
    }
    public int TELEPORT_TO_CLOSEST_VEHICLE()
    {
        int closestVeh = GET_CLOSEST_VEHICLE;
        int myPed = new Ped().PLAYER_PED_ID;
        SET_PED_INTO_VEHICLE(myPed, closestVeh, -1);
        return closestVeh;
    }

}
public class Blip : RPC
{
    public enum Blips
    {
        Defult = 0,
        Standard = 1,
        BigBlip = 2,
        PoliceOfficer = 3,
        PoliceArea = 4,
        Square = 5,
        Player = 6,
        North = 7,
        Waypoint = 8,
        BigCircle = 9,
        BigCircleOutline = 10,
        ArrowUpOutlined = 11,
        ArrowDownOutlined = 12,
        ArrowUp = 13,
        ArrowDown = 14,
        PoliceHelicopterAnimated = 15,
        Jet = 16,
        Number1 = 17,
        Number2 = 18,
        Number3 = 19,
        Number4 = 20,
        Number5 = 21,
        Number6 = 22,
        Number7 = 23,
        Number8 = 24,
        Number9 = 25,
        Number10 = 26,
        GTAOCrew = 27,
        GTAOFriendly = 28,
        Lift = 36,
        RaceFinish = 38,
        Safehouse = 40,
        PoliceOfficer2 = 41,
        PoliceCarDot = 42,
        PoliceHelicopter = 43,
        ChatBubble = 47,
        Garage2 = 50,
        Drugs = 51,
        Store = 52,
        PoliceCar = 56,
        PolicePlayer = 58,
        PoliceStation = 60,
        Hospital = 61,
        Helicopter = 64,
        StrangersAndFreaks = 65,
        ArmoredTruck = 66,
        TowTruck = 68,
        Barber = 71,
        LosSantosCustoms = 72,
        Clothes = 73,
        TattooParlor = 75,
        Simeon = 76,
        Lester = 77,
        Michael = 78,
        Trevor = 79,
        Rampage = 84,
        VinewoodTours = 85,
        Lamar = 86,
        Franklin = 88,
        Chinese = 89,
        Airport = 90,
        Bar = 93,
        BaseJump = 94,
        CarWash = 100,
        ComedyClub = 102,
        Dart = 103,
        FIB = 106,
        DollarSign = 108,
        Golf = 109,
        AmmuNation = 110,
        Exile = 112,
        ShootingRange = 119,
        Solomon = 120,
        StripClub = 121,
        Tennis = 122,
        Triathlon = 126,
        OffRoadRaceFinish = 127,
        Key = 134,
        MovieTheater = 135,
        Music = 136,
        Marijuana = 140,
        Hunting = 141,
        ArmsTraffickingGround = 147,
        Nigel = 149,
        AssaultRifle = 150,
        Bat = 151,
        Grenade = 152,
        Health = 153,
        Knife = 154,
        Molotov = 155,
        Pistol = 156,
        RPG = 157,
        Shotgun = 158,
        SMG = 159,
        Sniper = 160,
        SonicWave = 161,
        PointOfInterest = 162,
        GTAOPassive = 163,
        GTAOUsingMenu = 164,
        Link = 171,
        Minigun = 173,
        GrenadeLauncher = 174,
        Armor = 175,
        Castle = 176,
        Camera = 184,
        Handcuffs = 188,
        Yoga = 197,
        Cab = 198,
        Number11 = 199,
        Number12 = 200,
        Number13 = 201,
        Number14 = 202,
        Number15 = 203,
        Number16 = 204,
        Shrink = 205,
        Epsilon = 206,
        PersonalVehicleCar = 225,
        PersonalVehicleBike = 226,
        Custody = 237,
        ArmsTraffickingAir = 251,
        Fairground = 266,
        PropertyManagement = 267,
        Altruist = 269,
        Enemy = 270,
        Chop = 273,
        Dead = 274,
        Hooker = 279,
        Friend = 280,
        BountyHit = 303,
        GTAOMission = 304,
        GTAOSurvival = 305,
        CrateDrop = 306,
        PlaneDrop = 307,
        Sub = 308,
        Race = 309,
        Deathmatch = 310,
        ArmWrestling = 311,
        AmmuNationShootingRange = 313,
        RaceAir = 314,
        RaceCar = 315,
        RaceSea = 316,
        GarbageTruck = 318,
        SafehouseForSale = 350,
        Package = 351,
        MartinMadrazo = 352,
        EnemyHelicopter = 353,
        Boost = 354,
        Devin = 355,
        Marina = 356,
        Garage = 357,
        GolfFlag = 358,
        Hangar = 359,
        Helipad = 360,
        JerryCan = 361,
        Masks = 362,
        HeistSetup = 363,
        Incapacitated = 364,
        PickupSpawn = 365,
        BoilerSuit = 366,
        Completed = 367,
        Rockets = 368,
        GarageForSale = 369,
        HelipadForSale = 370,
        MarinaForSale = 371,
        HangarForSale = 372,
        Business = 374,
        BusinessForSale = 375,
        RaceBike = 376,
        Parachute = 377,
        TeamDeathmatch = 378,
        RaceFoot = 379,
        VehicleDeathmatch = 380,
        Barry = 381,
        Dom = 382,
        MaryAnn = 383,
        Cletus = 384,
        Josh = 385,
        Minute = 386,
        Omega = 387,
        Tonya = 388,
        Paparazzo = 389,
        Crosshair = 390,
        Creator = 398,
        CreatorDirection = 399,
        Abigail = 400,
        Blimp = 401,
        Repair = 402,
        Testosterone = 403,
        Dinghy = 404,
        Fanatic = 405,
        Information = 407,
        CaptureBriefcase = 408,
        LastTeamStanding = 409,
        Boat = 410,
        CaptureHouse = 411,
        JerryCan2 = 415,
        RP = 416,
        GTAOPlayerSafehouse = 417,
        GTAOPlayerSafehouseDead = 418,
        CaptureAmericanFlag = 419,
        CaptureFlag = 420,
        Tank = 421,
        HelicopterAnimated = 422,
        Plane = 423,
        PlayerNoColor = 425,
        GunCar = 426,
        Speedboat = 427,
        Heist = 428,
        Stopwatch = 430,
        DollarSignCircled = 431,
        Crosshair2 = 432,
        DollarSignSquared = 434,
    }
    public enum  BlipColor
	{
		White = 0,
		Red = 1,
		Green = 2,
		Blue = 3,
		Yellow = 66,
	};
    public float[] GetMyLocByBlip
    {
        get
        {
            Call("GET_BLIP_COORDS", 0x10010500, GET_MAIN_PLAYER_BLIP_ID);
            return PS3.Extension.ReadFloat(0x10010500, 3);
        }
    }
    public bool DOES_BLIP_EXIST(int blip)
    {
        return Convert.ToBoolean(Call("DOES_BLIP_EXIST", blip));
    }
    public void REMOVE_BLIP(int blip)
    {
        if (DOES_BLIP_EXIST(blip))
        {
            PS3.Extension.WriteInt32(0x10020350, blip);
            Call("REMOVE_BLIP", 0x10020350);
        }
    }
    public int ADD_CUSTOM_BLIP_TO_ENTITY(int entity, BlipColor color, Blips blipindex, int blipalpha, float blipscale)
    {
        int blip = ADD_BLIP_FOR_ENTITY(entity);
        if (blipindex != 0)
            SET_BLIP_SPRITE(blip, blipindex);
        SET_BLIP_SCALE(blip, blipscale);
        SET_BLIP_ALPHA(blip, blipalpha);
        SET_BLIP_FLASH_TIMER(blip, 10000);
        SET_BLIP_COLOUR(blip, color);
        return blip;
    }
    public int ADD_CUSTOM_BLIP_TO_ENTITY(float[] loc, BlipColor color, Blips blipindex, int blipalpha, float blipscale)
    {
        int blip = ADD_BLIP_FOR_COORD(loc);
        SET_BLIP_COLOUR(blip, color);
        if (blipindex != 0)
            SET_BLIP_SPRITE(blip, blipindex);
        SET_BLIP_SCALE(blip, blipscale);
        SET_BLIP_ALPHA(blip, blipalpha);
        SET_BLIP_FLASH_TIMER(blip, 10000);
        return blip;
    }
    public int SET_BLIP_FLASH_TIMER(int blip, int duration)
    {
        Call("SET_BLIP_FLASHES", blip, 1);
        return Call("SET_BLIP_FLASH_TIMER", blip, duration);
    }
    public int SET_BLIP_SPRITE(int blip, Blips blipIndex)
    {
        return Call("set_blip_sprite", blip, (int)blipIndex);
    }
    public int SET_BLIP_SCALE(int blip, float scale)
    {
        return Call("set_blip_scale", blip, scale);
    }
    public int ADD_BLIP_FOR_ENTITY(int entity)
    {
        return Call("ADD_BLIP_FOR_ENTITY", entity);
    }
    public int ADD_BLIP_FOR_COORD(float[] loc)
    {
        return Call("ADD_BLIP_FOR_COORD", loc);
    }
    public int SET_BLIP_COLOUR(int blip, BlipColor color)
    {
        return Call("SET_BLIP_COLOUR", blip, (int)color);
    }
    public int SET_BLIP_ALPHA(int blip, int alpha)
    {
        return Call("SET_BLIP_ALPHA", blip, alpha);
    }
    public int GET_MAIN_PLAYER_BLIP_ID
    {
        get
        {
            return Call("get_main_player_blip_id");
        }
    }
    public int GET_BLIP_SPRITE
    {
        get
        {
            return Call("GET_BLIP_SPRITE", GET_MAIN_PLAYER_BLIP_ID);
        }
    }
    public int _GET_BLIP_SPRITE(int blip)
    {
        return Call("GET_BLIP_SPRITE", blip);
    }
}

public class Entity : RPC
{
    public bool IS_THIS_MODEL_A_CAR(uint modelHash)
    {
        return Convert.ToBoolean(Call("IS_THIS_MODEL_A_CAR", modelHash));
    }
    public uint GET_ENTITY_MODEL(int entity)
    {
        return (uint)Call("GET_ENTITY_MODEL", entity);
    }
    public float[] GET_ENTITY_COORDS(int Entity)
    {
        Call("GET_ENTITY_COORDS", new object[] { 0x10030000, Entity });
        return PS3.Extension.ReadFloat(0x10030000, 3);
    }
    public void DELETE_ENTITY(int entity)
    {
        PS3.Extension.WriteInt32(0x10030460, entity);
        Call("SET_ENTITY_AS_MISSION_ENTITY", entity, 0, 1);
        Call("DELETE_ENTITY", 0x10030460);
    }
    public void SET_ENTITY_COORDS(int Entity, float[] coords)
    {
        Call("SET_ENTITY_COORDS", new object[] { Entity, coords, 1, 0, 0, 1 });
    }
    public int GET_ENTITY_TYPE(int entity)
    {
        return Call("GET_ENTITY_TYPE", entity);
    }
    public int _GET_AIMED_ENTITY(int player)
    {
        Call("0x8866D9D0", player, 0x10030400);
        return PS3.Extension.ReadInt32(0x10030400);
    }
    public int Attach_Entity_To_Entity(int ent1, int ent2)
    {
        return Call("ATTACH_ENTITY_TO_ENTITY", new object[] { ent1, ent2, 0, 0x10030010, 0x10030010, 0, 0, 0, 0, 2, 0 });
    }
    public int DETACH_ENTITY(int entity)
    {
        return Call("DETACH_ENTITY", entity, 1, 1);
    }
    public void SET_ENTITY_PROOFS(int entity, bool toggle)
    {
        Call("SET_ENTITY_PROOFS", entity, toggle, toggle, toggle, toggle, toggle, toggle, toggle, toggle, toggle, toggle);
    }
    public void SET_ENTITY_INVINCIBLE(int entity, bool toggle)
    {
        Call("SET_ENTITY_INVINCIBLE", entity, toggle);
    }
    public void SET_ENTITY_VISIBLE(int entity, bool toggle)
    {
        Call("SET_ENTITY_VISIBLE", entity, toggle);
    }
    public void SET_ENTITY_HEALTH(int entity, int health)
    {
        Call("SET_ENTITY_HEALTH", entity, health);
    }
    public void SET_ENTITY_ALPHA(int entity, int alpha)
    {
        Call("SET_ENTITY_ALPHA", entity, alpha, 1);
    }
    public int GET_ENTITY_ALPHA(int entity)
    {
        return Call("GET_ENTITY_ALPHA", entity);
    }
    public void RESET_ENTITY_ALPHA(int entity)
    {
        Call("RESET_ENTITY_ALPHA", entity);
    }
    public bool IS_ENTITY_DEAD(int entity)
    {
        return Convert.ToBoolean(Call("IS_ENTITY_DEAD", entity));
    }
    public bool IS_ENTITY_IN_WATER(int entity)
    {
        return Convert.ToBoolean(Call("IS_ENTITY_IN_WATER", entity));
    }
    public void FREEZE_ENTITY_POSITION(int entity, bool toggle)
    {
        Call("FREEZE_ENTITY_POSITION", entity, toggle);
    }
}

public class Ped : RPC
{
    public int GET_PLAYER_PED(int player)
    {
        return Call("GET_PLAYER_PED", player);
    }
    public void DELETE_PED(int ped)
    {
        PS3.Extension.WriteInt32(0x10030500, ped);
        Call("DELETE_PED", 0x10030500);
    }
    public int PLAYER_PED_ID
    {
        get { return Call("PLAYER_PED_ID"); }
    }
    public int SET_PED_RANDOM_COMPONENT_VARIATION(int ped)
    {
        return Call("SET_PED_RANDOM_COMPONENT_VARIATION", ped, 1);
    }
    public int PLAYER_PED_ARMOUR
    {
        get { return Call("GET_PED_ARMOUR", new Ped().PLAYER_PED_ID); }
        set { Call("SET_PED_ARMOUR", new Ped().PLAYER_PED_ID, value); }
    }
    public int CLONE_PED(int ped)
    {
        return Call("CLONE_PED", ped, 1, 1, 1);
    }
    public int CREATE_PED(object modelName, float[] Pos)
    {
        int spawnedPed = 0;
        uint modelHash = 0;
        if (Pos[0] != 0)
        {
            Streaming stream = new Streaming();
            if (modelName is string)
                modelHash = Hash(modelName.ToString());
            else modelHash = (uint)modelName;
            PS3.InitTarget();
            stream.REQUEST_MODEL(modelHash);
            for (int i = 0; i < 3000; i++)
            {
                if (stream.HAS_MODEL_LOADED(modelHash))
                {
                    spawnedPed = Call("CREATE_PED", 1, modelHash, Pos, 0f, 1, 1);
                    stream.SET_MODEL_AS_NO_LONGER_NEEDED(modelHash);
                    new Entity().SET_ENTITY_COORDS(spawnedPed, Pos);
                    break;
                }
            }
        }
        return spawnedPed;
    }
    public void SET_PED_AS_GROUP_MEMBER(int ped, int groupId)
    {
        Call("SET_PED_AS_GROUP_MEMBER", ped, groupId);
    }
    public void SET_PED_AS_GROUP_LEADER(int ped, int groupId)
    {
        Call("SET_PED_AS_GROUP_LEADER", ped, groupId);
    }
    public void SET_PED_NEVER_LEAVES_GROUP(int ped, bool toggle)
    {
        Call("SET_PED_NEVER_LEAVES_GROUP", ped, toggle);
    }
    public void SET_PED_COMBAT_ABILITY(int ped)
    {
        Call("SET_PED_COMBAT_ABILITY", ped, 100);
    }
    public void SET_PED_CAN_SWITCH_WEAPON(int ped, bool toggle)
    {
        Call("SET_PED_CAN_SWITCH_WEAPON", ped, toggle);
    }
    public void PED_AS_BODY_GUARD(int ped, int leader, int groupId)
    {
        SET_PED_AS_GROUP_LEADER(leader, groupId);
        SET_PED_AS_GROUP_MEMBER(ped, groupId);
        SET_PED_NEVER_LEAVES_GROUP(ped, true);
        SET_PED_CAN_SWITCH_WEAPON(ped, true);
        SET_PED_COMBAT_ABILITY(ped);
    }
    public void SET_CAN_ATTACK_FRIENDLY(int ped)
    {
        Call("SET_CAN_ATTACK_FRIENDLY", ped, 1, 0);
    }
    public void SET_PED_CAN_BE_TARGETTED(int ped, bool toggle)
    {
        Call("SET_PED_CAN_BE_TARGETTED", ped, toggle);
    }
    public void REMOVE_PED_FROM_GROUP(int ped)
    {
        Call("REMOVE_PED_FROM_GROUP", ped);
    }
    public void SET_PED_COORDS_NO_GANG(int ped, float[] pos)
    {
        Call("SET_PED_COORDS_NO_GANG", ped, pos);
    }
    public  void SET_PED_SHOOTS_AT_COORD(int ped, float[] targetXYZ)
    {
        Call("SET_PED_SHOOTS_AT_COORD", ped, targetXYZ, 1);
    }
    public bool IS_PED_IN_ANY_VEHICLE(int ped)
    {
        return Convert.ToBoolean(Call("IS_PED_IN_ANY_VEHICLE", ped, 0));
    }
    public bool IS_PED_MALE(int ped)
    {
        return Convert.ToBoolean(Call("IS_PED_MALE", ped));
    }
    public void DUNCE_CAP(bool toggle)
    {
        Call("SET_PED_PROP_INDEX", new object[] { new Ped().PLAYER_PED_ID, 0, toggle == true ? 1 : 0, 0, 0 });
    }
    public void DEBUG_MODEL(bool toggle)
    {
        Call("SET_PED_LOD_MULTIPLIER", new Ped().PLAYER_PED_ID, toggle == true ? -1f : 1f);
    }
    public void SET_PED_RANDOM_PROPS(int ped)
    {
        Call("SET_PED_RANDOM_PROPS", ped);
    }
    public void APPLY_DAMAGE_TO_PED(int ped, int damageLvl)
    {
        Call("APPLY_DAMAGE_TO_PED", ped, damageLvl, 1);
    }

}
public class Cam : RPC
{
    bool patched = false;
    public void ATTACH_CAM_TO_ENTITY(int entity, bool value)
    {
        int cam = 0;
        if (!patched)
            new Scripts().SetMultiOffsets(new byte[] { 0x60, 00, 00, 00 }, 0x17F778C, 0x3A4048, 0x3A4050, 0x3A405C);
        cam = Call("CREATE_CAM", "DEFAULT_SCRIPTED_CAMERA", 1);
        Call("ATTACH_CAM_TO_ENTITY", cam, entity, 1f, -2f, 0f, 1);
        Call("set_cam_active", cam, value);
        Call("render_script_cams", value, 0, cam, 0, 0);
        patched = true;
    }
    void SET_GAMEPLAY_CAM_RELATIVE_PITCH()
    {
        Call("SET_GAMEPLAY_CAM_RELATIVE_PITCH", new Blip().GetMyLocByBlip[0], 9.000000f);
    }
    public void _ANIMATE_GAMEPLAY_CAM_ZOOM()
    {
        SET_GAMEPLAY_CAM_RELATIVE_PITCH();
        Call("0x77340650", 1f, 10000f);
    }
    public float[] GET_GAMEPLAY_CAM_COORD
    {
        get
        {
            Call("GET_GAMEPLAY_CAM_COORD", 0x10030150);
            return PS3.Extension.ReadFloat(0x10030150, 3);
        }
    }
    public enum ShakeType
    {
        FAMILY5_DRUG_TRIP_SHAKE,
        ROAD_VIBRATION_SHAKE
    }
    public void SHAKE_GAMEPLAY_CAM(ShakeType Mode, bool value, float val = 5f)
    {
        Call("SHAKE_GAMEPLAY_CAM", Mode.ToString(), value == true ? val : 0f);
    }
    public void _ENABLE_GAMEPLAY_CAM(bool value)
    {
        Call("0xE3802533", value);
    }
}
public class Weapon : RPC
{
    public enum WeaponTint
    {
        Normal,
        Green,
        Gold,
        Pink,
        Army,
        LSPD,
        Orange,
        Platinum,
    }

    public enum Weapons : uint
    {
        ADVANCEDRIFLE = 0xaf113f99,
        ANIMAL = 0xf9fbaebe,
        APPISTOL = 0x22d8fe39,
        ASSAULTRIFLE = 0xbfefff6d,
        ASSAULTSHOTGUN = 0xe284c527,
        ASSAULTSMG = 0xefe7e2df,
        BALL = 0x23c9f95c,
        BOTTLE = 0xf9e6aa4b,
        BRIEFCASE = 0x88c78eb7,
        BRIEFCASE_02 = 0x1b79f17,
        BULLPUPRIFLE = 0x7f229f94,
        BULLPUPSHOTGUN = 0x9d61e50f,
        BZGAS = 0xa0973d5e,
        CARBINERIFLE = 0x83bf0278,
        COMBATMG = 0x7fd62962,
        COMBATPDW = 0xa3d4d34,
        COMBATPISTOL = 0x5ef9fec4,
        COUGAR = 0x8d4be52,
        CROWBAR = 0x84bd7bfd,
        DAGGER = 0x92a27487,
        DIGISCANNER = 0xfdbadced,
        FIREEXTINGUISHER = 0x60ec506,
        FIREWORK = 0x7f7497e5,
        FLARE = 0x497facc3,
        FLAREGUN = 0x47757124,
        GOLFCLUB = 0x440e4788,
        GRENADE = 0x93e220bd,
        GRENADELAUNCHER = 0xa284510b,
        GRENADELAUNCHER_SMOKE = 0x4dd2dc56,
        GUSENBERG = 0x61012683,
        HAMMER = 0x4e875f73,
        HEAVYPISTOL = 0xd205520e,
        HEAVYSHOTGUN = 0x3aabbbaa,
        HEAVYSNIPER = 0xc472fe2,
        HOMINGLAUNCHER = 0x63ab0442,
        KNIFE = 0x99b507ea,
        MARKSMANRIFLE = 0xc734385a,
        MICROSMG = 0x13532244,
        MINIGUN = 0x42bf8a85,
        MOLOTOV = 0x24b17070,
        MUSKET = 0xa89cb99e,
        NIGHTSTICK = 0x678b81b1,
        PETROLCAN = 0x34a67b97,
        PISTOL = 0x1b06d571,
        PISTOL50 = 0x99aeeb3b,
        PROXMINE = 0xab564b93,
        PUMPSHOTGUN = 0x1d073a89,
        SAWNOFFSHOTGUN = 0x7846a318,
        SMOKEGRENADE = 0xfdbc8a50,
        SNIPERRIFLE = 0x5fc3c11,
        SNOWBALL = 0x787f0bb,
        SNSPISTOL = 0xbfd21232,
        SPECIALCARBINE = 0xc0a3098d,
        STICKYBOMB = 0x2c3731d9,
        STINGER = 0x687652ce,
        STUNGUN = 0x3656c8c1,
        UNARMED = 0xa2719263,
        VINTAGEPISTOL = 0x83839c4,
    }
    public void SET_PED_WEAPON_TINT_INDEX(int ped, uint hashWeapon, WeaponTint colorIndex)
    {
        Call("0xEB2A7B23", ped, hashWeapon, (int)colorIndex);
    }
    public float[] GET_PED_LAST_WEAPON_IMPACT_COORD(int ped)
    {
        Call("GET_PED_LAST_WEAPON_IMPACT_COORD", ped, 0x10030450);
        return PS3.Extension.ReadFloat(0x10030450, 3);
    }
    public int GET_CURRENT_PED_WEAPON(int ped)
    {
        Call("GET_CURRENT_PED_WEAPON", ped, 0x10030340, 1);
        return PS3.Extension.ReadInt32(0x10030340);
    }
    public int GET_SELECTED_PED_WEAPON(int ped)
    {
        return Call("GET_SELECTED_PED_WEAPON", ped);
    }
    public int GET_AMMO_IN_CLIP(int ped)
    {
        int pedID = ped;
        int weaponHash = GET_SELECTED_PED_WEAPON(pedID);
        Call("GET_AMMO_IN_CLIP", pedID, weaponHash, 0x10030280);
        return PS3.Extension.ReadInt32(0x10030280);
    }
    public int GIVE_WEAPON_TO_PED(int ped, Weapons weapon)
    {
        uint weaponHash = (uint)weapon;
        return Call("GIVE_WEAPON_TO_PED", ped, weaponHash, -1, 1,1);
    }
    public int GIVE_WEAPON_TO_PED(int ped, uint weapon)
    {
        uint weaponHash = (uint)weapon;
        return Call("GIVE_WEAPON_TO_PED", ped, weaponHash, -1, 1, 1);
    }
    public void REMOVE_ALL_PED_WEAPONS(int ped)
    {
        Call("REMOVE_ALL_PED_WEAPONS", ped, 1);
    }
    public void SET_PED_INFINITE_AMMO_CLIP(int ped, bool toggle)
    {
        Call("SET_PED_INFINITE_AMMO_CLIP", ped, toggle);
    }
    public void SET_PLAYER_MELEE_WEAPON_DAMAGE_MODIFIER(int player, float modifier)
    {
        Call("SET_PLAYER_MELEE_WEAPON_DAMAGE_MODIFIER", player, modifier);
    }
    public void SET_PLAYER_WEAPON_DAMAGE_MODIFIER(int player, float modifier)
    {
        Call("SET_PLAYER_WEAPON_DAMAGE_MODIFIER", player, modifier);
    }
    public void SET_PLAYER_WEAPON_DEFENSE_MODIFIER(int player, float modifier)
    {
        Call("SET_PLAYER_WEAPON_DEFENSE_MODIFIER", player, modifier);
    }
    public void SET_PLAYER_MELEE_WEAPON_DEFENSE_MODIFIER(int player, float modifier)
    {
        Call("SET_PLAYER_MELEE_WEAPON_DEFENSE_MODIFIER", player, modifier);
    }
}
public class PathFind : RPC
{
    public string StreetName
    {
        get
        {
            Call("GET_STREET_NAME_AT_COORD", new Entity().GET_ENTITY_COORDS(new Ped().PLAYER_PED_ID), 0x10030290, 0x10030294);
            return PS3.Extension.ReadString((uint)Call("GET_STREET_NAME_FROM_HASH_KEY", PS3.Extension.ReadInt32(0x10030290))) + "\n" + PS3.Extension.ReadString((uint)Call("GET_STREET_NAME_FROM_HASH_KEY", PS3.Extension.ReadInt32(0x10030294)));
        }
    }
}
public class Player : RPC
{
    public string GET_PLAYER_NAME(int player)
    {
        return PS3.Extension.ReadString((uint)Call("GET_PLAYER_NAME", player)).Replace("**Invalid**", "Null");
    }
    public int PLAYER_ID
    {
        get
        {
            return Call("PLAYER_ID");
        }
    }
    public void CHANGE_PLAYER_PED(int playerIndex, int ped)
    {
        int currentPed = new Ped().PLAYER_PED_ID;
        if (currentPed != ped)
            Call("CHANGE_PLAYER_PED", playerIndex, ped, 0, 0);
    }
    public int SET_PLAYER_MODEL(int player, string modelName)
    {
        return Call("SET_PLAYER_MODEL", player, Hash(modelName));
    }
    public int GET_NUMBER_OF_PLAYERS
    {
        get
        {
            return Call("GET_NUMBER_OF_PLAYERS");
        }
    }
    public int GET_PLAYER_TEAM(int player)
    {
        return Call("GET_PLAYER_TEAM", player);
    }
    public int SET_PLAYER_TEAM(int player, int team)
    {
        return Call("SET_PLAYER_TEAM", player, team);
    }
    int SET_PLAYER_WANTED_LEVEL(int player, int level)
    {
        return Call("SET_PLAYER_WANTED_LEVEL", player, level, 0);
    }
    public int SET_PLAYER_WANTED_LEVEL_NOW(int player, int level)
    {
        SET_PLAYER_WANTED_LEVEL(player, level);
        return Call("SET_PLAYER_WANTED_LEVEL_NOW", player, 0);
    }
    public int GET_PLAYER_WANTED_LEVEL(int player)
    {
        return Call("GET_PLAYER_WANTED_LEVEL", player);
    }
    public int CLEAR_PLAYER_WANTED_LEVEL(int player)
    {
        return Call("CLEAR_PLAYER_WANTED_LEVEL", player);
    }
    public bool IS_PLAYER_DEAD(int player)
    {
        return Convert.ToBoolean(Call("IS_PLAYER_DEAD", player));
    }
    public int SET_POLICE_IGNORE_PLAYER(int player, bool toggle)
    {
        return Call("SET_POLICE_IGNORE_PLAYER", player, toggle);
    }
    public int SET_PLAYER_SPRINT(int player, bool toggle)
    {
        return Call("SET_PLAYER_SPRINT", player, toggle);
    }
    public void SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER(int player, float value)
    {
        Call("SET_RUN_SPRINT_MULTIPLIER_FOR_PLAYER", player, value);
        Call("SET_SWIM_MULTIPLIER_FOR_PLAYER", player, value);
        SET_PLAYER_SPRINT(player, value == 1f ? false : true);
    }
    public int GET_PLAYERS_LAST_VEHICLE
    {
        get
        {
            return Call("GET_PLAYERS_LAST_VEHICLE");
        }
    }
    public int GET_PLAYER_INDEX
    {
        get { return Call("GET_PLAYER_INDEX"); }
    }
    public enum Trophies
    {
        Welcom_To_Los_Santos = 1,
        Friendship,
        A_fair_days,
        The_Moment_of_Truth,
        To_live_or_die,
        Diamond_Hard,
        Subversive,
        Blitzed,
        Small_Town_Big_Job,
        The_Government_Gimps,
        The_Big_One,
        Solid_Gold,
        Career_Criminal,
        Trophie_14,
        Fare_in_Love_and_War,
        TP_Industries_Arms_Rac,
        Trophie_17,
        From_Beyond_the_Stars,
        A_Mystery,
        Waste_Management,
        Red_Mist,
        Trophie_22,
        Kifflom,
        Three_Man_Army,
        Out_of_your_Depth,
        Altruist_Acolyte,
        Trophie_27,
        Trading_Pure_Alpha,
        Pimp_my_Sidearm,
        Wanted_Alive,
        Los_Santos_Customs,
        Close_Shave,
        Off_the_Plane,
        Trophie_34,
        Trophie_35,
        Trophie_36,
        Trophie_37,
        Trophie_38,
        Trophie_39,
        Backseat_Driver,
        Trophie_41,
        Trophie_42,
        Trophie_43,
        Trophie_44,
        Trophie_45,
        Trophie_46,
        Trophie_47,
        Trophie_48,
        Trophie_49,
        Trophie_50,
        Trophie_51,
        Trophie_52,
        Trophie_53,
        Trophie_54,
        Trophie_55,
        Trophie_56,
        Trophie_57,
        Trophie_58
    }
    public void GIVE_ACHIEVEMENT_TO_PLAYER(Trophies achievement)
    {
        if (!HAS_ACHIEVEMENT_BEEN_PASSED((int)achievement))
            Call("GIVE_ACHIEVEMENT_TO_PLAYER", (int)achievement);
    }
    public bool HAS_ACHIEVEMENT_BEEN_PASSED(int achievement)
    {
        return Convert.ToBoolean(Call("HAS_ACHIEVEMENT_BEEN_PASSED", achievement));
    }
    public void SET_PLAYER_INVINCIBLE(int player, bool toggle)
    {
        Call("SET_PLAYER_INVINCIBLE", player, toggle);
    }
    public void SET_PLAYER_FORCED_AIM(int player, bool toggle)
    {
        Call("SET_PLAYER_FORCED_AIM", player, toggle);
    }
    public void SET_PLAYER_FORCED_ZOOM(int player, bool toggle)
    {
        Call("SET_PLAYER_FORCED_ZOOM", player, toggle);
    }
    public void SET_PLAYER_FORCE_SKIP_AIM_INTRO(int player, bool toggle)
    {
        Call("SET_PLAYER_FORCE_SKIP_AIM_INTRO", player, toggle);
    }
    public void DISABLE_PLAYER_FIRING(int player, bool toggle)
    {
        Call("DISABLE_PLAYER_FIRING", player, toggle);
    }
    public int PLAYER_ARMOUR
    {
        get { return Call("GET_PLAYER_MAX_ARMOUR", PLAYER_ID); }
        set { Call("SET_PLAYER_MAX_ARMOUR", PLAYER_ID, value); }
    }
    public int GET_PLAYER_GROUP(int player)
    {
        return Call("GET_PLAYER_GROUP", player);
    }
    public void SET_POLICE_RADAR_BLIPS(bool toggle)
    {
        Call("SET_POLICE_RADAR_BLIPS", toggle);
    }
    public void SET_MAX_WANTED_LEVEL(int maxWantedLevel)
    {
        if (maxWantedLevel == 0)
            SET_PLAYER_WANTED_LEVEL_NOW(PLAYER_ID, maxWantedLevel);
        Call("SET_MAX_WANTED_LEVEL", maxWantedLevel);
    }
    public void SET_MODEL(string modelName)
    {
        uint modelHash = Hash(modelName);
        Streaming stream = new Streaming();
        stream.REQUEST_MODEL(modelHash);
        int timeOut = 0;
        while (!stream.HAS_MODEL_LOADED(modelHash))
        {
            Application.DoEvents();
            timeOut++;
            if (timeOut == 5000)
                break;
        }
        Call("SET_PLAYER_MODEL", new Player().PLAYER_ID, modelHash);
        stream.SET_MODEL_AS_NO_LONGER_NEEDED(modelHash);
    }
}
public class Object : RPC
{
    public int CREATE_OBJECT(float[] loc, string objectName)
    {
        uint hash = Hash(objectName);
        float[] flt = loc;
        return Call("CREATE_OBJECT", new object[] { hash, flt, 0f, 1, 1 });
    }
}
public class Vector : RPC
{
    public float[] Get_Way_point
    {
        get
        {
            if (Convert.ToBoolean(Call("IS_WAYPOINT_ACTIVE")))
            {
                int blip = Call("GET_FIRST_BLIP_INFO_ID", 8);
                Call("GET_BLIP_COORDS", 0x10030200, blip);
                return PS3.Extension.ReadFloat(0x10030200, 3);
            }
            return default(float[]);
        }
    }
    public float GET_GROUND_Z_FOR_3D_COORD(float[] flt)
    {
        Call("0xA1BFD5E0", flt, 0x10020420);
        return PS3.Extension.ReadFloat(0x10020420);
    }
}
public class Controls : RPC
{
    public enum Buttons
    {
        Button_Back = 0xBF,
        Button_Triangle = 0xC0,
        Button_Cross = 0xC1,
        Button_Square = 0xC2,
        Button_Circle = 0xC3,
        Button_L1 = 0xC4,
        Button_R1 = 0xC5,
        Button_L2 = 0xC6,
        Button_R2 = 0xC7,
        Button_L3 = 0xC8,
        Button_R3 = 0xC9,
        Dpad_Up = 0xCA,
        Dpad_Down = 0xCB,
        Dpad_Left = 0xCC,
        Dpad_Right = 0xCD,
    };
    public bool IS_DISABLED_CONTROL_PRESSED(Buttons ButtonID)
    {
        return Convert.ToBoolean(Call("IS_DISABLED_CONTROL_PRESSED", 0, ButtonID));
    }
}
public class Task : RPC
{
    public void TASK_WARP_PED_INTO_VEHICLE(int ped, int veh, int seat)
    {
        Call("TASK_WARP_PED_INTO_VEHICLE", ped, veh, seat);
    }
    public void TASK_LEAVE_ANY_VEHICLE(int ped)
    {
        Call("TASK_LEAVE_ANY_VEHICLE", ped, 1, 1);
    }
    public enum Scenarios
    {
        WORLD_HUMAN_BINOCULARS,
        WORLD_HUMAN_BUM_FREEWAY,
        WORLD_HUMAN_BUM_SLUMPED,
        WORLD_HUMAN_BUM_STANDING,
        WORLD_HUMAN_BUM_WASH,
        WORLD_HUMAN_CAR_PARK_ATTENDANT,
        WORLD_HUMAN_CHEERING,
        WORLD_HUMAN_CLIPBOARD,
        WORLD_HUMAN_CONST_DRILL,
        WORLD_HUMAN_COP_IDLES,
        WORLD_HUMAN_DRINKING,
        WORLD_HUMAN_DRUG_DEALER_HARD,
        WORLD_HUMAN_MOBILE_FILM_SHOCKING,
        WORLD_HUMAN_GARDENER_LEAF_BLOWER,
        WORLD_HUMAN_GARDENER_PLANT,
        WORLD_HUMAN_GOLF_PLAYER,
        WORLD_HUMAN_GUARD_STAND,
        WORLD_HUMAN_HAMMERING,
        WORLD_HUMAN_HANG_OUT_STREET,
        WORLD_HUMAN_HIKER_STANDING,
        WORLD_HUMAN_HUMAN_STATUE,
        WORLD_HUMAN_JANITOR,
        WORLD_HUMAN_JOG_STANDING,
        WORLD_HUMAN_LEANING,
        WORLD_HUMAN_MAID_CLEAN,
        WORLD_HUMAN_MUSCLE_FLEX,
        WORLD_HUMAN_MUSCLE_FREE_WEIGHTS,
        WORLD_HUMAN_MUSICIAN,
        WORLD_HUMAN_PAPARAZZI,
        WORLD_HUMAN_PICNIC,
        WORLD_HUMAN_PROSTITUTE_HIGH_class,
        WORLD_HUMAN_PROSTITUTE_LOW_class,
        WORLD_HUMAN_PUSH_UPS,
        WORLD_HUMAN_SEAT_LEDGE,
        WORLD_HUMAN_SEAT_STEPS,
        WORLD_HUMAN_SEAT_WALL,
        WORLD_HUMAN_SEAT_WALL_TABLET,
        WORLD_HUMAN_SECURITY_SHINE_TORCH,
        WORLD_HUMAN_SIT_UPS,
        WORLD_HUMAN_SMOKING,
        WORLD_HUMAN_SMOKING_POT,
        WORLD_HUMAN_STAND_FIRE,
        WORLD_HUMAN_STAND_FISHING,
        WORLD_HUMAN_STAND_IMPATIENT,
        WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT,
        WORLD_HUMAN_STAND_MOBILE,
        WORLD_HUMAN_STAND_MOBILE_UPRIGHT,
        WORLD_HUMAN_STRIP_WATCH_STAND,
        WORLD_HUMAN_STUPOR,
        WORLD_HUMAN_SUNBATHE,
        WORLD_HUMAN_SUNBATHE_BACK,
        WORLD_HUMAN_TENNIS_PLAYER,
        WORLD_HUMAN_TOURIST_MAP,
        WORLD_HUMAN_TOURIST_MOBILE,
        WORLD_HUMAN_VEHICLE_MECHANIC,
        WORLD_HUMAN_WELDING,
        WORLD_HUMAN_WINDOW_SHOP_BROWSE,
        WORLD_HUMAN_YOGA,
        WORLD_BOAR_GRAZING,
        WORLD_COW_GRAZING,
        WORLD_COYOTE_HOWL,
        WORLD_COYOTE_REST,
        WORLD_CHICKENHAWK_FEEDING,
        WORLD_CHICKENHAWK_STANDING,
        WORLD_CORMORANT_STANDING,
        WORLD_CROW_FEEDING,
        WORLD_CROW_STANDING,
        WORLD_DEER_GRAZING,
        WORLD_DOG_BARKING_ROTTWEILER,
        WORLD_DOG_BARKING_RETRIEVER,
        WORLD_DOG_BARKING_SHEPHERD,
        WORLD_DOG_SITTING_ROTTWEILER,
        WORLD_DOG_SITTING_RETRIEVER,
        WORLD_DOG_SITTING_SHEPHERD,
        WORLD_DOG_BARKING_SMALL,
        WORLD_DOG_SITTING_SMALL,
        WORLD_FISH_IDLE,
        WORLD_GULL_FEEDING,
        WORLD_GULL_STANDING,
        WORLD_HEN_PECKING,
        WORLD_HEN_STANDING,
        WORLD_MOUNTAIN_LION_REST,
        WORLD_MOUNTAIN_LION_WANDER,
        WORLD_PIG_GRAZING,
        WORLD_PIGEON_FEEDING,
        WORLD_PIGEON_STANDING,
        WORLD_RABBIT_EATING,
        WORLD_RATS_EATING,
        WORLD_SHARK_SWIM,
        PROP_BIRD_IN_TREE,
        PROP_BIRD_TELEGRAPH_POLE,
        PROP_HUMAN_ATM,
        PROP_HUMAN_BBQ,
        PROP_HUMAN_BUM_BIN,
        PROP_HUMAN_BUM_SHOPPING_CART,
        PROP_HUMAN_MUSCLE_CHIN_UPS,
        PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY,
        PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON,
        PROP_HUMAN_PARKING_METER,
        PROP_HUMAN_SEAT_ARMCHAIR,
        PROP_HUMAN_SEAT_BAR,
        PROP_HUMAN_SEAT_BENCH,
        PROP_HUMAN_SEAT_BENCH_DRINK,
        PROP_HUMAN_SEAT_BENCH_DRINK_BEER,
        PROP_HUMAN_SEAT_BENCH_FOOD,
        PROP_HUMAN_SEAT_BUS_STOP_WAIT,
        PROP_HUMAN_SEAT_CHAIR,
        PROP_HUMAN_SEAT_CHAIR_DRINK,
        PROP_HUMAN_SEAT_CHAIR_DRINK_BEER,
        PROP_HUMAN_SEAT_CHAIR_FOOD,
        PROP_HUMAN_SEAT_CHAIR_UPRIGHT,
        PROP_HUMAN_SEAT_CHAIR_MP_PLAYER,
        PROP_HUMAN_SEAT_COMPUTER,
        PROP_HUMAN_SEAT_DECKCHAIR,
        PROP_HUMAN_SEAT_DECKCHAIR_DRINK,
        PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS,
        PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS_PRISON,
        PROP_HUMAN_SEAT_STRIP_WATCH,
        PROP_HUMAN_SEAT_SUNLOUNGER,
        PROP_HUMAN_STAND_IMPATIENT,
        CODE_HUMAN_CROSS_ROAD_WAIT,
        CODE_HUMAN_MEDIC_KNEEL,
        CODE_HUMAN_MEDIC_TEND_TO_DEAD,
        CODE_HUMAN_MEDIC_TIME_OF_DEATH,
        CODE_HUMAN_POLICE_CROWD_CONTROL,
        CODE_HUMAN_POLICE_INVESTIGATE
    }
    public void TASK_START_SCENARIO_IN_PLACE(Scenarios Scenario)
    {
        int ped = Call("player_ped_id");
        Call("TASK_START_SCENARIO_IN_PLACE", ped, Scenario.ToString(), 0, 1);
    }
    public void TASK_VEHICLE_DRIVE_WANDER(int ped, int vehId, bool toggle, float unk = 40f)
    {
        if (!toggle)
        {
            CLEAR_PED_TASKS(ped);
            return;
        }
        Call("TASK_VEHICLE_DRIVE_WANDER", ped, vehId, unk, 0xC00AB);
    }
    public void CLEAR_PED_TASKS(int ped)
    {
        Call("CLEAR_PED_TASKS", ped);
    }
    public void TASK_COMBAT_PED(int ped, int targetPed)
    {
        new Ped().SET_PED_NEVER_LEAVES_GROUP(ped, false);
        Call("TASK_COMBAT_PED", ped, targetPed, 0, 16);
    }
    public void CLEAR_PED_TASKS_IMMEDIATELY(int ped)
    {
        Call("CLEAR_PED_TASKS_IMMEDIATELY", ped);
    }
}
public class Fire : RPC
{
    public int START_SCRIPT_FIRE(float[] loc)
    {
        return Call("START_SCRIPT_FIRE", loc, 10, 1);
    }

    float[] GET_CLOSEST_FIRE_POS(float[] currentLoc)
    {
        Call("GET_CLOSEST_FIRE_POS", 0x10040500, currentLoc);
        return PS3.Extension.ReadFloat(0x10040500, 3);
    }
    public void ENTITY_FIRE(int entity, bool toggle)
    {
        string address = toggle == true ? "START_ENTITY_FIRE" : "STOP_ENTITY_FIRE";
        Call(address, entity);
    }
    public int ADD_OWNED_EXPLOSION(int ped, float[] loc)
    {
        return Call("ADD_OWNED_EXPLOSION", new object[] { ped, loc, 0x1d, 5f, 0, 1, 5f });
    }
}
public class Graphics : RPC
{
    public enum ModifierName
    {
        Clear,
        Bank_HLWD,
        Barry1_Stoned,
        BarryFadeOut,
        baseTONEMAPPING,
        Bikers,
        BikersSPLASH,
        blackNwhite,
        BlackOut,
        Bloom,
        BloomLight,
        buildingTOP,
        BulletTimeDark,
        BulletTimeLight,
        CAMERA_BW,
        CAMERA_secuirity,
        CAMERA_secuirity_FUZZ,
        canyon_mission,
        carMOD_underpass,
        carpark,
        carpark_dt1_02,
        carpark_dt1_03,
        cashdepot,
        cashdepotEMERGENCY,
        cBank_back,
        cBank_front,
        ch2_tunnel_whitelight,
        CH3_06_water,
        CHOP,
        cinema,
        cinema_001,
        cops,
        CopsSPLASH,
        crane_cam,
        crane_cam_cinematic,
        CS1_railwayB_tunnel,
        CS3_rail_tunnel,
        CUSTOM_streetlight,
        damage,
        death,
        DEFAULT,
        DefaultColorCode,
        DONT_overide_sunpos,
        Dont_tazeme_bro,
        dont_tazeme_bro_b,
        downtown_FIB_cascades_opt,
        DrivingFocusDark,
        DrivingFocusLight,
        DRUG_2_drive,
        Drug_deadman,
        Drug_deadman_blend,
        drug_drive_blend01,
        drug_drive_blend02,
        drug_flying_01,
        drug_flying_02,
        drug_flying_base,
        DRUG_gas_huffin,
        drug_wobbly,
        Drunk,
        dying,
        eatra_bouncelight_beach,
        epsilion,
        exile1_exit,
        exile1_plane,
        ExplosionJosh,
        ext_int_extlight_large,
        EXTRA_bouncelight,
        eyeINtheSKY,
        Facebook_NEW,
        facebook_serveroom,
        FIB_5,
        FIB_6,
        FIB_A,
        FIB_B,
        FIB_interview,
        FIB_interview_optimise,
        FinaleBank,
        FinaleBankexit,
        FinaleBankMid,
        fireDEPT,
        FORdoron_delete,
        Forest,
        FrankilinsHOUSEhills,
        frankilnsAUNTS_new,
        frankilnsAUNTS_SUNdir,
        FRANKLIN,
        FranklinColorCode,
        FranklinColorCodeBasic,
        FullAmbientmult_interior,
        gallery_refmod,
        garage,
        gorge_reflection_gpu,
        gorge_reflectionoffset,
        gorge_reflectionoffset2,
        graveyard_shootout,
        gunclub,
        gunclubrange,
        gunshop,
        gunstore,
        half_direct,
        hangar_lightsmod,
        Hanger_INTmods,
        heathaze,
        helicamfirst,
        Hicksbar,
        HicksbarNEW,
        hillstunnel,
        Hint_cam,
        hitped,
        hud_def_blur,
        hud_def_colorgrade,
        hud_def_desat_cold,
        hud_def_desat_cold_kill,
        hud_def_desat_Franklin,
        hud_def_desat_Michael,
        hud_def_desat_Neutral,
        hud_def_desat_switch,
        hud_def_desat_Trevor,
        hud_def_desatcrunch,
        hud_def_flash,
        hud_def_focus,
        hud_def_Franklin,
        hud_def_lensdistortion,
        hud_def_Michael,
        hud_def_Trevor,
        id1_11_tunnel,
        int_amb_mult_large,
        int_Barber1,
        int_carmod_small,
        int_carshowroom,
        int_chopshop,
        int_clean_extlight_large,
        int_clean_extlight_none,
        int_clean_extlight_small,
        int_ClothesHi,
        int_clotheslow_large,
        int_cluckinfactory_none,
        int_cluckinfactory_small,
        int_ControlTower_none,
        int_ControlTower_small,
        int_dockcontrol_small,
        int_extlght_sm_cntrst,
        int_extlight_large,
        int_extlight_large_fog,
        int_extlight_none,
        int_extlight_none_dark,
        int_extlight_none_dark_fog,
        int_extlight_none_fog,
        int_extlight_small,
        int_extlight_small_clipped,
        int_extlight_small_fog,
        int_Farmhouse_none,
        int_Farmhouse_small,
        int_FranklinAunt_small,
        INT_FullAmbientmult,
        INT_FULLAmbientmult_art,
        INT_FULLAmbientmult_both,
        INT_garage,
        int_GasStation,
        int_hanger_none,
        int_hanger_small,
        int_Hospital2_DM,
        int_Hospital_Blue,
        int_Hospital_BlueB,
        int_Hospital_DM,
        int_lesters,
        int_Lost_none,
        int_Lost_small,
        int_methlab_small,
        int_motelroom,
        INT_NO_fogALPHA,
        INT_NoAmbientmult,
        INT_NoAmbientmult_art,
        INT_NoAmbientmult_both,
        INT_NOdirectLight,
        INT_nowaterREF,
        int_office_Lobby,
        int_office_LobbyHall,
        INT_posh_hairdresser,
        INT_streetlighting,
        int_tattoo,
        int_tattoo_B,
        int_tunnel_none_dark,
        interior_WATER_lighting,
        introblue,
        jewel_gas,
        jewel_optim,
        jewelry_entrance,
        jewelry_entrance_INT,
        jewelry_entrance_INT_fog,
        KT_underpass,
        lab_none,
        lab_none_dark,
        lab_none_dark_fog,
        lab_none_exit,
        LifeInvaderLOD,
        lightning,
        lightning_cloud,
        lightning_strong,
        lightning_weak,
        LightPollutionHills,
        lightpolution,
        LIGHTSreduceFALLOFF,
        LODmult_global_reduce,
        LODmult_global_reduce_NOHD,
        LODmult_HD_orphan_LOD_reduce,
        LODmult_HD_orphan_reduce,
        LODmult_LOD_reduce,
        LODmult_SLOD1_reduce,
        LODmult_SLOD2_reduce,
        LODmult_SLOD3_reduce,
        metro,
        METRO_platform,
        METRO_Tunnels,
        METRO_Tunnels_entrance,
        MichaelColorCode,
        MichaelColorCodeBasic,
        MichaelsDarkroom,
        MichaelsDirectional,
        MichaelsNODirectional,
        micheal,
        micheals_lightsOFF,
        michealspliff,
        michealspliff_blend,
        michealspliff_blend02,
        militarybase_nightlight,
        morebloomnumMods = 3,
        morgue_dark,
        Mp_apart_mid,
        MP_Bull_tost,
        MP_Bull_tost_blend,
        MP_corona_switch,
        MP_death_grade,
        MP_death_grade_blend01,
        MP_death_grade_blend02,
        MP_Garage_L,
        MP_heli_cam,
        MP_intro_logo,
        MP_job_load,
        MP_job_lose,
        MP_job_win,
        MP_Killstreak,
        MP_Killstreak_blend,
        MP_Loser,
        MP_Loser_blend,
        MP_lowgarage,
        MP_MedGarage,
        MP_Powerplay,
        MP_Powerplay_blend,
        MP_race_finish,
        MP_select,
        MP_Studio_Lo,
        MPApartHigh,
        Multipayer_spectatorCam,
        multiplayer_ped_fight,
        nervousRON_fog,
        NeutralColorCode,
        NeutralColorCodeBasic,
        NeutralColorCodeLight,
        NEW_abattoir,
        new_bank,
        NEW_jewel,
        NEW_jewel_EXIT,
        NEW_lesters,
        NEW_ornate_bank,
        NEW_ornate_bank_entrance,
        NEW_ornate_bank_office,
        NEW_ornate_bank_safe,
        New_sewers,
        NEW_shrinksOffice,
        NEW_station_unfinished,
        new_stripper_changing,
        NEW_trevorstrailer,
        NEW_tunnels,
        NEW_tunnels_ditch,
        new_tunnels_entrance,
        NEW_tunnels_hole,
        NEW_yellowtunnels,
        NewMicheal,
        NewMicheal_night,
        NewMicheal_upstairs,
        NewMichealgirly,
        NewMichealstoilet,
        NewMichealupstairs,
        NewMod,
        nextgen,
        NO_coronas,
        NO_fog_alpha,
        NO_streetAmbient,
        NO_weather,
        NoAmbientmult,
        NoAmbientmult_interior,
        NOdirectLight,
        NOrain,
        overwater,
        Paleto,
        paleto_nightlight,
        paleto_opt,
        PERSHING_water_reflect,
        phone_cam,
        phone_cam1,
        phone_cam10,
        phone_cam11,
        phone_cam12,
        phone_cam13,
        phone_cam2,
        phone_cam3,
        phone_cam4,
        phone_cam5,
        phone_cam6,
        phone_cam7,
        phone_cam9,
        plane_inside_mode,
        player_transition,
        player_transition_no_scanlines,
        player_transition_scanlines,
        PlayerSwitchNeutralFlash,
        PlayerSwitchPulse,
        PoliceStation,
        PoliceStationDark,
        polluted,
        poolsidewaterreflection2,
        PORT_heist_underwater,
        powerplant_nightlight,
        powerstation,
        prison_nightlight,
        projector,
        prologue,
        prologue_ending_fog,
        prologue_ext_art_amb,
        prologue_reflection_opt,
        prologue_shootout,
        Prologue_shootout_opt,
        pulse,
        RaceTurboDark,
        RaceTurboFlash,
        RaceTurboLight,
        ranch,
        REDMIST,
        REDMIST_blend,
        ReduceDrawDistance,
        ReduceDrawDistanceMAP,
        ReduceDrawDistanceMission,
        reducelightingcost,
        ReduceSSAO,
        reducewaterREF,
        refit,
        reflection_correct_ambient,
        RemoteSniper,
        resvoire_reflection,
        SALTONSEA,
        sandyshore_nightlight,
        SAWMILL,
        scanline_cam,
        scanline_cam_cheap,
        scope_zoom_in,
        scope_zoom_out,
        secret_camera,
        services_nightlight,
        shades_pink,
        shades_yellow,
        SheriffStation,
        ship_explosion_underwater,
        ship_lighting,
        Shop247,
        Shop247_none,
        sleeping,
        SnipernumMods = 7,
        SP1_03_drawDistance,
        spectator1,
        spectator10,
        spectator2,
        spectator3,
        spectator4,
        spectator5,
        spectator6,
        spectator7,
        spectator8,
        spectator9,
        StadLobby,
        stc_coroners,
        stc_deviant_bedroom,
        stc_deviant_lounge,
        stc_franklinsHouse,
        stc_trevors,
        stoned,
        stoned_aliens,
        stoned_cutscene,
        stoned_monkeys,
        StreetLightingJunction,
        StreetLightingtraffic,
        STRIP_changing,
        STRIP_nofog,
        STRIP_office,
        STRIP_stage,
        subBASE_water_ref,
        sunglasses,
        superDARK,
        switch_cam_1,
        switch_cam_2,
        telescope,
        torpedo,
        traffic_skycam,
        trailer_explosion_optimise,
        TREVOR,
        TrevorColorCode,
        TrevorColorCodeBasic,
        Trevors_room,
        trevorspliff,
        trevorspliff_blend,
        trevorspliff_blend02,
        Tunnel,
        tunnel_entrance,
        tunnel_entrance_INT,
        TUNNEL_green,
        Tunnel_green1,
        TUNNEL_green_ext,
        TUNNEL_orange,
        TUNNEL_orange_exterior,
        TUNNEL_white,
        TUNNEL_yellow,
        TUNNEL_yellow_ext,
        ufo,
        ufo_deathray,
        underwater,
        underwater_deep,
        underwater_deep_clear,
        v_abattoir,
        V_Abattoir_Cold,
        v_bahama,
        v_cashdepot,
        V_FIB_IT3,
        V_FIB_IT3_alt,
        V_FIB_IT3_alt5,
        V_FIB_stairs,
        v_foundry,
        v_janitor,
        v_jewel2,
        v_metro,
        V_Metro2,
        V_Metro_station,
        v_michael,
        v_michael_lounge,
        V_Office_smoke,
        V_Office_smoke_ext,
        V_Office_smoke_Fire,
        v_recycle,
        V_recycle_dark,
        V_recycle_light,
        V_recycle_mainroom,
        v_rockclub,
        V_Solomons,
        v_strip3,
        V_strip_nofog,
        V_strip_office,
        v_strpchangerm,
        v_sweat,
        v_sweat_entrance,
        v_sweat_NoDirLight,
        v_torture,
        Vagos,
        vagos_extlight_small,
        VAGOS_new_garage,
        VAGOS_new_hangout,
        VagosSPLASH,
        VC_tunnel_entrance,
        venice_canal_tunnel,
        vespucci_garage,
        warehouse,
        WATER_hills,
        WATER_lab,
        WATER_lab_cooling,
        WATER_militaryPOOP,
        WATER_muddy,
        WATER_port,
        WATER_REF_malibu,
        WATER_refmap_high,
        WATER_refmap_hollywoodlake,
        WATER_refmap_low,
        WATER_refmap_med,
        WATER_refmap_off,
        WATER_refmap_poolside,
        WATER_refmap_silverlake,
        WATER_refmap_venice,
        WATER_refmap_verylow,
        WATER_resevoir,
        WATER_river,
        WATER_salton,
        WATER_salton_bottom,
        WATER_shore,
        WATER_silty,
        WATER_silverlake,
        whitenightlighting,
        WhiteOut,
        yell_tunnel_nodirect, 
    }
    public void SET_FLASH(int duration)
    {
        Call("SET_FLASH", 0, 0, 1000, duration, 1000);
    }
    public void SET_TIMECYCLE_MODIFIER(ModifierName modifierName, float strength)
    {
        if (modifierName == ModifierName.Clear)
        {
            Call("CLEAR_TIMECYCLE_MODIFIER");
            return;
        }
        Call("SET_TIMECYCLE_MODIFIER", modifierName.ToString());
        Call("SET_TIMECYCLE_MODIFIER_STRENGTH", strength);
    }
    public enum EffectName
    {
        CamPushInFranklin,
        CamPushInMichael,
        CamPushInNeutral,
        CamPushInTrevor,
        ChopVision,
        DeathFailMPDark,
        DeathFailMPIn,
        DeathFailNeutralIn,
        DeathFailOut,
        DMT_flight,
        DMT_flight_intro,
        Dont_tazeme_bro,
        DrugsDrivingIn,
        DrugsDrivingOut,
        DrugsMichaelAliensFight,
        DrugsMichaelAliensFightIn,
        DrugsMichaelAliensFightOut,
        DrugsTrevorClownsFight,
        DrugsTrevorClownsFightIn,
        DrugsTrevorClownsFightOut,
        ExplosionJosh3,
        FocusIn,
        FocusOut,
        HeistCelebEnd,
        HeistCelebPass,
        HeistCelebPassBW,
        HeistCelebToast,
        HeistLocate,
        HeistTripSkipFade,
        MenuMGHeistIn,
        MenuMGHeistOut,
        MenuMGSelectionIn,
        MenuMGSelectionTint,
        MenuMGTournamentIn,
        MinigameEndFranklin,
        MinigameEndMichael,
        MinigameEndNeutral,
        MinigameEndTrevor,
        MinigameTransitionIn,
        MinigameTransitionOut,
        MP_Celeb_Lose,
        MP_Celeb_Lose_Out,
        MP_Celeb_Preload_Fade,
        MP_Celeb_Win,
        MP_Celeb_Win_Out,
        MP_corona_switch,
        MP_intro_logo,
        MP_job_load,
        MP_race_crash,
        RaceTurbo,
        SuccessFranklin,
        SuccessMichael,
        SuccessNeutral,
        SuccessTrevor,
        SwitchHUDFranklinOut,
        SwitchHUDIn,
        SwitchHUDMichaelOut,
        SwitchHUDOut,
        SwitchHUDTrevorOut,
        SwitchOpenFranklinIn,
        SwitchOpenMichaelIn,
        SwitchOpenNeutralFIB5,
        SwitchOpenTrevorIn,
        SwitchSceneFranklin,
        SwitchSceneMichael,
        SwitchSceneNeutral,
        SwitchSceneTrevor,
        SwitchShortFranklinIn,
        SwitchShortFranklinMid,
        SwitchShortMichaelIn,
        SwitchShortMichaelMid,
        SwitchShortNeutralIn,
        SwitchShortTrevorIn,
        SwitchShortTrevorMid, 
    }
    public void START_SCREEN_EFFECT(EffectName effectName, int playLength)
    {
        Call("0x1D980479", effectName.ToString(), playLength, 1);
    }
    public void STOP_ALL_SCREEN_EFFECTS()
    {
        Call("0x4E6D875B");
    }
    public void SET_SEETHROUGH(bool toggle)
    {
        Call("SET_SEETHROUGH", toggle);
    }
    public void DISPLAY_RADAR(bool toggle)
    {
        Call("DISPLAY_RADAR", toggle);
    }
}
public class Streaming : RPC
{
    public void REQUEST_MODEL(uint hash)
    {
        Call("REQUEST_MODEL", hash);
    }
    public bool HAS_MODEL_LOADED(uint hash)
    {
        return Convert.ToBoolean(Call("HAS_MODEL_LOADED", hash));
    }
    public int SET_MODEL_AS_NO_LONGER_NEEDED(uint hash)
    {
        return Call("SET_MODEL_AS_NO_LONGER_NEEDED", hash);
    }
    public bool IS_MODEL_IN_CDIMAGE(uint hash)
    {
        return Convert.ToBoolean(Call("IS_MODEL_IN_CDIMAGE", hash));
    }
    public bool IS_MODEL_VALID(uint hash)
    {
        return Convert.ToBoolean(Call("IS_MODEL_VALID", hash));
    }
    public uint GET_HASH_KEY(string nativeString)
    {
        return (uint)Call("GET_HASH_KEY", nativeString);
    }
}
public class Stats : RPC
{
    public enum Houses
        {
            No_Property = 0,
            _ECLIPSE_TOWERS__APT_31 = 1,
            _ECLIPSE_TOWERS__APT_9 = 2,
            _ECLIPSE_TOWERS__APT_40 = 3,
            _ECLIPSE_TOWERS__APT_5 = 4,
            _3_ALTA_ST__APT_10 = 5,
            _3_ALTA_ST__APT_57 = 6,
            _DEL_PERRO_HEIGHTS__APT_20 = 7,
            _1162_POWER_ST__APT_3 = 8,
            _0650_SPANISH_AVE__APT_1 = 9,
            _0604_LAS_LAGUNAS_BLVD__APT_4 = 10,
            _0184_MILTON_RD__APT_13 = 11,
            _THE_ROYALE__APT_19 = 12,
            _0504_S_MO_MILTON_DR = 13,
            _0115_BAY_CITY_AVE__APT_45 = 14,
            _0325_SOUTH_ROCKFORD_DR = 15,
            _DREAM_TOWERS__APT_15 = 16,
            _2143_LAS_LAGUNAS_BLVD__APT_9 = 17,
            _1561_SAN_VITAS__APT_2 = 18,
            _0112_ROCKFORD_DR__APT_13 = 19,
            _2057_VESPUCCI_BLVD__APT_1 = 20,
            _0069_COUGAR_AVE__APT_19 = 21,
            _1237_PROSPERITY_ST__APT_21 = 22,
            _1115_BLVD_DEL_PERRO__APT_18 = 23,
            _0120_MURRIETS_HEIGHTS = 24,
            _UNIT_14_POPULAR_ST = 25,
            _UNIT_2_POPULAR_ST = 26,
            _331_SUPPLY_ST = 27,
            _UNIT_1_OLYMPIC_FWY = 28,
            _0754_ROY_LOWENSTEIN_BLVD = 29,
            _12_LITTLE_BIGHORN_AVE = 30,
            _0552_ROY_LOWENSTEIN_BLVD = 32,
            _0432_DAVIS_AVE = 33,
            _UNIT_124_POPULAR_ST = 34,
            _WEAZEL_PLAZA__APT_101 = 35,
            _WEAZEL_PLAZA__APT_70 = 36,
            _WEAZEL_PLAZA__APT_26 = 37,
            _4_INTEGRITY_WAY__APT_30 = 38,
            _4_INTEGRITY_WAY__APT_35 = 39,
            _RICHARDS_MAJESTIC__APT_4 = 40,
            _RICHARDS_MAJESTIC__APT_51 = 41,
            _TINSEL_TOWERS__APT_45 = 42,
            _TINSEL_TOWERS__APT_29 = 43,
            _142__PALETO_BLVD = 44,
            _1932_GRAPESEED_AVE = 46,
            _1920_SENORA_WAY = 47,
            _2000_GREAT_OCEAN_HIGHWAY = 48,
            _197_ROUTE_68 = 49,
            _870_ROUTE_68_APPROACH = 50,
            _1200_ROUTE_68 = 51,
            _8754_ROUTE_68 = 52,
            _1905_DAVIS_AVE = 53,
            _1623_SOUTH_SHAMBLES_ST = 54,
            _4531_DRY_DOCK_ST = 55,
            _1337_EXCEPTIONALISTS_WAY = 56,
            _UNIT_76_GREENWICH_PARKWAY = 57,
            _GARAGE_INNOCENCE_BLVD = 58,
            _634_BLVD_DEL_PERRO = 59,
            _0897_MIRROR_PARK_BLVD = 60,
            _ECLIPSE_TOWERS__APT_3 = 61,
            _DEL_PERRO_HEIGHTS__APT_4 = 62,
            _RICHARDS_MAJESTIC__APT_2 = 63,
            _TINSEL_TOWERS__APT_42 = 64,
            _4_INTEGRITY_WAY__APT_28 = 65,
            _4_HANGMAN_AVE = 66,
            _12_SUSTANCIA_RD = 67,
            _4584_PROCOPIO_DR = 68,
            _4401_PROCOPIO_DR = 69,
            _0232_PALETO_BLVD = 70,
            _140_ZANCUDO_AVE = 71,
            _1893_GRAPESEED_AVE = 72, 
        }
    public void SetCash(int cash)
    {
        Call(Addresses.AddCash, cash, 0);
    }
    public int SHOOTING_ABILITY
    {
        get { return new Scripts().STAT_GET_INT("SHOOTING_ABILITY"); }
        set { new Scripts().STAT_SET_INT("SHOOTING_ABILITY", value, 1); }
    }

    public int ARMOUR
    {
        set
        {
            for (int i = 1; i <= 5; i++)
            {
                new Scripts().STAT_SET_INT("MP_CHAR_ARMOUR_" + i + "_COUNT", value, 1);
                Application.DoEvents();
            }
        }
    }
    public int FIREWORK
    {
        set
        {
            string[] Stats =  { "WHITE", "RED", "BLUE" };

            foreach (var item in Stats)
            {
                for (int i = 1; i <= 4; i++)
                {
                    new Scripts().STAT_SET_INT("FIREWORK_TYPE_" + i + "_" + item, value, 1);
                    Application.DoEvents();
                } 
            }
        }
    }
    public int SMOKES
    {
        set
        {
            new Scripts().STAT_SET_INT("CIGARETTES_BOUGHT", value, 1);
        }
    }
    public int SNACKS
    {
        set
        {
            string[] snacks = { "NO_BOUGHT_YUM_SNACKS", "NO_BOUGHT_HEALTH_SNACKS", "NO_BOUGHT_EPIC_SNACKS", "NUMBER_OF_ORANGE_BOUGHT", "NUMBER_OF_BOURGE_BOUGHT" };
            foreach (var snack in snacks)
            {
                new Scripts().STAT_SET_INT(snack, value, 1);
                Application.DoEvents();
            }
        }
    }
    public float PLAYER_MENTAL_STATE
    {
        set { new Scripts().STAT_SET_FLOAT("PLAYER_MENTAL_STATE", value, 1); }
        get { return new Scripts().STAT_GET_FLOAT("PLAYER_MENTAL_STATE"); }
    }
    static List<string> houses;
     static string[] Allhouses
    {
        get
        {
            if (houses != null)
                return houses.ToArray();
            houses = new List<string>();
            foreach (var item in Enum.GetNames(typeof(Houses)))
            {
                houses.Add(item.Replace("_", " ").TrimStart(' '));
            }
            return houses.ToArray();
        }
    }

    public string[] LoadHouses
    {
        get
        {
            return Allhouses;
        }
    }
    void BuyHouse(int propertyNum, Houses house)
    {
        if (!(propertyNum < 1 || propertyNum > 3))
        {
            string property = "";
            if (propertyNum == 1)
                property = "PROPERTY_HOUSE";
            else if (propertyNum == 2)
                property = "MULTI_PROPERTY_1";
            else property = "MULTI_PROPERTY_2";
            new Scripts().STAT_SET_INT(property, (int)house, 1);
        }
    }
    public  void BuyHouse(int propertyNum, string house)
    {
        BuyHouse(propertyNum, (Houses)Enum.Parse(typeof(Houses), "_" + house.Replace(" ", "_")));
    }
    #region levels
    static ulong[] _1_99 = new ulong[] { 
  0L, 0L, 800L, 0x834L, 0xed8L, 0x17d4L, 0x251cL, 0x30d4L, 0x3e80L, 0x4d58L, 0x5dc0L, 0x6f54L, 0x8278L, 0x972cL, 0xaca8L, 0xc418L, 
  0xdc50L, 0xf618L, 0x1110cL, 0x12d2cL, 0x14adcL, 0x16954L, 0x1895cL, 0x1aa90L, 0x1ccf0L, 0x1f07cL, 0x21534L, 0x23b18L, 0x26228L, 0x28a64L, 0x2b3ccL, 0x2de60L, 
  0x30a20L, 0x3370cL, 0x364c0L, 0x39404L, 0x3c410L, 0x3f548L, 0x427acL, 0x45b3cL, 0x48ff8L, 0x4c57cL, 0x4fc90L, 0x53408L, 0x56d10L, 0x5a744L, 0x5e240L, 0x61e68L, 
  0x65b58L, 0x699d8L, 0x6d920L, 0x71930L, 0x75ad0L, 0x79d38L, 0x7e0ccL, 0x82528L, 0x86ab0L, 0x8b100L, 0x8f8e0L, 0x94124L, 0x98af8L, 0x9d594L, 0xa20f8L, 0xa6d88L, 
  0xabb44L, 0xb09c8L, 0xb5978L, 0xbaa54L, 0xbfbf8L, 0xc4e64L, 0xca1fcL, 0xcf6c0L, 0xd4c4cL, 0xda2a0L, 0xdfa20L, 0xe52ccL, 0xeac40L, 0xf067cL, 0xf61e4L, 0xfbe78L, 
  0x101bd4L, 0x1079f8L, 0x10d948L, 0x113960L, 0x119aa4L, 0x11fcb0L, 0x125fe8L, 0x12c3e8L, 0x1328b0L, 0x138ea4L, 0x13f5c4L, 0x145d48L, 0x14c5f8L, 0x152fd4L, 0x159a78L, 0x1605e4L, 
  0x16727cL, 0x16dfdcL, 0x174e04L, 0x17bd58L
    };

    public static ulong ToRP(int Level)
    {
        int num = 0;
        List<ulong> list = new List<ulong>();
        ulong num2 = 1L;
        ulong num3 = 100L;
        while (num2 < num3)
        {
            list.Add(_1_99[(int)((IntPtr)num2)]);
            num++;
            if (num == 10)
            {
                num = 0;
            }
            num2 += (ulong)1L;
        }
        ulong num4 = 0L;
        num2 = 1L;
        num3 = 0x1edeL;
        while (num2 < num3)
        {
            num4 += num2;
            list.Add((((ulong)0x17bd58L) + (((ulong)0x6f54L) * num2)) + (num4 * ((ulong)50L)));
            num++;
            if (num == 10)
            {
                num = 0;
            }
            num2 += (ulong)1L;
        }
        return list[Level - 1];
    }
    #endregion
    public int SET_RANK
    {
        set
        {
            ulong val = ToRP(value);
            new Scripts().STAT_SET_INT("CHAR_XP_FM", (int)val, 0);
        }
    }
    public int NETWORK_SPENT_CASH_DROP
    {
        set { Call("NETWORK_SPENT_CASH_DROP", value); }
    }
    public int SKILLS
    {
        set
        {
            string[] skills = { "STAM", "STRN", "LUNG", "DRIV", "FLY", "SHO", "STL", "MECH" };
            for (int i = 0; i < skills.Length; i++)
            {
                new Scripts().STAT_SET_INT("SCRIPT_INCREASE_" + skills[i], value, 1);
                Application.DoEvents();
            }
        }
    }
    public float Kill_Death_Ratio
    {
        set { new Scripts().STAT_SET_FLOAT("MPPLY_KILL_DEATH_RATIO", value, 1); }
        get { return new Scripts().STAT_GET_FLOAT("PLAYER_MENTAL_STATE"); }
    }
    public int DEATHS_PLAYER
    {
        set { new Scripts().STAT_SET_INT("MPPLY_DEATHS_PLAYER", value, 1); }
        get { return new Scripts().STAT_GET_INT("MPPLY_DEATHS_PLAYER"); }
    }
    public int KILLS_PLAYERS
    {
        set { new Scripts().STAT_SET_INT("MPPLY_KILLS_PLAYERS", value, 1); }
        get { return new Scripts().STAT_GET_INT("MPPLY_KILLS_PLAYERS"); }
    }
    public  string ClearReports()
    {
        int _null = 0;
        new Scripts().STAT_SET_INT("MPPLY_GRIEFING", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_VC_ANNOYINGME", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_VC_HATE", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_OFFENSIVE_LANGUAGE", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_OFFENSIVE_TAGPLATE", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_OFFENSIVE_UGC", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_BAD_CREW_NAME", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_BAD_CREW_MOTTO", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_BAD_CREW_STATUS", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_BAD_CREW_EMBLEM", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_GAME_EXPLOITS", _null, 1);
        new Scripts().STAT_SET_INT("MPPLY_EXPLOITS", _null, 1);
        return "All reports has been cleared";
    }
    public  int TimeFormat(decimal days, decimal hours, decimal minutes, decimal seconds)
    {
        return Convert.ToInt32((days * 86400000) + (hours * 3600000) + (minutes * 60000) + (seconds * 1000));
    }
}
public class Network : RPC
{
    public void NETWORK_EARN_FROM_ROCKSTAR(int value)
    {
        Call("0x5A3733CC", value);
    }
    public void NETWORK_SESSION_LEAVE_SINGLE_PLAYER()
    {
        Call("NETWORK_SESSION_LEAVE_SINGLE_PLAYER");
    }
    public void SHUTDOWN_AND_LAUNCH_SINGLE_PLAYER_GAME()
    {
        Call("SHUTDOWN_AND_LAUNCH_SINGLE_PLAYER_GAME");
    }
    public void NETWORK_SESSION_KICK_PLAYER(int player)
    {
        Call("NETWORK_SESSION_KICK_PLAYER", player);
    }
    public bool NETWORK_IS_HOST
    {
        get { return Convert.ToBoolean(Call("NETWORK_IS_HOST")); }
    }
    public int NETWORK_GET_NUM_CONNECTED_PLAYERS
    {
        get { return Call("NETWORK_GET_NUM_CONNECTED_PLAYERS"); }
    }
    public int NETWORK_GET_NETWORK_ID_FROM_ENTITY(int entity)
    {
        return Call("NETWORK_GET_NETWORK_ID_FROM_ENTITY", entity);
    }
    public string GET_HOST
    {
        get { return new Player().GET_PLAYER_NAME(Call("NETWORK_GET_HOST_OF_THIS_SCRIPT")); }
    }
    public bool NETWORK_AM_I_MUTED_BY_PLAYER(int player)
    {
        return Convert.ToBoolean(Call("NETWORK_AM_I_MUTED_BY_PLAYER", player));
    }
    public bool NETWORK_PLAYER_HAS_HEADSET(int player)
    {
        return Convert.ToBoolean(Call("NETWORK_PLAYER_HAS_HEADSET", player));
    }
    public bool NETWORK_IS_PLAYER_TALKING(int player)
    {
        return Convert.ToBoolean(Call("NETWORK_IS_PLAYER_TALKING", player));
    }
    static bool stopMoney;
    public  void MoneyDrop(int clientIndex, int AmountOfTimes, int amount, bool cont)
    {
        uint modelHash = 0x113FD533;
        stopMoney = cont;
        float[] XYZ = new Entity().GET_ENTITY_COORDS(new Ped().GET_PLAYER_PED(clientIndex));
        var streaming = new Streaming();
        streaming.REQUEST_MODEL(modelHash);
        Scripts.Delay(1.5);
        if (streaming.HAS_MODEL_LOADED(modelHash))
        {
            for (int i = 0; i < AmountOfTimes; i++)
            {
                if (!stopMoney)
                    break;
                Call("CREATE_AMBIENT_PICKUP", 0xCE6FDD6B, XYZ, 0, amount, 0x113FD533, 0, 1);
                Application.DoEvents();
            }
           streaming.SET_MODEL_AS_NO_LONGER_NEEDED(modelHash);
        }
    }
}
public class GamePlay : RPC
{
    public int SET_GRAVITY_LEVEL
    {
        set
        {
            Call("SET_GRAVITY_LEVEL", value);
        }
    }
    public void SET_OVERRIDE_WEATHER(string weatherType)
    {
        Call("SET_OVERRIDE_WEATHER", weatherType);
    }
    public void CLEAR_OVERRIDE_WEATHER()
    {
        Call("CLEAR_OVERRIDE_WEATHER");
    }
    public void _SET_BLACKOUT(bool toggle)
    {
        Call("0xAA2A0EAF", toggle);
    }
    public void SET_RADAR_ZOOM(int value)
    {
        Call("SET_RADAR_ZOOM", value);
    }
    public void SET_SEETHROUGH(bool toggle)
    {
        Call("SET_SEETHROUGH", toggle);
    }
    public void NETWORK_OVERRIDE_CLOCK_TIME(int hours, int seconds)
    {
        Call("NETWORK_OVERRIDE_CLOCK_TIME", hours, seconds);
    }
    public void SET_TIME_SCALE(float value)
    {
        Call("SET_TIME_SCALE", value);
    }
}
public class Buttonz : API
{
    public enum Buttons
    {
        Circle = 32,
        Cross = 64,
        DpadDown = 16384,
        DpadLeft = 32768,
        DpadRight = 8192,
        DpadUp = 4096,
        L1 = 4,
        L2 = 1,
        L3 = 512,
        R1 = 8,
        R2 = 2,
        R3 = 1024,
        Select = 256,
        Select_DpadUp = 4352,
        Square = 128,
        Start = 2048,
        Triangle = 16,
        L1_DpadLeft = 32772,
        L1_DpadRight = 8196,
        L1_X = 68,
        R1_DpadRight = 8200,
        R1_DpadLeft = 32776,
        R1_DpadDown = 16392,
        R1_DpadUp = 4104,

    }

    public bool ButtonPrassed(Buttons btn)
    {
        if (PS3.Extension.ReadUInt32(0x1FD87F8, true) == (int)btn)
            return true;
        else return false;
    }
}
public class Mobile : RPC
{
    public enum PhoneType
    {
        Michael,
        Trevor,
        Franklin,
        Unknown,
        Prologue
    }
    public int CREATE_MOBILE_PHONE(PhoneType phone)
    {
        return Call("CREATE_MOBILE_PHONE", (int)phone);
    }
    public void DESTROY_MOBILE_PHONE()
    {
        Call("DESTROY_MOBILE_PHONE");
    }
    public void SET_MOBILE_PHONE_SCALE(float scale)
    {
        Call("SET_MOBILE_PHONE_SCALE", scale);
    }
}
public class Zone : RPC
{
    public int GET_ZONE_AT_COORDS(float[] xyz)
    {
        return Call("GET_ZONE_AT_COORDS", xyz);
    }
    public string GET_NAME_OF_ZONE(float[] xyz)
    {
        return PS3.Extension.ReadString((uint)Call("GET_NAME_OF_ZONE"));
    }
    public int GET_ZONE_FROM_NAME_ID(float[] xyz)
    {
        return Call("GET_ZONE_FROM_NAME_ID", GET_NAME_OF_ZONE(xyz));
    }
    public int GET_ZONE_POPSCHEDULE(float[] xyz)
    {
        return Call("GET_ZONE_POPSCHEDULE", GET_ZONE_AT_COORDS(xyz));
    }
    public void SET_ZONE_ENABLED(float[] xyz, bool toggle)
    {
        Call("SET_ZONE_ENABLED", GET_ZONE_AT_COORDS(xyz), toggle);
    }
    public bool IS_ENTITY_IN_ZONE(int entity, float[] yxz)
    {
        return Convert.ToBoolean(Call("IS_ENTITY_IN_ZONE", entity,GET_NAME_OF_ZONE(yxz)));
    }
}
public class PickUp : RPC
{
    public enum Pickup
    {
        PICKUP_WEAPON_PISTOL,//=-105925489
        PICKUP_WEAPON_COMBATPISTOL,//=-1989692173
        PICKUP_WEAPON_APPISTOL,//=996550793
        PICKUP_WEAPON_MICROSMG,//=496339155
        PICKUP_WEAPON_SMG,//=978070226
        PICKUP_WEAPON_ASSAULTRIFLE,//=-214137936
        PICKUP_WEAPON_CARBINERIFLE,//=-546236071
        PICKUP_WEAPON_ADVANCEDRIFLE,//=-1296747938
        PICKUP_WEAPON_SAWNOFFSHOTGUN,//=-1766583645
        PICKUP_WEAPON_PUMPSHOTGUN,//=-1456120371
        PICKUP_WEAPON_ASSAULTSHOTGUN,//=-1835415205
        PICKUP_WEAPON_SNIPERRIFLE,//=-30788308
        PICKUP_WEAPON_HEAVYSNIPER,//=1765114797
        PICKUP_WEAPON_MG,//=-2050315855
        PICKUP_WEAPON_COMBATMG,//=-1298986476
        PICKUP_WEAPON_GRENADELAUNCHER,//=779501861
        PICKUP_WEAPON_RPG,//=1295434569
        PICKUP_WEAPON_MINIGUN,//=792114228
        PICKUP_WEAPON_GRENADE,//=1577485217
        PICKUP_WEAPON_STICKYBOMB,//=2081529176
        PICKUP_WEAPON_MOLOTOV,//=768803961
        PICKUP_WEAPON_PETROLCAN,//=-962731009
        PICKUP_WEAPON_SMOKEGRENADE,//=483787975
        PICKUP_WEAPON_KNIFE,//=663586612
        PICKUP_WEAPON_BAT,//=-2115084258
        PICKUP_WEAPON_HAMMER,//=693539241
        PICKUP_WEAPON_CROWBAR,//=-2027042680
        PICKUP_WEAPON_GOLFCLUB,//=-1997886297
        PICKUP_WEAPON_NIGHTSTICK,//=1587637620
        PICKUP_WEAPON_FIREEXTINGUISHER,//=-887893374
        PICKUP_WEAPON_LASSO,//=1724937680
        PICKUP_WEAPON_LOUDHAILER,//=2017151059
        PICKUP_PARACHUTE,//=1735599485
        PICKUP_ARMOUR_STANDARD,//=1274757841
        PICKUP_HEALTH_STANDARD,//=-1888453608
        PICKUP_VEHICLE_WEAPON_PISTOL,//=-1521817673
        PICKUP_VEHICLE_WEAPON_COMBATPISTOL,//=-794112265
        PICKUP_VEHICLE_WEAPON_APPISTOL,//=-863291131
        PICKUP_VEHICLE_WEAPON_MICROSMG,//=-1200951717
        PICKUP_VEHICLE_WEAPON_SMG,//=-864236261
        PICKUP_VEHICLE_WEAPON_SAWNOFF,//=772217690
        PICKUP_VEHICLE_WEAPON_GRENADE,//=-1491601256
        PICKUP_VEHICLE_WEAPON_MOLOTOV,//=-2066319660
        PICKUP_VEHICLE_WEAPON_SMOKEGRENADE,//=1705498857
        PICKUP_VEHICLE_WEAPON_STICKYBOMB,//=746606563
        PICKUP_VEHICLE_HEALTH_STANDARD,//=160266735
        PICKUP_VEHICLE_ARMOUR_STANDARD,//=1125567497
        PICKUP_MONEY_CASE,//=-831529621
        PICKUP_MONEY_DEP_BAG,//=545862290
        PICKUP_MONEY_MED_BAG,//=341217064
        PICKUP_MONEY_PAPER_BAG,//=1897726628
        PICKUP_PORTABLE_CRATE_UNFIXED,//=1852930709
        PICKUP_PORTABLE_PACKAGE,//=-2136239332
        PICKUP_AMMO_BULLET_MP,//=1426343849
        PICKUP_AMMO_MISSILE_MP,//=-107080240
        PICKUP_CAMERA,//=-482507216
        PICKUP_CUSTOM_SCRIPT,//=738282662
        PICKUP_HANDCUFF_KEY,//=155886031
        PICKUP_HEALTH_SNACK,//=483577702
        PICKUP_MONEY_PURSE,//=513448440
        PICKUP_MONEY_SECURITY_CASE,//=-562499202
        PICKUP_MONEY_VARIABLE,//=-31919185
        PICKUP_MONEY_WALLET,//=1575005502
        PICKUP_SUBMARINE,//=-405862452
        PICKUP_TYPE_INVALID,//=-723152950
        PICKUP_VEHICLE_CUSTOM_SCRIPT,//=-1514616151
    }
    public int CREATE_PICKUP(Pickup pickName, float[] loc, Props.PropsName prosName)
    {
        uint pickHash = Hash(pickName.ToString());
        uint propHash = 0;
        if (prosName != Props.PropsName.defult)
            propHash = Hash(prosName.ToString());
        return Call("CREATE_PICKUP", pickHash, loc, 700, 1, 1, propHash);
    }
    public int CREATE_AMBIENT_PICKUP(Pickup pickName, float[] loc, int amount)
    {
        return Call("CREATE_AMBIENT_PICKUP", Hash(pickName.ToString()), loc, 0, amount, 0x113FD533, 0, 1);
    }
}
static public class Modz
{
    public static uint CarHeight = 0xBC,
        SuperFast1 = 0x74,
        SuperFast2 = 0x50,
        Drift = 0x04;
}
public class Handling : RPC
{
    public Handling(uint handlingOffset)
    {
        this.HandlingOffset = handlingOffset;
    }
    List<uint> AllCars = new List<uint>();

    public uint CurrentVeh { get; set; }
    public uint HandlingOffset { get; set; }

    public float Speed { get; set; }
    public float Height { get; set; }

    public string CarName { get; set; }
    byte[] returnVehcileBuffer(uint hash)
    {
        return BitConverter.GetBytes(hash).Reverse<byte>().ToArray();
    }
    public uint InitVeh()
    {
        CarName = "";
        uint vehId = (uint)new Vehicle().GET_MY_PLAYER_VEHICLE_IN;
        if (vehId != 0)
        {
            CarName = new Vehicle().GET_DISPLAY_NAME_FROM_VEHICLE_MODEL((int)vehId);
            byte[] array = returnVehcileBuffer(Hash(CarName));
            CurrentVeh = new Finder().FindOffset(HandlingOffset, 0x75300, array, 0);
            Speed = returnValue(Modz.SuperFast1);
            Height = returnValue(Modz.CarHeight);
        }
        return CurrentVeh;
    }
    public float returnValue(uint differnt)
    {
        return BitConverter.ToSingle(PS3.GetBytes((CurrentVeh + differnt), 4).Reverse<byte>().ToArray(), 0);
    }
    public void SetValue(uint different, float value)
    {
        PS3.Extension.WriteFloat(CurrentVeh + different, value);
    }
    public void VehDrift()
    {
        PS3.SetMemory(CurrentVeh + Modz.Drift, driftBuffer);
        Speed = returnValue(Modz.SuperFast1);
        Height = returnValue(Modz.CarHeight);
    }
    byte[] driftBuffer = new byte[] { 0x48, 0xF4, 0x24, 0x00, 0x3A, 0x23, 0x0D, 0xB6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x99, 0x99, 0x9A, 0x3F, 0x99, 0x99, 0x9A, 0x3F, 0xCC, 0xCC, 0xCD, 0x3F, 0x99, 0x99, 0x9A, 0x42, 0xAA, 0x00, 0x00, 0x3F, 0x96, 0x96, 0x97, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x3F, 0xA6, 0x66, 0x66, 0x3F, 0xCC, 0xCC, 0xCD, 0x3F, 0xCC, 0xCC, 0xCD, 0x40, 0xAD, 0xC2, 0x8F, 0x42, 0xF0, 0x00, 0x02, 0x42, 0xC8, 0x00, 0x01, 0x3F, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x8C, 0xCC, 0xCD, 0x3F, 0x66, 0x66, 0x66, 0x3F, 0x99, 0x99, 0x9A, 0x3F, 0x8C, 0xBE, 0x4C, 0x3F, 0x68, 0xD2, 0x2A, 0x3F, 0x66, 0x66, 0x66, 0x3F, 0x8E, 0x38, 0xE4, 0x3F, 0xB3, 0x33, 0x33, 0x4C, 0xBE, 0xBC, 0x20, 0x3E, 0xC9, 0x0F, 0xDB, 0x40, 0x22, 0xF9, 0x83, 0x3E, 0x19, 0x99, 0x9A, 0x40, 0xD5, 0x55, 0x55, 0x3F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x40, 0x40, 0x00, 0x00, 0x3E, 0x38, 0x51, 0xEB, 0x3E, 0x8F, 0x5C, 0x29, 0x3D, 0xCC, 0xCC, 0xCD, 0xBE, 0x23, 0xD7, 0x0A, 0xBC, 0xA3, 0xD7, 0x0A, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x70, 0xA3, 0xD7, 0x3F, 0x87, 0xAE, 0x14, 0x3E, 0x4C, 0xCC, 0xCD, 0x3E, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x82, 0x00, 0x00, 0x40, 0xA0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3D, 0xCC, 0xCC, 0xCD, 0x00, 0x02, 0x49, 0xF0, 0x44, 0x00, 0x10, 0x00, 0x00, 0x44, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC7, 0xB2, 0xC3, 0xC0, 0x6E, 0x3C, 0x5C, 0x6B };
    bool launchOffsets = false;
    public void DriftAllVehicle(TextBox info, string[] vehicles)
    {
        PS3.InitTarget();
        if (!launchOffsets)
        {

            foreach (var item in vehicles)
            {
                uint car = Hash(item);
                if (new Entity().IS_THIS_MODEL_A_CAR(car))
                {
                    uint offset = new Finder().FindOffset(HandlingOffset, 0x89999, returnVehcileBuffer(car), 0);
                    if (offset != 0)
                    {
                        AllCars.Add(offset);
                        info.AppendText(new Vehicle().GET_DISPLAY_NAME_FROM_VEHICLE_MODEL((int)car) + " Applied\n");
                    }
                }
            }
            info.AppendText("All Vehicles successfully Applied\n");
            launchOffsets = true;
        }
        for (int i = 0; i < AllCars.Count; i++)
        {
            PS3.SetMemory(AllCars[i] + Modz.Drift, driftBuffer);
        }
    }
}
public class Finder : API
{
    public uint FindOffset(uint StartOffset, int length, byte[] toFind, int add)
    {
        byte[] toSearch = new byte[length];
        PS3.GetMemory(StartOffset, toSearch);
        int num = 0;
        while (num + toFind.Length < toSearch.Length)
        {
            bool flag = true;
            for (int index = 0; index <= toFind.Length - 1; index++)
            {
                if (Convert.ToInt32(toSearch[num + index]) != Convert.ToInt32(toFind[index]))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                return StartOffset + Convert.ToUInt32(num + add);
            }
            num += 4;
        }
        return 0;
    }
}
public class Props : RPC
{
    public enum PropsName
    {
        defult,
        prop_a4_pile_01,
prop_a4_sheet_01,
prop_a4_sheet_02,
prop_a4_sheet_03,
prop_a4_sheet_04,
prop_a4_sheet_05,
prop_abat_roller_static,
prop_abat_slide,
prop_acc_guitar_01,
prop_acc_guitar_01_d1,
prop_aerial_01a,
prop_aerial_01b,
prop_aerial_01c,
prop_aerial_01d,
prop_afsign_amun,
prop_afsign_vbike,
prop_agave_01,
prop_agave_02,
prop_aiprort_sign_01,
prop_aiprort_sign_02,
prop_aircon_l_01,
prop_aircon_l_02,
prop_aircon_l_03,
prop_aircon_l_04,
prop_aircon_m_09,
prop_aircon_s_01a,
prop_aircon_s_02a,
prop_aircon_s_02b,
prop_aircon_s_03a,
prop_aircon_s_03b,
prop_aircon_s_04a,
prop_aircon_s_05a,
prop_aircon_s_06a,
prop_aircon_s_07a,
prop_aircon_s_07b,
prop_airhockey_01,
prop_air_bagloader,
prop_air_bagloader2,
prop_air_barrier,
prop_air_bench_01,
prop_air_bench_02,
prop_air_bigradar_l1,
prop_air_bigradar_l2,
prop_air_bigradar_slod,
prop_air_blastfence_01,
prop_air_blastfence_02,
prop_air_bridge01,
prop_air_bridge02,
prop_air_cargoloader_01,
prop_air_cargo_01a,
prop_air_cargo_01b,
prop_air_cargo_01c,
prop_air_cargo_02a,
prop_air_cargo_02b,
prop_air_cargo_03a,
prop_air_cargo_04a,
prop_air_cargo_04b,
prop_air_cargo_04c,
prop_air_chock_01,
prop_air_chock_03,
prop_air_chock_04,
prop_air_fueltrail1,
prop_air_fueltrail2,
prop_air_gasbogey_01,
prop_air_generator_01,
prop_air_generator_03,
prop_air_hoc_paddle_01,
prop_air_hoc_paddle_02,
prop_air_lights_01a,
prop_air_lights_01b,
prop_air_lights_03a,
prop_air_luggtrolley,
prop_air_mast_01,
prop_air_mast_02,
prop_air_monhut_01,
prop_air_monhut_02,
prop_air_monhut_03,
prop_air_propeller01,
prop_air_radar_01,
prop_air_stair_01,
prop_air_stair_02,
prop_air_stair_03,
prop_air_stair_04a,
prop_air_stair_04b,
prop_air_towbar_01,
prop_air_towbar_02,
prop_air_towbar_03,
prop_air_trailer_4a,
prop_air_trailer_4b,
prop_air_trailer_4c,
prop_air_watertank1,
prop_air_watertank2,
prop_air_windsock_base,
prop_air_woodsteps,
prop_alarm_01,
prop_alarm_02,
prop_alien_egg_01,
prop_aloevera_01,
prop_amanda_note_01,
prop_amanda_note_01b,
prop_amb_40oz_02,
prop_amb_40oz_03,
prop_amb_beer_bottle,
prop_amb_ciggy_01,
prop_amb_donut,
prop_amb_handbag_01,
prop_amb_phone,
prop_amp_01,
prop_am_box_wood_01,
prop_anim_cash_note,
prop_anim_cash_note_b,
prop_anim_cash_pile_01,
prop_anim_cash_pile_02,
prop_apple_box_01,
prop_apple_box_02,
prop_arcade_01,
prop_arcade_02,
prop_arc_blueprints_01,
prop_armchair_01,
prop_armenian_gate,
prop_armour_pickup,
prop_arm_gate_l,
prop_arm_wrestle_01,
prop_artgallery_02_dl,
prop_artgallery_02_dr,
prop_artgallery_dl,
prop_artgallery_dr,
prop_artifact_01,
prop_ashtray_01,
prop_asteroid_01,
prop_atm_02,
prop_atm_03,
prop_attache_case_01,
prop_aviators_01,
prop_a_base_bars_01,
prop_a_trailer_door_01,
prop_bahammenu,
prop_ballistic_shield,
prop_ballistic_shield_lod1,
prop_bandsaw_01,
prop_bank_shutter,
prop_bank_vaultdoor,
prop_barbell_01,
prop_barbell_02,
prop_barbell_100kg,
prop_barbell_10kg,
prop_barbell_20kg,
prop_barbell_30kg,
prop_barbell_40kg,
prop_barbell_50kg,
prop_barbell_60kg,
prop_barbell_80kg,
prop_barier_conc_01b,
prop_barier_conc_01c,
prop_barier_conc_02b,
prop_barier_conc_02c,
prop_barier_conc_03a,
prop_barier_conc_04a,
prop_barier_conc_05a,
prop_barier_conc_05b,
prop_barn_door_l,
prop_barn_door_r,
prop_barrachneon,
prop_barrel_01a,
prop_barrel_02a,
prop_barrel_02b,
prop_barrel_03a,
prop_barrel_03d,
prop_barrel_float_1,
prop_barrel_float_2,
prop_barriercrash_03,
prop_barriercrash_04,
prop_barrier_wat_01a,
prop_barrier_wat_03b,
prop_barrier_work01c,
prop_barry_table_detail,
prop_bar_coastbarr,
prop_bar_coastchamp,
prop_bar_coastdusc,
prop_bar_coastmount,
prop_bar_cooler_01,
prop_bar_cooler_03,
prop_bar_fridge_01,
prop_bar_fridge_02,
prop_bar_fridge_03,
prop_bar_fridge_04,
prop_bar_ice_01,
prop_bar_napkindisp,
prop_bar_pump_01,
prop_bar_pump_04,
prop_bar_pump_05,
prop_bar_pump_06,
prop_bar_pump_07,
prop_bar_pump_08,
prop_bar_pump_09,
prop_bar_pump_10,
prop_bar_sink_01,
prop_bar_stool_01,
prop_basejump_target_01,
prop_basketball_net,
prop_bath_dirt_01,
prop_battery_01,
prop_battery_02,
prop_bball_arcade_01,
prop_bbq_2,
prop_bbq_3,
prop_beachbag_01,
prop_beachbag_02,
prop_beachbag_03,
prop_beachbag_04,
prop_beachbag_05,
prop_beachbag_06,
prop_beachbag_combo_01,
prop_beachbag_combo_02,
prop_beachball_02,
prop_beachflag_le,
prop_beach_bars_01,
prop_beach_bars_02,
prop_beach_bbq,
prop_beach_dip_bars_01,
prop_beach_dip_bars_02,
prop_beach_fire,
prop_beach_lg_float,
prop_beach_lg_stretch,
prop_beach_lg_surf,
prop_beach_lotion_01,
prop_beach_lotion_02,
prop_beach_lotion_03,
prop_beach_punchbag,
prop_beach_rings_01,
prop_beach_sculp_01,
prop_beach_towel_02,
prop_beach_volball01,
prop_beach_volball02,
prop_beerneon,
prop_beer_bison,
prop_beer_box_01,
prop_beer_neon_01,
prop_beer_neon_02,
prop_beer_neon_03,
prop_beer_neon_04,
prop_beggers_sign_01,
prop_beggers_sign_02,
prop_beggers_sign_03,
prop_beggers_sign_04,
prop_bench_01b,
prop_bench_01c,
prop_bench_04,
prop_bench_05,
prop_bench_09,
prop_beta_tape,
prop_beware_dog_sign,
prop_bh1_03_gate_l,
prop_bh1_03_gate_r,
prop_bh1_08_mp_gar,
prop_bh1_09_mp_gar,
prop_bh1_09_mp_l,
prop_bh1_09_mp_r,
prop_bh1_16_display,
prop_bh1_44_door_01l,
prop_bh1_44_door_01r,
prop_bh1_48_backdoor_l,
prop_bh1_48_backdoor_r,
prop_bh1_48_gate_1,
prop_bhhotel_door_l,
prop_bhhotel_door_r,
prop_big_bag_01,
prop_big_clock_01,
prop_big_shit_01,
prop_big_shit_02,
prop_bikerack_2,
prop_bikini_disp_01,
prop_bikini_disp_02,
prop_bikini_disp_03,
prop_bikini_disp_04,
prop_bikini_disp_05,
prop_bikini_disp_06,
prop_billboard_01,
prop_billboard_02,
prop_billboard_03,
prop_billboard_04,
prop_billboard_05,
prop_billboard_06,
prop_billboard_07,
prop_billboard_08,
prop_billboard_09,
prop_billboard_09wall,
prop_billboard_10,
prop_billboard_11,
prop_billboard_12,
prop_billboard_13,
prop_billboard_14,
prop_billboard_15,
prop_billboard_16,
prop_billb_frame01a,
prop_billb_frame01b,
prop_billb_frame02a,
prop_billb_frame02b,
prop_billb_frame03a,
prop_billb_frame03b,
prop_billb_frame03c,
prop_billb_frame04a,
prop_billb_frame04b,
prop_binoc_01,
prop_bin_04a,
prop_bin_10a,
prop_bin_10b,
prop_bin_11a,
prop_bin_11b,
prop_bin_12a,
prop_bin_13a,
prop_bin_14a,
prop_bin_14b,
prop_bin_beach_01d,
prop_bin_delpiero,
prop_bin_delpiero_b,
prop_biolab_g_door,
prop_biotech_store,
prop_bison_winch,
prop_blackjack_01,
prop_bleachers_01,
prop_bleachers_02,
prop_bleachers_03,
prop_bleachers_04,
prop_bleachers_05,
prop_blox_spray,
prop_bmu_01,
prop_bmu_01_b,
prop_bmu_02,
prop_bmu_02_ld,
prop_bmu_02_ld_cab,
prop_bmu_02_ld_sup,
prop_bmu_track01,
prop_bmu_track02,
prop_bmu_track03,
prop_bodyarmour_02,
prop_bodyarmour_03,
prop_bodyarmour_04,
prop_bodyarmour_05,
prop_bodyarmour_06,
prop_bollard_01a,
prop_bollard_01b,
prop_bollard_01c,
prop_bollard_03a,
prop_bomb_01,
prop_bomb_01_s,
prop_bonesaw,
prop_bongos_01,
prop_bong_01,
prop_boogbd_stack_01,
prop_boogbd_stack_02,
prop_boogieboard_01,
prop_boogieboard_02,
prop_boogieboard_03,
prop_boogieboard_04,
prop_boogieboard_05,
prop_boogieboard_06,
prop_boogieboard_07,
prop_boogieboard_08,
prop_boogieboard_09,
prop_boogieboard_10,
prop_boombox_01,
prop_bottle_cap_01,
prop_bowling_ball,
prop_bowling_pin,
prop_bowl_crisps,
prop_boxcar5_handle,
prop_boxing_glove_01,
prop_boxpile_10a,
prop_boxpile_10b,
prop_box_ammo01a,
prop_box_ammo02a,
prop_box_ammo03a_set,
prop_box_ammo03a_set2,
prop_box_ammo04a,
prop_box_ammo05b,
prop_box_ammo07a,
prop_box_ammo07b,
prop_box_guncase_01a,
prop_box_guncase_02a,
prop_box_guncase_03a,
prop_box_tea01a,
prop_box_wood05a,
prop_box_wood05b,
prop_box_wood08a,
prop_breadbin_01,
prop_bread_rack_01,
prop_bread_rack_02,
prop_broken_cboard_p1,
prop_broken_cboard_p2,
prop_broken_cell_gate_01,
prop_broom_unit_01,
prop_bskball_01,
prop_bs_map_door_01,
prop_buckets_02,
prop_bucket_01a,
prop_bucket_01b,
prop_bucket_02a,
prop_buck_spade_01,
prop_buck_spade_02,
prop_buck_spade_03,
prop_buck_spade_04,
prop_buck_spade_05,
prop_buck_spade_06,
prop_buck_spade_07,
prop_buck_spade_08,
prop_buck_spade_09,
prop_buck_spade_10,
prop_bumper_01,
prop_bumper_02,
prop_bumper_03,
prop_bumper_04,
prop_bumper_05,
prop_bumper_06,
prop_bumper_car_01,
prop_burto_gate_01,
prop_bush_dead_02,
prop_bush_grape_01,
prop_bush_ivy_01_1m,
prop_bush_ivy_01_2m,
prop_bush_ivy_01_bk,
prop_bush_ivy_01_l,
prop_bush_ivy_01_pot,
prop_bush_ivy_01_r,
prop_bush_ivy_01_top,
prop_bush_ivy_02_1m,
prop_bush_ivy_02_2m,
prop_bush_ivy_02_l,
prop_bush_ivy_02_pot,
prop_bush_ivy_02_r,
prop_bush_ivy_02_top,
prop_bush_lrg_01,
prop_bush_lrg_01b,
prop_bush_lrg_01c,
prop_bush_lrg_01d,
prop_bush_lrg_01e,
prop_bush_lrg_02,
prop_bush_lrg_02b,
prop_bush_lrg_03,
prop_bush_lrg_03b,
prop_bush_lrg_04b,
prop_bush_lrg_04c,
prop_bush_lrg_04d,
prop_bush_med_01,
prop_bush_med_02,
prop_bush_med_03,
prop_bush_med_05,
prop_bush_med_06,
prop_bush_med_07,
prop_bush_neat_01,
prop_bush_neat_02,
prop_bush_neat_03,
prop_bush_neat_04,
prop_bush_neat_05,
prop_bush_neat_06,
prop_bush_neat_07,
prop_bush_neat_08,
prop_bush_ornament_01,
prop_bush_ornament_02,
prop_bush_ornament_03,
prop_bush_ornament_04,
prop_busker_hat_01,
prop_byard_bench01,
prop_byard_bench02,
prop_byard_block_01,
prop_byard_boat01,
prop_byard_boat02,
prop_byard_chains01,
prop_byard_dingy,
prop_byard_elecbox01,
prop_byard_elecbox02,
prop_byard_elecbox03,
prop_byard_elecbox04,
prop_byard_floatpile,
prop_byard_float_01,
prop_byard_float_01b,
prop_byard_float_02,
prop_byard_float_02b,
prop_byard_hoist,
prop_byard_hoist_2,
prop_byard_hoses01,
prop_byard_hoses02,
prop_byard_ladder01,
prop_byard_machine01,
prop_byard_machine02,
prop_byard_machine03,
prop_byard_motor_01,
prop_byard_motor_02,
prop_byard_motor_03,
prop_byard_net02,
prop_byard_phone,
prop_byard_pipes01,
prop_byard_pipe_01,
prop_byard_planks01,
prop_byard_pulley01,
prop_byard_rack,
prop_byard_ramp,
prop_byard_rampold,
prop_byard_rowboat1,
prop_byard_rowboat2,
prop_byard_rowboat3,
prop_byard_rowboat4,
prop_byard_rowboat5,
prop_byard_scfhold01,
prop_byard_sleeper01,
prop_byard_sleeper02,
prop_byard_steps_01,
prop_byard_tank_01,
prop_byard_trailer01,
prop_byard_trailer02,
prop_b_board_blank,
prop_c4_final,
prop_c4_final_green,
prop_cabinet_01,
prop_cabinet_01b,
prop_cabinet_02b,
prop_cablespool_01a,
prop_cablespool_01b,
prop_cablespool_02,
prop_cablespool_03,
prop_cablespool_04,
prop_cablespool_05,
prop_cablespool_06,
prop_cable_hook_01,
prop_camera_strap,
prop_candy_pqs,
prop_can_canoe,
prop_cap_01,
prop_cap_01b,
prop_cap_row_01,
prop_cap_row_01b,
prop_cap_row_02,
prop_cap_row_02b,
prop_carcreeper,
prop_cargo_int,
prop_carjack,
prop_carjack_l2,
prop_carrier_bag_01,
prop_carrier_bag_01_lod,
prop_cartwheel_01,
prop_carwash_roller_horz,
prop_carwash_roller_vert,
prop_car_battery_01,
prop_car_bonnet_01,
prop_car_bonnet_02,
prop_car_door_01,
prop_car_door_02,
prop_car_door_03,
prop_car_door_04,
prop_car_engine_01,
prop_car_exhaust_01,
prop_car_ignition,
prop_car_seat,
prop_casey_sec_id,
prop_cash_case_01,
prop_cash_case_02,
prop_cash_crate_01,
prop_cash_dep_bag_01,
prop_cash_envelope_01,
prop_cash_note_01,
prop_cash_pile_01,
prop_cash_pile_02,
prop_cash_trolly,
prop_casino_door_01l,
prop_casino_door_01r,
prop_cattlecrush,
prop_cat_tail_01,
prop_cctv_02_sm,
prop_cctv_cont_01,
prop_cctv_cont_03,
prop_cctv_cont_04,
prop_cctv_cont_05,
prop_cctv_cont_06,
prop_cctv_unit_01,
prop_cctv_unit_02,
prop_cctv_unit_05,
prop_cementmixer_01a,
prop_cementmixer_02a,
prop_ceramic_jug_01,
prop_ceramic_jug_cork,
prop_ch1_07_door_01l,
prop_ch1_07_door_01r,
prop_ch1_07_door_02l,
prop_ch1_07_door_02r,
prop_ch2_05d_g_door,
prop_ch2_07b_20_g_door,
prop_ch2_09b_door,
prop_ch2_09c_garage_door,
prop_ch3_01_trlrdoor_l,
prop_ch3_01_trlrdoor_r,
prop_ch3_04_door_01l,
prop_ch3_04_door_01r,
prop_ch3_04_door_02,
prop_chair_01a,
prop_chair_01b,
prop_chair_02,
prop_chair_03,
prop_chair_04a,
prop_chair_04b,
prop_chair_05,
prop_chair_06,
prop_chair_07,
prop_chair_08,
prop_chair_09,
prop_chair_10,
prop_chair_pile_01,
prop_chall_lamp_01,
prop_chall_lamp_01n,
prop_chall_lamp_02,
prop_chateau_chair_01,
prop_cheetah_covered,
prop_chem_grill,
prop_chem_grill_bit,
prop_chem_vial_02,
prop_chem_vial_02b,
prop_cherenneon,
prop_chickencoop_a,
prop_chip_fryer,
prop_choc_ego,
prop_choc_meto,
prop_choc_pq,
prop_ch_025c_g_door_01,
prop_cigar_01,
prop_cigar_02,
prop_cigar_03,
prop_cigar_pack_01,
prop_cigar_pack_02,
prop_clapper_brd_01,
prop_cleaver,
prop_cliff_paper,
prop_clippers_01,
prop_clothes_rail_02,
prop_clothes_rail_03,
prop_clothes_rail_2b,
prop_clothes_tub_01,
prop_clown_chair,
prop_cntrdoor_ld_l,
prop_cntrdoor_ld_r,
prop_coathook_01,
prop_cockneon,
prop_coffee_cup_trailer,
prop_coffee_mac_02,
prop_coffin_02,
prop_coffin_02b,
prop_coke_block_01,
prop_coke_block_half_a,
prop_coke_block_half_b,
prop_compressor_01,
prop_compressor_02,
prop_compressor_03,
prop_com_gar_door_01,
prop_com_ls_door_01,
prop_conc_sacks_02a,
prop_cone_float_1,
prop_conschute,
prop_consign_01c,
prop_consign_02a,
prop_conslift_base,
prop_conslift_brace,
prop_conslift_cage,
prop_conslift_door,
prop_conslift_lift,
prop_conslift_rail,
prop_conslift_rail2,
prop_conslift_steps,
prop_console_01,
prop_construcionlamp_01,
prop_const_fence01a,
prop_const_fence01b,
prop_const_fence02a,
prop_const_fence02b,
prop_const_fence03b,
prop_cons_crate,
prop_cons_plank,
prop_cons_ply01,
prop_cons_ply02,
prop_container_01a,
prop_container_01b,
prop_container_01c,
prop_container_01d,
prop_container_01e,
prop_container_01f,
prop_container_01g,
prop_container_01h,
prop_container_01mb,
prop_container_02a,
prop_container_03a,
prop_container_03b,
prop_container_03mb,
prop_container_03_ld,
prop_container_04a,
prop_container_04mb,
prop_container_05mb,
prop_container_door_mb_l,
prop_container_door_mb_r,
prop_container_hole,
prop_container_ld,
prop_container_ld2,
prop_container_old1,
prop_contnr_pile_01a,
prop_controller_01,
prop_control_rm_door_01,
prop_cont_chiller_01,
prop_cooker_03,
prop_copier_01,
prop_copper_pan,
prop_coral_bush_01,
prop_coral_flat_01,
prop_coral_flat_01_l1,
prop_coral_flat_02,
prop_coral_flat_brainy,
prop_coral_flat_clam,
prop_coral_grass_01,
prop_coral_grass_02,
prop_coral_kelp_01,
prop_coral_kelp_01_l1,
prop_coral_kelp_02,
prop_coral_kelp_02_l1,
prop_coral_kelp_03,
prop_coral_kelp_03a,
prop_coral_kelp_03b,
prop_coral_kelp_03c,
prop_coral_kelp_03d,
prop_coral_kelp_03_l1,
prop_coral_kelp_04,
prop_coral_kelp_04_l1,
prop_coral_pillar_01,
prop_coral_pillar_02,
prop_coral_spikey_01,
prop_coral_stone_03,
prop_coral_stone_04,
prop_coral_sweed_01,
prop_coral_sweed_02,
prop_coral_sweed_03,
prop_coral_sweed_04,
prop_cora_clam_01,
prop_cork_board,
prop_couch_01,
prop_couch_03,
prop_couch_04,
prop_couch_lg_02,
prop_couch_lg_05,
prop_couch_lg_06,
prop_couch_lg_07,
prop_couch_lg_08,
prop_couch_sm1_07,
prop_couch_sm2_07,
prop_couch_sm_02,
prop_couch_sm_05,
prop_couch_sm_06,
prop_couch_sm_07,
prop_crane_01_truck1,
prop_crane_01_truck2,
prop_cranial_saw,
prop_crashed_heli,
prop_cratepile_07a_l1,
prop_crate_01a,
prop_crate_02a,
prop_crate_08a,
prop_crate_09a,
prop_crate_10a,
prop_crate_11a,
prop_crate_11b,
prop_crate_11c,
prop_crate_11d,
prop_crate_float_1,
prop_creosote_b_01,
prop_crisp,
prop_crisp_small,
prop_crosssaw_01,
prop_cs1_14b_traind,
prop_cs1_14b_traind_dam,
prop_cs4_05_tdoor,
prop_cs4_10_tr_gd_01,
prop_cs4_11_door,
prop_cs6_03_door_l,
prop_cs6_03_door_r,
prop_cs_20m_rope,
prop_cs_30m_rope,
prop_cs_abattoir_switch,
prop_cs_aircon_01,
prop_cs_aircon_fan,
prop_cs_amanda_shoe,
prop_cs_ashtray,
prop_cs_bandana,
prop_cs_bar,
prop_cs_beachtowel_01,
prop_cs_beer_bot_01,
prop_cs_beer_bot_01b,
prop_cs_beer_bot_01lod,
prop_cs_beer_bot_02,
prop_cs_beer_bot_03,
prop_cs_beer_bot_40oz,
prop_cs_beer_bot_40oz_02,
prop_cs_beer_bot_40oz_03,
prop_cs_beer_bot_test,
prop_cs_binder_01,
prop_cs_bin_01,
prop_cs_bin_01_lid,
prop_cs_bin_01_skinned,
prop_cs_bin_02,
prop_cs_bin_03,
prop_cs_book_01,
prop_cs_bottle_opener,
prop_cs_bowie_knife,
prop_cs_bowl_01,
prop_cs_bowl_01b,
prop_cs_box_clothes,
prop_cs_box_step,
prop_cs_brain_chunk,
prop_cs_bs_cup,
prop_cs_bucket_s,
prop_cs_bucket_s_lod,
prop_cs_burger_01,
prop_cs_business_card,
prop_cs_cardbox_01,
prop_cs_cash_note_01,
prop_cs_cctv,
prop_cs_champ_flute,
prop_cs_ciggy_01,
prop_cs_ciggy_01b,
prop_cs_clothes_box,
prop_cs_coke_line,
prop_cs_cont_latch,
prop_cs_crackpipe,
prop_cs_credit_card,
prop_cs_creeper_01,
prop_cs_crisps_01,
prop_cs_cuffs_01,
prop_cs_diaphram,
prop_cs_dildo_01,
prop_cs_documents_01,
prop_cs_dog_lead_2a,
prop_cs_dog_lead_2b,
prop_cs_dog_lead_2c,
prop_cs_dog_lead_3a,
prop_cs_dog_lead_3b,
prop_cs_dog_lead_a,
prop_cs_dog_lead_b,
prop_cs_dog_lead_c,
prop_cs_duffel_01,
prop_cs_duffel_01b,
prop_cs_dumpster_01a,
prop_cs_dumpster_lidl,
prop_cs_dumpster_lidr,
prop_cs_dvd,
prop_cs_dvd_case,
prop_cs_dvd_player,
prop_cs_envolope_01,
prop_cs_fertilizer,
prop_cs_film_reel_01,
prop_cs_folding_chair_01,
prop_cs_fork,
prop_cs_frank_photo,
prop_cs_freightdoor_l1,
prop_cs_freightdoor_r1,
prop_cs_fridge,
prop_cs_fridge_door,
prop_cs_fuel_hose,
prop_cs_fuel_nozle,
prop_cs_gascutter_1,
prop_cs_gascutter_2,
prop_cs_glass_scrap,
prop_cs_gravyard_gate_l,
prop_cs_gravyard_gate_r,
prop_cs_gunrack,
prop_cs_hand_radio,
prop_cs_heist_bag_01,
prop_cs_heist_bag_02,
prop_cs_heist_bag_strap_01,
prop_cs_heist_rope,
prop_cs_heist_rope_b,
prop_cs_hotdog_01,
prop_cs_hotdog_02,
prop_cs_h_bag_strap_01,
prop_cs_ice_locker,
prop_cs_ice_locker_door_l,
prop_cs_ice_locker_door_r,
prop_cs_ilev_blind_01,
prop_cs_ironing_board,
prop_cs_katana_01,
prop_cs_kettle_01,
prop_cs_keyboard_01,
prop_cs_keys_01,
prop_cs_kitchen_cab_l2,
prop_cs_kitchen_cab_ld,
prop_cs_kitchen_cab_rd,
prop_cs_lazlow_ponytail,
prop_cs_lazlow_shirt_01,
prop_cs_lazlow_shirt_01b,
prop_cs_leaf,
prop_cs_leg_chain_01,
prop_cs_lester_crate,
prop_cs_lipstick,
prop_cs_magazine,
prop_cs_marker_01,
prop_cs_meth_pipe,
prop_cs_milk_01,
prop_cs_mini_tv,
prop_cs_mopbucket_01,
prop_cs_mop_s,
prop_cs_mouse_01,
prop_cs_nail_file,
prop_cs_newspaper,
prop_cs_office_chair,
prop_cs_overalls_01,
prop_cs_package_01,
prop_cs_padlock,
prop_cs_pamphlet_01,
prop_cs_panel_01,
prop_cs_panties,
prop_cs_panties_02,
prop_cs_panties_03,
prop_cs_paper_cup,
prop_cs_para_ropebit,
prop_cs_para_ropes,
prop_cs_pebble,
prop_cs_pebble_02,
prop_cs_petrol_can,
prop_cs_phone_01,
prop_cs_photoframe_01,
prop_cs_pills,
prop_cs_plane_int_01,
prop_cs_planning_photo,
prop_cs_plant_01,
prop_cs_plate_01,
prop_cs_polaroid,
prop_cs_police_torch,
prop_cs_pour_tube,
prop_cs_power_cell,
prop_cs_power_cord,
prop_cs_protest_sign_01,
prop_cs_protest_sign_02,
prop_cs_protest_sign_02b,
prop_cs_protest_sign_03,
prop_cs_protest_sign_04a,
prop_cs_protest_sign_04b,
prop_cs_rage_statue_p1,
prop_cs_rage_statue_p2,
prop_cs_remote_01,
prop_cs_rolled_paper,
prop_cs_rope_tie_01,
prop_cs_rub_binbag_01,
prop_cs_rub_box_01,
prop_cs_rub_box_02,
prop_cs_r_business_card,
prop_cs_sack_01,
prop_cs_saucer_01,
prop_cs_sc1_11_gate,
prop_cs_scissors,
prop_cs_script_bottle,
prop_cs_script_bottle_01,
prop_cs_server_drive,
prop_cs_sheers,
prop_cs_shirt_01,
prop_cs_shopping_bag,
prop_cs_shot_glass,
prop_cs_silver_tray,
prop_cs_sink_filler,
prop_cs_sink_filler_02,
prop_cs_sink_filler_03,
prop_cs_sm_27_gate,
prop_cs_sol_glasses,
prop_cs_spray_can,
prop_cs_steak,
prop_cs_stock_book,
prop_cs_street_binbag_01,
prop_cs_street_card_01,
prop_cs_street_card_02,
prop_cs_sub_hook_01,
prop_cs_sub_rope_01,
prop_cs_swipe_card,
prop_cs_tablet,
prop_cs_tablet_02,
prop_cs_toaster,
prop_cs_trolley_01,
prop_cs_trowel,
prop_cs_truck_ladder,
prop_cs_tshirt_ball_01,
prop_cs_tv_stand,
prop_cs_t_shirt_pile,
prop_cs_valve,
prop_cs_vent_cover,
prop_cs_vial_01,
prop_cs_walkie_talkie,
prop_cs_walking_stick,
prop_cs_whiskey_bottle,
prop_cs_whiskey_bot_stop,
prop_cs_wrench,
prop_cub_door_lifeblurb,
prop_cub_lifeblurb,
prop_cuff_keys_01,
prop_cup_saucer_01,
prop_curl_bar_01,
prop_damdoor_01,
prop_dart_1,
prop_dart_2,
prop_dart_bd_01,
prop_dart_bd_cab_01,
prop_defilied_ragdoll_01,
prop_desert_iron_01,
prop_detergent_01a,
prop_detergent_01b,
prop_devin_box_01,
prop_devin_rope_01,
prop_diggerbkt_01,
prop_direct_chair_01,
prop_direct_chair_02,
prop_display_unit_01,
prop_display_unit_02,
prop_disp_cabinet_002,
prop_disp_cabinet_01,
prop_disp_razor_01,
prop_distantcar_day,
prop_distantcar_night,
prop_distantcar_truck,
prop_dj_deck_01,
prop_dj_deck_02,
prop_dock_bouy_1,
prop_dock_bouy_2,
prop_dock_bouy_3,
prop_dock_bouy_5,
prop_dock_crane_01,
prop_dock_crane_02,
prop_dock_crane_02_cab,
prop_dock_crane_02_hook,
prop_dock_crane_02_ld,
prop_dock_crane_04,
prop_dock_crane_lift,
prop_dock_float_1,
prop_dock_float_1b,
prop_dock_moor_01,
prop_dock_moor_04,
prop_dock_moor_05,
prop_dock_moor_06,
prop_dock_moor_07,
prop_dock_ropefloat,
prop_dock_ropetyre1,
prop_dock_ropetyre2,
prop_dock_ropetyre3,
prop_dock_rtg_01,
prop_dock_rtg_ld,
prop_dock_shippad,
prop_dock_sign_01,
prop_dock_woodpole1,
prop_dock_woodpole2,
prop_dock_woodpole3,
prop_dock_woodpole4,
prop_dock_woodpole5,
prop_dog_cage_01,
prop_dog_cage_02,
prop_dolly_01,
prop_dolly_02,
prop_donut_01,
prop_donut_02,
prop_donut_02b,
prop_door_01,
prop_door_balcony_frame,
prop_door_balcony_left,
prop_door_balcony_right,
prop_door_bell_01,
prop_double_grid_line,
prop_dress_disp_01,
prop_dress_disp_02,
prop_dress_disp_03,
prop_dress_disp_04,
prop_drop_armscrate_01,
prop_drop_armscrate_01b,
prop_drop_crate_01,
prop_drop_crate_01_set,
prop_drop_crate_01_set2,
prop_drug_burner,
prop_drug_package,
prop_drug_package_02,
prop_drywallpile_01,
prop_drywallpile_02,
prop_dt1_13_groundlight,
prop_dt1_13_walllightsource,
prop_dt1_20_mp_door_l,
prop_dt1_20_mp_door_r,
prop_dt1_20_mp_gar,
prop_ducktape_01,
prop_dummy_01,
prop_dummy_car,
prop_dummy_light,
prop_dummy_plane,
prop_dumpster_3a,
prop_dumpster_3step,
prop_dumpster_4a,
prop_dumpster_4b,
prop_d_balcony_l_light,
prop_d_balcony_r_light,
prop_ear_defenders_01,
prop_ecg_01,
prop_ecg_01_cable_01,
prop_ecg_01_cable_02,
prop_ecola_can,
prop_egg_clock_01,
prop_ejector_seat_01,
prop_elecbox_03a,
prop_elecbox_10,
prop_elecbox_12,
prop_elecbox_13,
prop_elecbox_14,
prop_elecbox_15,
prop_elecbox_16,
prop_elecbox_17,
prop_elecbox_18,
prop_elecbox_19,
prop_elecbox_20,
prop_elecbox_21,
prop_elecbox_22,
prop_elecbox_23,
prop_elecbox_24,
prop_elecbox_24b,
prop_elecbox_25,
prop_el_guitar_01,
prop_el_guitar_02,
prop_el_guitar_03,
prop_employee_month_01,
prop_employee_month_02,
prop_energy_drink,
prop_entityxf_covered,
prop_epsilon_door_l,
prop_epsilon_door_r,
prop_etricmotor_01,
prop_exer_bike_01,
prop_faceoffice_door_l,
prop_faceoffice_door_r,
prop_face_rag_01,
prop_facgate_01,
prop_facgate_01b,
prop_facgate_02pole,
prop_facgate_02_l,
prop_facgate_03post,
prop_facgate_03_l,
prop_facgate_03_ld_l,
prop_facgate_03_ld_r,
prop_facgate_03_r,
prop_facgate_04_l,
prop_facgate_04_r,
prop_facgate_05_r,
prop_facgate_05_r_dam_l1,
prop_facgate_05_r_l1,
prop_facgate_06_l,
prop_facgate_06_r,
prop_facgate_07,
prop_facgate_07b,
prop_facgate_08,
prop_facgate_08_frame,
prop_facgate_08_ld2,
prop_facgate_id1_27,
prop_fac_machine_02,
prop_fag_packet_01,
prop_fan_01,
prop_fan_palm_01a,
prop_fax_01,
prop_fbi3_coffee_table,
prop_fbibombbin,
prop_fbibombcupbrd,
prop_fbibombfile,
prop_fbibombplant,
prop_feeder1,
prop_feed_sack_01,
prop_feed_sack_02,
prop_fence_log_01,
prop_fence_log_02,
prop_ferris_car_01,
prop_ferris_car_01_lod1,
prop_ff_counter_01,
prop_ff_counter_02,
prop_ff_counter_03,
prop_ff_noodle_01,
prop_ff_noodle_02,
prop_ff_shelves_01,
prop_ff_sink_01,
prop_ff_sink_02,
prop_fib_badge,
prop_fib_broken_window,
prop_fib_skylight_piece,
prop_film_cam_01,
prop_fireescape_01a,
prop_fireescape_01b,
prop_fireescape_02a,
prop_fireescape_02b,
prop_fire_driser_1a,
prop_fire_driser_1b,
prop_fire_driser_2b,
prop_fire_driser_3b,
prop_fire_driser_4a,
prop_fire_driser_4b,
prop_fire_hosereel,
prop_fishing_rod_01,
prop_fishing_rod_02,
prop_fish_slice_01,
prop_flagpole_1a,
prop_flagpole_2a,
prop_flagpole_3a,
prop_flare_01,
prop_flare_01b,
prop_flash_unit,
prop_flatbed_strap,
prop_flatbed_strap_b,
prop_flatscreen_overlay,
prop_flattrailer_01a,
prop_flattruck_01a,
prop_fleeca_atm,
prop_flight_box_01,
prop_flight_box_insert,
prop_flight_box_insert2,
prop_flipchair_01,
prop_floor_duster_01,
prop_fncconstruc_02a,
prop_fnccorgm_05a,
prop_fnccorgm_05b,
prop_fnccorgm_06a,
prop_fnccorgm_06b,
prop_fnclink_01gate1,
prop_fnclink_02gate1,
prop_fnclink_02gate2,
prop_fnclink_02gate5,
prop_fnclink_02gate6_l,
prop_fnclink_02gate6_r,
prop_fnclink_02gate7,
prop_fnclink_03gate1,
prop_fnclink_03gate2,
prop_fnclink_03gate4,
prop_fnclink_03gate5,
prop_fnclink_04gate1,
prop_fnclink_04h_l2,
prop_fnclink_06gate2,
prop_fnclink_06gate3,
prop_fnclink_06gatepost,
prop_fnclink_07gate1,
prop_fnclink_07gate2,
prop_fnclink_07gate3,
prop_fnclink_09gate1,
prop_fnclink_10a,
prop_fnclink_10b,
prop_fnclink_10c,
prop_fnclink_10d,
prop_fnclink_10e,
prop_fnclog_01a,
prop_fnclog_01b,
prop_fncpeir_03a,
prop_fncres_02a,
prop_fncres_02b,
prop_fncres_02c,
prop_fncres_02d,
prop_fncres_02_gate1,
prop_fncres_03gate1,
prop_fncres_05c_l1,
prop_fncsec_01a,
prop_fncsec_01b,
prop_fncsec_01crnr,
prop_fncsec_01gate,
prop_fncsec_01pole,
prop_fncsec_02a,
prop_fncsec_02pole,
prop_fncsec_04a,
prop_fncwood_07gate1,
prop_fncwood_11a_l1,
prop_fncwood_16a,
prop_fncwood_16b,
prop_fncwood_16c,
prop_fncwood_18a,
prop_folded_polo_shirt,
prop_folder_01,
prop_folder_02,
prop_food_bin_01,
prop_food_bin_02,
prop_food_bs_bshelf,
prop_food_bs_cups01,
prop_food_bs_cups03,
prop_food_bs_soda_01,
prop_food_bs_soda_02,
prop_food_bs_tray_01,
prop_food_bs_tray_06,
prop_food_burg1,
prop_food_burg2,
prop_food_cb_bshelf,
prop_food_cb_burg01,
prop_food_cb_cups01,
prop_food_cb_donuts,
prop_food_cb_nugets,
prop_food_cb_soda_01,
prop_food_cb_soda_02,
prop_food_cb_tray_01,
prop_food_cups1,
prop_food_napkin_01,
prop_food_napkin_02,
prop_food_tray_01,
prop_food_van_01,
prop_food_van_02,
prop_forsalejr2,
prop_forsalejr3,
prop_forsalejr4,
prop_foundation_sponge,
prop_fountain1,
prop_fountain2,
prop_franklin_dl,
prop_freeweight_01,
prop_freeweight_02,
prop_fridge_01,
prop_fridge_03,
prop_front_seat_01,
prop_front_seat_02,
prop_front_seat_03,
prop_front_seat_04,
prop_front_seat_05,
prop_front_seat_06,
prop_front_seat_07,
prop_front_seat_row_01,
prop_fruitstand_b_nite,
prop_fruit_basket,
prop_ftowel_01,
prop_ftowel_07,
prop_ftowel_08,
prop_ftowel_10,
prop_f_b_insert_broken,
prop_f_duster_01_s,
prop_f_duster_02,
prop_gaffer_arm_bind,
prop_gaffer_arm_bind_cut,
prop_gaffer_leg_bind,
prop_gaffer_leg_bind_cut,
prop_gaffer_tape,
prop_gaffer_tape_strip,
prop_game_clock_01,
prop_game_clock_02,
prop_garden_dreamcatch_01,
prop_garden_edging_01,
prop_garden_edging_02,
prop_garden_zapper_01,
prop_gardnght_01,
prop_gar_door_01,
prop_gar_door_02,
prop_gar_door_03,
prop_gar_door_03_ld,
prop_gar_door_04,
prop_gar_door_05,
prop_gar_door_05_l,
prop_gar_door_05_r,
prop_gar_door_a_01,
prop_gar_door_plug,
prop_gascage01,
prop_gascyl_ramp_01,
prop_gascyl_ramp_door_01,
prop_gas_01,
prop_gas_02,
prop_gas_03,
prop_gas_04,
prop_gas_05,
prop_gas_grenade,
prop_gas_mask_hang_01,
prop_gatecom_02,
prop_gate_airport_01,
prop_gate_bridge_ld,
prop_gate_cult_01_l,
prop_gate_cult_01_r,
prop_gate_docks_ld,
prop_gate_farm_01a,
prop_gate_farm_post,
prop_gate_frame_01,
prop_gate_frame_02,
prop_gate_frame_04,
prop_gate_frame_05,
prop_gate_frame_06,
prop_gate_military_01,
prop_gate_prison_01,
prop_gate_tep_01_l,
prop_gate_tep_01_r,
prop_gazebo_03,
prop_gd_ch2_08,
prop_generator_02a,
prop_generator_03a,
prop_generator_04,
prop_ghettoblast_02,
prop_girder_01a,
prop_glasscutter_01,
prop_glass_suck_holder,
prop_glf_roller,
prop_glf_spreader,
prop_gold_bar,
prop_gold_cont_01,
prop_gold_cont_01b,
prop_gold_trolly,
prop_gold_trolly_full,
prop_gold_trolly_strap_01,
prop_golf_bag_01,
prop_golf_bag_01b,
prop_golf_bag_01c,
prop_golf_ball,
prop_golf_ball_p2,
prop_golf_ball_p3,
prop_golf_ball_p4,
prop_golf_ball_tee,
prop_golf_driver,
prop_golf_iron_01,
prop_golf_marker_01,
prop_golf_pitcher_01,
prop_golf_putter_01,
prop_golf_tee,
prop_golf_wood_01,
prop_grain_hopper,
prop_grapes_01,
prop_grapes_02,
prop_grass_dry_02,
prop_grass_dry_03,
prop_gravestones_01a,
prop_gravestones_02a,
prop_gravestones_03a,
prop_gravestones_04a,
prop_gravestones_05a,
prop_gravestones_06a,
prop_gravestones_07a,
prop_gravestones_08a,
prop_gravestones_09a,
prop_gravestones_10a,
prop_gravetomb_01a,
prop_gravetomb_02a,
prop_griddle_01,
prop_griddle_02,
prop_grumandoor_l,
prop_grumandoor_r,
prop_gshotsensor_01,
prop_gun_case_01,
prop_gun_case_02,
prop_gun_frame,
prop_hacky_sack_01,
prop_handdry_01,
prop_handdry_02,
prop_handrake,
prop_handtowels,
prop_hand_toilet,
prop_hanger_door_1,
prop_hard_hat_01,
prop_hat_box_01,
prop_hat_box_02,
prop_hat_box_03,
prop_hat_box_04,
prop_hat_box_05,
prop_hat_box_06,
prop_haybailer_01,
prop_haybale_01,
prop_haybale_02,
prop_haybale_stack_01,
prop_hd_seats_01,
prop_headphones_01,
prop_headset_01,
prop_hedge_trimmer_01,
prop_helipad_01,
prop_helipad_02,
prop_henna_disp_01,
prop_henna_disp_02,
prop_henna_disp_03,
prop_hifi_01,
prop_hobo_stove_01,
prop_hockey_bag_01,
prop_hole_plug_01,
prop_holster_01,
prop_homeless_matress_01,
prop_homeless_matress_02,
prop_hose_1,
prop_hose_2,
prop_hose_3,
prop_hose_nozzle,
prop_hospitaldoors_start,
prop_hospital_door_l,
prop_hospital_door_r,
prop_hotel_clock_01,
prop_hotel_trolley,
prop_hottub2,
prop_huf_rag_01,
prop_huge_display_01,
prop_huge_display_02,
prop_hunterhide,
prop_hw1_03_gardoor_01,
prop_hw1_04_door_l1,
prop_hw1_04_door_r1,
prop_hw1_23_door,
prop_hwbowl_pseat_6x1,
prop_hwbowl_seat_01,
prop_hwbowl_seat_02,
prop_hwbowl_seat_03,
prop_hwbowl_seat_03b,
prop_hwbowl_seat_6x6,
prop_hydro_platform_01,
prop_ice_box_01,
prop_ice_box_01_l1,
prop_ice_cube_01,
prop_ice_cube_02,
prop_ice_cube_03,
prop_id2_11_gdoor,
prop_id2_20_clock,
prop_idol_01,
prop_idol_01_error,
prop_idol_case,
prop_idol_case_01,
prop_idol_case_02,
prop_id_21_gardoor_01,
prop_id_21_gardoor_02,
prop_indus_meet_door_l,
prop_indus_meet_door_r,
prop_ind_barge_01,
prop_ind_barge_02,
prop_ind_coalcar_01,
prop_ind_coalcar_02,
prop_ind_coalcar_03,
prop_ind_conveyor_01,
prop_ind_conveyor_02,
prop_ind_conveyor_04,
prop_ind_crusher,
prop_ind_deiseltank,
prop_ind_light_01a,
prop_ind_light_01b,
prop_ind_light_01c,
prop_ind_mech_01c,
prop_ind_mech_02a,
prop_ind_mech_02b,
prop_ind_mech_03a,
prop_ind_mech_04a,
prop_ind_oldcrane,
prop_ind_washer_02,
prop_inflatearch_01,
prop_inflategate_01,
prop_ing_camera_01,
prop_ing_crowbar,
prop_inhaler_01,
prop_int_gate01,
prop_in_tray_01,
prop_irish_sign_01,
prop_irish_sign_02,
prop_irish_sign_03,
prop_irish_sign_04,
prop_irish_sign_05,
prop_irish_sign_06,
prop_irish_sign_07,
prop_irish_sign_08,
prop_irish_sign_09,
prop_irish_sign_10,
prop_irish_sign_11,
prop_irish_sign_12,
prop_irish_sign_13,
prop_iron_01,
prop_jb700_covered,
prop_jeans_01,
prop_jetski_ramp_01,
prop_jet_bloodsplat_01,
prop_jewel_02a,
prop_jewel_02b,
prop_jewel_02c,
prop_jewel_03a,
prop_jewel_03b,
prop_jewel_04a,
prop_jewel_04b,
prop_jewel_pickup_new_01,
prop_juice_dispenser,
prop_juice_pool_01,
prop_jukebox_01,
prop_jukebox_02,
prop_jyard_block_01a,
prop_j_disptray_01,
prop_j_disptray_01b,
prop_j_disptray_01_dam,
prop_j_disptray_02,
prop_j_disptray_02_dam,
prop_j_disptray_03,
prop_j_disptray_03_dam,
prop_j_disptray_04,
prop_j_disptray_04b,
prop_j_disptray_05,
prop_j_disptray_05b,
prop_j_heist_pic_01,
prop_j_heist_pic_02,
prop_j_heist_pic_03,
prop_j_heist_pic_04,
prop_j_neck_disp_01,
prop_j_neck_disp_02,
prop_j_neck_disp_03,
prop_kayak_01,
prop_kayak_01b,
prop_kebab_grill,
prop_keg_01,
prop_kettle,
prop_kettle_01,
prop_keyboard_01a,
prop_keyboard_01b,
prop_kino_light_01,
prop_kino_light_03,
prop_kitch_juicer,
prop_kitch_pot_fry,
prop_kitch_pot_huge,
prop_kitch_pot_lrg,
prop_kitch_pot_lrg2,
prop_kitch_pot_med,
prop_kitch_pot_sm,
prop_knife,
prop_knife_stand,
prop_kt1_06_door_l,
prop_kt1_06_door_r,
prop_kt1_10_mpdoor_l,
prop_kt1_10_mpdoor_r,
prop_ladel,
prop_laptop_02_closed,
prop_laptop_jimmy,
prop_laptop_lester,
prop_laptop_lester2,
prop_large_gold,
prop_large_gold_alt_a,
prop_large_gold_alt_b,
prop_large_gold_alt_c,
prop_large_gold_empty,
prop_lawnmower_01,
prop_ld_alarm_01,
prop_ld_alarm_01_dam,
prop_ld_alarm_alert,
prop_ld_ammo_pack_01,
prop_ld_ammo_pack_02,
prop_ld_ammo_pack_03,
prop_ld_armour,
prop_ld_balcfnc_01a,
prop_ld_balcfnc_02a,
prop_ld_balcfnc_02c,
prop_ld_bankdoors_02,
prop_ld_barrier_01,
prop_ld_binbag_01,
prop_ld_bomb,
prop_ld_bomb_01,
prop_ld_bomb_01_open,
prop_ld_bomb_anim,
prop_ld_cable,
prop_ld_cable_tie_01,
prop_ld_can_01,
prop_ld_case_01,
prop_ld_case_01_lod,
prop_ld_case_01_s,
prop_ld_contact_card,
prop_ld_container,
prop_ld_contain_dl,
prop_ld_contain_dl2,
prop_ld_contain_dr,
prop_ld_contain_dr2,
prop_ld_crate_01,
prop_ld_crate_lid_01,
prop_ld_crocclips01,
prop_ld_crocclips02,
prop_ld_dummy_rope,
prop_ld_fags_01,
prop_ld_fags_02,
prop_ld_fan_01,
prop_ld_fan_01_old,
prop_ld_faucet,
prop_ld_ferris_wheel,
prop_ld_fireaxe,
prop_ld_flow_bottle,
prop_ld_fragwall_01a,
prop_ld_garaged_01,
prop_ld_gold_tooth,
prop_ld_greenscreen_01,
prop_ld_handbag,
prop_ld_handbag_s,
prop_ld_hat_01,
prop_ld_haybail,
prop_ld_hdd_01,
prop_ld_health_pack,
prop_ld_hook,
prop_ld_int_safe_01,
prop_ld_jail_door,
prop_ld_jeans_01,
prop_ld_jeans_02,
prop_ld_jerrycan_01,
prop_ld_keypad_01,
prop_ld_keypad_01b,
prop_ld_keypad_01b_lod,
prop_ld_lap_top,
prop_ld_monitor_01,
prop_ld_peep_slider,
prop_ld_pipe_single_01,
prop_ld_planning_pin_01,
prop_ld_planning_pin_02,
prop_ld_planning_pin_03,
prop_ld_purse_01,
prop_ld_purse_01_lod,
prop_ld_rail_01,
prop_ld_rail_02,
prop_ld_rope_t,
prop_ld_rubble_01,
prop_ld_rubble_02,
prop_ld_rubble_03,
prop_ld_rubble_04,
prop_ld_rub_binbag_01,
prop_ld_scrap,
prop_ld_shirt_01,
prop_ld_shoe_01,
prop_ld_shoe_02,
prop_ld_shovel,
prop_ld_shovel_dirt,
prop_ld_snack_01,
prop_ld_suitcase_01,
prop_ld_suitcase_02,
prop_ld_test_01,
prop_ld_toilet_01,
prop_ld_tooth,
prop_ld_tshirt_01,
prop_ld_tshirt_02,
prop_ld_vault_door,
prop_ld_wallet_01,
prop_ld_wallet_01_s,
prop_ld_wallet_02,
prop_ld_wallet_pickup,
prop_ld_w_me_machette,
prop_leaf_blower_01,
prop_lectern_01,
prop_letterbox_04,
prop_lev_crate_01,
prop_lev_des_barge_01,
prop_lev_des_barge_02,
prop_lifeblurb_01,
prop_lifeblurb_01b,
prop_lifeblurb_02,
prop_lifeblurb_02b,
prop_life_ring_02,
prop_lift_overlay_01,
prop_lift_overlay_02,
prop_litter_picker,
prop_loggneon,
prop_logpile_05,
prop_logpile_06,
prop_logpile_06b,
prop_logpile_07,
prop_logpile_07b,
prop_log_01,
prop_log_02,
prop_log_03,
prop_loose_rag_01,
prop_lrggate_01c_l,
prop_lrggate_01c_r,
prop_lrggate_01_l,
prop_lrggate_01_pst,
prop_lrggate_01_r,
prop_lrggate_02_ld,
prop_lrggate_03a,
prop_lrggate_03b,
prop_lrggate_03b_ld,
prop_lrggate_04a,
prop_lrggate_05a,
prop_lrggate_06a,
prop_luggage_01a,
prop_luggage_02a,
prop_luggage_03a,
prop_luggage_04a,
prop_luggage_05a,
prop_luggage_06a,
prop_luggage_07a,
prop_luggage_08a,
prop_luggage_09a,
prop_magenta_door,
prop_makeup_trail_01,
prop_makeup_trail_02,
prop_map_door_01,
prop_mast_01,
prop_mat_box,
prop_mb_cargo_01a,
prop_mb_cargo_02a,
prop_mb_cargo_03a,
prop_mb_cargo_04a,
prop_mb_cargo_04b,
prop_mb_crate_01a,
prop_mb_crate_01a_set,
prop_mb_crate_01b,
prop_mb_hesco_06,
prop_mb_ordnance_01,
prop_mb_ordnance_03,
prop_mb_sandblock_01,
prop_mb_sandblock_02,
prop_mb_sandblock_03,
prop_mb_sandblock_04,
prop_mb_sandblock_05,
prop_medal_01,
prop_medstation_02,
prop_medstation_03,
prop_medstation_04,
prop_med_bag_01,
prop_med_bag_01b,
prop_med_jet_01,
prop_megaphone_01,
prop_mem_candle_04,
prop_mem_candle_05,
prop_mem_candle_06,
prop_mem_reef_01,
prop_mem_reef_02,
prop_mem_reef_03,
prop_mem_teddy_01,
prop_mem_teddy_02,
prop_metalfoodjar_01,
prop_metal_plates01,
prop_metal_plates02,
prop_meth_bag_01,
prop_michaels_credit_tv,
prop_michael_backpack,
prop_michael_balaclava,
prop_michael_door,
prop_michael_sec_id,
prop_microphone_02,
prop_microwave_1,
prop_micro_01,
prop_micro_02,
prop_micro_cs_01,
prop_micro_cs_01_door,
prop_military_pickup_01,
prop_mil_crate_01,
prop_mil_crate_02,
prop_minigun_01,
prop_mobile_mast_1,
prop_mobile_mast_2,
prop_money_bag_01,
prop_monitor_01c,
prop_monitor_01d,
prop_monitor_02,
prop_monitor_03b,
prop_motel_door_09,
prop_mouse_01,
prop_mouse_01a,
prop_mouse_01b,
prop_mouse_02,
prop_movie_rack,
prop_mp3_dock,
prop_mp_arrow_barrier_01,
prop_mp_barrier_01,
prop_mp_barrier_01b,
prop_mp_barrier_02,
prop_mp_barrier_02b,
prop_mp_base_marker,
prop_mp_boost_01,
prop_mp_cant_place_lrg,
prop_mp_cant_place_med,
prop_mp_cant_place_sm,
prop_mp_cone_01,
prop_mp_cone_02,
prop_mp_cone_03,
prop_mp_cone_04,
prop_mp_drug_package,
prop_mp_drug_pack_blue,
prop_mp_drug_pack_red,
prop_mp_icon_shad_lrg,
prop_mp_icon_shad_med,
prop_mp_icon_shad_sm,
prop_mp_max_out_lrg,
prop_mp_max_out_med,
prop_mp_max_out_sm,
prop_mp_num_0,
prop_mp_num_1,
prop_mp_num_2,
prop_mp_num_3,
prop_mp_num_4,
prop_mp_num_5,
prop_mp_num_6,
prop_mp_num_7,
prop_mp_num_8,
prop_mp_num_9,
prop_mp_placement,
prop_mp_placement_lrg,
prop_mp_placement_maxd,
prop_mp_placement_med,
prop_mp_placement_red,
prop_mp_placement_sm,
prop_mp_ramp_01,
prop_mp_ramp_02,
prop_mp_ramp_03,
prop_mp_repair,
prop_mp_repair_01,
prop_mp_respawn_02,
prop_mp_rocket_01,
prop_mp_spike_01,
prop_mr_rasberryclean,
prop_mr_raspberry_01,
prop_muscle_bench_01,
prop_muscle_bench_02,
prop_muscle_bench_03,
prop_muscle_bench_04,
prop_muscle_bench_05,
prop_muscle_bench_06,
prop_muster_wboard_01,
prop_muster_wboard_02,
prop_m_pack_int_01,
prop_necklace_board,
prop_news_disp_02a_s,
prop_new_drug_pack_01,
prop_nigel_bag_pickup,
prop_night_safe_01,
prop_notepad_01,
prop_notepad_02,
prop_novel_01,
prop_npc_phone,
prop_npc_phone_02,
prop_office_alarm_01,
prop_office_desk_01,
prop_offroad_bale01,
prop_offroad_bale02_l1_frag_,
prop_offroad_barrel01,
prop_offroad_tyres01,
prop_off_chair_01,
prop_off_chair_03,
prop_off_chair_04,
prop_off_chair_04b,
prop_off_chair_04_s,
prop_off_chair_05,
prop_off_phone_01,
prop_oiltub_01,
prop_oiltub_02,
prop_oiltub_03,
prop_oiltub_05,
prop_oiltub_06,
prop_oil_derrick_01,
prop_oil_guage_01,
prop_oil_spool_02,
prop_oil_valve_01,
prop_oil_valve_02,
prop_oil_wellhead_01,
prop_oil_wellhead_03,
prop_oil_wellhead_04,
prop_oil_wellhead_05,
prop_oil_wellhead_06,
prop_oldplough1,
prop_old_boot,
prop_old_churn_01,
prop_old_churn_02,
prop_old_deck_chair,
prop_old_deck_chair_02,
prop_old_farm_01,
prop_old_farm_02,
prop_old_wood_chair,
prop_old_wood_chair_lod,
prop_orang_can_01,
prop_outdoor_fan_01,
prop_out_door_speaker,
prop_overalls_01,
prop_owl_totem_01,
prop_paints_can01,
prop_paints_can02,
prop_paints_can03,
prop_paints_can04,
prop_paints_can05,
prop_paints_can06,
prop_paints_can07,
prop_paint_brush01,
prop_paint_brush02,
prop_paint_brush03,
prop_paint_brush04,
prop_paint_brush05,
prop_paint_roller,
prop_paint_spray01a,
prop_paint_spray01b,
prop_paint_stepl01,
prop_paint_stepl01b,
prop_paint_stepl02,
prop_paint_tray,
prop_paint_wpaper01,
prop_pallettruck_01,
prop_palm_fan_02_a,
prop_palm_fan_02_b,
prop_palm_fan_03_a,
prop_palm_fan_03_b,
prop_palm_fan_03_c,
prop_palm_fan_03_d,
prop_palm_fan_04_a,
prop_palm_fan_04_b,
prop_palm_fan_04_c,
prop_palm_fan_04_d,
prop_palm_huge_01a,
prop_palm_huge_01b,
prop_palm_med_01a,
prop_palm_med_01b,
prop_palm_med_01c,
prop_palm_med_01d,
prop_palm_sm_01a,
prop_palm_sm_01d,
prop_palm_sm_01e,
prop_palm_sm_01f,
prop_paper_bag_01,
prop_paper_bag_small,
prop_paper_ball,
prop_paper_box_01,
prop_paper_box_02,
prop_paper_box_03,
prop_paper_box_04,
prop_paper_box_05,
prop_pap_camera_01,
prop_parachute,
prop_parapack_01,
prop_parasol_01,
prop_parasol_01_b,
prop_parasol_01_c,
prop_parasol_01_down,
prop_parasol_02,
prop_parasol_02_b,
prop_parasol_02_c,
prop_parasol_03,
prop_parasol_03_b,
prop_parasol_03_c,
prop_parasol_04e,
prop_parasol_04e_lod1,
prop_parasol_bh_48,
prop_parking_sign_06,
prop_parking_sign_07,
prop_parking_sign_1,
prop_parking_sign_2,
prop_parking_wand_01,
prop_park_ticket_01,
prop_partsbox_01,
prop_passport_01,
prop_patio_heater_01,
prop_patio_lounger1,
prop_patio_lounger1b,
prop_patio_lounger1_table,
prop_patio_lounger_2,
prop_patio_lounger_3,
prop_patriotneon,
prop_paynspray_door_l,
prop_paynspray_door_r,
prop_pc_01a,
prop_pc_02a,
prop_peanut_bowl_01,
prop_ped_pic_01,
prop_ped_pic_01_sm,
prop_ped_pic_02,
prop_ped_pic_02_sm,
prop_ped_pic_03,
prop_ped_pic_03_sm,
prop_ped_pic_04,
prop_ped_pic_04_sm,
prop_ped_pic_05,
prop_ped_pic_05_sm,
prop_ped_pic_06,
prop_ped_pic_06_sm,
prop_ped_pic_07,
prop_ped_pic_07_sm,
prop_ped_pic_08,
prop_ped_pic_08_sm,
prop_pencil_01,
prop_pharm_sign_01,
prop_phonebox_05a,
prop_phone_ing,
prop_phone_ing_02,
prop_phone_ing_03,
prop_phone_overlay_01,
prop_phone_overlay_02,
prop_phone_overlay_anim,
prop_phone_proto,
prop_phone_proto_back,
prop_phone_proto_battery,
prop_picnictable_02,
prop_piercing_gun,
prop_pier_kiosk_01,
prop_pier_kiosk_02,
prop_pier_kiosk_03,
prop_pile_dirt_01,
prop_pile_dirt_02,
prop_pile_dirt_03,
prop_pile_dirt_04,
prop_pile_dirt_06,
prop_pile_dirt_07,
prop_ping_pong,
prop_pipes_01a,
prop_pipes_01b,
prop_pipes_03b,
prop_pipes_04a,
prop_pipes_05a,
prop_pipes_conc_01,
prop_pipes_conc_02,
prop_pipe_single_01,
prop_pistol_holster,
prop_pitcher_01_cs,
prop_pizza_box_01,
prop_pizza_box_02,
prop_pizza_oven_01,
prop_planer_01,
prop_plant_01a,
prop_plant_01b,
prop_plant_base_01,
prop_plant_base_02,
prop_plant_base_03,
prop_plant_cane_01a,
prop_plant_cane_01b,
prop_plant_cane_02a,
prop_plant_cane_02b,
prop_plant_clover_01,
prop_plant_clover_02,
prop_plant_fern_01a,
prop_plant_fern_01b,
prop_plant_fern_02a,
prop_plant_fern_02b,
prop_plant_fern_02c,
prop_plant_flower_01,
prop_plant_flower_02,
prop_plant_flower_03,
prop_plant_flower_04,
prop_plant_group_01,
prop_plant_group_02,
prop_plant_group_03,
prop_plant_group_04,
prop_plant_group_05,
prop_plant_group_05b,
prop_plant_group_05c,
prop_plant_group_05d,
prop_plant_group_06a,
prop_plant_group_06b,
prop_plant_group_06c,
prop_plant_int_02a,
prop_plant_int_02b,
prop_plant_int_05a,
prop_plant_int_05b,
prop_plant_int_06a,
prop_plant_int_06b,
prop_plant_int_06c,
prop_plant_paradise,
prop_plant_paradise_b,
prop_plastic_cup_02,
prop_plas_barier_01a,
prop_plate_04,
prop_plate_stand_01,
prop_plate_warmer,
prop_player_gasmask,
prop_player_phone_01,
prop_player_phone_02,
prop_pliers_01,
prop_plywoodpile_01a,
prop_plywoodpile_01b,
prop_podium_mic,
prop_police_door_l,
prop_police_door_l_dam,
prop_police_door_r,
prop_police_door_r_dam,
prop_police_door_surround,
prop_police_phone,
prop_police_radio_handset,
prop_police_radio_main,
prop_poly_bag_01,
prop_poly_bag_money,
prop_poolball_1,
prop_poolball_10,
prop_poolball_11,
prop_poolball_12,
prop_poolball_13,
prop_poolball_14,
prop_poolball_15,
prop_poolball_2,
prop_poolball_3,
prop_poolball_4,
prop_poolball_5,
prop_poolball_6,
prop_poolball_7,
prop_poolball_8,
prop_poolball_9,
prop_poolball_cue,
prop_poolskimmer,
prop_pooltable_02,
prop_pooltable_3b,
prop_pool_ball_01,
prop_pool_cue,
prop_pool_rack_01,
prop_pool_rack_02,
prop_pool_tri,
prop_porn_mag_01,
prop_porn_mag_02,
prop_porn_mag_03,
prop_porn_mag_04,
prop_portable_hifi_01,
prop_portacabin01,
prop_portasteps_01,
prop_portasteps_02,
prop_postcard_rack,
prop_poster_tube_01,
prop_poster_tube_02,
prop_postit_drive,
prop_postit_gun,
prop_postit_it,
prop_postit_lock,
prop_potatodigger,
prop_pot_01,
prop_pot_02,
prop_pot_03,
prop_pot_04,
prop_pot_05,
prop_pot_06,
prop_pot_plant_02a,
prop_pot_plant_02b,
prop_pot_plant_02c,
prop_pot_plant_02d,
prop_pot_plant_03a,
prop_pot_plant_04a,
prop_pot_plant_05d_l1,
prop_pot_plant_bh1,
prop_pot_rack,
prop_power_cell,
prop_power_cord_01,
prop_premier_fence_01,
prop_premier_fence_02,
prop_printer_01,
prop_printer_02,
prop_pris_bars_01,
prop_pris_bench_01,
prop_pris_door_01_l,
prop_pris_door_01_r,
prop_pris_door_02,
prop_pris_door_03,
prop_prlg_gravestone_05a_l1,
prop_prlg_gravestone_06a,
prop_projector_overlay,
prop_prologue_phone,
prop_prop_tree_01,
prop_prop_tree_02,
prop_protest_sign_01,
prop_protest_table_01,
prop_prototype_minibomb,
prop_proxy_chateau_table,
prop_punch_bag_l,
prop_pylon_01,
prop_pylon_02,
prop_pylon_03,
prop_pylon_04,
prop_p_jack_03_col,
prop_p_spider_01a,
prop_p_spider_01c,
prop_p_spider_01d,
prop_ql_revolving_door,
prop_quad_grid_line,
prop_radiomast01,
prop_radiomast02,
prop_rad_waste_barrel_01,
prop_ragganeon,
prop_rag_01,
prop_railsleepers01,
prop_railsleepers02,
prop_railstack01,
prop_railstack02,
prop_railstack03,
prop_railstack04,
prop_railstack05,
prop_rail_boxcar,
prop_rail_boxcar2,
prop_rail_boxcar3,
prop_rail_boxcar4,
prop_rail_boxcar5,
prop_rail_boxcar5_d,
prop_rail_buffer_01,
prop_rail_buffer_02,
prop_rail_controller,
prop_rail_crane_01,
prop_rail_points01,
prop_rail_points02,
prop_rail_sigbox01,
prop_rail_sigbox02,
prop_rail_signals02,
prop_rail_tankcar,
prop_rail_tankcar2,
prop_rail_tankcar3,
prop_rail_wellcar,
prop_rail_wellcar2,
prop_range_target_01,
prop_range_target_02,
prop_range_target_03,
prop_rebar_pile01,
prop_recyclebin_02a,
prop_recyclebin_02b,
prop_recyclebin_02_c,
prop_recyclebin_02_d,
prop_recyclebin_03_a,
prop_recyclebin_04_a,
prop_recyclebin_04_b,
prop_recyclebin_05_a,
prop_ret_door,
prop_ret_door_02,
prop_ret_door_03,
prop_ret_door_04,
prop_rf_conc_pillar,
prop_riding_crop_01,
prop_riot_shield,
prop_rio_del_01,
prop_roadcone01a,
prop_roadcone01b,
prop_roadcone01c,
prop_roadcone02a,
prop_roadcone02b,
prop_roadcone02c,
prop_roadheader_01,
prop_rock_1_a,
prop_rock_1_b,
prop_rock_1_c,
prop_rock_1_d,
prop_rock_1_e,
prop_rock_1_f,
prop_rock_1_g,
prop_rock_1_h,
prop_rock_1_i,
prop_rock_2_a,
prop_rock_2_c,
prop_rock_2_d,
prop_rock_2_f,
prop_rock_2_g,
prop_rock_3_a,
prop_rock_3_b,
prop_rock_3_c,
prop_rock_3_d,
prop_rock_3_e,
prop_rock_3_f,
prop_rock_3_g,
prop_rock_3_h,
prop_rock_3_i,
prop_rock_3_j,
prop_rock_4_c,
prop_rock_4_d,
prop_rock_chair_01,
prop_rolled_sock_01,
prop_rolled_sock_02,
prop_rolled_yoga_mat,
prop_roller_car_01,
prop_roller_car_02,
prop_ron_door_01,
prop_roofpipe_01,
prop_roofpipe_02,
prop_roofpipe_03,
prop_roofpipe_04,
prop_roofpipe_05,
prop_roofpipe_06,
prop_roofvent_011a,
prop_roofvent_01a,
prop_roofvent_01b,
prop_roofvent_02a,
prop_roofvent_02b,
prop_roofvent_03a,
prop_roofvent_04a,
prop_roofvent_05a,
prop_roofvent_05b,
prop_roofvent_07a,
prop_roofvent_08a,
prop_roofvent_09a,
prop_roofvent_10a,
prop_roofvent_10b,
prop_roofvent_11b,
prop_roofvent_11c,
prop_roofvent_12a,
prop_roofvent_13a,
prop_roofvent_15a,
prop_roofvent_16a,
prop_rope_family_3,
prop_rope_hook_01,
prop_roundbailer01,
prop_roundbailer02,
prop_rub_bike_01,
prop_rub_bike_02,
prop_rub_bike_03,
prop_rub_binbag_sd_01,
prop_rub_binbag_sd_02,
prop_rub_busdoor_01,
prop_rub_busdoor_02,
prop_rub_buswreck_01,
prop_rub_buswreck_03,
prop_rub_buswreck_06,
prop_rub_cabinet,
prop_rub_cabinet01,
prop_rub_cabinet02,
prop_rub_cabinet03,
prop_rub_cage01a,
prop_rub_carpart_02,
prop_rub_carpart_03,
prop_rub_carpart_04,
prop_rub_chassis_01,
prop_rub_chassis_02,
prop_rub_chassis_03,
prop_rub_cont_01a,
prop_rub_cont_01b,
prop_rub_cont_01c,
prop_rub_flotsam_01,
prop_rub_flotsam_02,
prop_rub_flotsam_03,
prop_rub_frklft,
prop_rub_litter_01,
prop_rub_litter_02,
prop_rub_litter_03,
prop_rub_litter_03b,
prop_rub_litter_03c,
prop_rub_litter_04,
prop_rub_litter_04b,
prop_rub_litter_05,
prop_rub_litter_06,
prop_rub_litter_07,
prop_rub_litter_09,
prop_rub_litter_8,
prop_rub_matress_01,
prop_rub_matress_02,
prop_rub_matress_03,
prop_rub_matress_04,
prop_rub_monitor,
prop_rub_pile_01,
prop_rub_pile_02,
prop_rub_planks_01,
prop_rub_planks_02,
prop_rub_planks_03,
prop_rub_planks_04,
prop_rub_railwreck_1,
prop_rub_railwreck_2,
prop_rub_railwreck_3,
prop_rub_scrap_02,
prop_rub_scrap_03,
prop_rub_scrap_04,
prop_rub_scrap_05,
prop_rub_scrap_06,
prop_rub_scrap_07,
prop_rub_stool,
prop_rub_sunktyre,
prop_rub_t34,
prop_rub_trainers_01,
prop_rub_trolley01a,
prop_rub_trolley02a,
prop_rub_trolley03a,
prop_rub_trukwreck_1,
prop_rub_trukwreck_2,
prop_rub_tyre_01,
prop_rub_tyre_02,
prop_rub_tyre_03,
prop_rub_tyre_dam1,
prop_rub_tyre_dam2,
prop_rub_tyre_dam3,
prop_rub_washer_01,
prop_rub_wheel_01,
prop_rub_wheel_02,
prop_rub_wreckage_3,
prop_rub_wreckage_4,
prop_rub_wreckage_5,
prop_rub_wreckage_6,
prop_rub_wreckage_7,
prop_rub_wreckage_8,
prop_rub_wreckage_9,
prop_rural_windmill_l1,
prop_rural_windmill_l2,
prop_rus_olive,
prop_rus_olive_wint,
prop_sacktruck_01,
prop_sacktruck_02a,
prop_safety_glasses,
prop_sam_01,
prop_sandwich_01,
prop_satdish_2_a,
prop_satdish_2_f,
prop_satdish_2_g,
prop_satdish_3_b,
prop_satdish_3_c,
prop_satdish_3_d,
prop_satdish_l_01,
prop_satdish_s_03,
prop_satdish_s_05a,
prop_satdish_s_05b,
prop_sc1_06_gate_l,
prop_sc1_06_gate_r,
prop_sc1_12_door,
prop_sc1_21_g_door_01,
prop_scaffold_pole,
prop_scafold_01a,
prop_scafold_01c,
prop_scafold_01f,
prop_scafold_02a,
prop_scafold_02c,
prop_scafold_03a,
prop_scafold_03b,
prop_scafold_03c,
prop_scafold_03f,
prop_scafold_04a,
prop_scafold_05a,
prop_scafold_06a,
prop_scafold_06b,
prop_scafold_06c,
prop_scafold_07a,
prop_scafold_08a,
prop_scafold_09a,
prop_scafold_frame1a,
prop_scafold_frame1b,
prop_scafold_frame1c,
prop_scafold_frame1f,
prop_scafold_frame2a,
prop_scafold_frame2b,
prop_scafold_frame2c,
prop_scafold_frame3a,
prop_scafold_frame3c,
prop_scafold_rail_01,
prop_scafold_rail_02,
prop_scafold_rail_03,
prop_scafold_xbrace,
prop_scalpel,
prop_scn_police_torch,
prop_scourer_01,
prop_scrap_2_crate,
prop_scrap_win_01,
prop_scrim_01,
prop_scythemower,
prop_section_garage_01,
prop_securityvan_lightrig,
prop_security_case_01,
prop_security_case_02,
prop_sec_gate_01b,
prop_sec_gate_01c,
prop_sec_gate_01d,
prop_set_generator_01,
prop_sewing_fabric,
prop_sewing_machine,
prop_sglasses_stand_01,
prop_sglasses_stand_02,
prop_sglasses_stand_02b,
prop_sglasses_stand_03,
prop_sglasss_1b_lod,
prop_sglasss_1_lod,
prop_shamal_crash,
prop_shelves_01,
prop_shelves_02,
prop_shelves_03,
prop_shopping_bags01,
prop_shopping_bags02,
prop_shop_front_door_l,
prop_shop_front_door_r,
prop_shots_glass_cs,
prop_shower_01,
prop_shower_rack_01,
prop_shower_towel,
prop_showroom_door_l,
prop_showroom_door_r,
prop_showroom_glass_1b,
prop_shredder_01,
prop_shrub_rake,
prop_shuttering01,
prop_shuttering02,
prop_shuttering03,
prop_shuttering04,
prop_sh_beer_pissh_01,
prop_sh_bong_01,
prop_sh_cigar_01,
prop_sh_joint_01,
prop_sh_mr_rasp_01,
prop_sh_shot_glass,
prop_sh_tall_glass,
prop_sh_tt_fridgedoor,
prop_sh_wine_glass,
prop_side_lights,
prop_side_spreader,
prop_sign_airp_01a,
prop_sign_airp_02a,
prop_sign_airp_02b,
prop_sign_big_01,
prop_sign_mallet,
prop_sign_road_04g_l1,
prop_single_grid_line,
prop_single_rose,
prop_sink_01,
prop_sink_02,
prop_sink_03,
prop_sink_04,
prop_sink_05,
prop_sink_06,
prop_skate_flatramp,
prop_skate_funbox,
prop_skate_halfpipe,
prop_skate_kickers,
prop_skate_quartpipe,
prop_skate_rail,
prop_skate_spiner,
prop_skid_chair_01,
prop_skid_chair_02,
prop_skid_chair_03,
prop_skid_sleepbag_1,
prop_skid_tent_01,
prop_skid_tent_01b,
prop_skid_tent_03,
prop_skip_01a,
prop_skip_02a,
prop_skip_03,
prop_skip_04,
prop_skip_05a,
prop_skip_05b,
prop_skip_06a,
prop_skip_08a,
prop_skip_08b,
prop_skip_10a,
prop_skip_rope_01,
prop_skunk_bush_01,
prop_skylight_01,
prop_skylight_02,
prop_skylight_03,
prop_skylight_04,
prop_skylight_05,
prop_skylight_06a,
prop_skylight_06b,
prop_skylight_06c,
prop_sky_cover_01,
prop_slacks_01,
prop_slacks_02,
prop_sluicegate,
prop_sluicegatel,
prop_sluicegater,
prop_slush_dispenser,
prop_sm1_11_doorl,
prop_sm1_11_doorr,
prop_sm1_11_garaged,
prop_smg_holster_01,
prop_sm_10_mp_door,
prop_sm_14_mp_gar,
prop_sm_19_clock,
prop_sm_27_door,
prop_sm_27_gate,
prop_sm_27_gate_02,
prop_sm_27_gate_03,
prop_sm_27_gate_04,
prop_sm_locker_door,
prop_snow_bailer_01,
prop_snow_barrel_pile_03,
prop_snow_bench_01,
prop_snow_bin_01,
prop_snow_bin_02,
prop_snow_bush_01_a,
prop_snow_bush_02_a,
prop_snow_bush_02_b,
prop_snow_bush_03,
prop_snow_bush_04,
prop_snow_bush_04b,
prop_snow_cam_03,
prop_snow_cam_03a,
prop_snow_diggerbkt_01,
prop_snow_dumpster_01,
prop_snow_elecbox_16,
prop_snow_facgate_01,
prop_snow_field_01,
prop_snow_field_02,
prop_snow_field_03,
prop_snow_field_04,
prop_snow_flower_01,
prop_snow_flower_02,
prop_snow_fnclink_03crnr2,
prop_snow_fnclink_03h,
prop_snow_fnclink_03i,
prop_snow_fncwood_14a,
prop_snow_fncwood_14b,
prop_snow_fncwood_14c,
prop_snow_fncwood_14d,
prop_snow_fncwood_14e,
prop_snow_fnc_01,
prop_snow_gate_farm_03,
prop_snow_grain_01,
prop_snow_grass_01,
prop_snow_light_01,
prop_snow_oldlight_01b,
prop_snow_rail_signals02,
prop_snow_rub_trukwreck_2,
prop_snow_side_spreader_01,
prop_snow_streetlight01,
prop_snow_streetlight_01_frag_,
prop_snow_sub_frame_01a,
prop_snow_sub_frame_04b,
prop_snow_telegraph_01a,
prop_snow_telegraph_02a,
prop_snow_telegraph_03,
prop_snow_traffic_rail_1a,
prop_snow_traffic_rail_1b,
prop_snow_trailer01,
prop_snow_tree_03_e,
prop_snow_tree_03_h,
prop_snow_tree_03_i,
prop_snow_tree_04_d,
prop_snow_tree_04_f,
prop_snow_truktrailer_01a,
prop_snow_tyre_01,
prop_snow_t_ml_01,
prop_snow_t_ml_02,
prop_snow_t_ml_03,
prop_snow_wall_light_15a,
prop_snow_watertower01,
prop_snow_watertower01_l2,
prop_snow_watertower03,
prop_snow_woodpile_04a,
prop_soap_disp_01,
prop_soap_disp_02,
prop_solarpanel_01,
prop_solarpanel_02,
prop_solarpanel_03,
prop_sol_chair,
prop_space_pistol,
prop_space_rifle,
prop_speaker_01,
prop_speaker_02,
prop_speaker_03,
prop_speaker_05,
prop_speaker_06,
prop_speaker_07,
prop_speaker_08,
prop_speedball_01,
prop_sponge_01,
prop_sports_clock_01,
prop_spot_01,
prop_spot_clamp,
prop_spot_clamp_02,
prop_sprayer,
prop_spraygun_01,
prop_spray_backpack_01,
prop_spray_jackframe,
prop_spray_jackleg,
prop_sprink_crop_01,
prop_sprink_golf_01,
prop_sprink_park_01,
prop_spycam,
prop_squeegee,
prop_ss1_05_mp_door,
prop_ss1_08_mp_door_l,
prop_ss1_08_mp_door_r,
prop_ss1_10_door_l,
prop_ss1_10_door_r,
prop_ss1_14_garage_door,
prop_ss1_mpint_door_l,
prop_ss1_mpint_door_r,
prop_ss1_mpint_garage,
prop_ss1_mpint_garage_cl,
prop_stag_do_rope,
prop_start_finish_line_01,
prop_start_grid_01,
prop_staticmixer_01,
prop_stat_pack_01,
prop_steam_basket_01,
prop_steam_basket_02,
prop_steps_big_01,
prop_stickbfly,
prop_stickhbird,
prop_still,
prop_stockade_wheel,
prop_stockade_wheel_flat,
prop_stool_01,
prop_storagetank_01,
prop_storagetank_02,
prop_storagetank_03,
prop_storagetank_03a,
prop_storagetank_03b,
prop_storagetank_04,
prop_storagetank_05,
prop_storagetank_06,
prop_storagetank_07a,
prop_stripmenu,
prop_strip_door_01,
prop_strip_pole_01,
prop_studio_light_02,
prop_studio_light_03,
prop_sub_chunk_01,
prop_sub_cover_01,
prop_sub_crane_hook,
prop_sub_frame_01a,
prop_sub_frame_01b,
prop_sub_frame_01c,
prop_sub_frame_02a,
prop_sub_frame_03a,
prop_sub_frame_04a,
prop_sub_frame_04b,
prop_sub_gantry,
prop_sub_release,
prop_sub_trans_01a,
prop_sub_trans_02a,
prop_sub_trans_03a,
prop_sub_trans_04a,
prop_sub_trans_05b,
prop_sub_trans_06b,
prop_suitcase_01,
prop_suitcase_01b,
prop_suitcase_01c,
prop_suitcase_01d,
prop_suitcase_02,
prop_suitcase_03,
prop_suitcase_03b,
prop_surf_board_ldn_01,
prop_surf_board_ldn_02,
prop_surf_board_ldn_03,
prop_surf_board_ldn_04,
prop_syringe_01,
prop_s_pine_dead_01,
prop_tablesaw_01,
prop_tablesmall_01,
prop_table_02,
prop_table_03b_cs,
prop_table_04,
prop_table_04_chr,
prop_table_05,
prop_table_05_chr,
prop_table_06,
prop_table_06_chr,
prop_table_07,
prop_table_07_l1,
prop_table_08,
prop_table_08_chr,
prop_table_08_side,
prop_table_mic_01,
prop_table_para_comb_04,
prop_table_tennis,
prop_table_ten_bat,
prop_taco_01,
prop_taco_02,
prop_tail_gate_col,
prop_tapeplayer_01,
prop_target_arm,
prop_target_arm_b,
prop_target_arm_long,
prop_target_arm_sm,
prop_target_backboard,
prop_target_backboard_b,
prop_target_blue,
prop_target_blue_arrow,
prop_target_bull,
prop_target_bull_b,
prop_target_comp_metal,
prop_target_comp_wood,
prop_target_frame_01,
prop_target_inner1,
prop_target_inner2,
prop_target_inner2_b,
prop_target_inner3,
prop_target_inner3_b,
prop_target_inner_b,
prop_target_orange_arrow,
prop_target_oran_cross,
prop_target_ora_purp_01,
prop_target_purp_arrow,
prop_target_purp_cross,
prop_target_red,
prop_target_red_arrow,
prop_target_red_blue_01,
prop_target_red_cross,
prop_tarp_strap,
prop_taxi_meter_1,
prop_taxi_meter_2,
prop_tea_trolly,
prop_tea_urn,
prop_telegraph_01a,
prop_telegraph_01b,
prop_telegraph_01c,
prop_telegraph_01d,
prop_telegraph_01e,
prop_telegraph_01f,
prop_telegraph_01g,
prop_telegraph_02a,
prop_telegraph_02b,
prop_telegraph_03,
prop_telegraph_04a,
prop_telegraph_04b,
prop_telegraph_05a,
prop_telegraph_05b,
prop_telegraph_05c,
prop_telegraph_06a,
prop_telegraph_06b,
prop_telegraph_06c,
prop_telegwall_01a,
prop_telegwall_01b,
prop_telegwall_02a,
prop_telegwall_03a,
prop_telegwall_03b,
prop_telegwall_04a,
prop_telescope,
prop_telescope_01,
prop_temp_block_blocker,
prop_tennis_bag_01,
prop_tennis_ball,
prop_tennis_ball_lobber,
prop_tennis_rack_01,
prop_tennis_rack_01b,
prop_test_boulder_01,
prop_test_boulder_02,
prop_test_boulder_03,
prop_test_boulder_04,
prop_test_elevator,
prop_test_elevator_dl,
prop_test_elevator_dr,
prop_tick,
prop_tick_02,
prop_till_01_dam,
prop_till_02,
prop_till_03,
prop_time_capsule_01,
prop_tint_towel,
prop_tint_towels_01,
prop_tint_towels_01b,
prop_toaster_01,
prop_toaster_02,
prop_toiletfoot_static,
prop_toilet_01,
prop_toilet_02,
prop_toilet_03,
prop_toilet_brush_01,
prop_toilet_cube_01,
prop_toilet_cube_02,
prop_toilet_roll_01,
prop_toilet_roll_02,
prop_toilet_roll_03,
prop_toilet_roll_04,
prop_toilet_roll_05,
prop_toilet_shamp_01,
prop_toilet_shamp_02,
prop_toilet_soap_01,
prop_toilet_soap_02,
prop_toilet_soap_03,
prop_toilet_soap_04,
prop_toolchest_01,
prop_toolchest_02,
prop_toolchest_03,
prop_toolchest_04,
prop_toolchest_05,
prop_tool_adjspanner,
prop_tool_bench01,
prop_tool_bluepnt,
prop_tool_box_01,
prop_tool_box_02,
prop_tool_box_03,
prop_tool_box_04,
prop_tool_box_05,
prop_tool_box_06,
prop_tool_box_07,
prop_tool_broom,
prop_tool_broom2,
prop_tool_broom2_l1,
prop_tool_cable01,
prop_tool_cable02,
prop_tool_consaw,
prop_tool_drill,
prop_tool_fireaxe,
prop_tool_hammer,
prop_tool_hardhat,
prop_tool_jackham,
prop_tool_mallet,
prop_tool_mopbucket,
prop_tool_nailgun,
prop_tool_pickaxe,
prop_tool_pliers,
prop_tool_rake,
prop_tool_rake_l1,
prop_tool_sawhorse,
prop_tool_screwdvr01,
prop_tool_screwdvr02,
prop_tool_screwdvr03,
prop_tool_shovel,
prop_tool_shovel006,
prop_tool_shovel2,
prop_tool_shovel3,
prop_tool_shovel4,
prop_tool_shovel5,
prop_tool_sledgeham,
prop_tool_spanner01,
prop_tool_spanner02,
prop_tool_spanner03,
prop_tool_torch,
prop_tool_wrench,
prop_toothbrush_01,
prop_toothb_cup_01,
prop_toothpaste_01,
prop_tornado_wheel,
prop_torture_01,
prop_torture_ch_01,
prop_tourist_map_01,
prop_towel2_01,
prop_towel2_02,
prop_towel_01,
prop_towel_rail_01,
prop_towel_rail_02,
prop_towel_shelf_01,
prop_towel_small_01,
prop_towercrane_01a,
prop_towercrane_02a,
prop_towercrane_02b,
prop_towercrane_02c,
prop_towercrane_02d,
prop_towercrane_02e,
prop_towercrane_02el,
prop_towercrane_02el2,
prop_traffic_rail_1c,
prop_traffic_rail_2,
prop_trailer01,
prop_trailer01_up,
prop_trailer_01_new,
prop_trailer_door_closed,
prop_trailer_door_open,
prop_trailer_monitor_01,
prop_trailr_base,
prop_trailr_base_static,
prop_train_ticket_02,
prop_tram_pole_double01,
prop_tram_pole_double02,
prop_tram_pole_double03,
prop_tram_pole_roadside,
prop_tram_pole_single01,
prop_tram_pole_single02,
prop_tram_pole_wide01,
prop_tree_birch_01,
prop_tree_birch_02,
prop_tree_birch_03,
prop_tree_birch_03b,
prop_tree_birch_04,
prop_tree_birch_05,
prop_tree_cedar_02,
prop_tree_cedar_03,
prop_tree_cedar_04,
prop_tree_cedar_s_01,
prop_tree_cedar_s_02,
prop_tree_cedar_s_04,
prop_tree_cedar_s_05,
prop_tree_cedar_s_06,
prop_tree_cypress_01,
prop_tree_eng_oak_01,
prop_tree_eucalip_01,
prop_tree_fallen_01,
prop_tree_fallen_02,
prop_tree_fallen_pine_01,
prop_tree_jacada_01,
prop_tree_jacada_02,
prop_tree_lficus_02,
prop_tree_lficus_03,
prop_tree_lficus_05,
prop_tree_lficus_06,
prop_tree_log_01,
prop_tree_log_02,
prop_tree_maple_02,
prop_tree_maple_03,
prop_tree_mquite_01,
prop_tree_oak_01,
prop_tree_olive_01,
prop_tree_pine_01,
prop_tree_pine_02,
prop_tree_stump_01,
prop_trevor_rope_01,
prop_trev_sec_id,
prop_trev_tv_01,
prop_triple_grid_line,
prop_tri_finish_banner,
prop_tri_pod,
prop_tri_pod_lod,
prop_tri_start_banner,
prop_tri_table_01,
prop_trough1,
prop_truktrailer_01a,
prop_tshirt_box_02,
prop_tshirt_shelf_1,
prop_tshirt_shelf_2,
prop_tshirt_shelf_2a,
prop_tshirt_shelf_2b,
prop_tshirt_shelf_2c,
prop_tshirt_stand_01,
prop_tshirt_stand_01b,
prop_tshirt_stand_02,
prop_tshirt_stand_04,
prop_tt_screenstatic,
prop_tumbler_01,
prop_tumbler_01b,
prop_tumbler_01_empty,
prop_tunnel_liner01,
prop_tunnel_liner02,
prop_tunnel_liner03,
prop_turkey_leg_01,
prop_turnstyle_01,
prop_tv_02,
prop_tv_03_overlay,
prop_tv_04,
prop_tv_05,
prop_tv_06,
prop_tv_07,
prop_tv_cabinet_03,
prop_tv_cabinet_04,
prop_tv_cabinet_05,
prop_tv_cam_02,
prop_tv_flat_01,
prop_tv_flat_01_screen,
prop_tv_flat_02b,
prop_tv_flat_03,
prop_tv_flat_03b,
prop_tv_flat_michael,
prop_tv_test,
prop_tyre_rack_01,
prop_tyre_spike_01,
prop_t_coffe_table,
prop_t_shirt_ironing,
prop_t_shirt_row_01,
prop_t_shirt_row_02,
prop_t_shirt_row_02b,
prop_t_shirt_row_03,
prop_t_shirt_row_04,
prop_t_shirt_row_05l,
prop_t_shirt_row_05r,
prop_t_sofa,
prop_t_sofa_02,
prop_t_telescope_01b,
prop_umpire_01,
prop_utensil,
prop_valet_03,
prop_vault_shutter,
prop_vb_34_tencrt_lighting,
prop_vcr_01,
prop_veg_corn_01,
prop_veg_crop_01,
prop_veg_crop_02,
prop_veg_crop_04,
prop_veg_crop_04_leaf,
prop_veg_crop_05,
prop_veg_crop_06,
prop_veg_crop_orange,
prop_veg_crop_tr_01,
prop_veg_crop_tr_02,
prop_veg_grass_01_a,
prop_veg_grass_01_b,
prop_veg_grass_01_c,
prop_veg_grass_01_d,
prop_veg_grass_02_a,
prop_vehicle_hook,
prop_vend_coffe_01,
prop_vend_condom_01,
prop_vend_fags_01,
prop_vend_fridge01,
prop_vend_snak_01,
prop_venice_board_01,
prop_venice_board_02,
prop_venice_board_03,
prop_venice_counter_01,
prop_venice_counter_02,
prop_venice_counter_03,
prop_venice_counter_04,
prop_venice_shop_front_01,
prop_venice_sign_09,
prop_venice_sign_10,
prop_venice_sign_11,
prop_venice_sign_12,
prop_venice_sign_14,
prop_venice_sign_15,
prop_venice_sign_16,
prop_venice_sign_17,
prop_venice_sign_18,
prop_ventsystem_01,
prop_ventsystem_02,
prop_ventsystem_03,
prop_ventsystem_04,
prop_ven_market_stool,
prop_ven_market_table1,
prop_ven_shop_1_counter,
prop_vertdrill_01,
prop_voltmeter_01,
prop_v_15_cars_clock,
prop_v_5_bclock,
prop_v_bmike_01,
prop_v_cam_01,
prop_v_door_44,
prop_v_hook_s,
prop_v_m_phone_01,
prop_v_m_phone_o1s,
prop_v_parachute,
prop_waiting_seat_01,
prop_wait_bench_01,
prop_walllight_ld_01b,
prop_wall_light_08a,
prop_wall_light_10a,
prop_wall_light_10b,
prop_wall_light_10c,
prop_wall_light_11,
prop_wall_light_12,
prop_wall_light_17b,
prop_wall_light_18a,
prop_wall_vent_01,
prop_wall_vent_02,
prop_wall_vent_03,
prop_wall_vent_04,
prop_wall_vent_05,
prop_wall_vent_06,
prop_wardrobe_door_01,
prop_warehseshelf01,
prop_warehseshelf02,
prop_warehseshelf03,
prop_warninglight_01,
prop_washer_01,
prop_washer_02,
prop_washer_03,
prop_washing_basket_01,
prop_watercrate_01,
prop_wateringcan,
prop_watertower01,
prop_watertower02,
prop_watertower03,
prop_watertower04,
prop_waterwheela,
prop_waterwheelb,
prop_water_bottle,
prop_water_bottle_dark,
prop_water_corpse_01,
prop_water_corpse_02,
prop_water_ramp_01,
prop_water_ramp_02,
prop_water_ramp_03,
prop_weed_01,
prop_weed_02,
prop_weed_block_01,
prop_weed_bottle,
prop_weed_pallet,
prop_weed_tub_01,
prop_weed_tub_01b,
prop_weight_10k,
prop_weight_15k,
prop_weight_1_5k,
prop_weight_20k,
prop_weight_2_5k,
prop_weight_5k,
prop_weight_rack_01,
prop_weight_rack_02,
prop_welding_mask_01,
prop_weld_torch,
prop_wheat_grass_empty,
prop_wheat_grass_glass,
prop_wheelbarrow01a,
prop_wheelbarrow02a,
prop_wheelchair_01,
prop_wheel_01,
prop_wheel_02,
prop_wheel_03,
prop_wheel_04,
prop_wheel_05,
prop_wheel_06,
prop_wheel_hub_01,
prop_wheel_hub_02_lod_02,
prop_wheel_rim_01,
prop_wheel_rim_02,
prop_wheel_rim_03,
prop_wheel_rim_04,
prop_wheel_rim_05,
prop_wheel_tyre,
prop_whisk,
prop_white_keyboard,
prop_winch_hook_long,
prop_winch_hook_short,
prop_windmill2,
prop_windmill_01_l1,
prop_windmill_01_slod,
prop_windmill_01_slod2,
prop_windowbox_a,
prop_windowbox_b,
prop_windowbox_broken,
prop_windowbox_small,
prop_win_plug_01,
prop_win_plug_01_dam,
prop_win_trailer_ld,
prop_wok,
prop_woodpile_02a,
prop_worklight_01a_l1,
prop_worklight_03a_l1,
prop_worklight_03b_l1,
prop_worklight_04a,
prop_worklight_04b_l1,
prop_worklight_04c_l1,
prop_worklight_04d_l1,
prop_workwall_01,
prop_workwall_02,
prop_wreckedcart,
prop_wrecked_buzzard,
prop_w_board_blank,
prop_w_board_blank_2,
prop_w_fountain_01,
prop_w_r_cedar_01,
prop_w_r_cedar_dead,
prop_yacht_lounger,
prop_yacht_seat_01,
prop_yacht_seat_02,
prop_yacht_seat_03,
prop_yacht_table_01,
prop_yacht_table_02,
prop_yacht_table_03,
prop_yaught_chair_01,
prop_yaught_sofa_01,
prop_yell_plastic_target,
prop_yoga_mat_01,
prop_yoga_mat_02,
prop_yoga_mat_03,
prop_ztype_covered,
p_ing_skiprope_01,
p_ing_skiprope_01_s,
p_skiprope_r_s,
test_prop_gravestones_04a,
test_prop_gravestones_05a,
test_prop_gravestones_07a,
test_prop_gravestones_08a,
test_prop_gravestones_09a,
test_prop_gravetomb_01a,
test_prop_gravetomb_02a,
v_prop_floatcandle,

    }
}
