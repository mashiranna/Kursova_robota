using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Huffman1._3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Node
        {
            public char Symbol { get; set; }
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            public List<bool> Traverse(char symbol, List<bool> data)
            {
                // Leaf
                if (Right == null && Left == null)
                {
                    if (symbol.Equals(this.Symbol))
                        return data;
                    else
                        return null;
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (Left != null)
                    {
                        List<bool> leftPath = new List<bool>();
                        leftPath.AddRange(data);
                        leftPath.Add(false);

                        left = Left.Traverse(symbol, leftPath);
                    }

                    if (Right != null)
                    {
                        List<bool> rightPath = new List<bool>();
                        rightPath.AddRange(data);
                        rightPath.Add(true);
                        right = Right.Traverse(symbol, rightPath);
                    }

                    if (left != null)
                        return left;
                    else
                        return right;
                }
            }
            }
        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }
            public Dictionary<char, int> Frequencies = new Dictionary<char, int>();

            public void Build(string source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (!Frequencies.ContainsKey(source[i]))
                    {
                        Frequencies.Add(source[i], 0);
                    }

                    Frequencies[source[i]]++;
                }

                foreach (KeyValuePair<char, int> symbol in Frequencies)
                {
                    nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
                }

                while (nodes.Count > 1)
                {
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                    if (orderedNodes.Count >= 2)
                    {
                        // Take first two items
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                        // Create a parent node by combining the frequencies
                        Node parent = new Node()
                        {
                            Symbol = '*',
                            Frequency = taken[0].Frequency + taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };

                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }
                    this.Root = nodes.FirstOrDefault();
                }
            }

            public List<string> code = new List<string>();
            public IEnumerable<string> codec;

            public BitArray Encode(string source)
            {
                List<bool> encodedSource = new List<bool>();

                for (int i = 0; i < source.Length; i++)
                {
                    List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                    encodedSource.AddRange(encodedSymbol);
                    this.code.Add("");

                    foreach (bool a in encodedSymbol.ToArray())
                        this.code[i] += Convert.ToInt32(a).ToString();
                }
                // IEnumerable<string> 
                codec = code.Distinct();
                BitArray bits = new BitArray(encodedSource.ToArray());
                return bits;
            }

            public string Decode(BitArray bits)
            {
                Node current = this.Root;
                string decoded = "";

                foreach (bool bit in bits)
                {
                    if (bit)
                    {
                        if (current.Right != null)
                            current = current.Right;
                    }
                    else
                    {
                        if (current.Left != null)
                            current = current.Left;
                    }
                    if (IsLeaf(current))
                    {
                        decoded += current.Symbol;
                        current = this.Root;
                    }
                }
                return decoded;
            }   

            public bool IsLeaf(Node node)
            {
                return (node.Left == null && node.Right == null);
            }

        }
        public HuffmanTree htree;
        public BitArray encoded;

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 0;
            richTextBox1.Text = "";
            htree.Build(textBox1.Text);
            encoded = htree.Encode(textBox1.Text);
            foreach (bool bit in encoded)
            {
                richTextBox1.Text += ((bit ? 1 : 0).ToString() + " ");
            }
            foreach (KeyValuePair<char, int> item in htree.Frequencies)
            {
                richTextBox3.Text += ("Символ: " + item.Key + "\t Кількість: " + item.Value.ToString() + "\t Частота: " + (Math.Round((float)item.Value / textBox1.Text.Length, 3)) + "\t Код " + htree.codec.ToArray()[i] + "\n");
                i++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            richTextBox2.AppendText(htree.Decode(encoded));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            htree = new HuffmanTree();
            textBox1.Width = this.Size.Width;
            richTextBox1.Width = this.Size.Width;
            richTextBox2.Width = this.Size.Width;
            richTextBox3.Width = this.Size.Width;
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }
}