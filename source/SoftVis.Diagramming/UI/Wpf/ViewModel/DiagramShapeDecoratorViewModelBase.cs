﻿using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A widget on a diagram shape. 
    /// Its placement is calculated by the view using the PlacementKey.
    /// </summary>
    public abstract class DiagramShapeDecoratorViewModelBase : DiagramViewModelBase, IDecoratorViewModel<DiagramNodeViewModel>
    {
        private bool _isVisible;

        public DiagramNodeViewModel HostViewModel { get; private set; }

        protected DiagramShapeDecoratorViewModelBase(IArrangedDiagram diagram)
            : base(diagram)
        {
            _isVisible = false;
        }

        /// <summary>
        /// An object that serves as the key when the view looks up the placement specification in a dictionary.
        /// Its value is dependent on which kind of diagram shape button is it.
        /// </summary>
        public abstract object PlacementKey { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual void AssociateWith(DiagramNodeViewModel diagramNodeViewModel)
        {
            HostViewModel = diagramNodeViewModel;
            IsVisible = true;
        }

        public virtual void Hide()
        {
            HostViewModel = null;
            IsVisible = false;
        }
    }
}