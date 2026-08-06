public class AutoPropertyPerson
{
    public string Name { get; set; } = "";

}

public class InitOnlyPerson
{
    public string Name { get; init; } = "";

}

public class PrimaryConstructorPerson(string name)
{
    public string Name { get; } = name;
}
