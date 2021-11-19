using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectorList", menuName ="ScriptableObjects/AffectorsList")]
public class AffectorsList : ScriptableObject
{
    [SerializeField]
    [Tooltip("place where you can manually edit list of detected objects(warning : program make everything automatic by default)")]
    private List<Affector> _affectorList;

    public List<Affector> AffectorList
    {
        get { return _affectorList; }
        set { _affectorList = value; }
    }
}
