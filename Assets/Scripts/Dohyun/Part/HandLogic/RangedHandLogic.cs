using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hand;
using static HandUtil;

public class RangedHandLogic : IHandLogic
{
    const float HANDCORRECTANGLE = 90;
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }

    public void SetLeftArm(GameObject arm, float armMovePower, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed, Quaternion fixedRot)
    {
        Quaternion rot;
        var weaponData = weapon.HandState;

        if (ApplyHandSwap(weaponData, HandType.Left, isChangeHand))
        {
            if (!IsRightHandStandard(weapon, isChangeHand))
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.afterGun;
                if (isFixed)
                    rot = fixedRot;
                else
                    rot = ForwardToMouse(arm, mainCamera, HANDCORRECTANGLE);
            }
            else
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.beforeGun;
                rot = ForwardToObj(arm, weapon.SecondHandLocation.position, HANDCORRECTANGLE);
            }
        }
        else
        {
            renderer.sortingLayerName = "Entity";
            renderer.sortingOrder = (int)HandLayer.InClothes;
            rot = Quaternion.Euler(new Vector3(0, 0, armMovePower));
        }

        arm.transform.rotation = rot;
    }

    public void SetRightArm(GameObject arm, float armMovePower, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed, Quaternion fixedRot)
    {
        Quaternion rot;
        var weaponData = weapon.HandState;

        if (ApplyHandSwap(weaponData, HandType.Right, isChangeHand))
        {
            if (IsRightHandStandard(weapon, isChangeHand))
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.afterGun;
                if(isFixed)
                    rot = fixedRot;
                else
                    rot = ForwardToMouse(arm, mainCamera, HANDCORRECTANGLE);
            }
            else
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.beforeGun;
                rot = ForwardToObj(arm, weapon.SecondHandLocation.position, HANDCORRECTANGLE);
            }
        }
        else
        {
            renderer.sortingLayerName = "Entity";
            renderer.sortingOrder = (int)HandLayer.InClothes;
            rot = Quaternion.Euler(new Vector3(0, 0, armMovePower));
        }

        arm.transform.rotation = rot;
    }
}