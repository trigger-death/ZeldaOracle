﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	public class RequestCloseAnchorable : LayoutAnchorable, IRequestClosePanel {

		private bool forceClosed;

		public RequestCloseAnchorable() {
			Closing += OnAnchorableClosing;
			Closed += OnAnchorableClosed;
			Hiding += OnAnchorableHiding;
			forceClosed = false;
			DesignerControl.AddOpenAnchorable(this);
		}

		private void OnAnchorableHiding(object sender, CancelEventArgs e) {
			// HACK: We don't want the X button to hide anchorables when not in the document viewer
			e.Cancel = true;
			Close();
		}

		private void OnAnchorableClosed(object sender, EventArgs e) {
			DesignerControl.RemoveOpenAnchorable(this);
		}

		public void ForceClose() {
			if (!forceClosed) {
				forceClosed = true;
				Closing -= OnAnchorableClosing;
				Close();
			}
		}


		private void OnAnchorableClosing(object sender, CancelEventArgs e) {
			e.Cancel = true;
			DesignerControl.AddClosingAnchorable(this);
		}
	}
}
