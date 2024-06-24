using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UPDB.CoreHelper;

namespace UPDB.Data.UITools.MenuUIController
{
    ///<summary>
    /// controller of every UI and buttons of menu Scene
    ///</summary>
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.Data + "/" + NamespaceID.UITools + "/" + NamespaceID.MenuUIController + "/MenuController")]
    public class MenuController : MonoBehaviour
    {
        #region variables

        [SerializeField, Tooltip("list of all panel used for every part of the menu")]
        private GameObject[] _panelList;

        [SerializeField, Tooltip("give the current loaded panel")]
        private Panel _panelDrawed;

        #endregion

        #region Public API

        /// <inheritdoc cref="_panelDrawed"/>
        public enum Panel
        {
            Main,
            Settings,
            Quit,
        }

        #endregion

        /// <summary>
        /// called when user click on play button
        /// </summary>
        public void OnPlayButton()
        {
            SceneManager.LoadScene("main");
        }

        /// <summary>
        /// called when user click on return button
        /// </summary>
        public void OnReturnButton()
        {
            //set panel value and update panel, depending on current panel value
            if ((int)_panelDrawed == 1 || (int)_panelDrawed == 2)
            {
                _panelDrawed--;
            }

            PanelUpdate();
        }

        /// <summary>
        /// called when user click on start button
        /// </summary>
        public void OnSettingsButton()
        {
            _panelDrawed++;

            PanelUpdate();
        }

        /// <summary>
        /// called when user click on quit button
        /// </summary>
        public void OnQuitButton()
        {
            //quit application
            Application.Quit();
        }

        /// <summary>
        /// called every time user change menu
        /// </summary>
        private void PanelUpdate()
        {
            // for each panel in panelList, activate only panel refering to current panel value
            for (int i = 0; i < _panelList.Length; i++)
            {
                if (i == (int)_panelDrawed)
                    _panelList[i].SetActive(true);
                else
                    _panelList[i].SetActive(false);
            }
        }
    } 
}
