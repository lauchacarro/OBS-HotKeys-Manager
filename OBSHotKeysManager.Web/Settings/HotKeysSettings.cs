using System.Collections.Generic;

namespace OBSHotKeysManager.Web.Settings
{
    public class HotKeysSettings
    {
        public Dictionary<string, string> HotKeys { get; set; }
        public Dictionary<string, byte> KeyCodes { get; set; }
    }
}
