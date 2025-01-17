﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    // 1
    private GameObject collidingObject;
    // 2
    private GameObject objectInHand;

    //public GameObject rightJoystick;
    //private Collider rightJoystickCollider;
    //private Bounds rightJoystickBounds;
    //private ConstrainMovementPlane rightJoystickScript;

    //private Collider thisCollider;
    //private Bounds thisBounds;

	public bool isGrabbing = false;

    //void Start()
    //{
    //    rightJoystickCollider = rightJoystick.GetComponent<Collider>();
    //    rightJoystickBounds = rightJoystickCollider.bounds;
    //    rightJoystickScript = rightJoystick.GetComponent<ConstrainMovementPlane>();

    //    thisCollider = this.GetComponent<Collider>();
    //    thisBounds = thisCollider.bounds;
    //}

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        objectInHand.GetComponent<ConstrainMovementPlane>().isGrabbed = true;

        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<ConstrainMovementPlane>().isGrabbed = false;
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // 4
        objectInHand = null;

    }

    // Update is called once per frame
    void Update()
    {
        // 1
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
				isGrabbing = true;
				//Debug.Log ("Object Grabbed");
            }
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
				ReleaseObject ();
				isGrabbing = false;
				//Debug.Log ("Object Released");
            }
        }
    }
}
