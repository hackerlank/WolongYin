using UnityEngine;

public interface IUIEventListener
{
    void OnClick(GameObject go);
    void OnHover(GameObject go, bool state);
    void OnPress(GameObject go, bool press);
    void OnDragStart(GameObject go);
    void OnDrag(GameObject go, Vector2 delta);
    void OnDragEnd(GameObject go);
}