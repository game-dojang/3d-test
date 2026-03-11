using System;

public interface IWeaponObserver<T>
{
    public void OnNext(T value);
    public void OnCompleted();
    public void OnError(Exception error);
}
