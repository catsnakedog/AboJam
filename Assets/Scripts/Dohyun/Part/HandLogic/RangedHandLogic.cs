using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hand;
using static HandUtil;

public class RangedHandLogic : IHandLogic
{
    const float HANDCORRECTANGLE = 90;

    public void SetLeftArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera)
    {
        Quaternion rot;
        var weaponData = weapon.HandState;

        if (ApplyHandSwap(weaponData, HandType.Left, isChangeHand))
        {
            if (!IsRightHandStandard(weapon, isChangeHand))
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.afterGun;
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
            renderer.sortingOrder = (int)HandLayer.afterBody;
            rot = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        arm.transform.rotation = rot;
    }

    public void SetRightArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera)
    {
        Quaternion rot;
        var weaponData = weapon.HandState;

        if (ApplyHandSwap(weaponData, HandType.Right, isChangeHand))
        {
            if (IsRightHandStandard(weapon, isChangeHand))
            {
                renderer.sortingLayerName = "Entity";
                renderer.sortingOrder = (int)HandLayer.afterGun;
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
            renderer.sortingOrder = (int)HandLayer.afterBody;
            rot = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        arm.transform.rotation = rot;
    }
}