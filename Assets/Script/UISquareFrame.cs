using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 四角形枠UI
/// </summary>
public class UISquareFrame : Graphic
{    
    // 太さ
    public float weight;

    // 横の長さ
    public float horizontalLength;

    // 縦の長さ
    public float verticalLength;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 position = Vector2.zero;

        Vector2 position1 = new Vector2(position.x - horizontalLength, position.y - verticalLength);
        Vector2 position2 = new Vector2(position.x + horizontalLength, position.y - verticalLength);
        Vector2 position3 = new Vector2(position.x + horizontalLength, position.y + verticalLength);
        Vector2 position4 = new Vector2(position.x - horizontalLength, position.y + verticalLength);

        ///線１

        // 垂直ベクトルの計算
        var pos1_to_2 = position2 - position1;
        var verticalVector = CalcurateVerticalVector(pos1_to_2);

        // 左下、左上のベクトルを計算
        var pos1Top = position1 + verticalVector * -weight / 2;
        var pos1Bottom = position1 + verticalVector * weight / 2;
        var pos2Top = position2 + verticalVector * -weight / 2;
        var pos2Bottom = position2 + verticalVector * weight / 2;

        // 頂点を頂点リストに追加
        AddVert(vh, pos1Top);
        AddVert(vh, pos1Bottom);
        AddVert(vh, pos2Top);
        AddVert(vh, pos2Bottom);

        // 頂点リストを元にメッシュを貼る
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(1, 2, 3);


        /// 線２

        // 垂直ベクトルの計算
        pos1_to_2 = position3 - position2;
        verticalVector = CalcurateVerticalVector(pos1_to_2);

        // 左下、左上のベクトルを計算
        pos1Top = position2 + verticalVector * -weight / 2;
        pos1Bottom = position2 + verticalVector * weight / 2;
        pos2Top = position3 + verticalVector * -weight / 2;
        pos2Bottom = position3 + verticalVector * weight / 2;

        // 頂点を頂点リストに追加
        AddVert(vh, pos1Top);
        AddVert(vh, pos1Bottom);
        AddVert(vh, pos2Top);
        AddVert(vh, pos2Bottom);

        // 頂点リストを元にメッシュを貼る
        vh.AddTriangle(4, 5, 6);
        vh.AddTriangle(5, 6, 7);

        /// 線３

        // 垂直ベクトルの計算
        pos1_to_2 = position4 - position3;
        verticalVector = CalcurateVerticalVector(pos1_to_2);

        // 左下、左上のベクトルを計算
        pos1Top = position3 + verticalVector * -weight / 2;
        pos1Bottom = position3 + verticalVector * weight / 2;
        pos2Top = position4 + verticalVector * -weight / 2;
        pos2Bottom = position4 + verticalVector * weight / 2;

        // 頂点を頂点リストに追加
        AddVert(vh, pos1Top);
        AddVert(vh, pos1Bottom);
        AddVert(vh, pos2Top);
        AddVert(vh, pos2Bottom);

        // 頂点リストを元にメッシュを貼る
        vh.AddTriangle(8, 9, 10);
        vh.AddTriangle(9, 10, 11);


        /// 線４

        // 垂直ベクトルの計算
        pos1_to_2 = position1 - position4;
        verticalVector = CalcurateVerticalVector(pos1_to_2);

        // 左下、左上のベクトルを計算
        pos1Top = position4 + verticalVector * -weight / 2;
        pos1Bottom = position4 + verticalVector * weight / 2;
        pos2Top = position1 + verticalVector * -weight / 2;
        pos2Bottom = position1 + verticalVector * weight / 2;

        // 頂点を頂点リストに追加
        AddVert(vh, pos1Top);
        AddVert(vh, pos1Bottom);
        AddVert(vh, pos2Top);
        AddVert(vh, pos2Bottom);

        // 頂点リストを元にメッシュを貼る
        vh.AddTriangle(12, 13, 14);
        vh.AddTriangle(13, 14, 15);
    }
    private void AddVert(VertexHelper vh, Vector2 pos)
    {
        var vert = UIVertex.simpleVert;
        vert.position = pos;
        vert.color = color;
        vh.AddVert(vert);

    }

    /// <summary>
    /// 垂線計算
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    private Vector2 CalcurateVerticalVector(Vector2 vec)
    {
        // 0除算の防止
        if (vec.y == 0)
        {
            return Vector2.up;
        }
        else
        {
            var verticalVector = new Vector2(1.0f, -vec.x / vec.y);
            return verticalVector.normalized;
        }
    }

    /// <summary>
    /// 線の長さ設定
    /// </summary>
    /// <param name="h"></param>
    /// <param name="v"></param>
    public void SetLength(float h,float v)
    {
        horizontalLength = h;
        verticalLength = v;
    }
}
