using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFH_loadoutchecker
{
    public enum kits
    {
        OPERATOR,
        MECHANIC,
        ENFORCER,
        PROFESSIONAL,
        VEHICLES
    }

    public class Loadout
    {
        public String personaID = "";
        public int activeKit = 0;
        public kits active_Kit = kits.OPERATOR;
        public Hashtable getActiveKit(string playerName)
        {
            BattlelogClient bclient = new BattlelogClient();
            Hashtable loadout = null;
            for (int i = 0; i < 5; i++)
            {
                if (i == 5)
                    return null;

                loadout = bclient.getStats(playerName, i);
                activeKit = i;
                active_Kit = (kits)i;
                if ((bool)loadout["isActive"])
                    break;
            }
            personaID = bclient.personaID;
            return (loadout);
        }

        public Hashtable getWantedKit(string playerName, int index)
        {
            BattlelogClient bclient = new BattlelogClient();
            Hashtable loadout = bclient.getStats(playerName, index);
            personaID = bclient.personaID;
            activeKit = index;
            active_Kit = (kits)index;
            return (loadout);
        }
    }
}
