using Phoenix;

namespace CalExtension
{
  public interface IUOItemType
  {
    Graphic Graphic { get; set; }
    UOColor Color { get; set; }
    string Name { get; set; }
  }
}
