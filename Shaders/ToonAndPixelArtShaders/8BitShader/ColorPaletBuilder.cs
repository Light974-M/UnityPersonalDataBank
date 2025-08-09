using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.HeightBitRenderer
{
    [ExecuteAlways]
    public class ColorPaletBuilder : UPDBBehaviour
    {
        [SerializeField]
        private Material _shaderMat;

        [SerializeField]
        private ColorPaletAsset _colorsPaletConfig;

        [SerializeField]
        private bool _build = false;

        private void Update()
        {
            if(_build)
            {
                _build = false;

                Texture2D colorPaletTexture = new Texture2D(_colorsPaletConfig.ColorsPalet.Count, 1, TextureFormat.ARGB32, false);
                colorPaletTexture.filterMode = FilterMode.Point;

                for (int x = 0; x < colorPaletTexture.width; x++)
                    colorPaletTexture.SetPixel(x, 1, _colorsPaletConfig.ColorsPalet[x]);

                colorPaletTexture.Apply();

                _shaderMat.SetTexture("_ColorPalet", colorPaletTexture);
            }
        }
    } 
}
