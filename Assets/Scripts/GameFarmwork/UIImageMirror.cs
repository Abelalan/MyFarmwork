using Nirvana;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effect/Mirror", 20)]
public class UIImageMirror : BaseMeshEffect
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum MirrorType
    {
        Horizontal,
        Vertical,
        Quarter
    }
    // ��������
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
    /// �ж�rectTransform�Ƿ���ֵ
    /// </summary>
    public RectTransform RectTransform { get => rectTransform ?? (rectTransform = GetComponent<RectTransform>()); }


    public void SetNativeSize()
    {
        if (graphic != null && graphic is Image)
        {
            // ��ȡ������ʾ�ľ���ͼƬ
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if (overrideSprite != null)
            {
                // overrideSprite.rect.width�Ǹþ�������Ŀ�ȣ���λ�����أ�
                // overrideSprite.rect.height�Ǹþ�������ĸ߶ȣ���λ�����أ�
                // ����/������/���絥λ�� = ���絥λ
                float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;

                // ����ê�㣬ʹ��ߴ����ֻӰ������Ĵ�С����Ӱ����ê��λ�á�
                rectTransform.anchorMax = rectTransform.anchorMin;
                // �������ͷ���ͼƬ�ߴ磬��ʱ��ʾ�ľ���ͼ��������״̬
                switch (imageMirrorType)
                {
                    case MirrorType.Horizontal:
                        // ˮƽ����x * 2
                        rectTransform.sizeDelta = new Vector2(w * 2, h);
                        break;
                    case MirrorType.Vertical:
                        // ��ֱ���� y * 2
                        rectTransform.sizeDelta = new Vector2(w, h * 2);
                        break;
                    case MirrorType.Quarter:
                        // �ķ�һ�Գƣ�ȫ��*2
                        rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                        break;
                }
                // ���Ϊ�࣬��һ�����»���ͼƬ
                graphic.SetVerticesDirty();
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="vh"></param>
    public override void ModifyMesh(VertexHelper vh)
    {
        // ���UIImageMirror�Ƿ��ڼ���״̬
        if (!IsActive())
        {
            return;
        }
        // ��ȡһ��UIVertex���� �ļ���
        var output = ListPool<UIVertex>.Obtain();
        // ��VertexHelper����ȡ�����������ݣ��洢��output��
        vh.GetUIVertexStream(output);
        // ��¼��������
        int count = output.Count;

        if (graphic is Image)
        {
            // ��ȡͼƬ���������
            Image.Type type = (graphic as Image).type;
            // �������ͽ��ж��㴦��
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
        // ���ԭ���Ķ�������
        vh.Clear();
        // ��������Ķ���������ӽ�ȥ
        vh.AddUIVertexTriangleStream(output);
        // �ͷż���
        ListPool<UIVertex>.Release(output);
    }

    /// <summary>
    /// ���Ƽ򵥰�
    /// </summary>
    /// <param name="output"></param>
    /// <param name="count"></param>
    protected void DrawSimple(List<UIVertex> output, int count)
    {
        // ��ȡ��ǰ UI Ԫ�ص����ص�����ľ�������
        // �����������һ�� Rect �ṹ����ʾ UI Ԫ�����丸�����е�λ�úʹ�С����λ�����ء�������ľ����������� UI Ԫ�ص�ê������ض�������ء�
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
        // ���񶥵�
        for (int i = 0; i < count; i++)
        {
            // ��ȡ����
            UIVertex vertex = output[i];
            // ��ȡ����λ��
            Vector3 position = vertex.position;

            if (isHorizontal)
            {
                // ���þ���󶥵��λ��
                position.x = rect.center.x * 2 - position.x;
            }
            else
            {
                position.y = rect.center.y * 2 - position.y;
            }
            // ��λ�ø�ֵ���˶���
            vertex.position = position;
            // �����ƺ�Ķ�����ӽ�����
            output.Add(vertex);


        }
    }


    /// <summary>
    /// ͨ��ʹ�ø÷���������ȷ������Ҫ��Ӵ���Ԫ�ص� List<UIVertex> ʱ����ǰ�����㹻���ڴ�ռ䣬����Ƶ�������·���͸��Ʋ������Ӷ�������ܡ�
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
    /// ����ԭʼ����
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="output"></param>
    /// <param name="count"></param>
    private void SimpleScale(Rect rect, List<UIVertex> output, int count)
    {
        // ����ԭʼ����
        for (int i = 0; i < count; i++)
        {
            // ��ȡ���㼰��λ��
            UIVertex vertex = output[i];
            Vector3 position = vertex.position;
            // ���ŵ����ڵ�һ����������
            if (imageMirrorType == MirrorType.Horizontal || imageMirrorType == MirrorType.Quarter)
            {
                position.x = (position.x + rect.x) * 0.5f;
            }
            // ���ŵ�ԭ����һ�룬��������
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

