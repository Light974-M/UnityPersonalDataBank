using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.CustomPropertyAttributes;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Renderers.AsciiEngine
{
    [ExecuteAlways]
    public class AsciiTableBuilder : UPDBBehaviour
    {
        [SerializeField]
        private Material _asciiRendererMat;

        [SerializeField]
        private AsciiTableAsset _asciiConfig;

        [SerializeField]
        private Vector2 _charactersBrightnessRange = Vector2.right;

        [SerializeField]
        private float _charactersBrightnessStepMin = 0;

        [SerializeField]
        private bool _getOneEmptySlotForBlack = true;

        [SerializeField]
        private bool _getOneEmptySlotForWhite = true;

        [SerializeField]
        private bool _build = false;

        [SerializeField, ReadOnly]
        private float _minBrightness = 0;

        [SerializeField, ReadOnly]
        private float _maxBrightness = 0;

        [SerializeField, ReadOnly]
        private float _averageBrightness = 0;

        [SerializeField, ReadOnly]
        private float _averageBrightnessStep = 0;

        private Texture2DArray _asciiTableTextureArray;

        private void Start()
        {
            BuildTextureArray();
        }

        private void Update()
        {
            if (_build)
            {
                _build = false;

                BuildTextureArray();
            }
        }

        private void BuildTextureArray()
        {
            if (_asciiConfig.BuildMode == AsciiTableBuildMode.SingleTexture)
            {
                List<Texture2D> charactersListGenerated = new List<Texture2D>();
                BuildTextureList(ref charactersListGenerated);
                BuildTextureArrayFromList(charactersListGenerated);
                return;
            }

            if (_asciiConfig.BuildMode == AsciiTableBuildMode.TextureList)
            {
                BuildTextureArrayFromList(_asciiConfig.CharactersList);
                return;
            }
        }

        private void BuildTextureList(ref List<Texture2D> charactersListToGenerate)
        {
            charactersListToGenerate = new List<Texture2D>();
            List<float> textureBrightnessList = new List<float>();

            bool hasOneEmptyCharacter = false;
            bool hasOneFullCharacter = false;

            //create list and brightness list
            for (int y = 0; y < (_asciiConfig.AsciiTextureBuildable.height / _asciiConfig.TextureBuildableCharacterSize.y); y++)
            {
                for (int x = 0; x < (_asciiConfig.AsciiTextureBuildable.width / _asciiConfig.TextureBuildableCharacterSize.x); x++)
                {
                    Texture2D characterToCreate = new Texture2D(_asciiConfig.CharacterSize.x, _asciiConfig.CharacterSize.y, TextureFormat.ARGB32, false);
                    characterToCreate.filterMode = FilterMode.Point;

                    float textureBrightness = 0;

                    for (int y2 = 0; y2 < _asciiConfig.CharacterSize.y; y2++)
                    {
                        for (int x2 = 0; x2 < _asciiConfig.CharacterSize.x; x2++)
                        {
                            Color charaPixelToSet = _asciiConfig.AsciiTextureBuildable.GetPixel(x2 + (x * _asciiConfig.TextureBuildableCharacterSize.x), y2 + (y * _asciiConfig.TextureBuildableCharacterSize.y));

                            textureBrightness += charaPixelToSet.BlackAndWhite();

                            characterToCreate.SetPixel(x2, y2, charaPixelToSet);
                        }
                    }

                    characterToCreate.Apply();

                    textureBrightness /= (_asciiConfig.CharacterSize.x * _asciiConfig.CharacterSize.y);

                    bool hasToAddEmpty = textureBrightness == 0 && !hasOneEmptyCharacter && _getOneEmptySlotForBlack;
                    bool hasToAddFull = textureBrightness == 1 && !hasOneFullCharacter && _getOneEmptySlotForWhite;

                    if (hasToAddFull || hasToAddEmpty || (textureBrightness != 0 && textureBrightness != 1))
                    {
                        charactersListToGenerate.Add(characterToCreate);
                        textureBrightnessList.Add(textureBrightness);
                    }

                    if (textureBrightness == 0)
                        hasOneEmptyCharacter = true;

                    if (textureBrightness == 1)
                        hasOneFullCharacter = true;
                }
            }

            //sort
            for (int i = 0; i < textureBrightnessList.Count - 1; i++)
            {
                for (int j = 0; j < textureBrightnessList.Count - i - 1; j++)
                {
                    if (textureBrightnessList[j] > textureBrightnessList[j + 1])
                    {
                        float tempBrightness = textureBrightnessList[j];
                        textureBrightnessList[j] = textureBrightnessList[j + 1];
                        textureBrightnessList[j + 1] = tempBrightness;

                        Texture2D tempText = charactersListToGenerate[j];
                        charactersListToGenerate[j] = charactersListToGenerate[j + 1];
                        charactersListToGenerate[j + 1] = tempText;
                    }
                }
            }

            //delete list elements that doesn't fit requirements
            for (int i = charactersListToGenerate.Count - 1; i >= 0; i--)
            {
                bool outsideOfBrightnessRange = textureBrightnessList[i] < _charactersBrightnessRange.x || textureBrightnessList[i] > _charactersBrightnessRange.y;

                if (outsideOfBrightnessRange)
                {
                    charactersListToGenerate.RemoveAt(i);
                    textureBrightnessList.RemoveAt(i);
                }
            }

            List<float> brightnessList = new List<float>();
            List<Texture2D> charactersList = new List<Texture2D>();

            brightnessList.Add(textureBrightnessList[0]);
            charactersList.Add(charactersListToGenerate[0]);

            int whileIndex = 0;
            while (whileIndex < charactersListToGenerate.Count - 1)
            {
                int toAddIndex = whileIndex + 1;

                while (toAddIndex < charactersListToGenerate.Count - 1 && textureBrightnessList[toAddIndex] - textureBrightnessList[whileIndex] < _charactersBrightnessStepMin)
                    toAddIndex++;

                brightnessList.Add(textureBrightnessList[toAddIndex]);
                charactersList.Add(charactersListToGenerate[toAddIndex]);

                if (toAddIndex >= charactersListToGenerate.Count - 1)
                    break;

                whileIndex = toAddIndex;
            }

            textureBrightnessList = brightnessList;
            charactersListToGenerate = charactersList;

            //display some interesting informations on readOnly
            _minBrightness = textureBrightnessList[0];
            _maxBrightness = textureBrightnessList[textureBrightnessList.Count - 1];

            _averageBrightness = 0;
            _averageBrightnessStep = 0;

            for (int i = 0; i < textureBrightnessList.Count; i++)
            {
                _averageBrightness += textureBrightnessList[i];

                if (i < textureBrightnessList.Count - 1)
                    _averageBrightnessStep += textureBrightnessList[i + 1] - textureBrightnessList[i];
            }

            _averageBrightness /= textureBrightnessList.Count;
            _averageBrightnessStep /= textureBrightnessList.Count - 1;

            _asciiTableTextureArray = new Texture2DArray(_asciiConfig.CharacterSize.x, _asciiConfig.CharacterSize.y, charactersListToGenerate.Count, TextureFormat.ARGB32, false);

            _asciiTableTextureArray.filterMode = FilterMode.Point;
            _asciiTableTextureArray.wrapMode = TextureWrapMode.Repeat;

            for (int i = 0; i < charactersListToGenerate.Count; i++)
                if (charactersListToGenerate[i])
                    Graphics.CopyTexture(charactersListToGenerate[i], 0, 0, _asciiTableTextureArray, i, 0);

            _asciiTableTextureArray.Apply();

            _asciiRendererMat.SetTexture("_AsciiTableTextureArray", _asciiTableTextureArray);
            _asciiRendererMat.SetFloat("_asciiCharacterNumbers", charactersListToGenerate.Count);
        }

        private void BuildTextureArrayFromList(List<Texture2D> listToCreateFrom)
        {

        }
    }
}
