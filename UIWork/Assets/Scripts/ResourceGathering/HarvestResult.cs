using UnityEngine;

public readonly struct HarvestResult
{
    public ItemDefinition Item { get; }
    public int Amount { get; }
    public int RemainingAmount { get; }
    public bool IsDepleted { get; }

    public HarvestResult(ItemDefinition item, int amount, int remainingAmount, bool isDepleted)
    {
        Item = item;
        Amount = amount;
        RemainingAmount = remainingAmount;
        IsDepleted = isDepleted;
    }
}
