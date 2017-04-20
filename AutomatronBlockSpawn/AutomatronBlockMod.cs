using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using spaar.Mods.Automatron;
using spaar.ModLoader.UI;
using spaar.ModLoader;
using UnityEngine;

namespace Sylver.AutomatronExtention
{
    class AutomatronBlockMod : AutomatronBlock
    {
        // override addActionWindowRect = new Rect(1200, 200, 200, 500);
        public void GuiCall()
        {
            OnGUI();
        }
        protected override void OnGUI()
        {
            addActionWindowRect.height = 500;
            if (!configuring) return;
            if (Game.IsSimulating) return;

            GUI.skin = ModGUI.Skin;
            if (!hidden)
            {
                windowRect = GUI.Window(windowId, windowRect, DoWindow, "Automatron Configuration");
                if (addingAction)
                {
                    addActionWindowRect = GUI.Window(addActionWindowId, addActionWindowRect, DoAddActionWindow, "Add Action");
                }
            }
            if (configuringAction)
            {
                foreach (var action in actions)
                {
                    action.OnGUI();
                }
            }        
        }       
    }
}
      

   



    

