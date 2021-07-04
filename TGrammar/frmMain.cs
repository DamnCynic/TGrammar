using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TGrammar
{
    public partial class frmMain : Form
    {
        private const string rArrow = "→";
        private const string epsilon = "ε";
        public frmMain()
        {
            InitializeComponent();
        }

        private void listBoxVT_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyCode)
            {
                listBoxVT.Items.Remove(listBoxVT.SelectedItem);                
            }
            BackusNaurAssembling();
        }

        private void listBoxVN_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyCode)
            {
                listBoxVN.Items.Remove(listBoxVN.SelectedItem);
            }
            BackusNaurAssembling();
        }

        private void listBoxP_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyCode)
            {
                listBoxP.Items.Remove(listBoxP.SelectedItem);
            }
            BackusNaurAssembling();
        }

        private void buttonVT_Click(object sender, EventArgs e)
        {
            if (!HasMatch(listBoxVN, textBoxVT.Text))
                if (HasMatch(listBoxVT, textBoxVT.Text))
                {
                    MessageBox.Show("Совпадение с имеющимся символом в текущем множестве (такой символ в алфавите уже есть).",
                        "Коллизия множеств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    listBoxVT.Items.Add(textBoxVT.Text);
            else
                MessageBox.Show("Совпадение с имеющимся символом в соседнем множестве. Множества терминалов и нетерминалов не должны пересекаться!", 
                    "Коллизия множеств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            textBoxVT.Clear();
            BackusNaurAssembling();
        }
        private void buttonVN_Click(object sender, EventArgs e)
        {
            if (!HasMatch(listBoxVT, textBoxVN.Text))
                if (HasMatch(listBoxVN, textBoxVN.Text))
                {
                    MessageBox.Show("Совпадение с имеющимся символом в текущем множестве (такой символ в алфавите уже есть).",
                        "Коллизия множеств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else 
                    listBoxVN.Items.Add(textBoxVN.Text);
            else
                MessageBox.Show("Совпадение с имеющимся символом в соседнем множестве. Множества терминалов и нетерминалов не должны пересекаться!",
                    "Коллизия множеств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            textBoxVN.Clear();
            BackusNaurAssembling();
        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void buttonP_Click(object sender, EventArgs e)
        {
            string buffer = "";
            if (HasMatch(listBoxVN, textBoxPa.Text))
                buffer = textBoxPa.Text + rArrow;
            else
            {
                MessageBox.Show("Левый аргумент правила отсутствует в перечне нетерминалов!",
                    "Отсутствие аргумента", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxPb.TextLength == 0)
                buffer += epsilon;
            else
            {
                for(int i = 0; i < textBoxPb.TextLength; i++)
                {
                    if (HasMatch(listBoxVT, textBoxPb.Text.Substring(i, 1)) || HasMatch(listBoxVN, textBoxPb.Text.Substring(i, 1)));
                    else
                    {
                        string s = String.Concat("Символ \"", textBoxPb.Text.Substring(i, 1));
                        s = String.Concat(s, "\" отсутствует в перечнях терминалов и нетерминалов!");
                        MessageBox.Show(s, "Отсутствие аргумента", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                buffer += textBoxPb.Text;
            }
            if (!HasMatch(listBoxP, buffer))
                listBoxP.Items.Add(buffer);
            else
            {
                MessageBox.Show("Такое правило уже есть: " + buffer,
                    "Отсутствие аргумента", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            textBoxPa.Clear();
            textBoxPb.Clear();
            BackusNaurAssembling();
        }

        private void textBoxVT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonVT_Click(sender, e);
                textBoxVT.Clear();
            }
        }

        private void textBoxVN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonVN_Click(sender, e);
                textBoxS_TextChanged(sender, e);
            }

            // allows only letters
            if (!char.IsLetter(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxPa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonP_Click(sender, e);
            }
        }

        private void textBoxPb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonP_Click(sender, e);
            }
        }

        private bool HasMatch(ListBox listBox, string textString)
        {
            if (!string.IsNullOrEmpty(textString))
            {
                for (int i = listBox.Items.Count; i > 0; i--)
                {
                    if (textString.Equals(listBox.Items[i - 1]))
                        return true;
                }
            }
            return false;
        }

        private void BackusNaurAssembling()
        {
            string leftPartBufferString = "";
            string formula = "G({";
            for (int i = 0; i < listBoxVT.Items.Count; i++)
            {
                formula = String.Concat(formula, listBoxVT.Items[i].ToString());
                if (i != listBoxVT.Items.Count - 1) formula = String.Concat(formula, ", ");                
            }
            formula = String.Concat(formula, "}, {");
            for (int i = 0; i < listBoxVN.Items.Count; i++)
            {
                formula = String.Concat(formula, listBoxVN.Items[i].ToString());
                if (i != listBoxVN.Items.Count - 1) formula = String.Concat(formula, ", ");
            }
            formula = String.Concat(formula, "}, {");
            for (int i = 0; i < listBoxP.Items.Count; i++)
            {
                if (string.Equals(leftPartBufferString, listBoxP.Items[i].ToString().Substring(0, 2)))
                {
                    formula = String.Concat(formula, " | ", listBoxP.Items[i].ToString().Substring(2, listBoxP.Items[i].ToString().Length - 2));
                }
                else
                {
                    if (i != 0) formula = String.Concat(formula, ", ");
                    leftPartBufferString = listBoxP.Items[i].ToString().Substring(0, 2);
                    formula = String.Concat(formula, listBoxP.Items[i].ToString());
                }                
            }
            formula = String.Concat(formula, "}, ", textBoxS.Text, ")");
            labelResult.Text = formula;
        }

        private void textBoxS_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxS.Text != "") && (!HasMatch(listBoxVN, textBoxS.Text)))
            {
                MessageBox.Show("Введённый символ отсутствует в множестве нетерминалов.",
                    "Коллизия множеств", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxS.Clear();
            }
            BackusNaurAssembling();
        }

        private void справкаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(@"tgrammar.html")
                {
                    UseShellExecute = true
                };
                p.Start();
            }
            catch
            {
                MessageBox.Show("Не удалось открыть справку (возможно, нет файла tgrammar.html в папке приложения)", "Ошибка вызова справки", MessageBoxButtons.OK,  MessageBoxIcon.Error);
            }
        }

        private void buttonApplyInternalGrammar_Click(object sender, EventArgs e)
        {
            //TODO: загружать синтаксически правильные образцы из файла/в файл (в рамках ТЗ это не требуется)
            FormReset();
            switch (comboBoxInternalGrammar.SelectedIndex)
            {
                case 0:
                    //G({0, 1, 2, 3, 4, 5, 6, 7, 8, 9, –, +}, { F, S, T}, { F → 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | , S → T | +T | –T, T → F | TF}, S)
                    listBoxVT.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "–", "+" });
                    listBoxVN.Items.AddRange(new object[] { "F", "S", "T" });
                    listBoxP.Items.AddRange(new object[] { "F→0", "F→1", "F→2", "F→3", "F→4", "F→5", "F→6", "F→7", "F→8", "F→9", "S→T", "S→+T", "S→-T", "T→F", "T→TF" });
                    textBoxS.Text = "S";
                    break;
                case 1:
                    //G({a, b}, { A, S}, { A → aA | a, S → aA | bS}, S)
                    listBoxVT.Items.AddRange(new object[] { "a", "b" });
                    listBoxVN.Items.AddRange(new object[] { "A", "S" });
                    listBoxP.Items.AddRange(new object[] { "A→aA", "A→a", "S→aA", "S→bS" });
                    textBoxS.Text = "S";
                    break;
                case 2:
                    //G({0}, { A, S}, { A → AAA | 0, S → AAA}, S)
                    listBoxVT.Items.AddRange(new object[] { "0" });
                    listBoxVN.Items.AddRange(new object[] { "A", "S" });
                    listBoxP.Items.AddRange(new object[] { "A→AAA", "A→0", "S→AAA" });
                    textBoxS.Text = "S";
                    break;
                case 3:
                    //G({(, )}, { S}, { S → (S)S | S(S) | ε}, S)
                    listBoxVT.Items.AddRange(new object[] { "(", ")" });
                    listBoxVN.Items.AddRange(new object[] { "S" });
                    listBoxP.Items.AddRange(new object[] { "S→(S)S", "S→S(S)", "S→ε" });
                    textBoxS.Text = "S";
                    break;
                case 4:
                    //G({a, b}, {S}, {S → aSbS | bSaS | ε}, S)
                    listBoxVT.Items.AddRange(new object[] { "a", "b" });
                    listBoxVN.Items.AddRange(new object[] { "S" });
                    listBoxP.Items.AddRange(new object[] { "S→aSbS", "S→bSaS", "S→ε" });
                    textBoxS.Text = "S";
                    break;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            FormReset();
            GrammarResolution.Clear();
        }

        private void FormReset()
        {
            listBoxVT.Items.Clear();
            listBoxVN.Items.Clear();
            listBoxP.Items.Clear();
            textBoxS.Text = "";
            listBoxResult.Items.Clear();
            treeView1.Nodes.Clear();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartAssembly();
        }

        private void StartAssembly()
        {
            if(listBoxVT.Items.Count == 0)
            {
                MessageBox.Show("Не заполнен список терминалов", "Ошибка запуска процедуры");
                return;
            }
            if (listBoxVN.Items.Count == 0)
            {
                MessageBox.Show("Не заполнен список нетерминалов", "Ошибка запуска процедуры");
                return;
            }
            if (listBoxP.Items.Count == 0)
            {
                MessageBox.Show("Не заполнен список правил", "Ошибка запуска процедуры");
                return;
            }
            if (textBoxS.Text.Length == 0)
            {
                MessageBox.Show("Не указан стартовый нетерминал", "Ошибка запуска процедуры");
                return;
            }
                //Очистить предыдущее состояние перед успешным запуском
            GrammarResolution.Clear();
            GrammarResolution.SetTerminalList(listBoxVT.Items.Cast<string>().ToList());
            GrammarResolution.SetNonterminalList(listBoxVN.Items.Cast<string>().ToList());
            GrammarResolution.SetProductionRulesList(listBoxP.Items.Cast<string>().ToList());
            if (radioButtonLeftSide.Checked) GrammarResolution.SetDirection(GrammarResolution.LeftSideComposition);
            if (radioButtonRightSide.Checked) GrammarResolution.SetDirection(GrammarResolution.RightSideComposition);
            GrammarResolution.SetStartingSymbol(textBoxS.Text);
            GrammarResolution.SetRange(((int)numericUpDown1.Value), ((int)numericUpDown2.Value));
                //Вызвать метод асинхронно
            buttonStart.Enabled = false;
            buttonReset.Enabled = false;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += bw_DoWork;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.RunWorkerAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show("Ошибка выполнения: " + e.Message, "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonStart.Enabled = true;
                buttonReset.Enabled = true;
            }
        }
            //Создаем цепочки в отдельном потоке
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            GrammarResolution.GenerateResolutions();
        }

            //Цепочки созданы.
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                listBoxResult.Items.Clear();
                for (int i = 0; i < GrammarResolution.TreeList.Count; i++)
                {
                    if (GrammarResolution.TreeList[i].GetLeavesNodeContent().Equals(""))
                    {
                        listBoxResult.Items.Add("(пусто)");
                    }
                    else
                        listBoxResult.Items.Add(GrammarResolution.TreeList[i].GetLeavesNodeContent());
                }
                this.Cursor = Cursors.Default;
                buttonStart.Enabled = true;
                buttonReset.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выводе в список: " + ex.Message, "Ошибка выполнения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonStart.Enabled = true;
                buttonReset.Enabled = true;
            }
        }

        private void listBoxResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            Node root = GrammarResolution.TreeList[listBoxResult.SelectedIndex].GetRoot();
            TreeNode treeRoot = new TreeNode();
            treeRoot.Text = root.TextData;
            GrammarResolution.TreeList[listBoxResult.SelectedIndex].TreeViewFill(root, treeRoot);
            treeView1.Nodes.Add(treeRoot);
            treeView1.ExpandAll();
        }
    }    
}
