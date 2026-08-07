public class PersonClass
{
    public string Name { get; }

    public PersonClass(string name)
    {
        Name = name;
    }
}

public record PersonRecord(string Name);


public struct PersonStruct
{
    public string Name;

    public PersonStruct(string name)
    {
        Name = name;
    }
}

public record struct PersonRecordStruct(string Name);

