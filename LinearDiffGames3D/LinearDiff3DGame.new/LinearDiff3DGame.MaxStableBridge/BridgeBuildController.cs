using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.Common;
using LinearDiff3DGame.Geometry3D.Polyhedron;
using LinearDiff3DGame.MaxStableBridge.Input;

namespace LinearDiff3DGame.MaxStableBridge
{
	public class BridgeBuildController
	{
		public BridgeBuildController(String inputData)
		{
			XmlSerializer serializer = new XmlSerializer(typeof (InputParams));
			InputParams inputParams;
			using (StreamReader sr = new StreamReader(inputData))
				inputParams = (InputParams) serializer.Deserialize(sr);
			approxComp = new ApproxComp(epsilon);
			BridgeBuilderData bridgeBuilderData = new BridgeBuilderData(inputParams, approxComp);
			builder = new BridgeBuilder(bridgeBuilderData);
		}

		public IList<Pair<Double, IPolyhedron3D>> GenerateBridge(Double finishTime)
		{
			IList<Pair<Double, IPolyhedron3D>> bridge = new List<Pair<Double, IPolyhedron3D>>();
			while (approxComp.LT(builder.InverseTime, finishTime))
			{
				bridge.Add(new Pair<Double, IPolyhedron3D>(builder.InverseTime, builder.CurrentTSection));
				builder.NextIteration();
			}
			return bridge;
		}

		public IEnumerable<Pair<Double, IPolyhedron3D>> CalculateBridge(Double finishTime,
		                                                                Boolean includeBoundary)
		{
			yield return new Pair<Double, IPolyhedron3D>(builder.InverseTime, builder.CurrentTSection);
			Func<Double, Boolean> breakCalculation =
				time => includeBoundary ? approxComp.GT(time, finishTime) : approxComp.GE(time, finishTime);
			while (!breakCalculation(builder.InverseTime + builder.DeltaT))
			{
				builder.NextIteration();
				yield return new Pair<Double, IPolyhedron3D>(builder.InverseTime, builder.CurrentTSection);
			}
		}

		public Int32 SectionCount(Double finishTime, Boolean includeBoundary)
		{
			// TODO : при данном подходе можем ошибиться на 1
			Int32 sectionCount = (Int32) (finishTime/builder.DeltaT);
			return sectionCount;
		}

		private readonly BridgeBuilder builder;
		private readonly ApproxComp approxComp;
		private const Double epsilon = 1e-9;
	}
}