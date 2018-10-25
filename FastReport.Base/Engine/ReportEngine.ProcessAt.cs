using FastReport.Utils;
using System.Collections.Generic;

namespace FastReport.Engine
{
    internal enum EngineState
    {
        ReportStarted,
        ReportFinished,
        ReportPageStarted,
        ReportPageFinished,
        PageStarted,
        PageFinished,
        ColumnStarted,
        ColumnFinished,
        BlockStarted,
        BlockFinished,
        GroupStarted,
        GroupFinished
    }

    internal class EngineStateChangedEventArgs
    {
        #region Fields

        private ReportEngine engine;
        private EngineState state;

        #endregion Fields

        #region Properties

        public ReportEngine Engine
        {
            get { return engine; }
        }

        public EngineState State
        {
            get { return state; }
        }

        #endregion Properties

        #region Constructors

        internal EngineStateChangedEventArgs(ReportEngine engine, EngineState state)
        {
            this.engine = engine;
            this.state = state;
        }

        #endregion Constructors
    }

    internal delegate void EngineStateChangedEventHandler(object sender, EngineStateChangedEventArgs e);

    internal class ProcessInfo
    {
        #region Fields

        private TextObject textObject;
        private XmlItem xmlItem;

        #endregion Fields

        #region Properties

        public TextObjectBase TextObject
        {
            get { return textObject; }
        }

        #endregion Properties

        #region Constructors

        public ProcessInfo(TextObject obj, XmlItem item)
        {
            textObject = obj;
            xmlItem = item;
        }

        #endregion Constructors

        #region Public Methods

        public void Process()
        {
            textObject.SaveState();
            try
            {
                textObject.GetData();
                string fill_clr = textObject.FillColor.IsNamedColor ? textObject.FillColor.Name :
                    "#" + textObject.FillColor.Name;
                string txt_clr = textObject.TextColor.IsNamedColor ? textObject.TextColor.Name :
                    "#" + textObject.TextColor.Name;

                xmlItem.SetProp("x", textObject.Text);
                xmlItem.SetProp("Fill.Color", fill_clr);
                xmlItem.SetProp("TextFill.Color", txt_clr);
                xmlItem.SetProp("Font.Name", textObject.Font.Name);
            }
            finally
            {
                textObject.RestoreState();
            }
        }

        public bool Process(object sender, EngineState state)
        {
            ProcessAt processAt = textObject.ProcessAt;
            bool canProcess = false;

            if ((processAt == ProcessAt.DataFinished && state == EngineState.BlockFinished) ||
                (processAt == ProcessAt.GroupFinished && state == EngineState.GroupFinished))
            {
                // check which data is finished
                BandBase topParentBand = textObject.Band;
                if (topParentBand is ChildBand)
                    topParentBand = (topParentBand as ChildBand).GetTopParentBand;

                if (processAt == ProcessAt.DataFinished && state == EngineState.BlockFinished)
                {
                    // total can be printed on the same data header, or on its parent data band
                    DataBand senderBand = sender as DataBand;
                    canProcess = true;
                    if (topParentBand is DataHeaderBand && (topParentBand.Parent != sender))
                        canProcess = false;
                    if (topParentBand is DataBand && senderBand.Parent != topParentBand)
                        canProcess = false;
                }
                else
                {
                    // total can be printed on the same group header
                    canProcess = sender == topParentBand;
                }
            }
            else
            {
                canProcess = (processAt == ProcessAt.ReportFinished && state == EngineState.ReportFinished) ||
                    (processAt == ProcessAt.ReportPageFinished && state == EngineState.ReportPageFinished) ||
                    (processAt == ProcessAt.PageFinished && state == EngineState.PageFinished) ||
                    (processAt == ProcessAt.ColumnFinished && state == EngineState.ColumnFinished);
            }

            if (canProcess)
            {
                Process();
                return true;
            }
            else
                return false;
        }

        #endregion Public Methods
    }

    public partial class ReportEngine
    {
        #region Fields

        private List<ProcessInfo> objectsToProcess;

        #endregion Fields

        #region Events

        internal event EngineStateChangedEventHandler StateChanged;

        #endregion Events

        #region Private Methods

        private void ProcessObjects(object sender, EngineState state)
        {
            for (int i = 0; i < objectsToProcess.Count; i++)
            {
                ProcessInfo info = objectsToProcess[i];
                if (info.Process(sender, state))
                {
                    objectsToProcess.RemoveAt(i);
                    i--;
                }
            }
        }

        private void OnStateChanged(object sender, EngineState state)
        {
            ProcessObjects(sender, state);
            if (StateChanged != null)
                StateChanged(sender, new EngineStateChangedEventArgs(this, state));
        }

        #endregion Private Methods

        #region Internal Methods

        internal void AddObjectToProcess(Base obj, XmlItem item)
        {
            TextObject textObj = obj as TextObject;
            if (textObj == null || textObj.ProcessAt == ProcessAt.Default)
                return;

            objectsToProcess.Add(new ProcessInfo(textObj, item));
        }

        #endregion Internal Methods

        #region Public Methods

        /// <summary>
        /// Processes the specified text object which <b>ProcessAt</b> property is set to <b>Custom</b>.
        /// </summary>
        /// <param name="obj">The text object to process.</param>
        public void ProcessObject(TextObjectBase obj)
        {
            for (int i = 0; i < objectsToProcess.Count; i++)
            {
                ProcessInfo info = objectsToProcess[i];
                if (info.TextObject == obj)
                {
                    info.Process();
                    objectsToProcess.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion Public Methods
    }
}
