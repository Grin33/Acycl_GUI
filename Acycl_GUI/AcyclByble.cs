namespace Acycl_GUI
{
  public static class AcyclByble
  {
    static object locker = new object();
    static List<Vertex> Ans = new List<Vertex>();

    static bool Deep_Check(List<Vertex> ToCheck, int Out, List<int> visitedPoints)
    {
      var newvisited = new List<int>(visitedPoints);
      newvisited.Add(Out);
      var togo = new List<int>();
      foreach (var ver in ToCheck)
      {
        if (ver.Out == Out) { togo.Add(ver.To); }
      }
      //нашли все исходящие ребра из вершины

      if (togo.Count == 0) //если конечная
      {
        return true;
      }
      var tech = true;
      foreach (var ver in togo)
      {
        if (newvisited.Contains(ver))
        {
          return false; //если из вершины идет ребро в уже прошедшие вершины
        }

        var bolch = Deep_Check(ToCheck, ver, newvisited);
        if (!bolch)
        {
          tech = false;
          break;
        }
      }
      return tech;

    }
    static bool Check_Acycl(List<Vertex> ToCheck)
    {
      //вернуть true если нет цикла, false при наличии
      var visitedPoints = new List<int>();
      var bolch = true;
      for (int i = 0; i < ToCheck.Count; i++)
      {
        var visitedpoints = new List<int>();
        visitedpoints.Add(ToCheck[i].Out);
        var tech = Deep_Check(ToCheck, ToCheck[i].To, visitedpoints);
        if (!tech)
        {
          bolch = false; // найден цикл
          break;
        }
      }
      return bolch; // если не пришло вообще ни единой вершины
    }

    static void Check_Is_Ans(List<Vertex> ToCheck)
    {
      bool Acycl = Check_Acycl(ToCheck);
      if (Acycl)
      {
        if ((Ans.Count == 0) && (ToCheck.Count != 0))
        {
          Ans = new List<Vertex>(ToCheck);
        }
        else if (Ans.Count < ToCheck.Count)
        {
          Ans = new List<Vertex>(ToCheck);
        }
      }
    }

    static void Nest_Shuffle(ref List<Vertex> conPoint, List<Vertex> PrevPoint, int v)
    {
      Check_Is_Ans(PrevPoint);
      int n = v + 1;
      for (int i = n; i < conPoint.Count; i++)
      {
        var temppoints = new List<Vertex>(PrevPoint) { conPoint[i] };
        Nest_Shuffle(ref conPoint, temppoints, i);
      }
    }

    static void Check_Is_ParAns(List<Vertex> ToCheck, ref List<Vertex> localAns)
    {
      bool Acycl = Check_Acycl(ToCheck);
      if (Acycl)
      {
        if ((localAns.Count == 0) && (ToCheck.Count != 0))
        {
          localAns = new List<Vertex>(ToCheck);
        }
        else if (localAns.Count < ToCheck.Count)
        {
          localAns = new List<Vertex>(ToCheck);
        }
      }
    }
    static void Nest_ParShuffle(ref List<Vertex> conPoint, List<Vertex> PrevPoint, int v, ref List<Vertex> localans)
    {
      Check_Is_ParAns(PrevPoint, ref localans);
      int n = v + 1;
      for (int i = n; i < conPoint.Count; i++)
      {
        var temppoints = new List<Vertex>(PrevPoint) { conPoint[i] };
        Nest_ParShuffle(ref conPoint, temppoints, i, ref localans);
      }
    }
    static void Par_Shuffle(List<Vertex> points)
    {
      Parallel.For(0, points.Count, () => new List<Vertex>(), (i, loop, localans) =>
      {
        var tempoints = new List<Vertex>() { points[i] };
        Nest_ParShuffle(ref points, tempoints, i, ref localans);
        return localans;
      },
      (x) =>
      {
        lock (locker)
        {
          if (Ans.Count < x.Count)
          {
            Ans = new List<Vertex>(x);
          }
        }
      }
      );
    }
    public static List<GraphPoint> Start(List<GraphPoint> List)
    {
      var Points = new List<Vertex>();
      var Vertexes = Parse(List);

      Ans = new List<Vertex>();
      Par_Shuffle(Vertexes);

      return ParseToPoints(Ans);
    }

    private static List<GraphPoint> ParseToPoints(List<Vertex> ans)
    {
      var toRet = new List<GraphPoint>();
      foreach (var v in ans)
      {
        if (toRet.FirstOrDefault(d => d.Number == v.Out) == null)
        {
          toRet.Add(new GraphPoint(v.Out));
        }
        var p = toRet.FirstOrDefault(d => d.Number == v.Out);
        p.OrientedTo.Add(v.To);
        if (toRet.FirstOrDefault(d => d.Number == v.To) == null)
        {
          toRet.Add(new GraphPoint(v.To));
        }
      }
      return toRet;
    }

    private static List<Vertex> Parse(List<GraphPoint> points)
    {
      var toRet = new List<Vertex>();
      foreach (var p in points)
      {
        if (p.OrientedTo != null && p.OrientedTo.Count != 0)
        {
          foreach (var pc in p.OrientedTo)
          {
            var newVert = new Vertex(p.Number, pc);
            toRet.Add(newVert);
          }
        }
      }
      return toRet;
    }
  }
}
