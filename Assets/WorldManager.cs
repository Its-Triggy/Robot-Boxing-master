using System;
using System.Collections;
using System.Collections.Generic;
using Pololu.Usc;
using Pololu.UsbWrapper;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public static string SerialNumberText;
    private static List<DeviceListItem> device_list;
    public static Usc usc = null;
    public GameObject LeapMotionParent;
    private GameObject LeftHand;
    private GameObject RightHand;
    public bool DisconnectUSC = false;
    public bool ReconnectUSC = false;

    private Usc StartUsc()
    {
        List<DeviceListItem> itemList = Usc.getConnectedDevices();
        foreach (DeviceListItem d in Usc.getConnectedDevices())
        {
            usc = new Usc(d);
            return usc;
        }

        Debug.LogError("Error in StartUsc: there was nothing from getConnectedDevices()");
        return null;
    }

    // Use this for initialization
    void Start () {

    }

    public Usc GetUsc()
    {
        try
        {
            if (usc == null)
            {
                usc = StartUsc();
                UpdateUSCSettings();
                return usc;
            }
            else
            {
                return usc;
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("Error starting USC: " + ex.ToString());
            return usc;
        }
    }

    /// <summary>
    /// Tell the servos that they can handle a higher range of microsecond values
    /// </summary>
    private void UpdateUSCSettings()
    {
        try
        {
            UscSettings oldSettings = usc.getUscSettings();
            foreach(ChannelSetting sc in oldSettings.channelSettings)
            {
                sc.home = (ushort)((AngleUtils.maxMicrosecond + AngleUtils.minMicrosecond) / 2);
                sc.neutral = (ushort)((AngleUtils.maxMicrosecond + AngleUtils.minMicrosecond) / 2);
                sc.range = (ushort)6350; //(ushort)(AngleUtils.maxMicrosecond - AngleUtils.minMicrosecond);
                sc.minimum = AngleUtils.minMicrosecond;
                sc.maximum = AngleUtils.maxMicrosecond;
            }
            usc.setUscSettings(oldSettings, false);
        }
        catch(Exception ex)
        {
            Debug.LogError("Unable to update the USC settings: " + ex.ToString());
        }
    }

    private void FixedUpdate()
    {
        if (DisconnectUSC == true)
        {
            try
            {
                usc.disconnect();
                DisconnectUSC = false;
            }
            catch(Exception ex)
            {
                Debug.Log("Error reported when disconnecting usc: " + ex.ToString());
            }
        }
        if (ReconnectUSC == true)
        {
            StartUsc();
            ReconnectUSC = false;
        }
    }

    #region LeapMotionGetters
    public GameObject GetLeapLeftHandPalm()
    {
        if (LeapMotionParent == null)
        {
            return null;
        }
        else
        {
            if (LeftHand == null)
            {
                GameObject handParent = LeapMotionParent.transform.Find("HandModels").gameObject;
                LeftHand = handParent.transform.Find("RigidRoundHand_L").Find("palm").gameObject;
            }
            return LeftHand;
        }
    }

    public GameObject GetLeapLeftHand()
    {
        if (LeapMotionParent == null)
        {
            return null;
        }
        else
        {
            if (LeftHand == null)
            {
                GameObject handParent = LeapMotionParent.transform.Find("HandModels").gameObject;
                LeftHand = handParent.transform.Find("RigidRoundHand_L").gameObject;
            }
            return LeftHand;
        }
    }

    public GameObject GetLeapRightHandPalm()
    {
        if (LeapMotionParent == null)
        {
            return null;
        }
        else
        {
            if (RightHand == null)
            {
                GameObject handParent = LeapMotionParent.transform.Find("HandModels").gameObject;
                RightHand = handParent.transform.Find("RigidRoundHand_R").Find("palm").gameObject;
            }
            return RightHand;
        }
    }
    public GameObject GetLeapRightHand()
    {
        if (LeapMotionParent == null)
        {
            return null;
        }
        else
        {
            if (RightHand == null)
            {
                GameObject handParent = LeapMotionParent.transform.Find("HandModels").gameObject;
                RightHand = handParent.transform.Find("RigidRoundHand_R").gameObject;
            }
            return RightHand;
        }
    }
    #endregion


    // Update is called once per frame
    void Update () {
		
	}
}
