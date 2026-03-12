using System;
using UnityEngine;

public class ChomperEnemyController : EnemyController, IWeaponObserver<GameObject>
{
    private MeleeWeaponController _meleeWeaponController;

    private void Start()
    {
        _meleeWeaponController = GetComponent<MeleeWeaponController>();
        _meleeWeaponController.Subscribe(this);
    }

    public void PlayStep() { }

    public void Grunt() { }

    public void AttackBegin()
    {
        _meleeWeaponController.StartTrigger();
    }

    public void AttackEnd()
    {
        _meleeWeaponController.EndTrigger();
    }

    public void OnNext(GameObject value)
    {
        var playerController = value.GetComponent<PlayerController>();
        if (playerController) playerController.SetHit(10, -transform.forward);
    }

    public void OnCompleted()
    {
        _meleeWeaponController.Unsubscribe(this);
    }

    public void OnError(Exception error) { }
}
