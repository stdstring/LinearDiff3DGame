using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.Serialization.Bridge;

namespace LinearDiff3DGame.MaxStableBridge.Generator
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			const Double epsilon = 1e-9;
			const String inputDataFile = "KnobInput.xml";
			const Double finishTime = 6;
			const String outputDataFile = "KnobBridge.dat";
			//const Double z0 = 2.5;

			BridgeBuildController controller = new BridgeBuildController(inputDataFile);
			Console.WriteLine("Generating bridge ...");
			IList<Pair<Double, IPolyhedron3D>> sourceBridge = controller.GenerateBridge(finishTime);
			Console.WriteLine("Post processing ...");
			PostProcess postProcess = new PostProcess(new ApproxComp(epsilon));
			IList<Pair<Double, IPolyhedron3D>> bridge =
				sourceBridge.Select(pair => new Pair<Double, IPolyhedron3D>(pair.Item1, postProcess.Process(pair.Item2))).ToList();
			Int32 sectionCount = sourceBridge.Count;
			Console.WriteLine("Serializing ...");
			BridgeSerializer bridgeSerializer = new BridgeSerializer();
			using (FileStream fs = new FileStream(outputDataFile, FileMode.Create, FileAccess.Write))
			{
				Pair<Int32, IEnumerable<Pair<Double, IPolyhedron3D>>> bridgeData =
					new Pair<int, IEnumerable<Pair<Double, IPolyhedron3D>>>(sectionCount, bridge);
				bridgeSerializer.Serialize(fs, bridgeData);
			}

			Console.WriteLine("Generation complete !!!");
			Console.ReadLine();
		}
	}
}