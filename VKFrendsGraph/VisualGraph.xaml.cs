using System;
using System.Collections.Generic;
using QuickGraph;
namespace VKFrendsGraph
{

    public partial class VisualGraph
    {
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualise;
        public Dictionary<NumbersId, List<Frends>> Dictionary;
        private readonly MainWindow _main;
        private string _layoutAlgorithmType;
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualise
        {
            get { return _graphToVisualise; }
        }
        public string LayoutAlgorithmType
        {
            get { return _layoutAlgorithmType; }
            set
            {
                _layoutAlgorithmType = value;
            }
        }
        public VisualGraph(Dictionary<NumbersId, List<Frends>> dictionary, MainWindow main,string algorithm)
        {
            LayoutAlgorithmType = algorithm;
            Dictionary = dictionary;
            _main = main;
            CreateGraphToVisualise();
            InitializeComponent();
        }

        private void CreateGraphToVisualise()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();
            var vertex = new string[Dictionary.Count];
            int h = 0;
            foreach (var item in Dictionary)
            {
                vertex[h] = item.Key.Id + "\n" + item.Key.Name + "\n" + item.Key.Surname;
                g.AddVertex(vertex[h]);
                h++;
            }

            int r =0; 
            foreach (var item in Dictionary)
            {

                    foreach (var frends in item.Value)
                    {
                        for (int b = 0; b < vertex.Length; b++)
                        {
                            var s = vertex[b];
                            if (s.IndexOf(frends.Id, StringComparison.Ordinal)==0)
                            {
                                g.AddEdge(new Edge<object>(vertex[r], vertex[b]));
                            }
                        }
                    }
                r++;
            }


            _graphToVisualise = g;
        }


        private void VisualGraph_OnClosed(object sender, EventArgs e)
        {
            _main.Show(); 
        }
    }
}
