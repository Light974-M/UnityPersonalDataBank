using System.IO;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.ProceduralGeneration.TextEncryptionTools
{
    [ExecuteAlways]
    public class ImageEncryptingManager : UPDBBehaviour
    {
        [SerializeField]
        private CryptingKeyAsset _key;

        [SerializeField]
        private string _imageCreationPath;

        [SerializeField]
        private string _textToEncode;

        [SerializeField]
        private bool _encode;

        [SerializeField]
        private Texture2D _imageToDecode;

        [SerializeField]
        private string _decodedImage;

        [SerializeField]
        private bool _decode;


        private void Update()
        {
            if (_encode)
            {
                _encode = false;

                Encode();
            }

            if (_decode)
            {
                _decode = false;

                Decode();
            }
        }

        private void Encode()
        {
            if (!_key)
                return;

            int dimensions = Mathf.CeilToInt(Mathf.Sqrt(_textToEncode.Length));
            Texture2D toCreateImage = new Texture2D(dimensions, dimensions, TextureFormat.RGBAFloat, false);
            toCreateImage.filterMode = FilterMode.Point;

            int i = 0;
            for (int y = 0; y < dimensions; y++)
            {
                for (int x = 0; x < dimensions; x++)
                {
                    float colorValue = _key.KeyList.IndexOf(_textToEncode[i]) / (float)_key.KeyList.Length;
                    toCreateImage.SetPixel(x, y, new Color(colorValue, colorValue, colorValue, 1));
                    i++;

                    if (i >= _textToEncode.Length)
                        break;
                }

                if (i >= _textToEncode.Length)
                    break;
            }

            toCreateImage.Apply();

            byte[] pngData = toCreateImage.EncodeToPNG();
            string filePath = Application.dataPath + "/" + _imageCreationPath + "/" + "encodedImage.png";

            if (pngData != null)
            {
                File.WriteAllBytes(filePath, pngData);
                Debug.Log("Texture sauvegardée dans : " + filePath);
            }
        }

        private void Decode()
        {
            _decodedImage = string.Empty;

            for (int y = 0; y < _imageToDecode.height; y++)
            {
                for (int x = 0; x < _imageToDecode.width; x++)
                {
                    int index = Mathf.RoundToInt(_imageToDecode.GetPixel(x, y).r * _key.KeyList.Length);
                    _decodedImage += _key.KeyList[index];

                }
            }
        }
    }
}
