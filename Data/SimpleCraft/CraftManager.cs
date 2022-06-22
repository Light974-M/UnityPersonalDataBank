using System.Collections.Generic;
using UnityEngine;

namespace Bug
{
    public class CraftManager : MonoBehaviour
    {
        [SerializeField, Tooltip("object that is detected has craftable")]
        private GameObject _craftableObject;

        [SerializeField, Tooltip("if enabled, recipe will be founded before player craft object")]
        private bool _preSearchRecipe = true;

        [SerializeField, Tooltip("mode of input for craft")]
        private CraftMode _craftInputMode = CraftMode.TwoButtons;

        [SerializeField, Tooltip("if disabled, objects will dispatch trough the table")]
        private bool _instanciateAtSamePos = false;

        [SerializeField, Tooltip("position of crafted object compare to crafting table")]
        private Vector3 _craftedObjectPosition = Vector3.zero;

        [SerializeField, Tooltip("list of all craftable Objects prefab that could be instanciated")]
        private GameObject[] _objectPrefabList;

        /// <summary>
        /// list containing all objects currently put on crafting table
        /// </summary>
        private List<GameObject> _activeIngredientsList = new List<GameObject>();

        #region Public API

        public List<GameObject> ActiveIngredientsList
        {
            get { return _activeIngredientsList; }
            set { _activeIngredientsList = value; }
        }

        /// <summary>
        /// select the mode of input that will be applied to craft
        /// </summary>
        public enum CraftMode
        {
            TwoButtons,
            CraftWhenMultiple,
        }

        public bool PreSearchRecipe => _preSearchRecipe;

        #endregion

        #region InputsCallBack

        /// <summary>
        /// called when user select the craft input(set up into unityEvent
        /// </summary>
        public void OnCraft()
        {
            if (InputTest() || _craftInputMode == CraftMode.TwoButtons)
            {
                if (!_preSearchRecipe)
                    SearchRecipe();

                if (_craftableObject != null)
                    Craft(true, _craftableObject);
                else
                    Craft(false, null);
            }
            else
                OnUncraft();
        }

        /// <summary>
        /// called when user select the uncraft input(set up into unityEvent)
        /// </summary>
        public void OnUncraft()
        {
            if (!InputTest() || _craftInputMode == CraftMode.TwoButtons)
            {
                int posOffsetX = 0;
                int posOffsetY = 0;
                float length = _activeIngredientsList.Count;

                for (int i = 0; i < length; i++)
                    if (_activeIngredientsList[0].GetComponent<ItemManager>().ItemAsset.Ingredients.Length > 0)
                        UnCraft(_activeIngredientsList[0], posOffsetX, posOffsetY);
            }
            else
                OnCraft();
        }

        #endregion

        /// <summary>
        /// select craft or uncraft, depending on input mode
        /// </summary>
        /// <returns></returns>
        private bool InputTest()
        {
            bool isCrafting = false;

            if (_craftInputMode == CraftMode.CraftWhenMultiple)
                if (_activeIngredientsList.Count > 1)
                    isCrafting = true;

            return isCrafting;
        }

        /// <summary>
        /// returns if craftable object is founded
        /// </summary>
        /// <returns></returns>
        private bool IsRecipeCraftable(int i)
        {
            bool hasAllIngredients = true;

            //list that show the recipe of the object to craft
            ItemAsset[] requiredAssetsToCraft = _objectPrefabList[i].GetComponent<ItemManager>().ItemAsset.Ingredients;
            List<ItemAsset> requiredAssetsToCraftCountDown = new List<ItemAsset>(requiredAssetsToCraft);

            //for each ingredients currently on table
            for (int j = 0; j < _activeIngredientsList.Count; j++)
            {
                ItemAsset ingredientAsset = _activeIngredientsList[j].GetComponent<ItemManager>().ItemAsset;
                bool isIngredientFounded = false;

                //for each ingredients in the recipe(removing items already founded)
                for (int k = 0; k < requiredAssetsToCraftCountDown.Count; k++)
                {
                    //if ingredient found himself in the recipe, then break, else, return false
                    if (requiredAssetsToCraftCountDown[k] == ingredientAsset)
                    {
                        isIngredientFounded = true;
                        requiredAssetsToCraftCountDown.Remove(requiredAssetsToCraftCountDown[k]);
                        break;
                    }
                }

                //if current ingredient is not in the recipe, break and return false
                if (!isIngredientFounded)
                {
                    hasAllIngredients = false;
                    break;
                }
            }

            //if craftable object is founded, put it in the object to craft
            return hasAllIngredients && requiredAssetsToCraft.Length == _activeIngredientsList.Count;
        }

        /// <summary>
        /// take object in parameter and craft it
        /// </summary>
        private void Craft(bool allowed, GameObject objToCraft)
        {
            if (allowed)
            {
                int length = _activeIngredientsList.Count;

                for (int i = 0; i < length; i++)
                {
                    GameObject objToRemove = _activeIngredientsList[0];

                    _activeIngredientsList.Remove(_activeIngredientsList[0]);
                    Destroy(objToRemove);
                }

                Instantiate(objToCraft, transform.position + _craftedObjectPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("refused");
            }
        }

        /// <summary>
        /// take object in parameter and uncraft it
        /// </summary>
        /// <param name="objToUncraft"></param>
        private void UnCraft(GameObject objToUncraft, float posOffsetX, float posOffsetY)
        {
            ItemAsset[] toInstanciate = objToUncraft.GetComponent<ItemManager>().ItemAsset.Ingredients;

            foreach (ItemAsset itemAsset in toInstanciate)
            {
                if (!_instanciateAtSamePos)
                {
                    posOffsetX++;
                    if (posOffsetX > 4)
                    {
                        posOffsetX = 0;
                        posOffsetY++;
                    }
                }

                Vector3 posOffset = new Vector3(posOffsetX, posOffsetY, 0);

                for (int i = 0; i < _objectPrefabList.Length; i++)
                    if (_objectPrefabList[i].GetComponent<ItemManager>().ItemAsset == itemAsset)
                        Instantiate(_objectPrefabList[i], transform.position + _craftedObjectPosition + posOffset, Quaternion.identity);
            }
            _activeIngredientsList.Remove(objToUncraft);
            Destroy(objToUncraft);
        }

        public void SearchRecipe()
        {
            //object that will be crafted, null means that no recipe is available
            GameObject objToCraft = null;

            //for each object set up in craftable object list
            for (int i = 0; i < _objectPrefabList.Length; i++)
                if (IsRecipeCraftable(i))
                    objToCraft = _objectPrefabList[i];

            _craftableObject = objToCraft;
        }
    }
}
