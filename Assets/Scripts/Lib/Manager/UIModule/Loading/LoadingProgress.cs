using UnityEngine;
using UnityEngine.UI;

public class LoadingProgress
{
    GameObject _root;

    GameObject _ui;

    Image _imgProgress;

    Text _text;
    public LoadingProgress(GameObject root)
    {
        _root = root;
    }

    public void Show(string txt = null)
    {
        if (_ui == null)
        {
            var prefab = ResourceManager.LoadSync<GameObject>("ArtResources/UI/Effect/LoadingProgress/LoadingProgress").Asset;
            _ui = GameObjectUtil.InstantiatePrefab(prefab, _root);
            _imgProgress = GameObjectUtil.FindChildComponent(prefab, "Img_ProgressBar/Img_ProgressSlot/Progress", "Image") as Image;
            _text = GameObjectUtil.FindChildComponent(prefab, "Text", "Text") as Text;
        }
        if (txt != null)
        {
            _text.text = txt;
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