using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MazeZeroWallWidthGenerator : MonoBehaviour
{
    [SerializeField]
    private bool _generate = false;

    [SerializeField]
    private int _width = 1;

    [SerializeField]
    private int _length = 1;

    [SerializeField]
    private int _height = 1;

    [SerializeField, Range(0, 4)]
    private int _entryNumber = 1;

    [SerializeField]
    private float _wallBold = 0.01f;

    [SerializeField]
    private GameObject _wallTilePrefab;

    private bool[,] _mazeArray;

    #region Public API

    public bool[,] MazeArray
    {
        get
        {
            if (_mazeArray == null)
                _mazeArray = new bool[_width, _length];

            return _mazeArray;
        }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        while (!(IsImpair(_width) && IsImpair(_width / 2)))
            _width++;

        while (!(IsImpair(_length) && IsImpair(_length / 2)))
            _length++;

        if (_generate)
        {
            _generate = false;
            GenerateMaze();
        }
    }

    private bool IsPair(int number)
    {
        return number % 2 == 0;
    }

    private bool IsImpair(int number)
    {
        return number % 2 != 0;
    }

    private void GenerateMaze()
    {
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        _mazeArray = new bool[_width, _length];

        for (int y = 0; y < _length; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                if (isBorder)
                {
                    _mazeArray[x, y] = true;

                    bool isMiddleOfWidth = x == (_width / 2);
                    bool isMiddleOfLength = y == (_length / 2);
                    bool isFirstGate = (y == 0 && _entryNumber >= 1);
                    bool isSecondGate = (y == _length - 1 && _entryNumber >= 2);
                    bool isThirdGate = (x == 0 && _entryNumber >= 3);
                    bool isFourthGate = (x == _width - 1 && _entryNumber >= 4);

                    if (isMiddleOfWidth || isMiddleOfLength)
                    {
                        if (isFirstGate || isSecondGate || isThirdGate || isFourthGate)
                            _mazeArray[x, y] = false;

                    }
                }
            }
        }


        for (int y = 0; y < _length; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                if (!isBorder)
                {
                    if (IsPair(x) && IsPair(y))
                    {
                        _mazeArray[x, y] = true;

                        int random = Random.Range(0, 4);

                        if (random == 0)
                            _mazeArray[x - 1, y] = true;

                        if (random == 1)
                            _mazeArray[x + 1, y] = true;

                        if (random == 2)
                            _mazeArray[x, y - 1] = true;

                        if (random == 3)
                            _mazeArray[x, y + 1] = true;
                    }
                    //else if (IsPair(x) || IsPair(y))
                    //{
                    //    int random = Random.Range(0, 2);

                    //    _mazeArray[x, y] = random == 1;
                    //}
                }
            }
        }

        for (int y = 0; y < _length; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == _width - 1 || y == _length - 1;

                if (!isBorder)
                {
                    if (IsImpair(x) && IsImpair(y))
                    {
                        bool isTileSurrounded = _mazeArray[x - 1, y] && _mazeArray[x + 1, y] && _mazeArray[x, y - 1] && _mazeArray[x, y + 1];

                        if (isTileSurrounded)
                        {
                            int random = Random.Range(0, 4);

                            if (random == 0)
                                _mazeArray[x - 1, y] = false;

                            if (random == 1)
                                _mazeArray[x + 1, y] = false;

                            if (random == 2)
                                _mazeArray[x, y - 1] = false;

                            if (random == 3)
                                _mazeArray[x, y + 1] = false;
                        }
                    }
                }
            }
        }


        for (int z = 0; z < _height; z++)
        {
            float wallKeyPointEraserY = 0;

            for (int y = 0; y < _length; y ++)
            {
                float wallKeyPointEraserX = 0;

                for (int x = 0; x < _width; x ++)
                {
                    if (_mazeArray[x, y] && (IsImpair(x) || IsImpair(y)))
                    {
                        GameObject tile = Instantiate(_wallTilePrefab, new Vector3(x - wallKeyPointEraserX, z, y - wallKeyPointEraserY + (IsPair(x) && IsImpair(y) ? 0.5f : 0)), Quaternion.identity);
                        tile.transform.SetParent(transform);

                        if (IsPair(x))
                        {
                            tile.transform.localScale = new Vector3(_wallBold, tile.transform.localScale.y, tile.transform.localScale.z);
                        }
                        else
                        {
                            tile.transform.localScale = new Vector3(tile.transform.localScale.x, tile.transform.localScale.y, _wallBold);
                        }
                    }

                    if ((IsPair(x) && IsPair(y)))
                    {
                        wallKeyPointEraserX++;

                        if (x == 0)
                            wallKeyPointEraserX -= 0.5f;
                    }

                    if((IsImpair(x) && IsImpair(y)))
                    {
                        wallKeyPointEraserX++;
                    }
                }

                if (IsPair(y))
                {
                    wallKeyPointEraserY++;
                }
            }
        }
    }
}
