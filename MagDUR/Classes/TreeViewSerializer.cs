using System.Text;
using System.Xml;
using Syncfusion.Windows.Forms.Tools;

namespace MagDUR.Classes
{
    class TreeViewSerializer
    {
        // Xml tag for node, e.g. 'node' in case of <node></node>
        private const string XmlNodeTag = "node";

        // Xml attributes for node e.g. <node text="Asia" tag="" imageindex="1"></node>
        private const string XmlNodeTextAtt = "text";
        private const string XmlNodeTagAtt = "tag";

        // System.IO.StringWriter s;
        public void SerializeTreeView(TreeViewAdv treeView, string fileName)
        {
            var textWriter = new XmlTextWriter(fileName, Encoding.UTF8);
            // writing the xml declaration tag
            textWriter.WriteStartDocument();
            //textWriter.WriteRaw("\r\n");
            // writing the main tag that encloses all node tags
            textWriter.WriteStartElement("TreeView");

            // save the nodes, recursive method
            SaveNodes(treeView.Nodes, textWriter);

            textWriter.WriteEndElement();

            textWriter.Close();
        }

        private void SaveNodes(TreeNodeAdvCollection nodesCollection, XmlTextWriter textWriter)
        {
            for (int i = 0; i < nodesCollection.Count; i++)
            {
                TreeNodeAdv node = nodesCollection[i];
                textWriter.WriteStartElement(XmlNodeTag);
                textWriter.WriteAttributeString(XmlNodeTextAtt, node.Text);
                if (node.Tag != null)
                    textWriter.WriteAttributeString(XmlNodeTagAtt, node.Tag.ToString());

                // add other node properties to serialize here

                if (node.Nodes.Count > 0)
                {
                    SaveNodes(node.Nodes, textWriter);
                }
                textWriter.WriteEndElement();
            }
        }

        public void DeserializeTreeView(TreeViewAdv treeView, string fileName)
        {
            XmlTextReader reader = null;
            try
            {
                // disabling re-drawing of treeview till all nodes are added
                treeView.BeginUpdate();
                reader =
                    new XmlTextReader(fileName);

                TreeNodeAdv parentNode = null;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlNodeTag)
                        {
                            var newNode = new TreeNodeAdv();
                            bool isEmptyElement = reader.IsEmptyElement;

                            // loading node attributes
                            int attributeCount = reader.AttributeCount;
                            if (attributeCount > 0)
                            {
                                for (int i = 0; i < attributeCount; i++)
                                {
                                    reader.MoveToAttribute(i);

                                    SetAttributeValue(newNode, reader.Name, reader.Value);
                                }
                            }

                            // add new node to Parent Node or TreeView
                            if (parentNode != null)
                                parentNode.Nodes.Add(newNode);
                            else
                                treeView.Nodes.Add(newNode);

                            // making current node 'ParentNode' if its not empty
                            if (!isEmptyElement)
                            {
                                parentNode = newNode;
                            }
                        }
                    }
                    // moving up to in TreeView if end tag is encountered
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == XmlNodeTag)
                        {
                            if (parentNode != null) parentNode = parentNode.Parent;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        //Ignore Xml Declaration
                    }
                    else if (reader.NodeType == XmlNodeType.None)
                    {
                        return;
                    }
                    else if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (parentNode != null) parentNode.Nodes.Add(reader.Value);
                    }
                }
            }
            finally
            {
                // enabling redrawing of treeview after all nodes are added
                treeView.EndUpdate();
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Used by Deserialize method for setting properties of TreeNode from xml node attributes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private void SetAttributeValue(TreeNodeAdv node, string propertyName, string value)
        {
            if (propertyName == XmlNodeTextAtt)
            {
                node.Text = value;
            }
            else if (propertyName == XmlNodeTagAtt)
            {
                node.Tag = value;
            }
        }
    }
}
