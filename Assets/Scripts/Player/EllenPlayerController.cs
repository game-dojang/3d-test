using Unity.VisualScripting;
using UnityEngine;

public class EllenPlayerController : PlayerController
{
    [SerializeField] private Transform weaponAttachTransform;

    private WeaponController _weaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _weaponController = Instantiate(staffObject, weaponAttachTransform).GetComponent<WeaponController>();
    }

    public void MeleeAttackStart()
    {
        
    }

    public void MeleeAttackEnd()
    {
        
    }
}
