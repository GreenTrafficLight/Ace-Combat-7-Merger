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
        /// <param name="gameCMN">Unmodified CMN to get the string count</param>
        /// <param name="exportCMN">The finished CMN at the end of the program where the variables will be added</param>
        /// <param name="modCMN">The modded CMN where we get the variable</param>
        /// <param name="exportDats">The finished DATs at the end of the program where the string will be added</param>
        /// <param name="moddedDats">The modded DATs that contains the added strings</param>
        /// <param name="parent"></param>
        public void MergeCMN(CMN gameCMN, CMN exportCMN, CMN modCMN, DAT[] exportDats, DAT[] moddedDats, KeyValuePair<string, CMNString> parent)
        {
            foreach (KeyValuePair<string, CMNString> child in parent.Value.childrens)
            {
                // Check if the variable is a new one by comparing with the unmodified game CMN strings count
                if (gameCMN.StringsCount < child.Value.StringNumber)
                {
                    string variable = modCMN.GetVariable(child);
                    exportCMN.AddVariable(variable);
                    // Loop every mod dat to add the string
                    foreach (DAT moddedDat in moddedDats)
                    {
                        DAT exportDat = exportDats[moddedDat.Letter - 65];
                        string newString = moddedDat.Strings[child.Value.StringNumber];
                        exportDat.Strings.Add(newString);
                    }
                }
                MergeCMN(gameCMN, exportCMN, modCMN, exportDats, moddedDats, child);
            }
        }
    }
}
