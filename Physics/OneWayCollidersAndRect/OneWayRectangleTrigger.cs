using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

public class OneWayRectangleTrigger : UPDBBehaviour
{
    [SerializeField]
    private Vector2 _bottomLeft;

    [SerializeField]
    private Vector2 _topRight;

    [SerializeField]
    private bool _enableTop = true;

    [SerializeField]
    private bool _enableBottom = true;

    [SerializeField]
    private bool _enableLeft = true;

    [SerializeField]
    private bool _enableRight = true;

    [SerializeField]
    private bool _invert = false;

    private bool isInside;
    private bool isLocked;

    void Start()
    {
        isInside = false;
        isLocked = false;
    }

    void Update()
    {
        Vector2 position = transform.position;
        UpdatePosition(position);
    }

    public void UpdatePosition(Vector2 position)
    {
        bool inside = position.x >= _bottomLeft.x && position.x <= _topRight.x &&
                      position.y >= _bottomLeft.y && position.y <= _topRight.y;

        if (!isLocked)
        {
            if (!_invert)
            {
                if (!isInside && inside)
                {
                    if ((_enableTop && position.y > _topRight.y) ||
                        (_enableBottom && position.y < _bottomLeft.y) ||
                        (_enableLeft && position.x < _bottomLeft.x) ||
                        (_enableRight && position.x > _topRight.x))
                    {
                        isLocked = true; // Verrouille l'entrée
                    }
                }
                else if (isInside && !inside)
                {
                    if ((_enableTop && position.y >= _topRight.y) ||
                        (_enableBottom && position.y <= _bottomLeft.y) ||
                        (_enableLeft && position.x <= _bottomLeft.x) ||
                        (_enableRight && position.x >= _topRight.x))
                    {
                        isLocked = true; // Verrouille la sortie
                    }
                }
            }
            else
            {
                if (isInside && !inside)
                {
                    if ((_enableTop && position.y >= _topRight.y) ||
                        (_enableBottom && position.y <= _bottomLeft.y) ||
                        (_enableLeft && position.x <= _bottomLeft.x) ||
                        (_enableRight && position.x >= _topRight.x))
                    {
                        isLocked = true; // Verrouille l'entrée au lieu de la sortie
                    }
                }
            }
        }

        isInside = inside;
    }

    public bool CanEnter(Vector2 position)
    {
        return !isLocked || isInside;
    }
}
