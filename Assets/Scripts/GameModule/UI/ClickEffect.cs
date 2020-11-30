using UnityEngine;
using UnityEngine.EventSystems;


public class ClickEffect : MonoBehaviour
{
    Vector3 _point;
    Camera _camera;
    AssetRef<GameObject> _effect;

    public void Initialize()
    {
        _camera = BlUIManager.UICamera;
        _effect = ResourceManager.LoadSync<GameObject>("ArtResources/UI/Prefabs/Eff_Ui_Click");
        EffectManager.Prewarm(_effect, 5);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() && _camera != null && _effect != null)
            {
                _point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
                _point = _camera.ScreenToWorldPoint(_point);
                EffectManager.Alloc(_effect, _point, 0.5f);
            }
        }
    }
    void OnDestroy()
    {
        EffectManager.Clear(_effect);
        ResourceManager.Unload(_effect);    
    }
}
