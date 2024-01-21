using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Acycl_GUI.CanvasStruct;

namespace Acycl_GUI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    public int elemCount = 1;
    public bool isPoint = true;
    public bool haveOneRoad = false;
    public Grid PickedRoad;
    public List<GraphPoint> points = new List<GraphPoint>();
    public Brush Point = Brushes.Black;
    public Brush Line = Brushes.Black;
    public Brush Found = Brushes.Red;
    public int Radius = 50;
    public int LineWidth = 10;

    public MainWindow()
    {
      InitializeComponent();
    }

    private void Add_Height_Click(object sender, RoutedEventArgs e)
    {
      isPoint = true;
    }

    private void Add_Line_Click(object sender, RoutedEventArgs e)
    {
      isPoint = false;
    }

    private void PointsCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if(isPoint)
      {
        PlacePoint(sender, e);
      }
    }

    private void PlacePoint(object sender, MouseButtonEventArgs e)
    {
      var coord = e.GetPosition(sender as Canvas);
      var canvas = sender as Canvas;
      var grid = new Grid()
      {
        Width = Radius,
        Height = Radius,
      };
      var ell = new Ellipse()
      {
        Fill = Point,
        Stroke = Point,
      };
      var txtblock = new TextBlock()
      {
        Text = this.elemCount++.ToString(),
        Margin = new Thickness(20, 16, 0, 0),
        Foreground = new SolidColorBrush(Colors.White),
      };
      grid.Children.Add(ell);
      grid.Children.Add(txtblock);
      Canvas.SetLeft(grid, coord.X - Radius / 2);
      Canvas.SetTop(grid, coord.Y - Radius / 2);
      grid.MouseDown += PointMouseDown;
      canvas.Children.Add(grid);
    }

    private void PointMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!isPoint)
      {
        if (haveOneRoad)
        {
          var finishPoint = sender as Grid;
          var startX = Canvas.GetLeft(PickedRoad);
          var startY = Canvas.GetTop(PickedRoad);
          var finishX = Canvas.GetLeft(finishPoint);
          var finishY = Canvas.GetTop(finishPoint);

          var newArrow = CreateLineWithArrowPointCollection(new Point(startX + (Radius / 2), startY + (Radius / 2)),
            new Point(finishX + (Radius / 2),finishY + (Radius / 2)), LineWidth);

          var finishPointnumber = Int32.Parse((finishPoint.Children[1] as TextBlock).Text);
          var startPointnumber = Int32.Parse((PickedRoad.Children[1] as TextBlock).Text);

          var existedFinishPoint = this.points.Where(x => x.Number == finishPointnumber).FirstOrDefault();
          var existedStartPoint = this.points.Where(x => x.Number == startPointnumber).FirstOrDefault();


          if (existedFinishPoint != null)
          {
          }
          else
          {
            var newPoint = new GraphPoint(finishPointnumber);
            this.points.Add(newPoint);
          }
          if (existedStartPoint != null)
          {
            existedStartPoint.OrientedTo.Add(finishPointnumber);
          }
          else
          {
            var newPoint = new GraphPoint(startPointnumber);
            newPoint.OrientedTo.Add(finishPointnumber);
            this.points.Add(newPoint);
          }

          var polygon = new Polygon();
          polygon.Points = newArrow;
          polygon.Fill = Line;
          PointsCanvas.Children.Add(polygon);
          haveOneRoad = false;
          List<Grid> points = new List<Grid>();
          for (int i = 0; i < PointsCanvas.Children.Count; i++)
          {
            if (PointsCanvas.Children[i] is Grid)
            {
              points.Add(PointsCanvas.Children[i] as Grid);
            }
          }
          foreach (var item in points)
          {
            PointsCanvas.Children.Remove(item);
            PointsCanvas.Children.Add(item);
          }
        }
        else
        {
          PickedRoad = sender as Grid;
          haveOneRoad = true;
        }
      }
    }

    private const double _maxArrowLengthPercent = 1; // factor that determines how the arrow is shortened for very short lines
    private const double _lineArrowLengthFactor = 5; // 15 degrees arrow:  = 1 / Math.Tan(15 * Math.PI / 180); 
    public static PointCollection CreateLineWithArrowPointCollection(Point startPoint, Point endPoint, double lineWidth)
    {
      Vector direction = endPoint - startPoint;

      Vector normalizedDirection = direction;
      normalizedDirection.Normalize();

      Vector normalizedlineWidenVector = new Vector(-normalizedDirection.Y, normalizedDirection.X); // Rotate by 90 degrees
      Vector lineWidenVector = normalizedlineWidenVector * lineWidth * 0.5;

      double lineLength = direction.Length;

      double defaultArrowLength = lineWidth * _lineArrowLengthFactor;

      // Prepare usedArrowLength
      // if the length is bigger than 1/3 (_maxArrowLengthPercent) of the line length adjust the arrow length to 1/3 of line length

      double usedArrowLength;
      if (lineLength * _maxArrowLengthPercent < defaultArrowLength)
        usedArrowLength = lineLength * _maxArrowLengthPercent;
      else
        usedArrowLength = defaultArrowLength;

      // Adjust arrow thickness for very thick lines
      double arrowWidthFactor;
      if (lineWidth <= 1.5)
        arrowWidthFactor = 3;
      else if (lineWidth <= 2.66)
        arrowWidthFactor = 4;
      else
        arrowWidthFactor = 1.5 * lineWidth;

      Vector arrowWidthVector = normalizedlineWidenVector * arrowWidthFactor;


      // Now we have all the vectors so we can create the arrow shape positions
      var pointCollection = new PointCollection(7);

      Point endArrowCenterPosition = endPoint - (normalizedDirection * usedArrowLength);

      pointCollection.Add(endPoint); // Start with tip of the arrow
      pointCollection.Add(endArrowCenterPosition + arrowWidthVector);
      pointCollection.Add(endArrowCenterPosition + lineWidenVector);
      pointCollection.Add(startPoint + lineWidenVector);
      pointCollection.Add(startPoint - lineWidenVector);
      pointCollection.Add(endArrowCenterPosition - lineWidenVector);
      pointCollection.Add(endArrowCenterPosition - arrowWidthVector);

      return pointCollection;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      PointsCanvas.Children.Clear();
      elemCount = 1;
      this.points.Clear();
    }

    private async void Start_Click(object sender, RoutedEventArgs e)
    {
      LoadingRing.Visibility = Visibility.Visible;
      var ans = await Task.Run(() => AcyclByble.Start(this.points));
      var i = PointsCanvas;
      var toRemove = new List<Polygon>();
      foreach (var p in PointsCanvas.Children)
      {
        if(p is Polygon)
        {
          toRemove.Add(p as Polygon);
        }
      }
      foreach(var poly in toRemove)
        PointsCanvas.Children.Remove(poly);

      var ToAdd = new List<Polygon>();
      foreach(var p in ans)
      {
        foreach(var item in PointsCanvas.Children)
        {
          if(item is Grid)
          {
            var numSTR = ((item as Grid).Children[1] as TextBlock).Text;
            var num = Int32.Parse(numSTR);
            if(num == p.Number)
            {
              PlaceArrows(p, item as Grid, ToAdd);
            }
          }
        }
      }
      List<Grid> points = new List<Grid>();
      for (int iter = 0; iter < PointsCanvas.Children.Count; iter++)
      {
        if (PointsCanvas.Children[iter] is Grid)
        {
          points.Add(PointsCanvas.Children[iter] as Grid);
        }
      }
      foreach (var arr in ToAdd)
        PointsCanvas.Children.Add(arr);
      foreach (var item in points)
      {
        PointsCanvas.Children.Remove(item);
        PointsCanvas.Children.Add(item);
      }
      this.points = ans;
      LoadingRing.Visibility = Visibility.Hidden;
    }

    private void PlaceArrows(GraphPoint p, Grid startOut, List<Polygon> toAdd)
    {
      foreach(var con in p.OrientedTo)
      {
        foreach(var item in PointsCanvas.Children)
        {
          if(item is Grid)
          {
            var numSTR = ((item as Grid).Children[1] as TextBlock).Text;
            var num = Int32.Parse(numSTR);
            if (num == con)
            {
              var startPoint = new Point(Canvas.GetLeft(startOut as Grid) + (Radius / 2), Canvas.GetTop(startOut as Grid) + (Radius / 2));
              var finishPoint = new Point(Canvas.GetLeft(item as Grid) + (Radius / 2), Canvas.GetTop(item as Grid) + (Radius / 2));
              var newArrow = CreateLineWithArrowPointCollection(startPoint, finishPoint, LineWidth); 
              var polygon = new Polygon();
              polygon.Points = newArrow;
              polygon.Fill = Line;
              toAdd.Add(polygon);
            }
          }
        }
      }
    }

    private void LoadBTN_Click(object sender, RoutedEventArgs e)
    {
      elemCount = 1;
      var fd = new OpenFileDialog();
      fd.Filter = "*.xaml|*.xaml";
      if (fd.ShowDialog() == false)
      {
        MessageBox.Show("Выбран неверный файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      var file = fd.FileName;
      FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
      var savedCanvas = XamlReader.Load(fs) as CanvasStruct;
      fs.Close();
      PointsCanvas.Children.Clear();
      var ch = new List<UIElement>();
      foreach (UIElement child in savedCanvas.Canvas.Children)
      {
        ch.Add(child);
      };
      savedCanvas.Canvas.Children.Clear();
      foreach (var child in ch)
      {
        if(child is Grid)
        {
          child.MouseDown += PointMouseDown;
        }
        PointsCanvas.Children.Add(child);
      }
      foreach (var p in savedCanvas.Points)
      {
        elemCount++;
        this.points.Add(new GraphPoint(p));
      }
    }

    private void SaveBTN_Click(object sender, RoutedEventArgs e)
    {
      var fileName = string.Empty;
      var sd = new SaveFileDialog();
      sd.DefaultExt = "xaml";
      if (sd.ShowDialog() == true)
      {
        fileName = sd.FileName;
      }
      if (fileName == string.Empty)
        return;
      FileStream fs = File.Open(fileName, FileMode.Create);

      var ps = new ListOfPoint();
      foreach (var p in this.points)
      {
        var cn = p.OrientedTo.ToArray();
        var np = new PointXAML()
        {
          Number = p.Number,
          OrientedTo = cn,
        };
        ps.Add(np);
      }
      var CanvasStruct = new CanvasStruct()
      {
        Points = ps,
        Canvas = PointsCanvas
      };

      XamlWriter.Save(CanvasStruct, fs);
      fs.Close();
    }

    private void SettingsBTN_Click(object sender, RoutedEventArgs e)
    {
      var newWindow = new SettingsWindow(Radius);
      newWindow.Show();
      newWindow.Closed += NewWindow_Closed;
    }

    private void NewWindow_Closed(object? sender, EventArgs e)
    {
      var window = sender as SettingsWindow;
      Radius = window.Radius;
    }

    private void HelpButtn_Click(object sender, RoutedEventArgs e)
    {

      MessageBox.Show("Программа для  \nАвтор:  \n\nИнструкция: " +
        "\n\n\t Кнопка \"Добавить вершину\" переключает режим действия. Теперь, при нажатии на полотно, вы будете проставлять вершину графа" +
        "\n\n\t Кнопка \"Добавить ребро\" переключает режим действия. Теперь, при последовательном нажатии на две вершины графа, вы будете строить между ними ребра" +
        "\n\n\t Кнопка \"Найти подграф\" рассчитывает и выводит на экран ");
    }
  }
}