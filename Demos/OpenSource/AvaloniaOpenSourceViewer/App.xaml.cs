using Avalonia;
using Avalonia.Markup.Xaml;

namespace Viewer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}