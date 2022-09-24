using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.OpenXR.Input;
using InfimaGames.LowPolyShooterPack;
using System.Runtime.Remoting.Contexts;


public class Shoot_2 : MonoBehaviour
{
    //Gesture recognising
    private float ShootAngle = 10.0f;
    private float ReloadAngle = 15.0f;
    MixedRealityPose Thumb1, Thumb2, Index1, Index2, 
        Middle, Ring, Pinky, Palm;
    Vector3 ThumbDirection, IndexDirection, MiddleDirection, RingDirection, PinkyDirection;
    float ThumbIndexAngle, MiddleAngle, RingAngle, PinkyAngle;

    //gun logic
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject laser;
    [SerializeField] GameManager GM;
    RaycastHit Hit;
    Ray ray;

    //gun animator
    [SerializeField] private WeaponBehaviour equippedWeapon;
    [SerializeField] private Animator characterAnimator;
    private float lastShotTime;
    private bool reloading;
    private int layerOverlay;
    private int layerActions;

    // Start is called before the first frame update
    void Start()
    {
        layerActions = characterAnimator.GetLayerIndex("Layer Actions");
        layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");
    }

    // Update is called once per frame
    void Update()
    {
        //update targeting
        UpdateRay();

        //testing for inputs
        CheckReload();
        CheckShoot();

        if (Input.GetKeyDown("f"))
        {
            StartCoroutine(Shoot());
        }
        if (Input.GetKeyDown("g"))
        {
            StartCoroutine(Reload());
        }
        //Debug.Log(equippedWeapon.GetAmmunitionCurrent() + "/" + equippedWeapon.GetAmmunitionTotal());
    }
    private void UpdateRay()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out Palm))
        {
            ray.origin = laser.transform.position;
            ray.direction = Palm.Forward;
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

    #region Shoot and Reload
    private void CheckShoot()
    {
        //testing for shoot involves getting the vector direction for thumb
        //and index finger and checking whether angle between is less than x
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out Thumb1))
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Right, out Thumb2))
            {
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out Index1))
                {
                    if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Right, out Index2))
                    {
                        ThumbDirection = (Thumb1.Position - Thumb2.Position).normalized;
                        IndexDirection = (Index1.Position - Index2.Position).normalized;
                        //absolute value between two joints is compared to the shoot angle
                        ThumbIndexAngle = Mathf.Abs(Vector3.Angle(IndexDirection, ThumbDirection));
                        if (ThumbIndexAngle < ShootAngle)
                        {
                            StartCoroutine(Shoot());
                        }
                    }
                }

            }
        }

    }

    private void CheckReload()
    {
        //testing for reload involves checking if middle, ring and pinky were extended
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out Middle))
        {
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out Ring))
            {
                if (HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out Pinky))
                {
                    if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out Palm))
                    {
                        MiddleDirection = (Middle.Position - Palm.Position).normalized;
                        MiddleAngle = Mathf.Abs(Vector3.Angle(MiddleDirection, Palm.Forward));
                        RingDirection = (Ring.Position - Palm.Position).normalized;
                        RingAngle = Mathf.Abs(Vector3.Angle(RingDirection, Palm.Forward));
                        PinkyDirection = (Pinky.Position - Palm.Position).normalized;
                        PinkyAngle = Mathf.Abs(Vector3.Angle(PinkyDirection, Palm.Forward));
                        if (MiddleAngle < ReloadAngle || RingAngle < ReloadAngle || PinkyAngle < ReloadAngle)
                        {
                            StartCoroutine(Reload());
                        }
                    }
                }
            }
        }
    }

    IEnumerator Shoot()
    {
        if (!CanPlayAnimationFire())
            yield break;

        //Check.
        if (equippedWeapon.HasAmmunition())
        {
            //Has fire rate passed.
            if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
            {
                Fire();
                GM.AmmoCount(equippedWeapon.GetAmmunitionCurrent() ,equippedWeapon.GetAmmunitionTotal());
                if (Physics.Raycast(ray, out Hit, 100))
                {
                    if (Hit.transform.tag.Equals("Target"))
                    {
                        Destroy(Hit.transform.gameObject);
                        GM.AddScore(100);
                    }
                }
            }
        }
        //Fire Empty.
        else
            FireEmpty();
        yield return null;
    }

    private void FireEmpty()
    {
        /*
         * Save Time. Even though we're not actually firing, we still need this for the fire rate between
         * empty shots.
         */
        lastShotTime = Time.time;
        //Play.
        characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0);
    }

    IEnumerator Reload()
    {
        if (!CanPlayAnimationReload()) { 
            yield break;
        }

        PlayReloadAnimation();
        yield return null;
    }

    #endregion

    #region GUN_ANIMATOR
    private bool CanPlayAnimationFire()
    {
        //Block.
        if (reloading)
            return false;

        //Return.
        return true;
    }
    private bool CanPlayAnimationReload()
    {
        //No reloading!
        if (reloading)
            return false;

        //Return.
        return true;
    }

    private void Fire()
    {
        //Save the shot time, so we can calculate the fire rate correctly.
        lastShotTime = Time.time;
        //Fire the weapon! Make sure that we also pass the scope's spread multiplier if we're aiming.
        equippedWeapon.Fire();

        //Play firing animation.
        const string stateName = "Fire";
        characterAnimator.CrossFade(stateName, 0.05f, layerOverlay, 0);
    }

    private void PlayReloadAnimation()
    {
        //Get the name of the animation state to play, which depends on weapon settings, and ammunition!
        string stateName = equippedWeapon.HasAmmunition() ? "Reload" : "Reload Empty";
        //Play the animation state!
        characterAnimator.Play(stateName, layerActions, 0.0f);

        //Set.
        reloading = true;

        //Reload.
        equippedWeapon.Reload();
    }

    public void DoneReloading()
    {
        reloading = false;
        GM.AmmoCount(equippedWeapon.GetAmmunitionCurrent(), equippedWeapon.GetAmmunitionTotal());
    }
    #endregion
}
