namespace Flight;
public record GenericValueObject<T>(T Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator T(GenericValueObject<T> valueObject)
    {
        return valueObject.Value;
    }
}
