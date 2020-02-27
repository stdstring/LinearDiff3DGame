using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.OpenGLVisualizer.Objects3D;
using LinearDiff3DGame.Serialization.Bridge;

namespace LinearDiff3DGame.OpenGLVisualizer.BridgeController
{
    internal class BridgeLoadAsync : IBridgeLoadAsync
    {
        public BridgeLoadAsync()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += SectionLoaded;
            backgroundWorker.RunWorkerCompleted += LoadCompleted;
            backgroundWorker.DoWork += LoadBridge;
        }

        public int Prepare(Stream dataStream)
        {
            LazyBridgeSerializer serizlizer = new LazyBridgeSerializer();
            preparedLoad = serizlizer.Deserialize(dataStream);
            return preparedLoad.Item1;
        }

        public void Run()
        {
            Bridge = emptyBridge;
            backgroundWorker.RunWorkerAsync(preparedLoad);
        }

        public void Cancel()
        {
            backgroundWorker.CancelAsync();
        }

        public event EventHandler OnSectionCompleted;

        public event EventHandler<BridgeCompletedEventArgs> OnBridgeCompleted;

        public IList<Pair<Double, Polyhedron>> Bridge { get; private set; }

        private void SectionLoaded(Object sender, ProgressChangedEventArgs e)
        {
            if(OnSectionCompleted != null)
                OnSectionCompleted(this, new EventArgs());
        }

        private void LoadCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            if(!e.Cancelled)
            {
                IList<Pair<Double, Polyhedron>> bridge = (IList<Pair<Double, Polyhedron>>)e.Result;
                Bridge = new ReadOnlyCollection<Pair<Double, Polyhedron>>(bridge);
            }
            if(OnBridgeCompleted != null)
                OnBridgeCompleted(this, new BridgeCompletedEventArgs(!e.Cancelled));
        }

        private void LoadBridge(Object sender, DoWorkEventArgs e)
        {
            Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> loadData =
                (Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>>)e.Argument;
            IList<Pair<Double, Polyhedron>> bridge = new List<Pair<Double, Polyhedron>>(loadData.Item1);
            foreach(Pair<Double, Polyhedron3D> section in loadData.Item2)
            {
                if(backgroundWorker.CancellationPending) break;
                Polyhedron polyhedron = new Polyhedron(section.Item2);
                bridge.Add(new Pair<Double, Polyhedron>(section.Item1, polyhedron));
                backgroundWorker.ReportProgress((100 * bridge.Count) / loadData.Item1);
            }
            e.Result = backgroundWorker.CancellationPending ? new List<Pair<Double, Polyhedron>>() : bridge;
        }

        private readonly BackgroundWorker backgroundWorker;

        private readonly IList<Pair<Double, Polyhedron>> emptyBridge =
            new ReadOnlyCollection<Pair<Double, Polyhedron>>(new List<Pair<Double, Polyhedron>>());

        private Pair<Int32, IEnumerable<Pair<Double, Polyhedron3D>>> preparedLoad;
    }
}