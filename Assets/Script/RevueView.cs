using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevueView : MonoBehaviour
{
    private struct RevueProperty
    {
        // ★の数
        public int revue;

        // ★イメージ
        public List<Image> starImages;

        public  RevueProperty(int _revue = 0)
        {
            revue = _revue;
            starImages = new List<Image>();
        }
        
        /// <summary>
        /// 評価チェック
        /// 既に付いた評価より低い評価の場合はfalse
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckRevue(int value)
        {
            if(revue < value)
            {
                return true;
            }

            return false;
        }
    }
    
    // ★画像
    [SerializeField]
    private List<Sprite> starSprites;
    private Sprite YellowStar { get { return starSprites[0]; } }
    private Sprite GrayStar { get { return starSprites[1]; } }

    private RevueProperty revueProperty = new RevueProperty();

    // Start is called before the first frame update
    void Start()
    {
        revueProperty.starImages = gameObject.GetComponentsOnlyChildren<Image>();
    }



    /// <summary>
    /// ★を灰色画像にする
    /// </summary>
    public void ClearStar()
    {
        foreach (var star in revueProperty.starImages)
        {
            star.sprite = GrayStar;
        }
    }

    /// <summary>
    /// 指定位置まで★を黄色にする
    /// </summary>
    public void LightOnStar(int index)
    {
        if(!revueProperty.CheckRevue(index + 1))
        {
            return;
        }

        if (revueProperty.starImages.Count < index + 1)
        {
            return;
        }

        // 最低保証 星1つ(☆★★★)
        for (int i = 0; i < index + 1; i++)
        {
            revueProperty.starImages[i].sprite = YellowStar;
        }
    }
}
