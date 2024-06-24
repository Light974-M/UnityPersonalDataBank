using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Data.SimpleCraft
{
    [CreateAssetMenu(fileName = "newItem", menuName = NamespaceID.UPDB + "/" + NamespaceID.Data + "/" + NamespaceID.SimpleCraft + "/Items")]
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
