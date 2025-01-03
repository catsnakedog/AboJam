using UnityEngine;
using static Hand;
using static HandUtil;

public interface IHandLogic
{
    public void SetLeftArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera);
    public void SetRightArm(GameObject arm, SpriteRenderer renderer, Weapon weapon, bool isChangeHand, Camera mainCamera);
}