using System;
using System.Collections.Generic;
using System.Linq;
using ModShardLauncher.Mods;
using Serilog;

namespace ModShardLauncher
{
    public static partial class Msl
    {
        public static Weapon GetWeapon(string id)
        {
            try
            {
                string weaponsName = ModLoader.Weapons.First(t => t.StartsWith(id));

                // for a lazy evaluation to avoid going through all the WeaponDescriptions list
                IEnumerator<string> weaponDescriptionEnumerator = ModLoader.WeaponDescriptions.Where(t => t.StartsWith(id)).GetEnumerator();

                // getting the first element - the localization name
                weaponDescriptionEnumerator.MoveNext();
                List<string> localizationNames = weaponDescriptionEnumerator.Current.Split(";").ToList();
                localizationNames.Remove("");
                localizationNames.RemoveAt(0);

                // getting the second element - the description
                weaponDescriptionEnumerator.MoveNext();
                List<string> weaponDescription = weaponDescriptionEnumerator.Current.Split(";").ToList();
                weaponDescription.Remove("");
                weaponDescription.RemoveAt(0);

                Log.Information(string.Format("Found weapon: {0}", weaponsName.ToString()));
                return new(weaponsName, weaponDescription, localizationNames);
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
        public static void SetWeapon(string id, Weapon weapon)
        {
            try
            {
                string targetName = ModLoader.Weapons.First(t => t.StartsWith(id));
                int indexTargetName = ModLoader.Weapons.IndexOf(targetName);

                // for a lazy evaluation to avoid going through all the WeaponDescriptions list
                IEnumerator<(int, string)> weaponDescriptionEnumerator = ModLoader.WeaponDescriptions.Where(t => t.StartsWith(id)).Enumerate().GetEnumerator();

                // getting the first element - the localization name
                weaponDescriptionEnumerator.MoveNext();
                (int indexLocalizationName, _) = weaponDescriptionEnumerator.Current;

                // getting the first element - the description
                weaponDescriptionEnumerator.MoveNext();
                (int indexDescription, _) = weaponDescriptionEnumerator.Current;

                (string, string, string) w2s = Weapon.Weapon2String(weapon);
                ModLoader.Weapons[indexTargetName] = w2s.Item1;
                ModLoader.WeaponDescriptions[indexDescription] = w2s.Item2;
                ModLoader.WeaponDescriptions[indexLocalizationName] = w2s.Item3;

                Log.Information(string.Format("Successfully set weapon: {0}", targetName.ToString()));
            }
            catch(Exception ex) 
            {
                Log.Error(ex, "Something went wrong");
                throw;
            }
        }
    }
}