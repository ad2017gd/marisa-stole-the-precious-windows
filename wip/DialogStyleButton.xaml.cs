using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wip
{
    /// <summary>
    /// Interaction logic for DialogStyleButton.xaml
    /// </summary>
    

    public partial class DialogStyleButton : UserControl
    {
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            name: "Text",
            propertyType: typeof(string),
            ownerType: typeof(DialogStyleButton),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: "OK"));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public DialogStyleButton()
        {
            DataContext = this;
            VisualTextRenderingMode = TextRenderingMode.ClearType;
            InitializeComponent();
        }
    }
}
