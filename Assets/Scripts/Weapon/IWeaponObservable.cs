public interface IWeaponObservable<T>
{
    public void Subscribe(IWeaponObserver<T> observer);
    public void Unsubscribe(IWeaponObserver<T> observer);
    public void Notify(T value);
}