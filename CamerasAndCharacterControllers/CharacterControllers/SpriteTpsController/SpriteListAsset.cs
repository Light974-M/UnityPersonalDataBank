using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.SpriteTpsController
{
    ///<summary>
    /// 
    ///</summary>
    [CreateAssetMenu(fileName = "NewSpriteListAsset", menuName = "UPDB/CamerasAndCharacterControllers/CharacterControllers/SpriteTpsController/SpriteListAsset")]
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
}