using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace_Combat_Merger
{
    public class DictionaryAC7DataTable
    {
        public List<string> RowNames = new List<string>();

        public Dictionary<string, List<string>> IDs = new Dictionary<string, List<string>>();

        public Dictionary<string, List<int>> PlaneSkinNoDictionary = new Dictionary<string, List<int>>();
    }
}
