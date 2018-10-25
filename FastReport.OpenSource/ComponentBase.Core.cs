using FastReport.Utils;

namespace FastReport
{
    partial class ComponentBase
    {
        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        /// <remarks>
        /// <para>This method is widely used in the FastReport. It is called each time when the object needs to draw 
        /// or print itself.</para>
        /// <para>In order to draw the object correctly, you should multiply the object's bounds by the <b>scale</b>
        /// parameter.</para>
        /// <para><b>cache</b> parameter is used to optimize the drawing speed. It holds all items such as
        /// pens, fonts, brushes, string formats that was used before. If the item with requested parameters
        /// exists in the cache, it will be returned (instead of create new item and then dispose it).</para>
        /// </remarks>
        public virtual void Draw(FRPaintEventArgs e)
        {

        }
    }
}
