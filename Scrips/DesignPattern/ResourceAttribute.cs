[System.AttributeUsage(System.AttributeTargets.Class)]
public class ResourceAttribute : System.Attribute
{
    public string Path;

    public ResourceAttribute(string key)
    {
        this.Path = key;
    }
}
