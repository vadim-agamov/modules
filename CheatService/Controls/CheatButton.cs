using System;
using UnityEngine;

namespace Modules.CheatService.Controls
{
    public class CheatButton
    {
        private readonly string _name;
        private readonly Action _action;
        private readonly bool _hideCheats;
        
        public CheatButton(string name, Action action, bool hideCheats = true)
        {
            _name = name;
            _action = action;
            _hideCheats = hideCheats;
        }
        
        public void OnGUI()
        {
            if (GUILayout.Button(_name))
            {
                _action?.Invoke();
                // if(_hideCheats)
                // {
                //     HideCheats();
                // }
            }
        }
    }
}