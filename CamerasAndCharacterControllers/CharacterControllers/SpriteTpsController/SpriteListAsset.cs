using UnityEngine;

///<summary>
/// 
///</summary>
[CreateAssetMenu(fileName = "NewSpriteListAsset", menuName = "SpriteListAsset")]
public class SpriteListAsset : ScriptableObject
{
    [SerializeField, Tooltip("")]
    private Sprite[] _spriteList;

    public Sprite[] SpriteList
    {
        get
        {
            return _spriteList;
        }

        set
        {
            _spriteList = value;
        }
    }
}