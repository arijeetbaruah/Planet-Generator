public class ShapeSettings : BaseSingleConfig<ShapeSettingsData, ShapeSettings>
{
}

public class ShapeSettingsData : IConfigData
{
    public string ID => nameof(ShapeSettingsData);

    public float planetRadius = 1;
}
