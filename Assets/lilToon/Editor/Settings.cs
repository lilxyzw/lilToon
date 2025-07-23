using System.Globalization;
using UnityEditor;

namespace lilToon
{
    [FilePath("jp.lilxyzw/liltoon.asset", FilePathAttribute.Location.PreferencesFolder)]
    internal class Settings : ScriptableSingleton<Settings>
    {
        public string language = CultureInfo.CurrentCulture.Name;

        internal void Save() => Save(true);
    }
}
