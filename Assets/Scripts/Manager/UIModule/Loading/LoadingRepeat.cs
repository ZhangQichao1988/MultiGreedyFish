using UnityEngine;

public class LoadingRepeat
{
    GameObject _root;
    int _referenceCount;

    GameObject _ui;
    public LoadingRepeat(GameObject root)
    {
        _root = root;
        
	    _referenceCount = 0;
    }

    public void Show()
    {
        _referenceCount++;
        if (_referenceCount == 1)
        {
            if (_ui == null)
            {
                var prefab = ResourceManager.LoadSync<GameObject>("LoadingRepeat/LoadingRepeat");
                _ui = GameObjectUtil.InstantiatePrefab(prefab, _root);
            }
        }
    }

    public void Hide()
    {
        _referenceCount--;
        if (_referenceCount < 0)
        {
             _referenceCount = 0;
        }

        if (_referenceCount == 0)
        {
            if (_ui != null)
            {
                Object.Destroy(_ui);
                _ui = null;
            }
        }
    }
}
