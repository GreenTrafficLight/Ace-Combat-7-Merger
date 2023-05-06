using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.ExportTypes;

namespace Ace_Combat_Merger
{
    public class StateDataTable
    {
        public List<StateDataTable> StateDataTableChild = new List<StateDataTable>();
        public List<State> States = new List<State>();

        public StateDataTable()
        {
        }

        public enum State
        {
            UNCHANGED,
            DELETED,
            MODIFIED,
            ADDED
        }
    }
}
