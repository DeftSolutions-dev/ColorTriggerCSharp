using InputSimulatorStandard;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

class Program
{
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    static async Task Main()
    {
        Console.Title = "https://t.me/devilLucifer69 / DS: desirepro";
        Console.WriteLine("Введите значение порога (от 4 до 20): ");
        int threshold;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out threshold) && threshold >= 4 && threshold <= 20)
                break;
            else
                Console.WriteLine("Некорректное значение порога. Пожалуйста, введите число от 4 до 20.");
        }

        Console.WriteLine("Выберите бинд (X, F, LALT, Mouse3, Mouse4): ");
        string selectedBind = Console.ReadLine();

        Console.WriteLine("Good!");
        Process currentProcess = Process.GetCurrentProcess();
        currentProcess.PriorityClass = ProcessPriorityClass.High;

        while (true)
        {
            if (IsKeyPressed(ConsoleKey.UpArrow))
            {
                if (threshold < 20)
                    threshold++;
                Console.WriteLine($"Порог увеличен до {threshold}");
            }
            else if (IsKeyPressed(ConsoleKey.DownArrow))
            {
                if (threshold > 4)
                    threshold--;
                Console.WriteLine($"Порог уменьшен до {threshold}");
            }

            if (IsBindKeyPressed(selectedBind))
            {
                POINT p;
                GetCursorPos(out p);
                int x = p.X + 2;
                int y = p.Y + 2;
                Color color1 = await GetPixelColorAsync(x, y);

                GetCursorPos(out p);
                x = p.X + 2;
                y = p.Y + 2;
                Color color2 = await GetPixelColorAsync(x, y);

                if (Math.Abs(color1.R - color2.R) > threshold ||
                    Math.Abs(color1.G - color2.G) > threshold ||
                    Math.Abs(color1.B - color2.B) > threshold)
                {
                    Console.WriteLine(color2);
                    var simulator = new InputSimulator();
                    simulator.Mouse.LeftButtonDown();
                    await Task.Delay(1);
                    simulator.Mouse.LeftButtonUp();
                }
            } 
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    private static async Task<Color> GetPixelColorAsync(int x, int y)
    {
        return await Task.Run(() => GetPixelColor(x, y));
    }
    private static Color GetPixelColor(int x, int y)
    {
        using (Bitmap screen = new Bitmap(1, 1))
        {
            using (Graphics gdest = Graphics.FromImage(screen))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int colorRef = GetPixel(hSrcDC, x, y);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                    Color color = Color.FromArgb(0xFF, colorRef & 0xFF, (colorRef >> 8) & 0xFF, (colorRef >> 16) & 0xFF);
                    return color;
                }
            }
        }
    }
    [DllImport("gdi32.dll")]
    public static extern int GetPixel(IntPtr hDC, int x, int y);

    private static bool IsKeyPressed(ConsoleKey key)
    {
        return Console.KeyAvailable && Console.ReadKey(intercept: true).Key == key;
    }

    private static bool IsBindKeyPressed(string bind)
    {
        switch (bind)
        {
            case "X":
                return GetAsyncKeyState((int)'X') < 0;
            case "F":
                return GetAsyncKeyState((int)'F') < 0;
            case "LALT":
                return (GetAsyncKeyState((int)Keys.LMenu) & 0x8000) != 0;
            case "Mouse3":
                return GetAsyncKeyState((int)MouseButtons.Middle) < 0;
            case "Mouse4":
                return GetAsyncKeyState((int)MouseButtons.XButton1) < 0;
            default:
                return false;
        }
    }
}
