using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acycl_GUI
{
  public class Vertex
  {
    public int Out;
    public int To;

    public override string ToString()
    {
      return $"Out: {Out} To: {To} ";
    }

    public Vertex(int Out, int To)
    {
      this.Out = Out; this.To = To;
    }
  }
}
