﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KerbTownModules
{    
    public class MouseEventHandler : MonoBehaviour
    {
        public delegate void FSHPgenericDelegate();
        public FSHPgenericDelegate mouseDownEvent;

        public void OnMouseDown()
        {
            mouseDownEvent();
        }
    }
}
