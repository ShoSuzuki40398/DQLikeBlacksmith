using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevueView : MonoBehaviour
{
    private struct RevueProperty
    {
        // ���̐�
        public int revue;

        // ���C���[�W
        public List<Image> starImages;

        public  RevueProperty(int _revue = 0)
        {
            revue = _revue;
            starImages = new List<Image>();
        }
        
        /// <summary>
        /// �]���`�F�b�N
        /// ���ɕt�����]�����Ⴂ�]���̏ꍇ��false
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
    
    // ���摜
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
    /// �����D�F�摜�ɂ���
    /// </summary>
    public void ClearStar()
    {
        foreach (var star in revueProperty.starImages)
        {
            star.sprite = GrayStar;
        }
    }

    /// <summary>
    /// �w��ʒu�܂Ł������F�ɂ���
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

        // �Œ�ۏ� ��1��(��������)
        for (int i = 0; i < index + 1; i++)
        {
            revueProperty.starImages[i].sprite = YellowStar;
        }
    }
}
