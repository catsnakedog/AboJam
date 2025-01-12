using UnityEngine;
using static Hand;
using static HandUtil;

public interface IHandLogic
{
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }

    public void SetLeftArm(GameObject arm, float armMovePower, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed = false, Quaternion fixedRot = default);
    public void SetRightArm(GameObject arm, float armMovePower, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed = false, Quaternion fixedRot = default);
}