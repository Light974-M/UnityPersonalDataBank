using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

public class OneWayRectangleCollider : UPDBBehaviour
{
    public Vector2 bottomLeft = new Vector2(-1, -1);
    public Vector2 topRight = new Vector2(1, 1);
    public bool enableTop = true;
    public bool enableBottom = true;
    public bool enableLeft = true;
    public bool enableRight = true;
    public bool invert = false;

    private bool isInside;
    private Vector2 lastValidPosition;

    void Start()
    {
        isInside = false;
        lastValidPosition = transform.position;
    }

    void Update()
    {
        Vector2 position = transform.position;
        bool inside = IsInside(position);

        if (!invert)
        {
            // Si on sort de la zone alors qu'on �tait dedans, emp�cher la sortie
            if (isInside && !inside)
            {
                transform.position = lastValidPosition;
            }
            // Si on rentre alors qu'on �tait dehors, emp�cher l'entr�e
            else if (!isInside && inside && IsBlockedByEdge(position))
            {
                transform.position = lastValidPosition;
                inside = false;
            }
        }
        else
        {
            // Mode invers� : on emp�che de sortir apr�s �tre rentr�
            if (isInside && !inside)
            {
                transform.position = lastValidPosition;
                inside = true;
            }
        }

        // Mise � jour des statuts
        if (inside)
        {
            lastValidPosition = position;
        }

        isInside = inside;
    }

    private bool IsInside(Vector2 position)
    {
        return position.x >= bottomLeft.x && position.x <= topRight.x &&
               position.y >= bottomLeft.y && position.y <= topRight.y;
    }

    private bool IsBlockedByEdge(Vector2 position)
    {
        return (enableTop && position.y > topRight.y) ||
               (enableBottom && position.y < bottomLeft.y) ||
               (enableLeft && position.x < bottomLeft.x) ||
               (enableRight && position.x > topRight.x);
    }
}
