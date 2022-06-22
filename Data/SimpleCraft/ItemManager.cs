using UnityEngine;

namespace Bug
{
    /// <summary>
    /// manage items merging and crafting
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [SerializeField, Tooltip("original asset")]
        private ItemAsset _asset;

        #region Public API

        /// <summary>
        /// return the right asset depending of the object state, flipped or not
        /// </summary>
        public ItemAsset ItemAsset => _asset;

        #endregion

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out CraftManager craftManager))
            {
                craftManager.ActiveIngredientsList.Remove(gameObject);
                craftManager.ActiveIngredientsList.Add(gameObject);

                if (craftManager.PreSearchRecipe)
                    craftManager.SearchRecipe();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out CraftManager craftManager))
            {
                craftManager.ActiveIngredientsList.Remove(gameObject);

                if (craftManager.PreSearchRecipe)
                    craftManager.SearchRecipe();
            }
        }
    }
}
