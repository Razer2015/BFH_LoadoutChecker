using PRoCon.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BFH_loadoutchecker
{
    public class Language
    {
        public static String[] en_US;
        public static String[] en_US_ID;
        public static String[] en_US_STRING;

        public static void retrieveNames(string file_name = "en_US.txt")
        {
            if (!File.Exists(file_name))
                return;
            en_US = File.ReadAllLines(file_name);
        }

        public static void parseLanguage(String[] en_US)
        {
            // Parsing
            en_US_ID = new String[en_US.Length];
            en_US_STRING = new String[en_US.Length];
            for (int i = 0; i < en_US.Length; i++)
            {
                string s = en_US[i];
                int index = s.IndexOf("=");

                // ID
                string lang_id = s.Substring(0, index);
                Match match = Regex.Match(lang_id, @"'([^']*)");
                if (match.Success)
                {
                    string id = match.Groups[1].Value;
                    en_US_ID[i] = id;
                }
                // STRING
                string lang_string = s.Substring(index + 1, (s.Length) - (index + 1));
                Match _match = Regex.Match(lang_string, "\"([^\"]*)");
                if (_match.Success)
                {
                    string _string = _match.Groups[1].Value;
                    en_US_STRING[i] = _string;
                }
            }
        }

        public static String getString(String ID)
        {
            if (en_US_ID.Contains(ID))
            {
                int index = Array.IndexOf(en_US_ID, ID);
                return (en_US_STRING[index]);
            }
            return (String.Empty);
        }
    }

    public class Codes
    {
        public static Hashtable compact = null;
        public static Hashtable compact_weapons = null;
        public static Hashtable compact_weaponaccessory = null;
        public static Hashtable compact_appearances = null;
        public static Hashtable compact_vehicles = null;
        public static Hashtable compact_kititems = null;
        public static Hashtable compact_vehicleunlocks = null;

        public static Hashtable compact_items_weapons = null;

        public static Hashtable parseCodes()
        {
            Hashtable json = (Hashtable)JSON.JsonDecode(File.ReadAllText("bfh.loadout.json"));
            compact = (Hashtable)json["compact"];
            compact_weapons = (Hashtable)compact["weapons"];
            compact_weaponaccessory = (Hashtable)compact["weaponaccessory"];
            compact_appearances = (Hashtable)compact["appearances"];
            compact_vehicles = (Hashtable)compact["vehicles"];
            compact_kititems = (Hashtable)compact["kititems"];
            compact_vehicleunlocks = (Hashtable)compact["vehicleunlocks"];

            Hashtable json_items = (Hashtable)JSON.JsonDecode(File.ReadAllText("bfh.items.json"));
            compact_items_weapons = (Hashtable)((Hashtable)json["compact"])["weapons"];

            //printAll();

            return (json);
        }

        public static void printAll(string file_name = "compact_", string file_extension = ".csv")
        {
            // Retrieve en_US
            if (Language.en_US == null)
                Language.retrieveNames();
            if (Language.en_US_ID == null || Language.en_US_STRING == null)
                Language.parseLanguage(Language.en_US);

            // Weapons
            using (StreamWriter sw = new StreamWriter(file_name + "weapons" + file_extension))
            {
                sw.WriteLine("key;category;rareness;name;categoryType;slug;desc;category_rn;name_rn;desc_rn");
                foreach (DictionaryEntry entry in compact_weapons)
                {
                    if (IsDigitsOnly(entry.Key.ToString()))
                    {
                        Hashtable temp = (Hashtable)compact_weapons[entry.Key];
                        string key = (string)entry.Key;
                        string category = (string)temp["category"];
                        string rareness = (string)temp["rareness"];
                        string name = (string)temp["name"];
                        string categoryType = (string)temp["categoryType"];
                        string slug = (string)temp["slug"];
                        string desc = (string)temp["desc"];
                        string category_rn = "";
                        string name_rn = "";
                        string desc_rn = "";
                        if (Language.en_US_ID.Contains(category)) { int index = Array.IndexOf(Language.en_US_ID, category); category_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(name)) { int index = Array.IndexOf(Language.en_US_ID, name); name_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(desc)) { int index = Array.IndexOf(Language.en_US_ID, desc); desc_rn = (Language.en_US_STRING[index]); }
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", key, category, rareness, name, categoryType, slug, desc, category_rn, name_rn, desc_rn);
                    }
                }
            }
            // Weaponaccessory
            using (StreamWriter sw = new StreamWriter(file_name + "weaponaccessory" + file_extension))
            {
                sw.WriteLine("key;category;rareness;name;slug;desc;category_rn;name_rn;desc_rn");
                foreach (DictionaryEntry entry in compact_weaponaccessory)
                {
                    if (IsDigitsOnly(entry.Key.ToString()))
                    {
                        Hashtable temp = (Hashtable)compact_weaponaccessory[entry.Key];
                        string key = (string)entry.Key;
                        string category = (string)temp["category"];
                        string rareness = (string)temp["rareness"];
                        string name = (string)temp["name"];
                        string slug = (string)temp["slug"];
                        string desc = (string)temp["desc"];
                        string category_rn = "";
                        string name_rn = "";
                        string desc_rn = "";
                        if (Language.en_US_ID.Contains(category)) { int index = Array.IndexOf(Language.en_US_ID, category); category_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(name)) { int index = Array.IndexOf(Language.en_US_ID, name); name_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(desc)) { int index = Array.IndexOf(Language.en_US_ID, desc); desc_rn = (Language.en_US_STRING[index]); }
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8}", key, category, rareness, name, slug, desc, category_rn, name_rn, desc_rn);
                    }
                }
            }
            // Appearances
            using (StreamWriter sw = new StreamWriter(file_name + "appearances" + file_extension))
            {
                sw.WriteLine("key;category;rareness;name;slug;desc;category_rn;name_rn;desc_rn");
                foreach (DictionaryEntry entry in compact_appearances)
                {
                    if (IsDigitsOnly(entry.Key.ToString()))
                    {
                        Hashtable temp = (Hashtable)compact_appearances[entry.Key];
                        string key = (string)entry.Key;
                        string category = (string)temp["category"];
                        string rareness = (string)temp["rareness"];
                        string name = (string)temp["name"];
                        string slug = (string)temp["slug"];
                        string desc = (string)temp["desc"];
                        string category_rn = "";
                        string name_rn = "";
                        string desc_rn = "";
                        if (Language.en_US_ID.Contains(category)) { int index = Array.IndexOf(Language.en_US_ID, category); category_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(name)) { int index = Array.IndexOf(Language.en_US_ID, name); name_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(desc)) { int index = Array.IndexOf(Language.en_US_ID, desc); desc_rn = (Language.en_US_STRING[index]); }
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8}", key, category, rareness, name, slug, desc, category_rn, name_rn, desc_rn);
                    }
                }
            }
            // Vehicles
            using (StreamWriter sw = new StreamWriter(file_name + "vehicles" + file_extension))
            {
                sw.WriteLine("key;category;rareness;name;categoryType;slug;desc;category_rn;name_rn;desc_rn");
                foreach (DictionaryEntry entry in compact_vehicles)
                {
                    if ((entry.Key.ToString()).StartsWith("WARSAW"))
                    {
                        Hashtable temp = (Hashtable)compact_vehicles[entry.Key];
                        string category = (string)entry.Key;
                        string rareness = (string)temp["rareness"];
                        string name = (string)temp["name"];
                        string categoryType = (string)temp["categoryType"];
                        string slug = (string)temp["slug"];
                        string desc = (string)temp["desc"];
                        string category_rn = "";
                        string name_rn = "";
                        string desc_rn = "";
                        if (Language.en_US_ID.Contains(category)) { int index = Array.IndexOf(Language.en_US_ID, category); category_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(name)) { int index = Array.IndexOf(Language.en_US_ID, name); name_rn = (Language.en_US_STRING[index]); }
                        if (Language.en_US_ID.Contains(desc)) { int index = Array.IndexOf(Language.en_US_ID, desc); desc_rn = (Language.en_US_STRING[index]); }
                        sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8}", category, rareness, name, categoryType, slug, desc, category_rn, name_rn, desc_rn);
                    }
                }
            }
        }

        static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
