using UnityEngine;
using UnityEngine.UI;

public class LoadingProgress
{
    GameObject _root;

    GameObject _ui;

    Image _imgProgress;
    public LoadingProgress(GameObject root)
    {
        _root = root;
    }

    public void Show()
    {
        if (_ui == null)
        {
            var prefab = ResourceManager.LoadSync<GameObject>("LoadingBase/LoadingProgress").Asset;
            _ui = GameObjectUtil.InstantiatePrefab(prefab, _root);
            _imgProgress = GameObjectUtil.FindChildComponent(prefab, "Img_ProgressBar/Img_ProgressSlot/Progress", "Image") as Image;
        }
    }

    public void Hide()
    {
        if (_ui != null)
        {
            Object.Destroy(_ui);
            _ui = null;
            _imgProgress = null;
        }
    }

    public void SetProgress(float percent)
    {
        if (_imgProgress != null)
        {
            _imgProgress.fillAmount = percent;
        }
    }
}