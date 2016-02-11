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
		private InterfaceBuilder _currentInterfaceBuilder;

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
			MemberAttributes attr = MemberAttributes.Public;
			switch (FieldAccessBox.Text)
			{
				case "public":
					attr = MemberAttributes.Public;
					break;
				case "protected":
					attr = MemberAttributes.Family;
					break;
				case "private":
					attr = MemberAttributes.Private;
					break;
				default:
					break;
			}
			switch (FieldTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddField<string>(FieldNameBox.Text, attr);
                    break;
                case "int":
                    _currentClassBuilder.AddField<int>(FieldNameBox.Text, attr);
                    break;
                case "bool":
                    _currentClassBuilder.AddField<bool>(FieldNameBox.Text, attr);
                    break;
                default:
                    break;
            }

            UpdateClassMembers();
            FieldNameBox.Text = "";
        }

        private void PropertyAddButton_Click(object sender, EventArgs e)
        {
			MemberAttributes attr = MemberAttributes.Private;
			switch (PropertyAccessBox.Text)
			{
				case "public":
					attr = MemberAttributes.Public;
					break;
				case "protected":
					attr = MemberAttributes.Family;
					break;
				case "private":
					attr = MemberAttributes.Private;
					break;
				default:
					break;
			}
			switch (PropertyTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddProperty<string>(PropertyNameBox.Text, attr);
                    break;
                case "int":
                    _currentClassBuilder.AddProperty<int>(PropertyNameBox.Text, attr);
                    break;
                case "bool":
                    _currentClassBuilder.AddProperty<bool>(PropertyNameBox.Text, attr);
                    break;
                default:
                    break;
            }

            UpdateClassMembers();
            PropertyNameBox.Text = "";
        }

        private void MethodAddButton_Click(object sender, EventArgs e)
        {
			MemberAttributes attr = MemberAttributes.Public;
			switch (PropertyAccessBox.Text)
			{
				case "public":
					attr = MemberAttributes.Public;
					break;
				case "protected":
					attr = MemberAttributes.Family;
					break;
				case "private":
					attr = MemberAttributes.Private;
					break;
				default:
					break;
			}
			switch (MethodTypeBox.Text)
            {
                case "string":
                    _currentClassBuilder.AddMethod<string>(MethodNameBox.Text, new ParameterItem[] { }, attr);
                    break;
                case "int":
                    _currentClassBuilder.AddMethod<int>(MethodNameBox.Text, new ParameterItem[] { }, attr);
                    break;
                case "bool":
                    _currentClassBuilder.AddMethod<bool>(MethodNameBox.Text, new ParameterItem[] { }, attr);
                    break;
                case "void":
                    _currentClassBuilder.AddVoidMethod(name: MethodNameBox.Text,parameterItems: new ParameterItem[] { },attr: attr);
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
			var language = GenerateCodeBox.Text;
			string sExt = "";
			saveFileDialog1.FileName = "MyNewClass";
			switch (language)
			{
				case "CSharp":
					sExt = "CSharp(*.cs|*.cs";
                    break;
				case "CPP":
					sExt = "CPP(*.cpp)|*.cpp";
					break;
				case "VB":
					sExt = "Visual Basic(*.vb)|*.vb";
					break;
				case "JScript":
					sExt = "JScript(*.js)|*.js";
					break;
				default:
					language = "CSharp";
					sExt = "CSharp(*.cs|*.cs";
					break;
			}
			saveFileDialog1.Filter = sExt;

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
