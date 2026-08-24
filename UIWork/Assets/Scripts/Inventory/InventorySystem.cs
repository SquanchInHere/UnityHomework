using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InventorySlotArea
{
    Inventory,
    Crafting,
    Result
}

public class InventorySystem : MonoBehaviour, IResourceReceiver
{
    private const int InventorySlotCount = 27;
    private const int CraftingSlotCount = 4;

    private readonly List<InventorySlotData> _inventorySlots = new();
    private readonly List<InventorySlotData> _craftingSlots = new();
    private readonly List<RecipeDefinition> _recipes = new();

    private readonly InventorySlotData _cursorSlot = new();

    public IReadOnlyList<InventorySlotData> InventorySlots => _inventorySlots;
    public IReadOnlyList<InventorySlotData> CraftingSlots => _craftingSlots;
    public InventorySlotData CursorSlot => _cursorSlot;

    public void Initialize(IEnumerable<RecipeDefinition> recipes)
    {
        _inventorySlots.Clear();
        _craftingSlots.Clear();
        _recipes.Clear();

        for (int i = 0; i < InventorySlotCount; i++)
        {
            _inventorySlots.Add(new InventorySlotData());
        }

        for (int i = 0; i < CraftingSlotCount; i++)
        {
            _craftingSlots.Add(new InventorySlotData());
        }

        _recipes.AddRange(recipes);
    }

    public InventorySlotData GetSlot(InventorySlotArea area, int index)
    {
        return area switch
        {
            InventorySlotArea.Inventory => _inventorySlots[index],
            InventorySlotArea.Crafting => _craftingSlots[index],
            _ => null
        };
    }

    public InventorySlotData GetCraftingResult()
    {
        RecipeDefinition recipe = FindMatchingRecipe();

        if (recipe == null)
        {
            return new InventorySlotData();
        }

        InventorySlotData result = new InventorySlotData();
        result.Set(recipe.Result, recipe.ResultAmount);
        return result;
    }

    public void HandleSlotClick(InventorySlotArea area, int index, PointerEventData.InputButton button, bool exactSplit)
    {
        if (area == InventorySlotArea.Result)
        {
            if (button == PointerEventData.InputButton.Left)
            {
                TryCraft();
            }

            return;
        }

        InventorySlotData slot = GetSlot(area, index);

        if (slot == null)
        {
            return;
        }

        if (exactSplit && button == PointerEventData.InputButton.Right)
        {
            return;
        }

        if (button == PointerEventData.InputButton.Left)
        {
            HandleLeftClick(slot);
            return;
        }

        if (button == PointerEventData.InputButton.Right)
        {
            HandleRightClick(slot);
        }
    }

    public bool TakeExactFromSlot(InventorySlotArea area, int index, int amount)
    {
        if (!_cursorSlot.IsEmpty)
        {
            return false;
        }

        InventorySlotData source = GetSlot(area, index);

        if (source == null || source.IsEmpty)
        {
            return false;
        }

        if (amount <= 0 || amount > source.Amount)
        {
            return false;
        }

        _cursorSlot.Set(source.Item, amount);
        source.Remove(amount);

        return true;
    }

    // Используется подсветкой: проверяет перенос, но пока не меняет данные.
    public bool CanTransferSlot(InventorySlotArea sourceArea, int sourceIndex, InventorySlotArea targetArea, int targetIndex)
    {

        if (sourceArea == InventorySlotArea.Result || targetArea == InventorySlotArea.Result)
        {
            return false;
        }

        if (sourceArea == targetArea && sourceIndex == targetIndex)
        {
            return false;
        }

        InventorySlotData source = GetSlot(sourceArea, sourceIndex);
        InventorySlotData target = GetSlot(targetArea, targetIndex);

        if (source == null || target == null || source.IsEmpty)
        {
            return false;
        }

        if (target.IsEmpty || target.Item != source.Item)
        {
            return true;
        }

        return target.Amount < target.Item.MaxStackSize;
    }

