using UnityEngine;

public class ColourSettings : BaseSingleConfig<ColourSettingsData, ColourSettings>
{
}

public class ColourSettingsData : IConfigData
{
    public string ID => nameof(ColourSettingsData);

    //public Gradient gradient;
    public Color planetColour;
}
