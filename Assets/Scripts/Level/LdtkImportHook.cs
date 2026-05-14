using UnityEngine;

namespace HollowStyleMVP.Level
{
    public class LdtkImportHook : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string note = "安装 LDtk Unity 插件后，把导入生成的 level root 挂到这里，按层命名为 Collision/Decor/Entities。";
    }
}
