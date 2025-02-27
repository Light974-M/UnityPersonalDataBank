float CalculateMeshCenter(float vertexPosition, inout float accumulatedCenter, inout float vertexCount)
{
    // Accumuler la position des sommets
    accumulatedCenter += vertexPosition;

    // Compter les sommets
    vertexCount += 1.0;

    // Retourner le centre actuel (moyenne approximée)
    return accumulatedCenter / vertexCount;
}
