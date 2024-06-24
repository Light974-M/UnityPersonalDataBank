using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.SpriteTpsController
{
    ///<summary>
    /// 
    ///</summary>
    [CreateAssetMenu(fileName = "NewSpriteListAsset", menuName = NamespaceID.UPDB + "/" + NamespaceID.CamerasAndCharacterControllers + "/" + NamespaceID.CharacterControllers + "/" + NamespaceID.SpriteTpsController + "/SpriteListAsset")]
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