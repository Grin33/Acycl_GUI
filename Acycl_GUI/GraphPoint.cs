using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acycl_GUI
{
  public class GraphPoint
  {
    public int Number;
    public List<int> OrientedTo;

    public override string ToString()
    {
      string listtostr = "";
      foreach (int i in OrientedTo) { listtostr += i.ToString(); ; listtostr += ","; }

      return ($"Number: {Number} OrientedTo {listtostr}");
    }
    public GraphPoint(int Number, List<int> OrientedTo)
    {
      this.Number = Number;
      this.OrientedTo = OrientedTo;
    }
    public GraphPoint(int Number)
    {
      this.Number = Number;
      this.OrientedTo = new List<int>();
    }
    public GraphPoint(PointXAML point)
    {
      this.Number = point.Number;
      this.OrientedTo = point.OrientedTo.ToList();
    }
  }
}
