using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OBSHotKeysManager.Web.Enums;
using OBSHotKeysManager.Web.Settings;
using OBSHotKeysManager.Web.SimulateKeys;

namespace OBSHotKeysManager.Web.Middlewares
{
    public class HotkeysMiddlware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, string> _hotKeysDic;
        private readonly Dictionary<string, byte> _keycodesDic;
        public HotkeysMiddlware(RequestDelegate next, IOptions<HotKeysSettings> optionsHotKeys)
        {
            _next = next;
            _hotKeysDic = optionsHotKeys.Value.HotKeys;
            _keycodesDic = optionsHotKeys.Value.KeyCodes;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool success = false;
            string messageError = string.Empty;
            if (context.Request.Path.StartsWithSegments("/api/hotkeys/simulate") && context.Request.Query.TryGetValue("hotKeyNames", out StringValues value))
            {
                string[] hotKeyNames = value.ToArray();
                IEnumerable<string> keys = Enumerable.Empty<string>();
                for (int i = 0; i < hotKeyNames.Length; i++)
                {
                    string hotKeyValue = _hotKeysDic[hotKeyNames[i]];

                    keys = keys.Concat(hotKeyValue.Split("+").Select(k => k.Trim()));

                }
                keys = keys.Distinct();

                StepsKeyPressEnum step = StepsKeyPressEnum.KeyDown;

                while (step < StepsKeyPressEnum.Finished)
                {
                    foreach (var key in keys)
                    {
                        byte keyCode = default;
                        if (_keycodesDic.TryGetValue(key, out byte keyCodeValue))
                        {
                            keyCode = keyCodeValue;
                        }
                        else
                        {
                            keyCode = (byte)key[0];
                        }

                        switch (step)
                        {
                            case StepsKeyPressEnum.KeyDown:
                                SimulateKey.KeyDown(keyCode);
                                break;
                            case StepsKeyPressEnum.KeyUp:
                                SimulateKey.KeyUp(keyCode);
                                break;
                        }
                    }

                    if (step == StepsKeyPressEnum.KeyDown)
                        Thread.Sleep(100);

                    step++;
                }

                success = true;
            }

            await _next(context);

            if (success)
            {
                context.Response.StatusCode = 200;
            }

        }
    }
}
