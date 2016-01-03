using System;
using System.CodeDom;
using System.Windows.Forms;
using CodeGen;

namespace CodeGenStudio
{
	public partial class CodeGenStudio : Form
	{
	    private CompileUnitBuilder _currentCompileUnitBuilder;

	    private ClassBuilder _currentClassBuilder;
        public CodeGenStudio()
		{
			InitializeComponent();
		}

	    private void ToggelClassesPanel()
	    {
	        ClassesPanel.Visible = _currentCompileUnitBuilder != null;
	        PublishButton.Enabled = _currentCompileUnitBuilder != null;
	        
	    }

	    private void ToggelClassPanel()
	    {
	        ClassPanel.Visible = _currentClassBuilder != null;
	    }

        

        private void NamespaceCreateButton_Click(object sender, EventArgs e)
        {
            _currentCompileUnitBuilder = new CompileUnitBuilder(NamespaceBox.Text);
            NamespaceBox.Enabled = false;
            NamespaceCreateButton.Enabled = false;
            ToggelClassesPanel();
            UpdateClasses();
        }

	    private void UpdateClasses()
	    {
            ClassesBox.Items.Clear();
	        foreach (CodeTypeDeclaration type in _currentCompileUnitBuilder.GetCompileUnit().Namespaces[0].Types)
	        {
	            ClassesBox.Items.Add(type.Name);
	        }
	    }

        private void CreateClassButton_Click(object sender, EventArgs e)
        {
            _currentClassBuilder = new ClassBuilder(ClassNameBox.Text);
            ToggelClassPanel();
            UpdateClassMembers();
        }

	    private void UpdateClassMembers()
	    {
	        MembersBox.Items.Clear();

	        foreach (CodeTypeMember member in _currentClassBuilder.GetDeclaration().Members)
	        {
	            MembersBox.Items.Add(member.Name);
	        }
	    }

        private void FieldAddButton_Click(object sender, EventArgs e)
        {
            switch (FieldTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddField<string>(FieldNameBox.Text);
                    break;
                case "int":
                    _currentClassBuilder.AddField<int>(FieldNameBox.Text);
                    break;
                case "bool":
                    _currentClassBuilder.AddField<bool>(FieldNameBox.Text);
                    break;
                default:
                    break;
            }

            UpdateClassMembers();
            FieldNameBox.Text = "";
        }

        private void PropertyAddButton_Click(object sender, EventArgs e)
        {
            switch (PropertyTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddProperty<string>(PropertyNameBox.Text);
                    break;
                case "int":
                    _currentClassBuilder.AddProperty<int>(PropertyNameBox.Text);
                    break;
                case "bool":
                    _currentClassBuilder.AddProperty<bool>(PropertyNameBox.Text);
                    break;
                default:
                    break;
            }

            UpdateClassMembers();
            PropertyNameBox.Text = "";
        }

        private void MethodAddButton_Click(object sender, EventArgs e)
        {
            switch (MethodTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddMethod<string>(MethodNameBox.Text, new ParameterItem[] { });
                    break;
                case "int":
                    _currentClassBuilder.AddMethod<int>(MethodNameBox.Text, new ParameterItem[] { });
                    break;
                case "bool":
                    _currentClassBuilder.AddMethod<bool>(MethodNameBox.Text, new ParameterItem[] { });
                    break;
                case "void":
                    _currentClassBuilder.AddVoidMethod(name: MethodNameBox.Text,parameterItems: new ParameterItem[] { }, lines: new string[] { "int i = 5;"});
					_currentClassBuilder.AddEntryPoint<int>("Main");
                    break;
                default:
                    break;
            }

            UpdateClassMembers();
            MethodNameBox.Text = "";
        }

        private void SaveClassButton_Click(object sender, EventArgs e)
        {
            _currentCompileUnitBuilder.AddClass(_currentClassBuilder.GetDeclaration());
            UpdateClasses();
            _currentClassBuilder = null;
            ToggelClassPanel();
            ClassNameBox.Text = "";
            FieldNameBox.Text = "";
            PropertyNameBox.Text = "";
            MethodNameBox.Text = "";
            MembersBox.Items.Clear();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "MyNewClass";
            saveFileDialog1.Filter = "CSharp(*.cs)|*.cs";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = saveFileDialog1.FileName;
                _currentCompileUnitBuilder.PublishCode(path);
            }

            _currentCompileUnitBuilder = null;
            NamespaceBox.Text = "";
            ClassesBox.Items.Clear();
            ToggelClassPanel();
        }
    }
}
