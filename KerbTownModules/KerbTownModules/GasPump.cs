using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KerbTownQuest;
using KerbTownQuest.GUI;

namespace KerbTownModules
{
    public class GasPump : MonoBehaviour
    {        
        public Vessel targetVessel;
        public bool vesselInRange;
        public Part targetTank;
        public PartResource targetResource;        
        public bool fuelTransferInitiated;
        public bool fuelDrainInitiated;
        public Transform triggerCollider;
        public MouseEventHandler mouseEventHandler;
        public bool showMenu;
        public List<Tank> tankList;
        public int tankToFill;

        public Rect windowRect;
        public GUIPopup popup;
        public PopupElement elementHeading;

        // supported: string, sbyte, short, int, long, byte, ushort, uint, ulong, float, double, decimal, char, bool, Vector2[d], Vector3[d], Vector4[d], Quaternion[D], Color[32], Enums
        [KSPField]
        public float range = 20f;
        [KSPField]
        public float transferRate = 0.05f;
        [KSPField]
        public string triggerColliderName = "pump0";
        [KSPField]
        public string fuel = "LiquidFuel";
        [KSPField]
        public string fuelGUIName = "Liquid Fuel";
        [KSPField]
        public Vector4 defaultWindowRect = new Vector4(300f, 300f, 400f, 100f);
        [KSPField]
        public Vector2 elementPadding = new Vector2(4f, 4f);
        [KSPField]
        public Vector2 fuelTextSize = new Vector2(150f, 30f);
        [KSPField]
        public int moduleID = 0;

        public void createTankList()
        {
            tankList = new List<Tank>();            
            if (targetVessel != null)
            {                    
                foreach (Part p in targetVessel.parts)
                {
                    PartResourceList resourceList = p.Resources;
                    foreach (PartResource resource in resourceList)
                    {
                        if (resource.resourceName == fuel)
                        {
                            tankList.Add(new Tank(p, resource, fuelGUIName));
                        }
                    }                        
                }
                Debug.Log("FSHPfuelTank: added " + tankList.Count + " tanks to the list");
            }
        }

        public void createGUIWindow()
        {
            elementHeading = new PopupElement("Choose tank to fill");
            popup = new GUIPopup(moduleID, GUIwindowID.gasPump + moduleID, windowRect, fuelGUIName + " Fuel Pump", elementHeading);
            popup.hideMenuEvent = OnPopupHide;
        }

        public void createGUIWindowElements()
        {
            float buttonWidth = (windowRect.width - fuelTextSize.x - popup.subElementSpacing * 5f) / 3f;
            int i = 0;
            foreach (Tank tank in tankList)
            {
                PopupElement element = new PopupElement(tank.fuelGUIName, new PopupButton("Fill", buttonWidth, initiateFillTank, i));
                element.titleSize = fuelTextSize.x;
                popup.elementList.Add(element);
                element.buttons.Add(new PopupButton("Stop", buttonWidth, stopTransfer));
                element.buttons.Add(new PopupButton("Drain", buttonWidth, initiateDrainTank, i));
                i++;
            }
        }

        public void updateGUIFuelElements()
        {
            for (int i = 0; i < popup.elementList.Count-1; i++) // -1 on the count because of the elementHeading Line
            {
                if (tankList.Count > i)
                {
                    popup.elementList[i + 1].titleText = tankList[i].part.name + " (" + (int)tankList[i].partResource.amount + " / " + (int)tankList[i].partResource.maxAmount + ")";
                }
            }
        }

        public void initiateFillTank(int ID)
        {
            Debug.Log("FSHPfuelTank: starting fill");
            stopTransfer();
            fuelTransferInitiated = true;
            tankToFill = ID;
        }

        public void fillTank(int ID)
        {            
            if (tankList.Count > ID)
            {
                if (tankList[ID].partResource != null)
                {
                    tankList[ID].partResource.amount += transferRate;
                    if (tankList[ID].partResource.amount > tankList[ID].partResource.maxAmount)
                    {
                        tankList[ID].partResource.amount = tankList[ID].partResource.maxAmount;
                        Debug.Log("FSHPgasPump: tank full, stopping transfer");
                        stopTransfer();
                    }
                }
            }
        }

        public void initiateDrainTank(int ID)
        {
            Debug.Log("FSHPfuelTank: starting drain");
            stopTransfer();
            fuelDrainInitiated = true;
            tankToFill = ID;
        }

        public void drainTank(int ID)
        {
            if (tankList.Count > ID)
            {
                if (tankList[ID].partResource != null)
                {
                    tankList[ID].partResource.amount -= transferRate;
                    if (tankList[ID].partResource.amount < 0f)
                    {
                        tankList[ID].partResource.amount = 0f;
                        Debug.Log("FSHPgasPump: tank empty, stopping transfer");
                        stopTransfer();
                    }
                }
            }
        }

        public void stopTransfer()
        {
            fuelTransferInitiated = false;
            fuelDrainInitiated = false;
            Debug.Log("FSHPgasPump: fuel transfer stopped");
        }

        public bool checkVesselInRange()
        {
            targetVessel = FlightGlobals.ActiveVessel;
            if (targetVessel == null)
                return false;
            if (Vector3.Distance(this.transform.position, targetVessel.transform.position) < range)
                return true;
            else
                return false;
        }

        public void OnPopupHide()
        {
            showMenu = false;
            Debug.Log("FSHPgasPump: popup close button pressed");
            stopTransfer();
        }

        public void mouseDown()
        {
            Debug.Log("Mouse Click!");
            vesselInRange = checkVesselInRange();
            if(vesselInRange)
            {
                createTankList();                
                createGUIWindow();
                createGUIWindowElements();
                popup.showMenu = true;
                showMenu = true;               
            }
        }

        public void Start()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            Debug.Log("GasPump: Start()");
            triggerCollider = transform.Find(triggerColliderName);
            if (triggerCollider != null)
            {
                mouseEventHandler = triggerCollider.gameObject.AddComponent<MouseEventHandler>();
                mouseEventHandler.mouseDownEvent = mouseDown;
            }
            else
            {
                Debug.Log("FSHPgasPump: triggerCollider not found: " + triggerColliderName);
            }
            windowRect = new Rect(defaultWindowRect.x, defaultWindowRect.y, defaultWindowRect.z, defaultWindowRect.w);
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {                                
                if (vesselInRange)
                {
                    vesselInRange = checkVesselInRange();
                    if (vesselInRange)
                    {
                        if (fuelTransferInitiated)
                        {
                            fillTank(tankToFill);
                        }
                        else if (fuelDrainInitiated)
                        {
                            drainTank(tankToFill);
                        }
                        //else
                        //    Debug.Log("FSHPgasPump: neither filling nor draining");
                        if (showMenu && popup != null)
                        {
                            updateGUIFuelElements();
                        }
                    }
                    else
                    {
                        stopTransfer();
                        showMenu = false;
                        if (popup != null)
                            popup.showMenu = false;
                    }
                }                
            }
        }

        public void OnGUI()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (showMenu)
                {           
                    if (popup != null)
                        popup.popup();
                }
            }
        }
    }
}
