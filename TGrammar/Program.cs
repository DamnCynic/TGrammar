using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

namespace TGrammar
{

    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }

    static class GrammarResolution
    {
        // для хранения и передачи заданной формулы и результатов работы
        static List<string> VTList;
        static List<string> VNList;
        static List<string> PList;
        static string SSymbol;
        public static List<Tree> TreeList { get; private set; }
        public static List<string> NullifiedNTList { get; private set; }
        static List<Nonterminal> NonterminalList;
        static Boolean isLeftSideGeneration;
        static int startQuantity;
        static int endQuantity;
        public static Boolean hasNullifiedNonterminal { get; private set; }

        public const Boolean LeftSideComposition = true;
        public const Boolean RightSideComposition = false;
        private const string rArrow = "→";
        private const string epsilon = "ε";

        static GrammarResolution()
        {
            VTList = new List<string>();
            VNList = new List<string>();
            PList = new List<string>();
            NullifiedNTList = new List<string>();
            NonterminalList = new List<Nonterminal>();
            string SSymbol;
            TreeList = new List<Tree>();
            isLeftSideGeneration = true;
            hasNullifiedNonterminal = false;
            startQuantity = 0;
            endQuantity = 1;
        }
        public static void Clear()
        {
            VTList.Clear();
            VNList.Clear();
            PList.Clear();
            NonterminalList.Clear();
            NullifiedNTList.Clear();
            hasNullifiedNonterminal = false;
            SSymbol = "";
            TreeList.Clear();
            isLeftSideGeneration = true;
            startQuantity = 0;
            endQuantity = 1;
        }

        private static Boolean NullifiedNonterminals()
        {
            Boolean b = false;
            foreach(var i in PList)
            {
                if (i.Contains(epsilon))
                {
                    b = true;
                    NullifiedNTList.Add(i.Substring(0, 1));
                }                    
            }
            return b;
        }
        public static void SetTerminalList(List<string> ListVT)
        {
            VTList = ListVT;
        }
        public static IList<string> GetTerminalList()
        {
            IList<string> roVTList = VTList.AsReadOnly();
            return roVTList;
        }
        public static void SetNonterminalList(List<string> ListVN)
        {
            VNList = ListVN;
        }
        public static void SetProductionRulesList(List<string> ListP)
        {
            PList = ListP;
            foreach (var i in VNList)
            {
                String searchString = i.ToString() + rArrow;
                Nonterminal nonterminal = new Nonterminal();
                nonterminal.Name = i.ToString();
                foreach(var p in PList)
                {
                    if (p.Contains(searchString))
                    {
                        nonterminal.Rules.Add(p.Substring(2));
                    }
                }
                NonterminalList.Add(nonterminal);
            }
            hasNullifiedNonterminal = NullifiedNonterminals();
        }
        public static void SetStartingSymbol(string Symbol)
        {
            SSymbol = Symbol;
        }
        public static void SetDirection(Boolean Direction)
        {
            isLeftSideGeneration = Direction;
        }
        public static void SetRange(int start, int end)
        {
            try
            {
                if (start < 0)
                {
                    throw new Exception("Установлено значение нижней границы выборки ниже допустимого");
                }
                if (end < start)
                {
                    throw new Exception("Установлено значение нижней границы выборки выше верхней границы");
                }
                if (end > 10)
                {
                    throw new Exception("Установлено значение верхней границы выборки выше допустимого");
                }
                startQuantity = start;
                endQuantity = end;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Ошибка модуля GrammarResolution: {e.Message}");
            }

        }
        public static void GenerateResolutions()
        {
                //Определяем начальное дерево из одного корневого элемента
            Tree tree = new Tree();
            tree.Insert(SSymbol);
                //Запускаем рекурсивную функцию
            Resolute(tree);
        }
        private static void Resolute(Tree workingTree)
        {
            Boolean hasNonterminal = false;
                //Определяем количество непустых листьев. Если выше предельного - выходим.
            List <Node> workingTreeLeaves = workingTree.GetLeavesNodes();
            int outputLength = workingTree.GetTerminalCountInLeaves();
            if (outputLength > GrammarResolution.endQuantity)
                return;
                //В листьях произвести поиск нетерминалов с нужной стороны. Ищем все существующие нетерминалы по очереди
            foreach (var nonterminal in GrammarResolution.NonterminalList)
            {
                for (int i = 0; i < workingTreeLeaves.Count; i++)
                {
                    int leafIndex = 0;
                    if (GrammarResolution.isLeftSideGeneration)
                        leafIndex = i;
                    else
                        leafIndex = workingTreeLeaves.Count - i - 1;
                    if(workingTreeLeaves[leafIndex].TextData == nonterminal.Name)
                    {
                        hasNonterminal = true;
                            //Найден нетерминал. Делаем подстановку
                        foreach(var rule in nonterminal.Rules)
                        {
                                //Копируем себе экземпляр дерева.
                            Tree appendedTree = workingTree.Clone();
                                //Ссылаемся на список листьев клонированного дерева
                            List<Node> appendedTreeLeaves = appendedTree.GetLeavesNodes();
                                //Размещаем узлы: индекс нужного листа нам известен, а деревья идентичны
                            appendedTree.Insert(appendedTreeLeaves[leafIndex], rule);
                                //Теперь проведём новый поиск в изменённом дереве
                            Resolute(appendedTree);
                        }
                    }
                }
            }
            if ((!hasNonterminal) && (outputLength >= GrammarResolution.startQuantity))
            {
                TreeList.Add(workingTree);
            }            
        }
    }

    class Node
    {
        public string TextData { get; set; }
        public List<Node> Children = new List<Node>();  //Обычно у узла бывает 2 потомка, но у нас их может быть больше - мы не знаем, сколько.
        public void DisplayNode()
        {
            System.Console.WriteLine(this.TextData);
        }       
    }
    class Tree 
    {
        private const string rArrow = "→";
        private const string epsilon = "ε";
        private Node root;
        private List<Node> leavesNode = new List<Node>();
        private int terminalCounter = 0;
        public int GetTerminalCountInLeaves()
        {
            int NullifiedNTCorrection = 0;
            if (GrammarResolution.hasNullifiedNonterminal)
            {
                foreach(var i in GrammarResolution.NullifiedNTList)
                {
                    foreach(var j in leavesNode)
                    {
                        if (i.Equals(j.TextData))
                            NullifiedNTCorrection++;
                    }
                }
            }
            //Коррекция уменьшает число символов в конечной строке на число содержащихся в строке нетерминалов, которые можно стереть (замена на "эпсилон")
            return terminalCounter - NullifiedNTCorrection;
        }
        public string GetLeavesNodeContent()
        {
            string strLeaves = "";
            foreach (var i in leavesNode)
            {
                strLeaves = String.Concat(strLeaves, i.TextData);
            }
            return strLeaves;
        }
        public string GetLeavesNodeTerminals()
        {            
            string strLeaves = "";
            foreach (var i in leavesNode)
            {
                if (GrammarResolution.GetTerminalList().Contains(i.TextData))
                    strLeaves = String.Concat(strLeaves, i.TextData);
            }
            return strLeaves;
        }
        public List<Node> GetLeavesNodes()
        {
            return leavesNode;
        }
        public Node GetRoot()
        {
            return root;
        }
        public void Insert(string nonterminalName)
        {
            //Просто создаем корень
            Node newNode = new Node();
            newNode.TextData = nonterminalName;
            root = newNode;
            Traverse();
        } 
        public void Insert(Node parent, string rule)
        {
            //Перегруженный метод для создания некорневых родителей.
            //Дерево разбора строится слева или справа в зависимости от указанного направления.
            //Задача поиска нужного узла для вставки возлагается на другую функцию.
            //Здесь - результат выполнения задачи: родитель и правило замены передаются параметрами,
            //наша задача - применить их.
            //Создаются сразу ВСЕ узлы из правила.
            for (int i = 0; i < rule.Length; i++)
            {
                Node newNode = new Node();
                if (rule == epsilon)                //Уничтожаемый нетерминал
                    newNode.TextData = "";
                else
                    newNode.TextData = rule.Substring(i, 1);
                parent.Children.Add(newNode);
            }
            Traverse();
        }
        public void Traverse()
        {
            leavesNode.Clear();         //Важно - до вызова рекурсивного метода
            terminalCounter = 0;
            Traverse(root);
        }

        private void Traverse(Node currentNode)
        {
            if (currentNode.Children.Count != 0)
            {
                for(int i = 0; i < currentNode.Children.Count; i++)
                {
                    Node childNode = currentNode.Children[i];
                    Traverse(childNode);
                }
            }
            else
            {
                leavesNode.Add(currentNode);
                if (!(currentNode.TextData.Equals("")))
                    terminalCounter++;
            }
        }
        public void TreeViewFill(Node currentNode, TreeNode treeNode)
        {
            if (currentNode.Children.Count != 0)
            {
                for (int i = 0; i < currentNode.Children.Count; i++)
                {
                    Node childNode = currentNode.Children[i];
                    TreeNode childTreeNode = new TreeNode();
                    if ((childNode.TextData.Equals("")))
                        childTreeNode.Text = epsilon;
                    else
                        childTreeNode.Text = childNode.TextData;
                    treeNode.Nodes.Add(childTreeNode);                    
                    TreeViewFill(childNode, childTreeNode);
                }
            }
        }
        private Node NodeCopy (Node currentNode)
        {
            Node actualNode = new Node();
            actualNode.TextData = currentNode.TextData;
            if (currentNode.Children.Count != 0)
            {
                for (int i = 0; i < currentNode.Children.Count; i++)
                {
                    actualNode.Children.Add(NodeCopy(currentNode.Children[i]));
                }
            }
            return actualNode;
        }
        public Tree Clone()
        {
            Tree cloneTree = new Tree();
            cloneTree.root = NodeCopy(root);
            cloneTree.Traverse();
            return cloneTree;
        }
    }
    public class Nonterminal
    {
        //По своей сути, этот класс - структура из нетерминала и тех правил грамматики, которые к нему относятся.
        //Поэтому работаем здесь с публично открытыми полями (в т.ч. списком).
        public string Name;
        public List<string> Rules = new List<string>();
    }
}