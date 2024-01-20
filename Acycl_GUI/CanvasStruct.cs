using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Acycl_GUI
{

  public class CanvasStruct
  {
    public Canvas Canvas { get; set; }
    public ListOfPoint Points { get; set; }
  }

  public class ListOfPoint : List<PointXAML>
  {

  }

  public class ListOfInt : List<int>
  {

  }

  public class PointXAML
  {
    public int Number { get; set; }
    public int[] OrientedTo { get; set; }
  }

}
