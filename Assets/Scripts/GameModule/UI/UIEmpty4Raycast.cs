using UnityEngine.UI;

public class UIEmpty4Raycast : MaskableGraphic
{
    protected UIEmpty4Raycast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}