    // Реальный перенос: перемещение, объединение одинаковых стаков или обмен разных.
    public bool TransferSlot(InventorySlotArea sourceArea, int sourceIndex, InventorySlotArea targetArea, int targetIndex)
    {
        if (!CanTransferSlot(sourceArea, sourceIndex, targetArea, targetIndex))
        {
            return false;
        }

        InventorySlotData source = GetSlot(sourceArea, sourceIndex);
        InventorySlotData target = GetSlot(targetArea, targetIndex);

        if (target.IsEmpty)
        {
            // Цель пустая — переносим весь стак.
            target.Set(source.Item, source.Amount);
            source.Clear();
            return true;
        }

        if (target.Item == source.Item)
        {
            // Одинаковые предметы — заполняем свободное место целевого стака.
            int availableSpace = target.Item.MaxStackSize - target.Amount;

            if (availableSpace <= 0)
            {
                return false;
            }

            int transferAmount = Mathf.Min(source.Amount, availableSpace);

            target.Add(transferAmount);
            source.Remove(transferAmount);

            return true;
        }

        // Разные предметы — меняем слоты местами.
        ItemDefinition sourceItem = source.Item;
        int sourceAmount = source.Amount;

        source.Set(target.Item, target.Amount);
        target.Set(sourceItem, sourceAmount);

        return true;
    }

    public bool TryAddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int remaining = amount;

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            InventorySlotData slot = _inventorySlots[i];

            if (slot.IsEmpty || slot.Item != item)
            {
                continue;
            }

            int availableSpace = item.MaxStackSize - slot.Amount;
            int addAmount = Mathf.Min(remaining, availableSpace);

            slot.Add(addAmount);
            remaining -= addAmount;

