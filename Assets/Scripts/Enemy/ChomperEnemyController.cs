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

    public void PlayStep()
    {
        
    }

    public void Grunt()
    {
        
    }

    public void AttackBegin()
    {
        
    }

    public void AttackEnd()
    {
        
    }

    public void OnNext(GameObject value)
    {
        // TODO: 플레이어에게 데미지를 전달
    }

    public void OnCompleted()
    {
        _meleeWeaponController.Unsubscribe(this);
    }

    public void OnError(Exception error) { }
}
