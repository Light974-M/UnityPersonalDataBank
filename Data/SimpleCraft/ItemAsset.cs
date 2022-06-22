using UnityEngine;

namespace Bug
{
    [CreateAssetMenu(fileName = "newItem", menuName = "ScriptableObjects/Items")]
    public class ItemAsset : ScriptableObject
    {
        [SerializeField, Tooltip("items used to craft this item")]
        private ItemAsset[] ingredients;


        #region Public API

        public ItemAsset[] Ingredients
        {
            get
            {
                return ingredients;
            }
        }

        #endregion
    }
}
