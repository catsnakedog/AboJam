using UnityEngine;
using static Hand;
using static HandUtil;

public interface IHandLogic
{
    public float MaxAngle { get; set; }
    public float MinAngle { get; set; }

    public void SetLeftArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed = false, Vector3 fixedLocation = default);
    public void SetRightArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera, bool isFixed = false, Vector3 fixedLocation = default);
}