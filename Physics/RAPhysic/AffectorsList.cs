using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;

namespace UPDB.Physic.RAPhysic
{
    /// <summary>
    /// scriptable that contain dynamic list of affected objects
    /// </summary>
    [CreateAssetMenu(fileName = "AffectorList", menuName = NamespaceID.UPDB + "/" + NamespaceID.Physic + "/" + NamespaceID.RAPhysic + "/AffectorsList"), HelpURL(URL.baseURL + "/tree/main/Physics/RAPhysic/README.md")]
    public class AffectorsList : ScriptableObject
    {
        [SerializeField, Tooltip("place where you can manually edit list of detected objects(warning : program make everything automatic by default)")]
        private List<Affector> _affectorList;

        public List<Affector> AffectorList
        {
            get { return _affectorList; }
            set { _affectorList = value; }
        }
    } 
}
