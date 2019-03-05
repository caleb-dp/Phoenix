using Phoenix.WorldData;
using Phoenix;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class BishopHat
  {
    //------------------------------------------------------------------------------------------

    public static readonly Graphic BishopGraphic = 0x1DB9;
    public static readonly UOColor BishopColor = 0x0BB0;

    //------------------------------------------------------------------------------------------

    private UOItem lastHeadArmor;
    private ushort? lastHatX;
    private ushort? lastHatY;
    private Serial lastHatContainer = Serial.Invalid;

    //------------------------------------------------------------------------------------------

    [Executable]
    public void SwitchBishopHat()
    {
      UOItem bishopHat = World.Player.FindType(BishopGraphic, BishopColor);

      if (bishopHat.ExistCust())
      {
        if (bishopHat.Layer == Layer.Hat)
        {
          UOItem lastContainer = new UOItem(lastHatContainer);
          if (!lastContainer.ExistCust())
            lastContainer = World.Player.Backpack;

          if (lastHeadArmor != null && lastHeadArmor.ExistCust())
            lastHeadArmor.Use();
          else
            bishopHat.Move(1, lastContainer, lastHatX.GetValueOrDefault(0), lastHatY.GetValueOrDefault(0));

          World.Player.Print(0x005d, "<Bishop sundan>");
        }
        else
        {
          lastHeadArmor = World.Player.Layers[Layer.Hat];
          lastHatContainer = bishopHat.Container;
          lastHatX = bishopHat.X;
          lastHatY = bishopHat.Y;
          bishopHat.Use();

          Game.Wait(100);

          World.Player.Print(0x0044, "<Bishop nasazen " + World.Player.Hits + ">");
        }
      }
      else
        World.Player.PrintMessage("[Nemas Bishop hat!]", MessageType.Error);
    }

    //------------------------------------------------------------------------------------------
  }
}