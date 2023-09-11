using FastReport.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.String;

namespace FastReport.Web.Toolbar
{
    public abstract class ToolbarElement
    {
        public ToolbarElement()
        {
            Name = ID.ToString();
        }

        internal Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Name of button required to interact with the element list
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines the visibility of the button on the toolbar
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Element styles to be specified in the style tag
        /// </summary>
        public string ElementCustomStyle { get; set; } = Empty;

        /// <summary>
        /// Classes to be specified in the class tag for the element
        /// </summary>
        public string ElementClasses { get; set; } = Empty;

        /// <summary>
        /// Text to be displayed when the element is hovered over
        /// </summary>
        public string Title { get; set; } = Empty;

        /// <summary>
        /// Property by which the buttons in the toolbar will be sorted
        /// </summary>
        public int Position { get; set; } = -1;

        internal abstract string Render(string template_FR);
    }


    /// <summary>
    /// Action that is triggered when the element is clicked
    /// </summary>
    public interface IClickAction
    {
    }

    /// <summary>
    /// Action that is triggered when the element is changed
    /// </summary>
    public interface IChangeAction
    {
    }

    /// <summary>
    /// A script that is called when you click on element
    /// </summary>
    public class ElementScript : IClickAction, IChangeAction
    {
        /// <summary>
        /// A script that is called when you click on element
        /// </summary>
        public string Script { get; set; }
    }

    /// <summary>
    /// Action that is called when you click on an item
    /// </summary>
    public class ElementClickAction : IClickAction
    {
        /// <summary>
        /// Action that is called when you click on an item. WebReport - WebReport for which the button is made
        /// </summary>
        public Func<WebReport, Task> OnClickAction { get; set; }
    }

    /// <summary>
    /// Action that is called when item changed
    /// </summary>
    public class ElementChangeAction : IChangeAction
    {
        /// <summary>
        /// Action that is called when item changed. WebReport - WebReport for which the button is mad
        /// </summary>
        public Func<WebReport, string, Task> OnChangeAction { get; set; }
    }

    /// <summary>
    /// The image that will be displayed in the toolbar
    /// </summary>
    public class ToolbarElementImage
    {
        private string defaultButtonImage;

        private string base64;

        internal string RenderedImage => IsNullOrEmpty(Url) ? $"data:image/svg+xml;base64,{Base64}" : Url;


        /// <summary>
        /// Toolbar element image in Base64 format
        /// </summary>
        public string Base64
        {
            get
            {
                if (!IsNullOrWhiteSpace(base64))
                    return base64;

                if (IsNullOrWhiteSpace(defaultButtonImage))
                    LoadDefaultImage();

                return defaultButtonImage;
            }

            set => base64 = value;
        }

        /// <summary>
        /// Link to the image of the toolbar element
        /// </summary>
        public string Url { get; set; }

        private void LoadDefaultImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream =
                assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.default-custom-button.svg");
            string resource;

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                resource = reader.ReadToEnd();
            }

            var svgBytes = System.Text.Encoding.UTF8.GetBytes(resource);
            var base64String = Convert.ToBase64String(svgBytes);

            defaultButtonImage = base64String;
        }
    }
}