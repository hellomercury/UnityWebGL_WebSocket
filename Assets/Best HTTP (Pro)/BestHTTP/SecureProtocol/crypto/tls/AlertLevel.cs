/*


CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// RFC 5246 7.2
    /// </summary>
    public abstract class AlertLevel
    {
        public const byte warning = 1;
        public const byte fatal = 2;

        public static string GetName(byte alertDescription)
        {
            switch (alertDescription)
            {
            case warning:
                return "warning";
            case fatal:
                return "fatal";
            default:
                return "UNKNOWN";
            }
        }

        public static string GetText(byte alertDescription)
        {
            return GetName(alertDescription) + "(" + alertDescription + ")";
        }
    }
}

#endif
