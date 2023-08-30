using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ace7Localization.Formats;
using static Ace7Localization.Formats.CMN;

namespace Ace_Combat_Merger
{
    public class LocalizationMerger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameCMN"></param>
        /// <param name="exportCMN"></param>
        /// <param name="modCMN"></param>
        /// <param name="parent"></param>
        public void MergeCMN(CMN gameCMN, CMN exportCMN, CMN modCMN, KeyValuePair<string, CMNString> parent)
        {
            foreach (KeyValuePair<string, CMNString> child in parent.Value.childrens)
            {
                if (gameCMN.StringsCount < child.Value.StringNumber)
                {
                    string variable = modCMN.GetVariable(child);
                    exportCMN.AddVariable(variable, child.Value.StringNumber);
                }
                MergeCMN(gameCMN, exportCMN, modCMN, child);
            }
        }
    }
}
