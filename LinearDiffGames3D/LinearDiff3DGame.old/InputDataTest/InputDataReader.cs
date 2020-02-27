using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using MathPostgraduateStudy.LinearDiff3DGame;

namespace InputDataTest
{
    public class InputDataReader
    {
        public InputDataReader(String inputDataFileName) : base()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputDataFileName);

            XmlNode root = doc.DocumentElement;

            XmlNode matrixANode = root.SelectSingleNode("MatrixA");
            XmlNodeList matrixARows = matrixANode.ChildNodes;

            m_MatrixA = new Matrix(3, 3);

            for (Int32 rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                XmlNode matrixARow = matrixARows[rowIndex];
                String[] strRowElems = matrixARow.InnerText.Split(' ');

                for (Int32 columnIndex = 0; columnIndex < 3; columnIndex++)
                {
                    Double currentElem = Double.Parse(strRowElems[columnIndex]);
                    m_MatrixA[rowIndex + 1, columnIndex + 1] = currentElem;
                }
            }

            XmlNode matrixBNode = root.SelectSingleNode("MatrixB");
            XmlNodeList matrixBRows = matrixBNode.ChildNodes;

            m_MatrixB = new Matrix(3, 1);

            for (Int32 rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                XmlNode matrixBRow = matrixBRows[rowIndex];
                String strRowElem = matrixBRow.InnerText;

                Double currentElem = Double.Parse(strRowElem);
                m_MatrixB[rowIndex + 1, 1] = currentElem;
            }

            XmlNode matrixCNode = root.SelectSingleNode("MatrixC");
            XmlNodeList matrixCRows = matrixCNode.ChildNodes;

            m_MatrixC = new Matrix(3, 1);

            for (Int32 rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                XmlNode matrixCRow = matrixCRows[rowIndex];
                String strRowElem = matrixCRow.InnerText;

                Double currentElem = Double.Parse(strRowElem);
                m_MatrixC[rowIndex + 1, 1] = currentElem;
            }
        }

        public Matrix MatrixA
        {
            get
            {
                return m_MatrixA;
            }
        }

        public Matrix MatrixB
        {
            get
            {
                return m_MatrixB;
            }
        }

        public Matrix MatrixC
        {
            get
            {
                return m_MatrixC;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixA;
        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixB;
        /// <summary>
        /// 
        /// </summary>
        private Matrix m_MatrixC;
    }
}
