using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class FollowHand : MonoBehaviour
{
    MixedRealityPose pose;
    public GameObject child1, child2;
    void Update()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out pose))
        {
            child1.SetActive(true);
            child2.SetActive(true);
            this.transform.position = pose.Position;
            Quaternion rotation = Quaternion.Euler(0, 0, 90);
            this.transform.rotation = pose.Rotation * rotation;
        }
        else
        {
            child1.SetActive(false);
            child2.SetActive(false);
        }
    }
}