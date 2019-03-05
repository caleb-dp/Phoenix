using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix;
using Phoenix.WorldData;

namespace CalExtension.UI.Status
{
  public interface ISupplyCounter : IDisposable
  {
    event EventHandler AmountChanged;
    int CurrentAmount { get; }
    void Recalc();
  }

  public class SupplyCounter : ISupplyCounter
  {
    private readonly UOObject container;
    private readonly ItemsCollection collection;
    //private readonly Graphic type;
    //private readonly UOColor color;
    public bool Track = false;

    private int amount;
    private readonly Dictionary<Serial, UOItem> tracked = new Dictionary<Serial, UOItem>();
    private readonly List<UOItemTypeBase> types;
    /// <summary>
    /// Initializes a new instance of the <see cref="T:SupplyCounter"/> class.
    /// </summary>
    public SupplyCounter(UOObject container, Graphic type, UOColor color) : this(container, new List<UOItemTypeBase>() {  new UOItemTypeBase(type, color) })
    {
    }

    public SupplyCounter(UOObject container, List<UOItemTypeBase> types)
    {
      this.types = new List<UOItemTypeBase>();
      if (!container.Serial.IsValid)
        throw new ArgumentException("container");

      this.container = container;
      //this.type = type;
      //this.color = color;
      this.types.AddRange(types.ToArray());

      if (container is UOItem)
      {
        collection = ((UOItem)container).AllItems;
      }
      else if (container is UOCharacter)
      {
        collection = ((UOCharacter)container).Layers;
        
        if (Track)
          Notepad.WriteLine("Layers " + ((UOCharacter)container).Layers.Count());
      }
      else {
        throw new ArgumentException("Invalid container type.");
      }

      container.Changed += new ObjectChangedEventHandler(container_Changed);
      // Init count
      Recalc();
    }

    #region Properties

    //public Graphic Type
    //{
    //  get { return type; }
    //}

    //public UOColor Color
    //{
    //  get { return color; }
    //}

    public UOObject Container
    {
      get { return container; }
    }

    public int CurrentAmount
    {
      get { return amount; }
    }

    #endregion

    public event EventHandler AmountChanged;

    protected virtual void OnAmountChanged(EventArgs e)
    {
      EventHandler h = AmountChanged;
      if (h != null)
        h(this, e);
    }

    public void Dispose()
    {
      container.Changed -= new ObjectChangedEventHandler(container_Changed);

      foreach (var item in tracked.Values)
      {
        item.Changed -= new ObjectChangedEventHandler(item_Changed);
      }

      tracked.Clear();
      amount = -1;
    }

    /// <summary>
    /// Prepocita to.
    /// </summary>
    public void Recalc()
    {
      lock (this)
      {
        // Reset
        foreach (var item in tracked.Values)
        {
          item.Changed -= new ObjectChangedEventHandler(item_Changed);
        }
        tracked.Clear();

        int a = 0;

        if (Track)
          Notepad.WriteLine("collection " + collection.Count());
        // Calculate
        foreach (var item in collection)
        {
          int foundTypes = this.types.Count(i => i.Graphic == item.Graphic && i.Color == item.Color);
          //if (item.Graphic != type || item.Color != color)

          if (foundTypes <= 0)
            continue;

          if (Track)
            Notepad.WriteLine("tracked add " + item.Description);
          
          tracked.Add(item.Serial, item);
          a += (item.Amount == 0 ? 1 : item.Amount);
        }

        amount = a;
      }

      // Fire event
      OnAmountChanged(EventArgs.Empty);
    }

    void container_Changed(object sender, ObjectChangedEventArgs e)
    {
      lock (this)
      {
        if (Track)
          Notepad.WriteLine("container_Changed " + e.Type + " / " + e.ItemSerial);

        switch (e.Type)
        {
          case ObjectChangeType.ItemOpened:
            // Reset
            foreach (var item in tracked.Values)
            {
              item.Changed -= new ObjectChangedEventHandler(item_Changed);
            }

            tracked.Clear();
            amount = 0;
            break;

          case ObjectChangeType.SubItemUpdated:
            // New item or update of existing
            {
              UOItem item = new UOItem(e.ItemSerial);
              int foundTypes = this.types.Count(i => i.Graphic == item.Graphic && i.Color == item.Color);
              //if (this.types.Count(i => i.Graphic == item.Graphic) == 0 ||  //item.Graphic != type || item.Color != color)

              if (foundTypes <= 0)
                return;

              // Make sure, that the item is in the list
              tracked[e.ItemSerial] = item;
              item.Changed += new ObjectChangedEventHandler(item_Changed);
              break;
            }

          case ObjectChangeType.SubItemRemoved:
            if (tracked.Remove(e.ItemSerial))
            {
              UOItem item = new UOItem(e.ItemSerial);
              item.Changed -= new ObjectChangedEventHandler(item_Changed);
            }
            else {
              // Item wasn't tracked, exit immediatly
              return;
            }
            break;

          default:
            return;
        }

        // Recount
        int a = 0;
        foreach (var item in tracked.Values.ToArray())
        {
          a += (item.Amount == 0 ? 1 : item.Amount);
        }

        amount = a;
      }

      // Fire event
      OnAmountChanged(EventArgs.Empty);
    }

    void item_Changed(object sender, ObjectChangedEventArgs e)
    {
      // Teoreticky se muze stat (a obcas stane), ze se item objevi jinde,
      // aniz by se pred tim odstranil. Tohle to osetri.
      if ((e.Type == ObjectChangeType.ItemUpdated && !collection.Contains(e.ItemSerial)))
      {
        bool fire = false;

        lock (this)
        {
          UOItem item = new UOItem(e.ItemSerial);
          item.Changed -= new ObjectChangedEventHandler(item_Changed);

          if (tracked.Remove(e.ItemSerial))
          {
            // Recount
            int a = 0;
            foreach (var i in tracked.Values.ToArray())
            {
              a += (i.Amount == 0 ? 1 : i.Amount);
            }

            amount = a;
            fire = true;
          }
        }

        // Fire event
        if (fire)
        {
          OnAmountChanged(EventArgs.Empty);
        }
      }
    }
  }
}
