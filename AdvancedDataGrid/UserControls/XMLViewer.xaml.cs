using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace TestClient.UserControls
{
    /// <summary>
    /// Interaction logic for XMLViewer.xaml
    /// </summary>
    public partial class XMLViewer : UserControl
    {
        public XMLViewer()
        {
            InitializeComponent();
        }

        private XmlDocument _xmldocument = null;
        public XmlDocument xmlDocument
        {
            get { return this._xmldocument; }
            set
            {
                this._xmldocument = value;
                if (this._xmldocument == null)
                {
                    this.xmlTree.ItemsSource = null;
                    return;
                }

                XmlDataProvider provider = new XmlDataProvider();
                provider.Document = this._xmldocument;
                Binding binding = new Binding();
                binding.Source = provider;
                binding.XPath = "child::node()";
                this.xmlTree.SetBinding(TreeView.ItemsSourceProperty, binding);
            }
        }
    }
}
