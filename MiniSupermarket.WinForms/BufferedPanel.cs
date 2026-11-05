using System.Windows.Forms;
namespace MiniSupermarket.WinForms;

// TC37: double buffering chống nhấp nháy khi thay màn hình.
public sealed class BufferedPanel : Panel
{
    public BufferedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }
}
