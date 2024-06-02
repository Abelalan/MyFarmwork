using Nirvana;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effect/Mirror", 20)]
public class UIImageMirror : BaseMeshEffect
{
    /// <summary>
    /// 镜像类型
    /// </summary>
    public enum MirrorType
    {
        Horizontal,
        Vertical,
        Quarter
    }
    // 镜像类型
    [SerializeField]
    private MirrorType imageMirrorType = MirrorType.Horizontal;


    private RectTransform rectTransform;
    public MirrorType ImageMirrorType
    {
        get
        {
            return imageMirrorType;
        }

        set
        {
            if (imageMirrorType != value)
            {
                imageMirrorType = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                    SetNativeSize();
                }
            }
        }
    }
    /// <summary>
    /// 判断rectTransform是否有值
    /// </summary>
    public RectTransform RectTransform { get => rectTransform ?? (rectTransform = GetComponent<RectTransform>()); }


    public void SetNativeSize()
    {
        if (graphic != null && graphic is Image)
        {
            // 获取正在显示的精灵图片
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if (overrideSprite != null)
            {
                // overrideSprite.rect.width是该矩形区域的宽度（单位：像素）
                // overrideSprite.rect.height是该矩形区域的高度（单位：像素）
                // 像素/（像素/世界单位） = 世界单位
                float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;

                // 设置锚点，使其尺寸调整只影响自身的大小而不影响其锚点位置。
                rectTransform.anchorMax = rectTransform.anchorMin;
                // 根据类型方大图片尺寸，此时显示的精灵图处于拉伸状态
                switch (imageMirrorType)
                {
                    case MirrorType.Horizontal:
                        // 水平方向，x * 2
                        rectTransform.sizeDelta = new Vector2(w * 2, h);
                        break;
                    case MirrorType.Vertical:
                        // 垂直方向 y * 2
                        rectTransform.sizeDelta = new Vector2(w, h * 2);
                        break;
                    case MirrorType.Quarter:
                        // 四分一对称，全部*2
                        rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                        break;
                }
                // 标记为脏，下一祯重新绘制图片
                graphic.SetVerticesDirty();
            }
        }
    }

    /// <summary>
    /// 根据
    /// </summary>
    /// <param name="vh"></param>
    public override void ModifyMesh(VertexHelper vh)
    {
        // 检查UIImageMirror是否处于激活状态
        if (!IsActive())
        {
            return;
        }
        // 获取一个UIVertex类型 的集合
        var output = ListPool<UIVertex>.Obtain();
        // 从VertexHelper中拿取顶点网格数据，存储在output中
        vh.GetUIVertexStream(output);
        // 记录顶点数量
        int count = output.Count;

        if (graphic is Image)
        {
            // 获取图片的填充类型
            Image.Type type = (graphic as Image).type;
            // 根据类型进行顶点处理
            switch (type)
            {
                case Image.Type.Simple:
                    DrawSimple(output, count);
                    break;
                case Image.Type.Sliced:
                    break;
                case Image.Type.Tiled:
                    break;
                case Image.Type.Filled:
                    break;
            }
        }
        else
        {
            DrawSimple(output, count);
        }
        // 清除原本的顶点数据
        vh.Clear();
        // 将处理过的顶点数据添加进去
        vh.AddUIVertexTriangleStream(output);
        // 释放集合
        ListPool<UIVertex>.Release(output);
    }

    /// <summary>
    /// 绘制简单版
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSimple(List<UIVertex> output, int count)
    {
        // 获取当前 UI 元素的像素调整后的矩形区域
        // 这个方法返回一个 Rect 结构，表示 UI 元素在其父容器中的位置和大小，单位是像素。调整后的矩形区域考虑了 UI 元素的锚点和像素对齐等因素。
        Rect rect = graphic.GetPixelAdjustedRect();

        SimpleScale(rect, output, count);
        switch (imageMirrorType)
        {
            case MirrorType.Horizontal:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, true);
                break;
            case MirrorType.Vertical:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, false);
                break;
            case MirrorType.Quarter:
                ExtendCapacity(output, count * 3);
                MirrorVerts(rect, output, count, true);
                MirrorVerts(rect, output, count * 2, false);
                break;
        }
    }

    private void MirrorVerts(Rect rect, List<UIVertex> output, int count, bool isHorizontal)
    {
        // 镜像顶点
        for (int i = 0; i < count; i++)
        {
            // 获取顶点
            UIVertex vertex = output[i];
            // 获取顶点位置
            Vector3 position = vertex.position;

            if (isHorizontal)
            {
                // 设置镜像后顶点的位置
                position.x = rect.center.x * 2 - position.x;
            }
            else
            {
                position.y = rect.center.y * 2 - position.y;
            }
            // 将位置赋值给此顶点
            vertex.position = position;
            // 将复制后的顶点添加进集合
            output.Add(vertex);


        }
    }


    /// <summary>
    /// 通过使用该方法，可以确保在需要添加大量元素到 List<UIVertex> 时，提前分配足够的内存空间，避免频繁的重新分配和复制操作，从而提高性能。
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    private void ExtendCapacity(List<UIVertex> output, int count)
    {
        var neededCapacity = output.Count + count;
        if (output.Capacity < neededCapacity)
        {
            output.Capacity = neededCapacity;
        }
    }

    /// <summary>
    /// 缩放原始顶点
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="output"></param>
    /// <param name="count"></param>
    private void SimpleScale(Rect rect, List<UIVertex> output, int count)
    {
        // 缩放原始顶点
        for (int i = 0; i < count; i++)
        {
            // 获取顶点及其位置
            UIVertex vertex = output[i];
            Vector3 position = vertex.position;
            // 缩放到现在的一半向左缩放
            if (imageMirrorType == MirrorType.Horizontal || imageMirrorType == MirrorType.Quarter)
            {
                position.x = (position.x + rect.x) * 0.5f;
            }
            // 缩放到原来的一半，左下缩放
            if (imageMirrorType == MirrorType.Vertical || imageMirrorType == MirrorType.Quarter)
            {
                position.y = (position.y + rect.y) * 0.5f;
            }

            vertex.position = position;
            output[i] = vertex;
        }

  
    }

  

}



//public class IndexTest
//{
//    public string LastName;
//    public string FirstName;
//    public string CityOfBirth;

//    public string this[int index]
//    {
//        get
//        {
//            switch (index)
//            {
//                case 0: return FirstName;
//                case 1: return FirstName;
//                case 2: return CityOfBirth;
//                default:
//                    throw new ArgumentOutOfRangeException("index");
//            }
//        }
//        set
//        {
//            switch (index)
//            {
//                case 0:
//                    LastName = value;
//                    break;
//                case 1:
//                    FirstName = value;
//                    break;
//                case 2:
//                    CityOfBirth = value;
//                    break;
//                default: throw new ArgumentOutOfRangeException("index");
//            }
//        }
//    }
//}

