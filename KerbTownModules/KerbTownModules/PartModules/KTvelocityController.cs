using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class KTvelocityController : PartModule
{
    [KSPField]
    public string thrustTransformName = "thruster";
    [KSPField]
    public float maxThrust = 2f;
    [KSPField]
    public float resourceConsumption = 1f;
    [KSPField]
    public float lowerSpeedThreshold = 0.1f;
    [KSPField]
    public float upperSpeedThreshold = 2.0f;

    public bool controllerActive = false;

    Transform[] transformArray;
    private bool transformsFound = false;
    private Vector3 velocityDirection = new Vector3(0f, 0f, 0f);

    public override void OnStart(PartModule.StartState state)
    {
        base.OnStart(state);
        transformArray = part.FindModelTransforms(thrustTransformName);
        if (transformArray.Length > 0)
        {

            transformsFound = true;
        }
        else
        {
            Debug.Log("KTvelocityController: Transforms not found named " + thrustTransformName + ", disabling the module");
            this.enabled = false;
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (transformsFound)
        {
            //Debug.Log("Entering fixed update");
            try
            {
                velocityDirection = part.gameObject.rigidbody.velocity;
            }
            catch
            {
                //Debug.Log("failed to find rigidbody velocity");
                return;
            }
            int i = 0;
            //Debug.Log("Entering loop");
            foreach (Transform t in transformArray)
            {
                i++;
                //Debug.Log("loop: " + i);
                float thrustModifier = Vector3.Dot(t.transform.forward, velocityDirection.normalized);
                if (thrustModifier < 0f)
                {
                    //Debug.Log("thrust mod:  " + thrustModifier);
                    if (Input.GetKey(KeyCode.Space))
                    {
                        try
                        {
                            part.gameObject.rigidbody.AddForceAtPosition(t.transform.forward * -thrustModifier * maxThrust, t.transform.position);
                        }
                        catch
                        {
                            Debug.Log("Error adding force");
                        }
                        //Debug.Log("force: " + thrustModifier + " at " + i);
                    }
                }
            }
        }
    }
}

