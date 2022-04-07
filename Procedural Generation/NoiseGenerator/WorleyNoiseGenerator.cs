using UnityEngine;

namespace UPDB.ProceduralGeneration.NoiseGenerator
{
    ///<summary>
    /// 
    ///</summary>
    public class WorleyNoiseGenerator : NoiseTextureGenerator
    {
        [SerializeField]
        private int _nbCells = 10;

        [SerializeField]
        private float _cellSize = 10f;

        [SerializeField]
        private int _seed = 0;

        public override Texture2D Generate()
        {
            if(_seed != 0)
                Random.InitState(_seed);


            Texture2D texture = new Texture2D(TextureSize.x, TextureSize.y);
            Vector2[] cellPositions = new Vector2[_nbCells];

            for (int i = 0; i < cellPositions.Length; i++)
                cellPositions[i] = new Vector2(Random.Range(0, TextureSize.x), Random.Range(0, TextureSize.y));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Vector2 closestCell = cellPositions[0];
                    float closestSqrDistance = (new Vector2(x, y) - closestCell).sqrMagnitude;

                    foreach (Vector2 cell in cellPositions)
                    {
                        float cellSqrDistance = (new Vector2(x, y) - cell).sqrMagnitude;

                        if(cellSqrDistance < closestSqrDistance)
                        {
                            closestCell = cell;
                            closestSqrDistance = cellSqrDistance;
                        }
                    }

                    float distance = Mathf.Sqrt(closestSqrDistance);
                    texture.SetPixel(x, y, Color.Lerp(Color.blue, new Color(1,1,1, 0), distance / _cellSize));
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }
    } 
}