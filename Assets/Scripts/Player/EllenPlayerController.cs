using System;
using Unity.VisualScripting;
using UnityEngine;

public class EllenPlayerController : PlayerController, IWeaponObserver<GameObject>
{
    [SerializeField] private Transform weaponAttachTransform;

    private MeleeWeaponController _meleeWeaponController;

    private void Start()
    {
        var staffObject = Resources.Load<GameObject>("Staff");
        _meleeWeaponController = Instantiate(staffObject, weaponAttachTransform).GetComponent<MeleeWeaponController>();
        _meleeWeaponController.Subscribe(this);
    }

    public void MeleeAttackStart()
    {
        _meleeWeaponController.StartTrigger();
    }

    public void MeleeAttackEnd()
    {
        _meleeWeaponController.EndTrigger();
    }

    public void OnNext(GameObject value)
    {
        var enemyController = value.GetComponent<EnemyController>();
        if (enemyController)
        {
            enemyController.SetHit(10, transform.forward);
        }
    }

    public void OnCompleted()
    {
        _meleeWeaponController.Unsubscribe(this);
    }

    public void OnError(Exception error) { }
}
