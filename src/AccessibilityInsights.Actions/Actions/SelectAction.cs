// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Attributes;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Enums;
using AccessibilityInsights.Actions.Trackers;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Misc;
using AccessibilityInsights.Desktop.Settings;
using AccessibilityInsights.Desktop.UIAutomation;
using System;
using System.Linq;

namespace AccessibilityInsights.Actions
{
    /// <summary>
    /// Class SelectionAction
    /// this class is to select unelement via focus or keyboard
    /// </summary>
    [InteractionLevel(UxInteractionLevel.NoUxInteraction)]
    public class SelectAction : IDisposable
    {
        /// <summary>
        /// UIATree state
        /// </summary>
        UIATreeState UIATreeState = UIATreeState.Resumed;

        /// <summary>
        /// Selector by Focus
        /// </summary>
        FocusTracker FocusTracker = null;

        /// <summary>
        /// On/Off Focus Select
        /// </summary>
        public bool IsFocusSelectOn { get; set; } = false;

        /// <summary>
        /// Mouse Selector Delay in Milliseconds
        /// </summary>
        public double IntervalMouseSelector
        {
            get
            {
                return this.MouseTracker.IntervalMouseSelector;
            }

            set
            {
                this.MouseTracker.IntervalMouseSelector = value;
            }
        } // default for the case

        /// <summary>
        /// Selector by Mouse Hover
        /// </summary>
        MouseTracker MouseTracker = null;

        /// <summary>
        /// On/Off Mouse Select
        /// </summary>
        public bool IsMouseSelectOn { get; set; } = false;

        public TreeTracker TreeTracker { get; private set; } = null;

        /// <summary>
        /// actual object for POI ElementContext.
        /// </summary>
        ElementContext _POIElementContext = null;

        /// <summary>
        /// Element Context of Point of Interest(a.k.a. selected)
        /// </summary>
        public ElementContext POIElementContext
        {
            get
            {
                return _POIElementContext;
            }

            private set
            {
                var dma = DataManager.GetDefaultInstance();
                if (_POIElementContext != null)
                {
                    dma.RemoveElementContext(_POIElementContext.Id);
                }

                _POIElementContext = value;
                dma.AddElementContext(value);
            }
        }

        /// <summary>
        /// context which is not selected yet. but sent by tracker
        /// </summary>
        ElementContext CandidateEC = null;

        /// <summary>
        /// Set the scope of Selection
        /// </summary>
        public SelectionScope Scope
        {
            get
            {
                return MouseTracker.Scope;
            }

            set
            {
                this.FocusTracker.Scope = value;
                this.MouseTracker.Scope = value;
            }
        }

        /// <summary>
        /// private Constructor
        /// </summary>
        private SelectAction()
        {
            this.FocusTracker = new FocusTracker(SetCandidateElement);
            this.MouseTracker = new MouseTracker(SetCandidateElement);
            this.TreeTracker = new TreeTracker(this.SetCandidateElement, this);
        }

        /// <summary>
        /// Stop/Pause selection
        /// </summary>
        public void Stop()
        {
            if (!this.IsPaused)
            {
                this.FocusTracker?.Stop();
                this.MouseTracker?.Stop();
            }
        }

        /// <summary>
        /// Start/Resume selection
        /// </summary>
        public void Start()
        {
            if (!this.IsPaused)
            {
                if (this.IsFocusSelectOn)
                {
                    this.FocusTracker?.Start();
                }

                if (this.IsMouseSelectOn)
                {
                    this.MouseTracker?.Start();
                }
            }
        }

        /// <summary>
        /// Pause UIA Tree in live mode
        /// </summary>
        public void PauseUIATreeTracker()
        {
            Stop();
            this.UIATreeState = UIATreeState.Paused;
        }

        /// <summary>
        /// Resume UIA Tree
        /// </summary>
        public void ResumeUIATreeTracker()
        {
            this.UIATreeState = UIATreeState.Resumed;
            Start();
        }


