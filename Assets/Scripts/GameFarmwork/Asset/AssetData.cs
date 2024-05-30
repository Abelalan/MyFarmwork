using UnityEngine;

/// <summary>
/// 资源数据
/// </summary>
 
    public class AssetData
    {
        // 资源路径
        public string Path { get; }
        // 资源对象
        public Object Asset { get; set; }
        // 状态
        public AssetLoadState State { get; set; }
        // lua资源
        public string LuaAsset { get; set; }
        // 资产数据
        public AssetData(string path)
        {
            Path = path.ToLower();

        }
    }
 
