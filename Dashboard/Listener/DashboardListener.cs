using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Dashboard 
{
    class DashboardListener 
    {
        public static void OnRefreshButtonClick()
        {
            foreach (var machine in DisplayController.machines)
            {
                machine.Value.speed.Reset();
                machine.Value.acceleration.Reset();
                machine.Value.palstance.Reset();
            }
        }
    }
}