        /// <summary>
        /// Set element for Next selection call
        /// it is for internal use. 
        /// </summary>
        /// <param name="el"></param>
        public void SetCandidateElement(A11yElement el)
        {
            lock (this)
            {
                if (el != null)
                {
                    if (el.IsRootElement() == false)
                    {
                        this.CandidateEC?.Dispose();
                        this.CandidateEC = new ElementContext(el);
                    }
                    else
                    {
                        el.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///return true if the UIA Tree is paused
        /// </summary>
        public bool IsPaused
        {
            get => UIATreeState == UIATreeState.Paused;
        }

        /// <summary>
        /// Set element for Next selection call
        /// ElementContext will be created with the clone of the selected element. 
        /// </summary>
        /// <param name="ecId"></param>
        /// <param name="eId"></param>
        public void SetCandidateElement(Guid ecId, int eId)
        {
            var el = DataManager.GetDefaultInstance().GetA11yElement(ecId, eId).CloneForSelection();

            SetCandidateElement(el);
        }

        /// <summary>
        /// Set candidate element by process Id
        /// </summary>
        /// <param name="pid">process Id</param>
        public void SetCandidateElementFromProcessId(int pId)
        {
            var handle = System.Diagnostics.Process.GetProcessById(pId).Handle;

            SetCandidateElementFromHandle(handle);
        }

        /// <summary>
        /// Set candidate element by handle. 
        /// </summary>
        /// <param name="handle"></param>
        public void SetCandidateElementFromHandle(IntPtr handle)
        {
            lock (this)
            {
                var element = A11yAutomation.ElementFromHandle(handle);
                this.CandidateEC?.Dispose();
                this.CandidateEC = new ElementContext(element);
            }
        }

        /// <summary>
        /// Select the element and return success(true)/failure(false)
        /// </summary>
        /// <returns>true when there is new selection</returns>
        public bool Select()
        {
            lock(this)
            {
                if(CandidateEC != null && ( POIElementContext == null || POIElementContext.Element.IsSameUIElement(CandidateEC.Element) == false))
                {
                    POIElementContext = CandidateEC;
                    CandidateEC = null;

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Select Loaded Data
        /// Load data from path and use the loaded data to set the selected element context. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Tuple of ElementContextId and SnapshotMetaInfo</returns>
        public Tuple<Guid, SnapshotMetaInfo> SelectLoadedData(string path, int? selectedElementId = null)
        {
            ClearSelectedContext();

            var parts = LoadAction.LoadSnapshotZip(path);
            var meta = parts.MetaInfo;
            var ec = new ElementContext(parts.Element.FindPOIElementFromLoadedData());

            SetSelectedContextWithLoadedData(ec);
            CaptureAction.SetTestModeDataContext(ec.Id, DataContextMode.Load, ec.Element.TreeWalkerMode);

            ec.DataContext.Screenshot = parts.Bmp ?? parts.SynthesizedBmp;
            ec.DataContext.ScreenshotElementId = meta.ScreenshotElementId;
            ec.DataContext.FocusedElementUniqueId = selectedElementId.HasValue ? selectedElementId.Value : meta.SelectedItems?.First();

            return new Tuple<Guid, SnapshotMetaInfo>(ec.Id, meta);
        }


        /// <summary>
        /// Set Selected context with loaded snapshot data
        /// </summary>
        /// <param name="ec"></param>
        void SetSelectedContextWithLoadedData(ElementContext ec)
        {
            lock (this)
            {
                POIElementContext?.Dispose();
                CandidateEC?.Dispose();
                CandidateEC = null;
                POIElementContext = ec;
            }
        }

        /// <summary>
        /// Clean up selected Context
        /// </summary>
        public void ClearSelectedContext()
        {
            if (!this.IsPaused)
            {
                lock (this)
                {
                    POIElementContext = null;
                    CandidateEC?.Dispose();
                    CandidateEC = null;

                    this.MouseTracker.Clear();
                    this.FocusTracker.Clear();
                }
            }
        }

        /// <summary>
        /// Get the selected ElementContext.
        /// </summary>
        /// <returns></returns>
        public Guid? GetSelectedElementContextId()
        {
            lock (this)
            {
                return POIElementContext != null ? POIElementContext.Id : (Guid?)null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasPOIElement()
        {
            return this.POIElementContext != null;
        }

        public TreeViewMode TreeViewMode
        {
            get
            {
                return (this.TreeTracker != null)
                    ? this.TreeTracker.TreeViewMode
                    : TreeViewMode.Raw;
            }

            set
            {
                if (this.TreeTracker == null) return;

                this.TreeTracker.TreeViewMode = value;
            }
        }

        #region static methods
        /// <summary>
        /// default instance
        /// </summary>
        private static SelectAction sDefaultInstance = null;

        /// <summary>
        /// Get the default instance of SelectAction
        /// </summary>
        /// <returns></returns>
        public static SelectAction GetDefaultInstance()
        {
            if(sDefaultInstance == null)
            {
                sDefaultInstance = GetInstance();
            }

            return sDefaultInstance;
        }

        /// <summary>
        /// Get an instance of SelectAction
        /// this is for the case that you need 2nd or more instances of SelectAction. 
        /// in most of cases, you can use GetDefaultInstance() for getting default SelectAction instance
        /// </summary>
        /// <returns></returns>
        public static SelectAction GetInstance()
        {
            return new SelectAction();
        }

        /// <summary>
        /// Clear default Instance. 
        /// </summary>
        public static void ClearDefaultInstance()
        {
            if(sDefaultInstance != null)
            {
                sDefaultInstance.Dispose();
                sDefaultInstance = null;
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.POIElementContext != null)
                    {
                        this.POIElementContext.Dispose();
                        this.POIElementContext = null;
                    }
                    if (this.CandidateEC != null)
                    {
                        this.CandidateEC.Dispose();
                        this.CandidateEC = null;
                    }
                    if (this.MouseTracker != null)
                    {
                        this.MouseTracker.Dispose();
                        this.MouseTracker = null;
                    }
                    if(this.FocusTracker != null)
                    {
                        this.FocusTracker.Stop();
                        this.FocusTracker.Dispose();
                        this.FocusTracker = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
