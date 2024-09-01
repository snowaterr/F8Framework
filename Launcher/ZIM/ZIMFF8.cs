using F8Framework.Core;
using F8Framework.Core.ZIM;
using F8Framework.F8ExcelDataClass;

namespace F8Framework.Launcher
{
    public static partial class FF8
    {
        /* ------------------------自定义模块 by zim------------------------ */

        // 场景管理
        private static SceneMgr _scene;
        public static SceneMgr Scene
        {
            get
            {
                if (_scene == null)
                    _scene = ModuleCenter.CreateModule<SceneMgr>();
                return _scene;
            }
            set
            {
                if (_scene == null)
                    _scene = value;
            }
        }
    }
}
