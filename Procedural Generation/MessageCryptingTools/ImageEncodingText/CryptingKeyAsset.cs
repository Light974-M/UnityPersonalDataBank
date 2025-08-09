using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CryptingKeyAsset", menuName = "UPDB/ProceduralGeneration/TextEncryptionTools/ImageEncodingText/CryptingKeyAsset")]
public class CryptingKeyAsset : ScriptableObject
{
    [SerializeField]
    private string _keyList;

    public string KeyList => _keyList;
}
