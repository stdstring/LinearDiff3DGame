using System;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.MaxStableBridge.Input;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    public class FirstGamerInitData : GamerInitData
    {
        public FirstGamerInitData(GamerParams gamerParams, Double deltaT, ApproxComp approxComp, Double separateNodeValue)
            : base(gamerParams, deltaT, approxComp)
        {
            SeparateNodeValue = separateNodeValue;
        }

        public Double SeparateNodeValue { get; private set; }
    }
}