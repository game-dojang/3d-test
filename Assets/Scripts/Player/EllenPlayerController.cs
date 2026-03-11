using Unity.VisualScripting;
using UnityEngine;

public class EllenPlayerController : PlayerController
{
    [SerializeField] private Transform weaponAttachTransform;

    private MeleeWeaponController _meleeWeaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _meleeWeaponController = Instantiate(staffObject, weaponAttachTransform).GetComponent<MeleeWeaponController>();
    }

    public void MeleeAttackStart()
    {
        
    }

    public void MeleeAttackEnd()
    {
        
    }
}
