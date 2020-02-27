using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.MaxStableBridge;
using LinearDiff3DGame.OpenGLVisualizerTest.Objects3D;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
	internal class BridgeCalcAsync : IBridgeCalcAsync
	{
		public BridgeCalcAsync()
		{
			backgroundWorker = new BackgroundWorker();
			backgroundWorker.WorkerReportsProgress = true;
			backgroundWorker.WorkerSupportsCancellation = true;
			backgroundWorker.ProgressChanged += SectionCalculated;
			backgroundWorker.RunWorkerCompleted += CalculationCompleted;
			backgroundWorker.DoWork += BridgeCalculation;
		}

		public Int32 PrepareCalculation(String inputDataFile, Double finishTime)
		{
			BridgeBuildController buildController = new BridgeBuildController(inputDataFile);
			preparedCalculation = new Pair<BridgeBuildController, Double>(buildController, finishTime);
			return buildController.SectionCount(finishTime, true);
		}

		public void Run()
		{
			Bridge = emptyBridge;
			backgroundWorker.RunWorkerAsync(preparedCalculation);
		}

		public void Cancel()
		{
			backgroundWorker.CancelAsync();
		}

		public event EventHandler OnSectionCompleted;
		public event EventHandler<BridgeCompletedEventArgs> OnBridgeCompleted;

		public IList<Pair<Double, Polyhedron>> Bridge { get; private set; }

		private void SectionCalculated(Object sender, ProgressChangedEventArgs e)
		{
			if (OnSectionCompleted != null)
				OnSectionCompleted(this, new EventArgs());
		}

		private void CalculationCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				IList<Pair<Double, Polyhedron>> bridge = (IList<Pair<Double, Polyhedron>>) e.Result;
				Bridge = new ReadOnlyCollection<Pair<Double, Polyhedron>>(bridge);
			}
			if (OnBridgeCompleted != null)
				OnBridgeCompleted(this, new BridgeCompletedEventArgs(!e.Cancelled));
		}

		private void BridgeCalculation(Object sender, DoWorkEventArgs e)
		{
			Pair<BridgeBuildController, Double> data = (Pair<BridgeBuildController, Double>) e.Argument;
			BridgeBuildController buildController = data.Item1;
			Double finishTime = data.Item2;
			Int32 sectionCount = buildController.SectionCount(finishTime, true);
			IList<Pair<Double, Polyhedron>> bridge = new List<Pair<Double, Polyhedron>>(sectionCount);
			foreach (Pair<Double, IPolyhedron3D> section in buildController.CalculateBridge(finishTime, true))
			{
				if (backgroundWorker.CancellationPending) break;
				Polyhedron polyhedron = new Polyhedron(section.Item2);
				bridge.Add(new Pair<Double, Polyhedron>(section.Item1, polyhedron));
				backgroundWorker.ReportProgress((100*bridge.Count)/sectionCount);
			}
			e.Result = backgroundWorker.CancellationPending ? new List<Pair<Double, Polyhedron>>() : bridge;
		}

		private readonly BackgroundWorker backgroundWorker;

		private readonly IList<Pair<Double, Polyhedron>> emptyBridge =
			new ReadOnlyCollection<Pair<Double, Polyhedron>>(new List<Pair<Double, Polyhedron>>());

		private Pair<BridgeBuildController, Double> preparedCalculation;
	}
}