            if (remaining <= 0)
            {
                return true;
            }
        }

        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            InventorySlotData slot = _inventorySlots[i];

            if (!slot.IsEmpty)
            {
                continue;
            }

            int addAmount = Mathf.Min(remaining, item.MaxStackSize);
            slot.Set(item, addAmount);
            remaining -= addAmount;

            if (remaining <= 0)
            {
                return true;
            }
        }

        return remaining <= 0;
    }

    public bool CanAddItem(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return false;
        }

        int availableSpace = 0;

        foreach (InventorySlotData slot in _inventorySlots)
        {
            if (slot.IsEmpty)
            {
                availableSpace += item.MaxStackSize;
            }
            else if (slot.Item == item)
            {
                availableSpace += item.MaxStackSize - slot.Amount;
            }

            if (availableSpace >= amount)
            {
                return true;
            }
        }

        return false;
    }

    public void AddStartingItem(ItemDefinition item, int amount)
    {
        TryAddItem(item, amount);
    }

    public int Add(ItemDefinition item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return 0;
        }

        ItemDefinition inventoryItem = ResolveInventoryItem(item);
        int amountBefore = GetItemAmount(inventoryItem);

        TryAddItem(inventoryItem, amount);

        return GetItemAmount(inventoryItem) - amountBefore;
    }

    private void HandleLeftClick(InventorySlotData slot)
    {
        if (_cursorSlot.IsEmpty)
        {
            if (!slot.IsEmpty)
            {
                _cursorSlot.Set(slot.Item, slot.Amount);
                slot.Clear();
            }

            return;
        }

        if (slot.IsEmpty)
        {
            int amount = Mathf.Min(_cursorSlot.Amount, _cursorSlot.Item.MaxStackSize);
            slot.Set(_cursorSlot.Item, amount);
            _cursorSlot.Remove(amount);
            return;
        }

        if (slot.Item == _cursorSlot.Item)
        {
            int availableSpace = slot.Item.MaxStackSize - slot.Amount;

            if (availableSpace <= 0)
            {
                return;
            }

            int amount = Mathf.Min(_cursorSlot.Amount, availableSpace);

            slot.Add(amount);
            _cursorSlot.Remove(amount);

            return;
        }

        ItemDefinition cursorItem = _cursorSlot.Item;
        int cursorAmount = _cursorSlot.Amount;

        _cursorSlot.Set(slot.Item, slot.Amount);
        slot.Set(cursorItem, cursorAmount);
    }

    private void HandleRightClick(InventorySlotData slot)
    {
        if (_cursorSlot.IsEmpty)
        {
            if (slot.IsEmpty)
            {
                return;
            }

            int amount = Mathf.CeilToInt(slot.Amount / 2f);

            _cursorSlot.Set(slot.Item, amount);
            slot.Remove(amount);

            return;
        }

        if (slot.IsEmpty)
        {
            slot.Set(_cursorSlot.Item, 1);
            _cursorSlot.Remove(1);
            return;
        }

        if (slot.Item != _cursorSlot.Item)
        {
            return;
        }

        if (slot.Amount >= slot.Item.MaxStackSize)
        {
            return;
        }

        slot.Add(1);
        _cursorSlot.Remove(1);
    }

    private void TryCraft()
    {
        RecipeDefinition recipe = FindMatchingRecipe();

        if (recipe == null)
        {
            return;
        }

        if (!_cursorSlot.IsEmpty)
        {
            if (_cursorSlot.Item != recipe.Result)
            {
                return;
            }

            if (_cursorSlot.Amount + recipe.ResultAmount > recipe.Result.MaxStackSize)
            {
                return;
            }
        }

        ConsumeRecipe(recipe);

        if (_cursorSlot.IsEmpty)
        {
            _cursorSlot.Set(recipe.Result, recipe.ResultAmount);
        }
        else
        {
            _cursorSlot.Add(recipe.ResultAmount);
        }
    }

    private RecipeDefinition FindMatchingRecipe()
    {
        foreach (RecipeDefinition recipe in _recipes)
        {
            if (MatchesRecipe(recipe))
            {
                return recipe;
            }
        }

        return null;
    }

    private bool MatchesRecipe(RecipeDefinition recipe)
    {
        Dictionary<ItemDefinition, int> currentItems = new();

        foreach (InventorySlotData slot in _craftingSlots)
        {
            if (slot.IsEmpty)
            {
                continue;
            }

            if (!currentItems.ContainsKey(slot.Item))
            {
                currentItems.Add(slot.Item, 0);
            }

            currentItems[slot.Item] += slot.Amount;
        }

        Dictionary<ItemDefinition, int> requiredItems = new();

        foreach (RecipeIngredient ingredient in recipe.Ingredients)
        {
            if (!requiredItems.ContainsKey(ingredient.Item))
            {
                requiredItems.Add(ingredient.Item, 0);
            }

            requiredItems[ingredient.Item] += ingredient.Amount;
        }

        if (currentItems.Count != requiredItems.Count)
        {
            return false;
        }

        foreach (KeyValuePair<ItemDefinition, int> required in requiredItems)
        {
            if (!currentItems.TryGetValue(required.Key, out int currentAmount))
            {
                return false;
            }

            if (currentAmount != required.Value)
            {
                return false;
            }
        }

        return true;
    }

    private void ConsumeRecipe(RecipeDefinition recipe)
    {
        foreach (RecipeIngredient ingredient in recipe.Ingredients)
        {
            int remaining = ingredient.Amount;

            foreach (InventorySlotData slot in _craftingSlots)
            {
                if (slot.IsEmpty || slot.Item != ingredient.Item)
                {
                    continue;
                }

                int removeAmount = Mathf.Min(remaining, slot.Amount);
                slot.Remove(removeAmount);
                remaining -= removeAmount;

                if (remaining <= 0)
                {
                    break;
                }
            }
        }
    }

    private ItemDefinition ResolveInventoryItem(ItemDefinition item)
    {
        if (string.IsNullOrEmpty(item.ItemId))
        {
            return item;
        }

        foreach (InventorySlotData slot in _inventorySlots)
        {
            if (!slot.IsEmpty && slot.Item.ItemId == item.ItemId)
            {
                return slot.Item;
            }
        }

        foreach (RecipeDefinition recipe in _recipes)
        {
            if (recipe.Result != null && recipe.Result.ItemId == item.ItemId)
            {
                return recipe.Result;
            }

            foreach (RecipeIngredient ingredient in recipe.Ingredients)
            {
                if (ingredient.Item != null && ingredient.Item.ItemId == item.ItemId)
                {
                    return ingredient.Item;
                }
            }
        }

        return item;
    }

    private int GetItemAmount(ItemDefinition item)
    {
        int amount = 0;

        foreach (InventorySlotData slot in _inventorySlots)
        {
            if (!slot.IsEmpty && slot.Item == item)
            {
                amount += slot.Amount;
            }
        }

        return amount;
    }
}
