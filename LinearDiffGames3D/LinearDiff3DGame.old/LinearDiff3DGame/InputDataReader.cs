using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MathPostgraduateStudy.LinearDiff3DGame
{
    /// <summary>
    /// 
    /// </summary>
    public class InputDataReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputDataFileName"></param>
        public InputDataReader(String inputDataFileName)
        {
            m_InputData = new Dictionary<String, Object>();

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
                    Double currentElem = Double.Parse(strRowElems[columnIndex]);
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

                matrixB[rowIndex + 1, 1] = Double.Parse(matrixBRow.InnerText);
            }
            m_InputData.Add("MatrixB", matrixB);

            XmlNode matrixCNode = root.SelectSingleNode("MatrixC");
            XmlNodeList matrixCRows = matrixCNode.ChildNodes;

            Matrix matrixC = new Matrix(3, 1);
            for (Int32 rowIndex = 0; rowIndex < matrixC.RowCount; rowIndex++)
            {
                XmlNode matrixCRow = matrixCRows[rowIndex];

                matrixC[rowIndex + 1, 1] = Double.Parse(matrixCRow.InnerText);
            }
            m_InputData.Add("MatrixC", matrixC);

            XmlNode finalSetNode = root.SelectSingleNode("FinalSet");
            XmlNodeList finalSetPoints = finalSetNode.ChildNodes;

            Point3D[] finalSet = new Point3D[finalSetPoints.Count];
            for (Int32 pointIndex = 0; pointIndex < finalSetPoints.Count; pointIndex++)
            {
                String[] strPointCoords = finalSetPoints[pointIndex].InnerText.Split(' ');

                Double coordX = Double.Parse(strPointCoords[0]);
                Double coordY = Double.Parse(strPointCoords[1]);
                Double coordZ = Double.Parse(strPointCoords[2]);

                finalSet[pointIndex] = new Point3D(coordX, coordY, coordZ);
            }
            m_InputData.Add("FinalSet", finalSet);

            XmlNode firstGamerNode = root.SelectSingleNode("FirstGamer");
            XmlNode mpNode = firstGamerNode.ChildNodes[0];
            Double mp = Double.Parse(mpNode.InnerText);
            m_InputData.Add("Mp", mp);

            XmlNode secondGamerNode = root.SelectSingleNode("SecondGamer");
            XmlNode mqNode = secondGamerNode.ChildNodes[0];
            Double mq = Double.Parse(mqNode.InnerText);
            m_InputData.Add("Mq", mq);

            XmlNode deltaTNode = root.SelectSingleNode("DeltaT");
            Double deltaT = Double.Parse(deltaTNode.InnerText);
            m_InputData.Add("DeltaT", deltaT);

            XmlNode distinguishAngleNode = root.SelectSingleNode("MinVectorDistinguishAngle");
            Double minVectorDistinguishAngle = Double.Parse(distinguishAngleNode.InnerText);
            m_InputData.Add("MinVectorDistinguishAngle", minVectorDistinguishAngle);

            XmlNode epsilonNode = root.SelectSingleNode("Epsilon");
            Double epsilon = Double.Parse(epsilonNode.InnerText);
            m_InputData.Add("Epsilon", epsilon);
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, Object> InputData
        {
            get
            {
                return m_InputData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<String, Object> m_InputData;
    }
}
