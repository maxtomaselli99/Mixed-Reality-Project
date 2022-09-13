using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class FollowHand : MonoBehaviour
{
    MixedRealityPose pose;
    public GameObject Model;
    void Update()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out pose))
        {
            Model.SetActive(true);
            this.transform.position = pose.Position;
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            this.transform.rotation = pose.Rotation * rotation;
        }
        else
        {
            Model.SetActive(false);
        }
    }
}