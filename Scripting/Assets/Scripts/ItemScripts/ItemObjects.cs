using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemObjects : MonoBehaviour
{
    [SerializeField] private ItemByTag[] _items;

    private ItemByTag _currentRightHandItem;
    private ItemByTag _currentLeftHandItem;

    private GameObject _currentRightHandWorldObject;
    private GameObject _currentLeftHandWorldObject;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentRightHandWorldObject = null;
        _currentLeftHandWorldObject = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (ItemByTag item in _items)
        {
            if (!other.CompareTag(item.Tag))
                continue;

            EquipItem(item, other.gameObject);
            return;
        }
    }

    private void EquipItem(ItemByTag newItem, GameObject newWorldObject)
    {
        ItemByTag currentItem = GetCurrentItem(newItem.Slot);
        GameObject currentWorldObject = GetCurrentWorldObject(newItem.Slot);

        if (currentItem != null)
        {
            if (currentItem.PlayerItemObject != null)
                currentItem.PlayerItemObject.SetActive(false);

            if (currentWorldObject != null)
                currentWorldObject.SetActive(true);
        }

        if (newWorldObject != null)
            newWorldObject.SetActive(false);

        if (newItem.PlayerItemObject != null)
            newItem.PlayerItemObject.SetActive(true);

        SetCurrentItem(newItem.Slot, newItem);
        SetCurrentWorldObject(newItem.Slot, newWorldObject);

        if (OnLoadObj.Instance != null)
        {
            if (newItem.Slot == ItemSlot.RightHand)
                OnLoadObj.Instance._rightHandItemTag = newItem.Tag;

            if (newItem.Slot == ItemSlot.LeftHand)
                OnLoadObj.Instance._leftHandItemTag = newItem.Tag;
        }
    }

    private ItemByTag GetCurrentItem(ItemSlot slot)
    {
        if (slot == ItemSlot.RightHand)
            return _currentRightHandItem;

        if (slot == ItemSlot.LeftHand)
            return _currentLeftHandItem;

        return null;
    }

    private void SetCurrentItem(ItemSlot slot, ItemByTag item)
    {
        if (slot == ItemSlot.RightHand)
            _currentRightHandItem = item;

        if (slot == ItemSlot.LeftHand)
            _currentLeftHandItem = item;
    }

    private GameObject GetCurrentWorldObject(ItemSlot slot)
    {
        if (slot == ItemSlot.RightHand)
            return _currentRightHandWorldObject;

        if (slot == ItemSlot.LeftHand)
            return _currentLeftHandWorldObject;

        return null;
    }

    private void SetCurrentWorldObject(ItemSlot slot, GameObject worldObject)
    {
        if (slot == ItemSlot.RightHand)
            _currentRightHandWorldObject = worldObject;

        if (slot == ItemSlot.LeftHand)
            _currentLeftHandWorldObject = worldObject;
    }
}
