using UnityEngine;

/// <summary>
/// ��Դ����
/// </summary>
 
    public class AssetData
    {
        // ��Դ·��
        public string Path { get; }
        // ��Դ����
        public Object Asset { get; set; }
        // ״̬
        public AssetLoadState State { get; set; }
        // lua��Դ
        public string LuaAsset { get; set; }
        // �ʲ�����
        public AssetData(string path)
        {
            Path = path.ToLower();

        }
    }
 
