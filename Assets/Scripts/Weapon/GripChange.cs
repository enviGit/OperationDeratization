using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GripChange : MonoBehaviour
{
    public Transform inventory;
    public TwoBoneIKConstraint rightHand;
    public TwoBoneIKConstraint leftHand;
    private Transform activeWeapon;
    private RigBuilder rigBuilder;

    private void Start()
    {
        rigBuilder = GetComponent<RigBuilder>();
    }
    private void Update()
    {
        if (inventory != null)
        {
            foreach (Transform child in inventory)
            {
                if (child.gameObject.activeSelf)
                {
                    activeWeapon = child;
                    break;
                }
            }
        }
        if (activeWeapon != null)
        {
            Transform rightGrip = activeWeapon.Find("RightGrip");
            Transform leftGrip = activeWeapon.Find("LeftGrip");

            if (rightGrip != null && leftGrip != null)
            {
                rightHand.data.target = rightGrip;
                leftHand.data.target = leftGrip;
                rigBuilder.Build();
            }
        }
    }
}
