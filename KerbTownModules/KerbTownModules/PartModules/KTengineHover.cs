using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KTengineHover : PartModule
{
    [KSPField]
    public float verticalSpeedIncrements = 1f;
    [KSPField(guiActive = true, guiName = "Vertical Speed")]
    public float verticalSpeed = 0f;
    [KSPField(guiActive = true, guiName = "Hover Active")]
    public bool hoverActive = false;
    [KSPField]
    public float thrustSmooth = 0.1f;


    private ModuleEngines engine;
    private float maxThrust = 0f;
    private float currentThrustNormalized = 0f;
    private float targetThrustNormalized = 0f;

    [KSPEvent(guiName = "Toggle Hover")]
    public void toggleHoverEvent()
    {
        hoverActive = !hoverActive;
        verticalSpeed = 0f;
    }

    [KSPAction("Toggle Hover")]
    public void toggleHoverAction(KSPActionParam param)
    {
        toggleHoverEvent();
    }

    [KSPAction("Increase vSpeed")]
    public void increaseVerticalSpeed(KSPActionParam param)
    {
        verticalSpeed += verticalSpeedIncrements;
    }

    [KSPAction("Decrease vSpeed")]
    public void decreaseVerticalSpeed(KSPActionParam param)
    {
        verticalSpeed -= verticalSpeedIncrements;
    }

    public override void OnStart(PartModule.StartState state)
    {
        Debug.Log("KTengineHover OnStart");
        base.OnStart(state);
        if (HighLogic.LoadedSceneIsFlight)
        {
            engine = part.FindModelComponent<ModuleEngines>();
            if (engine != null)
            {
                maxThrust = engine.maxThrust;
            }
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();            
        if (HighLogic.LoadedSceneIsFlight && vessel == FlightGlobals.ActiveVessel)
        {
            if (hoverActive)
            {
                if (vessel.verticalSpeed > 0f)
                    targetThrustNormalized = 0f;
                else if (vessel.verticalSpeed < 0f)
                    targetThrustNormalized = 1f;

                currentThrustNormalized = Mathf.Lerp(currentThrustNormalized, targetThrustNormalized, thrustSmooth);

                engine.maxThrust = maxThrust * currentThrustNormalized;
            }
        }
    }
}
