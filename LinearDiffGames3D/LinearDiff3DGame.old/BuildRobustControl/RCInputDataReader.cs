using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace MathPostgraduateStudy.BuildRobustControl
{
    /// <summary>
    /// 
    /// </summary>
    public class RCInputDataReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDataFileName"></param>
        public RCInputDataReader(String inputDataFileName)
        {
            CultureInfo ci = new CultureInfo("ru-RU");
            ci.NumberFormat.NumberDecimalSeparator = ",";

            m_InputData = new DataContainer();

            XmlDocument doc = new XmlDocument();
            doc.Load(inputDataFileName);

            XmlNode root = doc.DocumentElement;

            XmlNode matrixANode = root.SelectSingleNode("MatrixA");
            XmlNodeList matrixARows = matrixANode.ChildNodes;

            Matrix matrixA = new Matrix(3, 3);
            for (Int32 rowIndex = 0; rowIndex < matrixA.RowCount; rowIndex++)
            {
                XmlNode matrixARow = matrixARows[rowIndex];
                String[] strRowElems = matrixARow.InnerText.Split(' ');

                for (Int32 columnIndex = 0; columnIndex < matrixA.ColumnCount; columnIndex++)
                {
                    Double currentElem = Double.Parse(strRowElems[columnIndex], ci);
                    matrixA[rowIndex + 1, columnIndex + 1] = currentElem;
                }
            }
            m_InputData.Add("MatrixA", matrixA);

            XmlNode matrixBNode = root.SelectSingleNode("MatrixB");
            XmlNodeList matrixBRows = matrixBNode.ChildNodes;

            Matrix matrixB = new Matrix(3, 1);
            for (Int32 rowIndex = 0; rowIndex < matrixB.RowCount; rowIndex++)
            {
                XmlNode matrixBRow = matrixBRows[rowIndex];

                matrixB[rowIndex + 1, 1] = Double.Parse(matrixBRow.InnerText, ci);
            }
            m_InputData.Add("MatrixB", matrixB);

            XmlNode matrixCNode = root.SelectSingleNode("MatrixC");
            XmlNodeList matrixCRows = matrixCNode.ChildNodes;

            Matrix matrixC = new Matrix(3, 1);
            for (Int32 rowIndex = 0; rowIndex < matrixC.RowCount; rowIndex++)
            {
                XmlNode matrixCRow = matrixCRows[rowIndex];

                matrixC[rowIndex + 1, 1] = Double.Parse(matrixCRow.InnerText, ci);
            }
            m_InputData.Add("MatrixC", matrixC);

            XmlNode finalSetNode = root.SelectSingleNode("FinalSet");
            XmlNodeList finalSetPoints = finalSetNode.ChildNodes;

            Point3D[] finalSet = new Point3D[finalSetPoints.Count];
            for (Int32 pointIndex = 0; pointIndex < finalSetPoints.Count; pointIndex++)
            {
                String[] strPointCoords = finalSetPoints[pointIndex].InnerText.Split(' ');

                Double coordX = Double.Parse(strPointCoords[0], ci);
                Double coordY = Double.Parse(strPointCoords[1], ci);
                Double coordZ = Double.Parse(strPointCoords[2], ci);

                finalSet[pointIndex] = new Point3D(coordX, coordY, coordZ);
            }
            m_InputData.Add("FinalSet", finalSet);

            XmlNode firstGamerNode = root.SelectSingleNode("FirstGamer");
            XmlNode mpNode = firstGamerNode.ChildNodes[0];
            Double mp = Double.Parse(mpNode.InnerText, ci);
            m_InputData.Add("Mp", mp);

            XmlNode secondGamerNode = root.SelectSingleNode("SecondGamer");
            XmlNode mqNode = secondGamerNode.ChildNodes[0];
            Double mq = Double.Parse(mqNode.InnerText, ci);
            m_InputData.Add("Mq", mq);

            XmlNode maxVValueNode = secondGamerNode.ChildNodes[1];
            Double maxVValue = Double.Parse(maxVValueNode.InnerText, ci);
            m_InputData.Add("MaxVValue", maxVValue);

            XmlNode deltaTNode = root.SelectSingleNode("DeltaT");
            Double deltaT = Double.Parse(deltaTNode.InnerText, ci);
            m_InputData.Add("DeltaT", deltaT);

            XmlNode distinguishAngleNode = root.SelectSingleNode("MinVectorDistinguishAngle");
            Double minVectorDistinguishAngle = Double.Parse(distinguishAngleNode.InnerText, ci);
            m_InputData.Add("MinVectorDistinguishAngle", minVectorDistinguishAngle);

            XmlNode epsilonNode = root.SelectSingleNode("Epsilon");
            Double epsilon = Double.Parse(epsilonNode.InnerText, ci);
            m_InputData.Add("Epsilon", epsilon);

            XmlNode startTimeNode = root.SelectSingleNode("StartTime");
            Double startTime = Double.Parse(startTimeNode.InnerText, ci);
            m_InputData.Add("StartTime", startTime);

            XmlNode finishTimeNode = root.SelectSingleNode("FinishTime");
            Double finishTime = Double.Parse(finishTimeNode.InnerText, ci);
            m_InputData.Add("FinishTime", finishTime);

            XmlNode deltaScaleNode = root.SelectSingleNode("DeltaScale");
            Double deltaScale = Double.Parse(deltaScaleNode.InnerText, ci);
            m_InputData.Add("DeltaScale", deltaScale);

            XmlNode startPointNode = root.SelectSingleNode("StartPoint");
            String[] strStartPointCoords = startPointNode.InnerText.Split(' ');
            Point3D startPoint = new Point3D(Double.Parse(strStartPointCoords[0], ci),
                                             Double.Parse(strStartPointCoords[1], ci),
                                             Double.Parse(strStartPointCoords[2], ci));
            m_InputData.Add("StartPoint", startPoint);

            XmlNode deltaMqNode = root.SelectSingleNode("DeltaMq");
            Double deltaMq = Double.Parse(deltaMqNode.InnerText, ci);
            m_InputData.Add("DeltaMq", deltaMq);

            XmlNode deltaFSKoeffNode = root.SelectSingleNode("DeltaFSKoeff");
            Double deltaFSKoeff = Double.Parse(deltaFSKoeffNode.InnerText, ci);
            m_InputData.Add("DeltaFSKoeff", deltaFSKoeff);

            XmlNode bridgesCountNode = root.SelectSingleNode("BridgesCount");
            Int32 bridgesCount = Int32.Parse(bridgesCountNode.InnerText, ci);
            m_InputData.Add("BridgesCount", bridgesCount);
        }

        /// <summary>
        /// 
        /// </summary>
        public DataContainer InputData
        {
            get
            {
                return m_InputData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DataContainer m_InputData;
    }
}
