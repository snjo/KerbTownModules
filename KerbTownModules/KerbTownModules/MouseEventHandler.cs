using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KerbTownModules
{
    public delegate void FSHPgenericDelegate();

    public class MouseEventHandler : MonoBehaviour
    {
        public FSHPgenericDelegate mouseDownEvent;

        public void OnMouseDown()
        {
            mouseDownEvent();
        }
    }
}
