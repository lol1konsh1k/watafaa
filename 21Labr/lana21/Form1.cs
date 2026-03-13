using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DisciplineManager
{
    public partial class Form1 : Form
    {
        private class Discipline
        {
            public string Name { get; set; }
            public bool IsPassed { get; set; }
        }

        private List<Discipline> _disciplines = new List<Discipline>();

        private FlowLayoutPanel _listPanel;
        private Panel _bottomPanel;
        private Panel _filterPanel;
        private RadioButton _rbAll;
        private RadioButton _rbPassed;
        private RadioButton _rbFailed;
        private Button _btnAdd;
        private Button _btnChangeStatus;
        private Button _btnDelete;
        private Label _lblInfo;

        private Discipline _selectedDiscipline = null;
        private Panel _selectedRowPanel = null;

        public Form1()
        {
            InitializeComponent();

            _disciplines.Add(new Discipline { Name = "География", IsPassed = true });
            _disciplines.Add(new Discipline { Name = "Философия", IsPassed = false });
            _disciplines.Add(new Discipline { Name = "Литература", IsPassed = true });
            _disciplines.Add(new Discipline { Name = "Русский", IsPassed = true });
            _disciplines.Add(new Discipline { Name = "Математика", IsPassed = false });

            RefreshList();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "Учет дисциплин";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F);
            this.BackColor = Color.WhiteSmoke;

            _filterPanel = new Panel();
            _filterPanel.Dock = DockStyle.Top;
            _filterPanel.Height = 60;
            _filterPanel.BackColor = Color.FromArgb(205, 245, 245);
            _filterPanel.Padding = new Padding(10);

            _rbAll = CreateRadioButton("Все", 10, 15);
            _rbPassed = CreateRadioButton("Успешно сданные", 100, 15);
            _rbFailed = CreateRadioButton("Не сданные", 280, 15);

            _rbAll.Checked = true;
            _rbAll.CheckedChanged += (s, e) => RefreshList();
            _rbPassed.CheckedChanged += (s, e) => RefreshList();
            _rbFailed.CheckedChanged += (s, e) => RefreshList();

            _filterPanel.Controls.AddRange(new Control[] { _rbAll, _rbPassed, _rbFailed });
            this.Controls.Add(_filterPanel);

            _listPanel = new FlowLayoutPanel();
            _listPanel.Dock = DockStyle.Fill;
            _listPanel.AutoScroll = true;
            _listPanel.FlowDirection = FlowDirection.TopDown;
            _listPanel.WrapContents = false;
            _listPanel.Padding = new Padding(10);
            _listPanel.Resize += (s, e) => {
                foreach (Control control in _listPanel.Controls)
                {
                    if (control is Panel row)
                    {
                        row.Width = _listPanel.ClientSize.Width - 25;
                    }
                }
            };
            this.Controls.Add(_listPanel);

            _bottomPanel = new Panel();
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Height = 90;
            _bottomPanel.BackColor = Color.White;
            _bottomPanel.BorderStyle = BorderStyle.FixedSingle;

            _lblInfo = new Label();
            _lblInfo.Text = "Выберите дисциплину из списка";
            _lblInfo.Location = new Point(15, 10);
            _lblInfo.AutoSize = true;
            _lblInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblInfo.ForeColor = Color.Gray;
            _bottomPanel.Controls.Add(_lblInfo);

            _btnChangeStatus = new Button();
            _btnChangeStatus.Text = "Изменить статус";
            _btnChangeStatus.Location = new Point(15, 40);
            _btnChangeStatus.Size = new Size(140, 35);
            _btnChangeStatus.Enabled = false;
            _btnChangeStatus.Click += BtnChangeStatus_Click;
            _bottomPanel.Controls.Add(_btnChangeStatus);

            _btnDelete = new Button();
            _btnDelete.Text = "Удалить";
            _btnDelete.Location = new Point(165, 40);
            _btnDelete.Size = new Size(100, 35);
            _btnDelete.BackColor = Color.LightCoral;
            _btnDelete.Enabled = false;
            _btnDelete.Click += BtnDelete_Click;
            _bottomPanel.Controls.Add(_btnDelete);

            _btnAdd = new Button();
            _btnAdd.Text = "+ Добавить дисциплину";
            _btnAdd.Size = new Size(200, 40);
            _btnAdd.BackColor = Color.LightGreen;
            _btnAdd.FlatStyle = FlatStyle.Popup;
            _btnAdd.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnAdd.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            _btnAdd.Location = new Point(_bottomPanel.Width - _btnAdd.Width - 15, 25);
            _btnAdd.Click += BtnAdd_Click;

            _bottomPanel.Resize += (s, e) => {
                _btnAdd.Location = new Point(_bottomPanel.Width - _btnAdd.Width - 15, 25);
            };

            _bottomPanel.Controls.Add(_btnAdd);
            this.Controls.Add(_bottomPanel);

            this.ResumeLayout(false);
        }

        private RadioButton CreateRadioButton(string text, int x, int y)
        {
            var rb = new RadioButton();
            rb.Text = text;
            rb.Location = new Point(x, y);
            rb.AutoSize = true;
            return rb;
        }

        private void RefreshList()
        {
            _listPanel.Controls.Clear();
            _selectedDiscipline = null;
            _selectedRowPanel = null;
            UpdateBottomPanelState();

            _listPanel.Padding = new Padding(10, 70, 10, 10);

            IEnumerable<Discipline> filteredData = _disciplines;

            if (_rbPassed.Checked)
                filteredData = _disciplines.Where(d => d.IsPassed);
            else if (_rbFailed.Checked)
                filteredData = _disciplines.Where(d => !d.IsPassed);

            foreach (var disc in filteredData)
            {
                var row = CreateDisciplineRow(disc);
                _listPanel.Controls.Add(row);
            }

            if (_listPanel.Controls.Count > 0)
            {
                _listPanel.Controls[0].Width = _listPanel.ClientSize.Width - 25;
            }
        }

        private Panel CreateDisciplineRow(Discipline disc)
        {
            Panel row = new Panel();
            row.Width = _listPanel.ClientSize.Width - 25;
            row.Height = 50;
            row.Margin = new Padding(0, 0, 0, 5);
            row.BackColor = Color.White;
            row.BorderStyle = BorderStyle.FixedSingle;
            row.Tag = disc;
            row.Cursor = Cursors.Hand;

            Label lblName = new Label();
            lblName.Text = disc.Name;
            lblName.Location = new Point(15, 15);
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 11F);
            lblName.MouseClick += (s, e) => SelectRow(row);

            Panel statusBox = new Panel();
            statusBox.Size = new Size(40, 30);
            statusBox.Location = new Point(row.Width - 60, 10);
            statusBox.BackColor = disc.IsPassed ? Color.FromArgb(50, 250, 50) : Color.FromArgb(250, 50, 50);
            statusBox.MouseClick += (s, e) => SelectRow(row);

            Label lblStatusText = new Label();
            lblStatusText.Text = disc.IsPassed ? "СДАЛ" : "НЕ СДАЛ";
            lblStatusText.Location = new Point(row.Width - 130, 15);
            lblStatusText.AutoSize = true;
            lblStatusText.ForeColor = disc.IsPassed ? Color.FromArgb(50, 150, 50) : Color.FromArgb(150, 50, 50);
            lblStatusText.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatusText.MouseClick += (s, e) => SelectRow(row);

            row.Controls.Add(lblName);
            row.Controls.Add(lblStatusText);
            row.Controls.Add(statusBox);

            row.MouseClick += (s, e) => SelectRow(row);

            row.Resize += (s, e) => {
                lblStatusText.Location = new Point(row.Width - 130, 15);
                statusBox.Location = new Point(row.Width - 60, 10);
            };

            return row;
        }

        private void SelectRow(Panel row)
        {
            if (_selectedRowPanel != null)
                _selectedRowPanel.BackColor = Color.White;

            _selectedRowPanel = row;
            _selectedRowPanel.BackColor = Color.FromArgb(100, 240, 200);

            _selectedDiscipline = (Discipline)row.Tag;

            _lblInfo.Text = $"Выбрано: {_selectedDiscipline.Name}";
            _lblInfo.ForeColor = _selectedDiscipline.IsPassed ? Color.FromArgb(50, 150, 50) : Color.FromArgb(150, 50, 50);

            UpdateBottomPanelState();
        }

        private void UpdateBottomPanelState()
        {
            bool isSelected = _selectedDiscipline != null;
            _btnChangeStatus.Enabled = isSelected;
            _btnDelete.Enabled = isSelected;

            if (!isSelected)
            {
                _lblInfo.Text = "Выберите дисциплину из списка";
                _lblInfo.ForeColor = Color.Gray;
            }
        }

        private void BtnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_selectedDiscipline == null) return;

            _selectedDiscipline.IsPassed = !_selectedDiscipline.IsPassed;

            MessageBox.Show($"Статус дисциплины '{_selectedDiscipline.Name}' изменен на: {(_selectedDiscipline.IsPassed ? "Сдал" : "Не сдал")}",
                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshList();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedDiscipline == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить дисциплину '{_selectedDiscipline.Name}'?",
                "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _disciplines.Remove(_selectedDiscipline);
                RefreshList();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            Form addForm = new Form();
            addForm.Text = "Добавить дисциплину";
            addForm.Size = new Size(350, 200);
            addForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            addForm.StartPosition = FormStartPosition.CenterParent;
            addForm.MaximizeBox = false;
            addForm.MinimizeBox = false;

            Label lblName = new Label();
            lblName.Text = "Название дисциплины:";
            lblName.Location = new Point(20, 20);
            lblName.AutoSize = true;
            addForm.Controls.Add(lblName);

            TextBox txtName = new TextBox();
            txtName.Location = new Point(20, 45);
            txtName.Width = 290;
            addForm.Controls.Add(txtName);

            Label lblStatus = new Label();
            lblStatus.Text = "Статус:";
            lblStatus.Location = new Point(20, 80);
            lblStatus.AutoSize = true;
            addForm.Controls.Add(lblStatus);

            ComboBox cbStatus = new ComboBox();
            cbStatus.Items.Add("Не сдал");
            cbStatus.Items.Add("Сдал");
            cbStatus.SelectedIndex = 0;
            cbStatus.Location = new Point(20, 105);
            cbStatus.Width = 290;
            cbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            addForm.Controls.Add(cbStatus);

            Button btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(130, 140);
            btnSave.Width = 80;
            btnSave.DialogResult = DialogResult.OK;
            addForm.Controls.Add(btnSave);
            addForm.AcceptButton = btnSave;

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название дисциплины!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var newDisc = new Discipline
                {
                    Name = txtName.Text,
                    IsPassed = (cbStatus.SelectedIndex == 1)
                };

                _disciplines.Add(newDisc);
                RefreshList();

                if (_listPanel.Controls.Count > 0)
                    _listPanel.ScrollControlIntoView(_listPanel.Controls[_listPanel.Controls.Count - 1]);
            }
        }
    }
}