using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;

public class Shoot : MonoBehaviour, IMixedRealityGestureHandler
{
    [SerializeField] private MixedRealityInputAction Action;
    MixedRealityPose pose;
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject GunModel;
    [SerializeField] private GameObject gunshot;
    [SerializeField] GameManager GM;
    RaycastHit Hit;
    Ray ray;

    /*public void OnPointerDown(MixedRealityPointerEventData data) { Debug.Log("down"); }
       public void OnPointerDragged(MixedRealityPointerEventData data) { Debug.Log("dragged"); }
       public void OnPointerUp(MixedRealityPointerEventData data) { Debug.Log("up"); }
       public void OnPointerClicked(MixedRealityPointerEventData data) { Debug.Log("clicked"); }
   */

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler>(this);
        //Debug.Log("active");
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityGestureHandler>(this);
        //Debug.Log("deactive");
    }

    public void FixedUpdate()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out pose))
        {
            ray.origin = GunModel.transform.position;
            ray.direction = pose.Forward;
            line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out Hit, 100))
            {
                line.SetPosition(1, Hit.point);
            }
            else
            {
                line.SetPosition(1, ray.origin + (ray.direction * 100));
            }
        }
    }
    public void OnGestureStarted(InputEventData eventData) { }
    public void OnGestureUpdated(InputEventData eventData) { }
    public void OnGestureCompleted(InputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == Action)
        {
            if (Physics.Raycast(ray, out Hit, 100))
            {
                Instantiate(gunshot, Hit.point, Quaternion.Euler(0, 0, 0));
                if (Hit.transform.tag.Equals("Target"))
                {
                    Destroy(Hit.transform.gameObject);
                    GM.AddScore(100);
                }
            }
        }
    }
    public void OnGestureCanceled(InputEventData eventData) { }

}
