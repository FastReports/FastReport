using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport
{
    /// <summary>
    /// Container object that may contain child objects.
    /// </summary>
    public partial class ContainerObject : ReportComponentBase, IParent
    {
        #region Fields
        private ReportComponentCollection objects;
        private bool updatingLayout;
        private string beforeLayoutEvent;
        private string afterLayoutEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of child objects.
        /// </summary>
        [Browsable(false)]
        public ReportComponentCollection Objects
        {
            get { return objects; }
        }

        /// <summary>
        /// This event occurs before the container layouts its child objects.
        /// </summary>
        public event EventHandler BeforeLayout;

        /// <summary>
        /// This event occurs after the child objects layout was finished.
        /// </summary>
        public event EventHandler AfterLayout;


        /// <summary>
        /// Gets or sets a script event name that will be fired before the container layouts its child objects.
        /// </summary>
        [Category("Build")]
        public string BeforeLayoutEvent
        {
            get { return beforeLayoutEvent; }
            set { beforeLayoutEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired after the child objects layout was finished.
        /// </summary>
        [Category("Build")]
        public string AfterLayoutEvent
        {
            get { return afterLayoutEvent; }
            set { afterLayoutEvent = value; }
        }

        #endregion

        #region IParent
        /// <inheritdoc/>
        public virtual void GetChildObjects(ObjectCollection list)
        {
            foreach (ReportComponentBase c in objects)
            {
                list.Add(c);
            }
        }

        /// <inheritdoc/>
        public virtual bool CanContain(Base child)
        {
            return (child is ReportComponentBase);
        }

        /// <inheritdoc/>
        public virtual void AddChild(Base child)
        {
            if (child is ReportComponentBase)
                objects.Add(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public virtual void RemoveChild(Base child)
        {
            if (child is ReportComponentBase)
                objects.Remove(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public virtual int GetChildOrder(Base child)
        {
            return objects.IndexOf(child as ReportComponentBase);
        }

        /// <inheritdoc/>
        public virtual void SetChildOrder(Base child, int order)
        {
            int oldOrder = child.ZOrder;
            if (oldOrder != -1 && order != -1 && oldOrder != order)
            {
                if (order > objects.Count)
                    order = objects.Count;
                if (oldOrder <= order)
                    order--;
                objects.Remove(child as ReportComponentBase);
                objects.Insert(order, child as ReportComponentBase);
            }
        }

        /// <inheritdoc/>
        public virtual void UpdateLayout(float dx, float dy)
        {
            if (updatingLayout)
                return;
            updatingLayout = true;
            try
            {
                RectangleF remainingBounds = new RectangleF(0, 0, Width, Height);
                remainingBounds.Width += dx;
                remainingBounds.Height += dy;
                foreach (ReportComponentBase c in Objects)
                {
                    if ((c.Anchor & AnchorStyles.Right) != 0)
                    {
                        if ((c.Anchor & AnchorStyles.Left) != 0)
                            c.Width += dx;
                        else
                            c.Left += dx;
                    }
                    else if ((c.Anchor & AnchorStyles.Left) == 0)
                    {
                        c.Left += dx / 2;
                    }
                    if ((c.Anchor & AnchorStyles.Bottom) != 0)
                    {
                        if ((c.Anchor & AnchorStyles.Top) != 0)
                            c.Height += dy;
                        else
                            c.Top += dy;
                    }
                    else if ((c.Anchor & AnchorStyles.Top) == 0)
                    {
                        c.Top += dy / 2;
                    }
                    switch (c.Dock)
                    {
                        case DockStyle.Left:
                            c.Bounds = new RectangleF(remainingBounds.Left, remainingBounds.Top, c.Width, remainingBounds.Height);
                            remainingBounds.X += c.Width;
                            remainingBounds.Width -= c.Width;
                            break;

                        case DockStyle.Top:
                            c.Bounds = new RectangleF(remainingBounds.Left, remainingBounds.Top, remainingBounds.Width, c.Height);
                            remainingBounds.Y += c.Height;
                            remainingBounds.Height -= c.Height;
                            break;

                        case DockStyle.Right:
                            c.Bounds = new RectangleF(remainingBounds.Right - c.Width, remainingBounds.Top, c.Width, remainingBounds.Height);
                            remainingBounds.Width -= c.Width;
                            break;

                        case DockStyle.Bottom:
                            c.Bounds = new RectangleF(remainingBounds.Left, remainingBounds.Bottom - c.Height, remainingBounds.Width, c.Height);
                            remainingBounds.Height -= c.Height;
                            break;

                        case DockStyle.Fill:
                            c.Bounds = remainingBounds;
                            remainingBounds.Width = 0;
                            remainingBounds.Height = 0;
                            break;
                    }
                }
            }
            finally
            {
                updatingLayout = false;
            }
        }
        #endregion

        #region Report engine
        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            SetRunning(true);
            SetDesigning(false);
            OnBeforePrint(EventArgs.Empty);

            foreach (ReportComponentBase obj in Objects)
            {
                obj.SaveState();
                obj.SetRunning(true);
                obj.SetDesigning(false);
                obj.OnBeforePrint(EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            OnAfterPrint(EventArgs.Empty);
            base.RestoreState();
            SetRunning(false);

            foreach (ReportComponentBase obj in Objects)
            {
                obj.OnAfterPrint(EventArgs.Empty);
                obj.RestoreState();
                obj.SetRunning(false);
            }
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            foreach (ReportComponentBase obj in Objects)
            {
                obj.GetData();
                obj.OnAfterData();

                // break the component if it is of BreakableComponent an has non-empty BreakTo property
                if (obj is BreakableComponent && (obj as BreakableComponent).BreakTo != null &&
                    (obj as BreakableComponent).BreakTo.GetType() == obj.GetType())
                    (obj as BreakableComponent).Break((obj as BreakableComponent).BreakTo);
            }
            OnAfterData();
        }

        /// <inheritdoc/>
        public override float CalcHeight()
        {
            OnBeforeLayout(EventArgs.Empty);

            // sort objects by Top
            ReportComponentCollection sortedObjects = Objects.SortByTop();

            // calc height of each object
            float[] heights = new float[sortedObjects.Count];
            for (int i = 0; i < sortedObjects.Count; i++)
            {
                ReportComponentBase obj = sortedObjects[i];
                float height = obj.Height;
                if (obj.Visible && (obj.CanGrow || obj.CanShrink))
                {
                    float height1 = obj.CalcHeight();
                    if ((obj.CanGrow && height1 > height) || (obj.CanShrink && height1 < height))
                        height = height1;
                }
                heights[i] = height;
            }

            // calc shift amounts
            float[] shifts = new float[sortedObjects.Count];
            for (int i = 0; i < sortedObjects.Count; i++)
            {
                ReportComponentBase parent = sortedObjects[i];
                float shift = heights[i] - parent.Height;
                if (shift == 0)
                    continue;

                for (int j = i + 1; j < sortedObjects.Count; j++)
                {
                    ReportComponentBase child = sortedObjects[j];
                    if (child.ShiftMode == ShiftMode.Never)
                        continue;

                    if (child.Top >= parent.Bottom - 1e-4)
                    {
                        if (child.ShiftMode == ShiftMode.WhenOverlapped &&
                          (child.Left > parent.Right - 1e-4 || parent.Left > child.Right - 1e-4))
                            continue;

                        float parentShift = shifts[i];
                        float childShift = shifts[j];
                        if (shift > 0)
                            childShift = Math.Max(shift + parentShift, childShift);
                        else
                            childShift = Math.Min(shift + parentShift, childShift);
                        shifts[j] = childShift;
                    }
                }
            }

            // update location and size of each component, calc max height
            float maxHeight = 0;
            for (int i = 0; i < sortedObjects.Count; i++)
            {
                ReportComponentBase obj = sortedObjects[i];
                DockStyle saveDock = obj.Dock;
                obj.Dock = DockStyle.None;
                obj.Height = heights[i];
                obj.Top += shifts[i];
                if (obj.Visible && obj.Bottom > maxHeight)
                    maxHeight = obj.Bottom;
                obj.Dock = saveDock;
            }

            if ((CanGrow && maxHeight > Height) || (CanShrink && maxHeight < Height))
                Height = maxHeight;

            // perform grow to bottom
            foreach (ReportComponentBase obj in Objects)
            {
                if (obj.GrowToBottom)
                    obj.Height = Height - obj.Top;
            }

            OnAfterLayout(EventArgs.Empty);
            return Height;
        }

        /// <summary>
        /// This method fires the <b>BeforeLayout</b> event and the script code connected to the <b>BeforeLayoutEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnBeforeLayout(EventArgs e)
        {
            if (BeforeLayout != null)
                BeforeLayout(this, e);
            InvokeEvent(BeforeLayoutEvent, e);
        }

        /// <summary>
        /// This method fires the <b>AfterLayout</b> event and the script code connected to the <b>AfterLayoutEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public void OnAfterLayout(EventArgs e)
        {
            if (AfterLayout != null)
                AfterLayout(this, e);
            InvokeEvent(AfterLayoutEvent, e);
        }
        #endregion

        #region Public methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            ContainerObject src = source as ContainerObject;
            BeforeLayoutEvent = src.BeforeLayoutEvent;
            AfterLayoutEvent = src.AfterLayoutEvent;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            ContainerObject c = writer.DiffObject as ContainerObject;
            base.Serialize(writer);

            if (writer.SerializeTo == SerializeTo.Preview)
                return;

            if (BeforeLayoutEvent != c.BeforeLayoutEvent)
                writer.WriteStr("BeforeLayoutEvent", BeforeLayoutEvent);
            if (AfterLayoutEvent != c.AfterLayoutEvent)
                writer.WriteStr("AfterLayoutEvent", AfterLayoutEvent);
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            DrawBackground(e);
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            base.Draw(e);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>ContainerObject</b> class with default settings. 
        /// </summary>
        public ContainerObject()
        {
            objects = new ReportComponentCollection(this);
            beforeLayoutEvent = "";
            afterLayoutEvent = "";
        }
    }
}
