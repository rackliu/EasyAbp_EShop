using System;
using System.Threading.Tasks;
using Dapr;
using Dapr.Actors.Runtime;
using JetBrains.Annotations;

namespace EasyAbp.EShop.Plugins.Inventories.DaprActors;

public class InventoryActor : Actor, IInventoryActor
{
    public static string InventoryStateName { get; set; } = "i";

    protected bool FlashSalesInventoryUpdated { get; set; }

    [CanBeNull]
    protected InventoryStateModel State { get; set; }

    public InventoryActor(ActorHost host) : base(host)
    {
    }

    protected override async Task OnActivateAsync()
    {
        await StateManager.TryAddStateAsync(InventoryStateName, new InventoryStateModel());
    }

    public virtual async Task<InventoryStateModel> GetInventoryStateAsync() =>
        State ??= await StateManager.GetStateAsync<InventoryStateModel>(InventoryStateName);

    public virtual async Task IncreaseInventoryAsync(int quantity, bool decreaseSold, bool isFlashSale)
    {
        var state = await GetInventoryStateAsync();

        InternalIncreaseInventory(state, quantity, decreaseSold);

        if (!isFlashSale)
        {
            await SetInventoryStateAsync(state);
        }
        else
        {
            await TryStartIntervalPersistInventoryAsync();
        }
    }

    public virtual async Task ReduceInventoryAsync(int quantity, bool increaseSold, bool isFlashSale)
    {
        var state = await GetInventoryStateAsync();

        InternalReduceInventory(state, quantity, increaseSold);

        if (!isFlashSale)
        {
            await SetInventoryStateAsync(state);
        }
        else
        {
            await TryStartIntervalPersistInventoryAsync();
        }
    }

    protected virtual async Task TryStartIntervalPersistInventoryAsync()
    {
        if (FlashSalesInventoryUpdated)
        {
            return;
        }

        FlashSalesInventoryUpdated = true;

        await SetInventoryStateAsync(await GetInventoryStateAsync());

        await RegisterTimerAsync(null, nameof(TimerSetInventoryStateAsync), null, TimeSpan.FromSeconds(5),
            TimeSpan.FromMilliseconds(-1));
    }

    protected virtual async Task TimerSetInventoryStateAsync()
    {
        if (!FlashSalesInventoryUpdated)
        {
            return;
        }

        await SetInventoryStateAsync(await GetInventoryStateAsync());

        FlashSalesInventoryUpdated = false;
    }

    protected virtual async Task SetInventoryStateAsync(InventoryStateModel state) =>
        await StateManager.SetStateAsync(InventoryStateName, state);

    protected override async Task OnDeactivateAsync() => await SetInventoryStateAsync(await GetInventoryStateAsync());

    protected virtual void InternalIncreaseInventory(InventoryStateModel stateModel, int quantity, bool decreaseSold)
    {
        if (quantity < 0)
        {
            throw new DaprException("Quantity should not be less than 0.");
        }

        if (decreaseSold && stateModel.Sold - quantity < 0)
        {
            throw new DaprException("Target Sold cannot be less than 0.");
        }

        stateModel.Inventory = checked(stateModel.Inventory + quantity);

        if (decreaseSold)
        {
            stateModel.Sold -= quantity;
        }
    }

    protected virtual void InternalReduceInventory(InventoryStateModel stateModel, int quantity, bool increaseSold)
    {
        if (quantity < 0)
        {
            throw new DaprException("Quantity should not be less than 0.");
        }

        if (quantity > stateModel.Inventory)
        {
            throw new DaprException("Insufficient inventory.");
        }

        if (increaseSold)
        {
            stateModel.Sold = checked(stateModel.Sold + quantity);
        }

        stateModel.Inventory -= quantity;
    }
}